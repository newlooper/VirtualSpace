/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LinqExpressionBuilder;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config.Events.Entity;
using Process = System.Diagnostics.Process;

namespace VirtualSpace.Config.Events.Expression
{
    public static partial class Conditions
    {
        private static readonly JsonParser        Jp            = new();
        private static readonly Channel<Behavior> ActionChannel = Channels.ActionChannel;

        public static async void CheckWindow( Window win )
        {
            var path = Manager.GetRulesPath();
            if ( !File.Exists( path ) ) return;

            var rules = FetchRuleList( path );
            foreach ( var rule in rules )
            {
                rule.Exp = Jp.ExpressionFromJsonDoc<Window>( rule.Expression );
            }

            Logger.Event( "Current Rules Profile: " + Manager.Configs.CurrentProfileName );

            await Task.Run( () =>
            {
                _ = GetWindowThreadProcessId( win.Handle, out var pId );
                using var pInfo = Process.GetProcessById( pId );

                win.ProcessName = pInfo.ProcessName;
                try
                {
                    win.ProcessPath = pInfo.MainModule?.FileName;
                }
                catch ( Exception ex )
                {
                    Logger.Warning( ex.Message );
                }

                var sbClass = new StringBuilder( Const.WindowClassMaxLength );
                _ = GetClassName( win.Handle, sbClass, sbClass.Capacity );
                win.WndClass = sbClass.ToString();

                var sbTitle = new StringBuilder( Const.WindowTitleMaxLength );
                var sw      = Stopwatch.StartNew();
                while ( sw.ElapsedMilliseconds < Const.WindowCheckTimeout )
                {
                    if ( !IsWindow( win.Handle ) ) break;

                    GetWindowText( win.Handle, sbTitle, sbTitle.Capacity );
                    win.Title = sbTitle.ToString();

                    foreach ( var r in rules )
                    {
                        if ( !r.Enabled ) continue;
                        var match = new List<Window> {win}.Where( r.Exp ).Any();
                        if ( match )
                        {
                            Logger.Event( win.Title + $" match rule [{r.Name}]" );
                            r.Action.Handle = win.Handle;
                            r.Action.RuleName = r.Name;
                            r.Action.WindowTitle = win.Title;
                            ActionChannel.Writer.TryWrite( r.Action );
                            return;
                        }
                    }

                    Thread.Sleep( Const.WindowCheckInterval );
                    Logger.Debug( "Check Rules Until Timeout." );
                }
            } );
        }

        public static List<RuleTemplate> FetchRuleList( string path )
        {
            using var fs     = new FileStream( path, FileMode.Open, FileAccess.Read );
            var       buffer = new byte[fs.Length];
            fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );

            var readOptions = new JsonSerializerOptions();
#if NETCOREAPP3_1 || NET5_0
            readOptions.Converters.Add( new JsonConverterJsonDocument() );
#endif
            return JsonSerializer.Deserialize<List<RuleTemplate>>( ref utf8Reader, readOptions );
        }

        public static ExpressionTemplate ParseExpressionTemplate( JsonDocument doc )
        {
            var readOptions = new JsonSerializerOptions();
#if NETCOREAPP3_1 || NET5_0
            readOptions.Converters.Add( new JsonConverterJsonDocument() );
            var json = JsonSerializer.Serialize( doc.RootElement, GetJsonSerializerOptions() );
            return JsonSerializer.Deserialize<ExpressionTemplate>( json, readOptions );
#elif NET6_0
            return JsonSerializer.Deserialize<ExpressionTemplate>( doc, readOptions );
#endif
        }

        public static JsonSerializerOptions GetJsonSerializerOptions()
        {
#if NETCOREAPP3_1 || NET5_0
            var writeOptions = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            writeOptions.Converters.Add( new JsonConverterJsonDocument() );
#elif NET6_0
            var writeOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
#endif
            return writeOptions;
        }

        public static async void SaveRules( string path, List<RuleTemplate> ruleList )
        {
            await File.WriteAllBytesAsync( path, JsonSerializer.SerializeToUtf8Bytes(
                ruleList, GetJsonSerializerOptions() ) );
        }

        #region WinApi

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool IsWindow( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        private static extern int GetWindowText( IntPtr hWnd, StringBuilder buf, int nMaxCount );

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        private static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

        [DllImport( "user32.dll" )]
        private static extern int GetWindowThreadProcessId( IntPtr hWnd, out int processId );

        #endregion
    }

#if NETCOREAPP3_1 || NET5_0
    internal sealed class JsonConverterJsonDocument : JsonConverter<JsonDocument>
    {
        public override JsonDocument Read( ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options )
        {
            return JsonDocument.ParseValue( ref reader );
        }

        public override void Write( Utf8JsonWriter writer, JsonDocument value, JsonSerializerOptions options )
        {
            value.RootElement.WriteTo( writer );
        }
    }
#endif
}