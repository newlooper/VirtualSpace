﻿/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using LinqExpressionBuilder;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Helpers;
using Process = System.Diagnostics.Process;

namespace VirtualSpace.Config.Events.Expression
{
    public static partial class Conditions
    {
        private static readonly JsonParser                        Jp = new();
        private static          List<RuleTemplate>                _rules;
        private static readonly Channel<Behavior>                 ActionProducer            = Channels.ActionChannel;
        private static readonly Channel<Window>                   VisibleWindowsConsumer    = Channels.VisibleWindowsChannel;
        private static readonly ConcurrentDictionary<IntPtr, int> WindowCheckTimes          = new();
        public static readonly  ConcurrentBag<IntPtr>             WndHandleIgnoreListByRule = new();
        private static          long                              _updateRuleLock;

        static Conditions()
        {
            _rules = InitRules();
            BuildRuleExp( _rules );
            RuleChecker();
        }

        private static async void RuleChecker()
        {
            while ( await VisibleWindowsConsumer.Reader.WaitToReadAsync() )
            {
                if ( VisibleWindowsConsumer.Reader.TryRead( out var win ) )
                {
                    CheckRulesForWindow( win );
                }
            }
        }

        private static List<RuleTemplate> InitRules()
        {
            var path  = Manager.GetRuleFilePath();
            var rules = new List<RuleTemplate>();
            if ( !File.Exists( path ) ) return rules;

            rules = ReadRuleFromFile( path );

            return rules;
        }

        private static void BuildRuleExp( List<RuleTemplate> rules )
        {
            foreach ( var rule in rules.Where( rule => rule.Exp is null ) )
            {
                rule.Exp = Jp.ExpressionFromJsonDoc<Window>( rule.Expression );
            }

            _rules.Sort( ( x, y ) => -x.Weight.CompareTo( y.Weight ) );
        }

        public static List<RuleTemplate> FetchRules()
        {
            return _rules;
        }

        private static async void CheckRulesForWindow( Window win )
        {
            if ( _rules.Count == 0 || Interlocked.Read( ref _updateRuleLock ) != 0 ) return;

            var rules = new List<RuleTemplate>( _rules );

            if ( !WindowCheckTimes.ContainsKey( win.Handle ) )
                WindowCheckTimes[win.Handle] = 0;

            var isOnePeriod = WindowCheckTimes[win.Handle] % Const.WindowCheckTimesLimit == 0;

            if ( isOnePeriod )
            {
                Logger.Debug( $"Checking rules for {win.Title}, current profile: {Manager.Configs.CurrentProfileName}" );
            }

            await Task.Run( () =>
            {
                _ = User32.GetWindowThreadProcessId( win.Handle, out var pId );
                using var pInfo = Process.GetProcessById( pId );

                win.ProcessName = pInfo.ProcessName;
                try
                {
                    win.ProcessPath = pInfo.MainModule?.FileName;
                    win.CommandLine = pInfo.GetCommandLineArgs();
                }
                catch ( Exception ex )
                {
                    Logger.Warning( "Get Process Info: " + ex.Message );
                }

                var screen      = Screen.FromHandle( win.Handle );
                var screenIndex = 0;
                var allScreens  = Screen.AllScreens;
                for ( var i = 0; i < allScreens.Length; i++ )
                {
                    if ( screen.DeviceName == allScreens[i].DeviceName )
                    {
                        screenIndex = i;
                        break;
                    }
                }

                win.WinInScreen = screenIndex.ToString();

                if ( !User32.IsWindow( win.Handle ) ) return;

                var hasMatchedRule = false;

                var l = new List<Window>();

                foreach ( var r in rules )
                {
                    if ( !r.Enabled ) continue;
                    l.Add( win );
                    var match = l.Where( r.Exp ).Any();
                    l.Clear();
                    if ( match )
                    {
                        hasMatchedRule = true;
                        Logger.Debug( win.Title + $" match rule [{r.Name}]" );
                        r.Action.Handle = win.Handle;
                        r.Action.RuleName = r.Name;
                        r.Action.WindowTitle = win.Title;
                        ActionProducer.Writer.TryWrite( r.Action );

                        ////////////////////////////////////////////////////////////////
                        // 某个窗口可能与多条规则匹配，继续循环就表示所有相应的动作都会按顺序执行
                        // 最终的方案：给规则添加一个属性，用于指定是否在匹配到规则后继续检查其他规则
                        // 默认为 false，即匹配到规则后立即退出循环
                        if ( !r.ContinueAfterHit )
                            break;
                    }
                }

                if ( hasMatchedRule )
                {
                    WndHandleIgnoreListByRule.Add( win.Handle );
                    return;
                }

                if ( isOnePeriod )
                {
                    Logger.Debug( $"Window [{win.Title}] has no matched rules." );
                }

                if ( Manager.CurrentProfile.IgnoreWindowOnRuleCheckTimeout )
                {
                    if ( WindowCheckTimes[win.Handle] >= Const.WindowCheckTimesLimit )
                    {
                        Logger.Debug( $"Try find rules for [{win.Title}] too many times, ignore the window." );
                        WndHandleIgnoreListByRule.Add( win.Handle );
                    }
                }

                WindowCheckTimes[win.Handle]++;
            } ).ConfigureAwait( false );
        }

        private static List<RuleTemplate> ReadRuleFromFile( string path )
        {
            using var fs     = new FileStream( path, FileMode.Open, FileAccess.Read );
            var       buffer = new byte[fs.Length];
            _ = fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );

            var readOptions = GetJsonDeserializerOptions();
            return JsonSerializer.Deserialize<List<RuleTemplate>>( ref utf8Reader, readOptions );
        }

        public static ExpressionTemplate ParseExpressionTemplate( JsonDocument doc )
        {
            var readOptions = GetJsonDeserializerOptions();
            return JsonSerializer.Deserialize<ExpressionTemplate>( doc, readOptions );
        }

        private static JsonSerializerOptions? _readOptions;
        private static JsonSerializerOptions? _writeOptions;

        private static JsonSerializerOptions GetJsonDeserializerOptions()
        {
            return _readOptions ??= new JsonSerializerOptions();
        }

        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return _writeOptions ??= new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public static async void SaveRules( List<RuleTemplate> ruleList, string? path = null )
        {
            Interlocked.Increment( ref _updateRuleLock );

            _rules = ruleList;
            BuildRuleExp( _rules );

            Interlocked.Decrement( ref _updateRuleLock );

            path ??= Manager.GetRuleFilePath();

            await File.WriteAllBytesAsync( path, JsonSerializer.SerializeToUtf8Bytes(
                ruleList, GetJsonSerializerOptions() ) );

            Logger.Info( $"[RULE]Rules.{Manager.Configs.CurrentProfileName} Saved." );
        }

        public static void SwitchRuleProfile()
        {
            Interlocked.Increment( ref _updateRuleLock );

            _rules = InitRules();
            BuildRuleExp( _rules );

            Interlocked.Decrement( ref _updateRuleLock );

            Logger.Info( $"[RULE]Switch Rule Profile: {Manager.Configs.CurrentProfileName}" );
        }
    }
}