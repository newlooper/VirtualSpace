/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Helpers;
using VirtualSpace.Tools;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

internal static class Daemon
{
    private static          int               _runlevel = 1;
    private static readonly ManualResetEvent  CanRun    = new( false );
    private static readonly ManualResetEvent  StopEvent = new( false );
    private static          Task?             _daemonTask;
    private static readonly char[]            WinInfoBuffer          = new char[Const.WindowTitleMaxLength];
    private static readonly Channel<Behavior> ActionConsumer         = Channels.ActionChannel;
    private static readonly Channel<Window>   VisibleWindowsProducer = Channels.VisibleWindowsChannel;

    private static async void WaitForAction()
    {
        while ( await ActionConsumer.Reader.WaitToReadAsync() )
        {
            if ( !ActionConsumer.Reader.TryRead( out var action ) )
            {
                continue;
            }

            if ( action.HideFromView )
            {
                Logger.Debug( $"[RULE.Action]HIDE.Win {action.Handle:X2}" );
                ImmutableInterlocked.Update( ref Filters.WndHandleIgnoreListByManual, list => list.Add( action.Handle ) );
            }

            if ( action.MoveToScreen >= 0 )
            {
                Logger.Debug( $"[RULE.Action]MOVE_TO_SCREEN.Win {action.Handle:X2} TO Screen[{action.MoveToScreen}]" );
                WindowTool.MoveWindowToScreen( action.Handle, action.MoveToScreen );
            }

            if ( action.PinApp )
            {
                Logger.Debug( $"[RULE.Action]PIN.App of {action.Handle:X2} TO All Desktops" );
                try
                {
                    DesktopWrapper.PinApp( action.Handle, false );
                }
                catch
                {
                    Logger.Error( $"[RULE.Action]PIN.App {action.Handle:X2} Failed" );
                }

                continue; // <- if PinApp, then PinWindow & MoveToDesktop is invalid
            }

            if ( action.PinWindow )
            {
                Logger.Debug( $"[RULE.Action]PIN.Win {action.Handle:X2} TO All Desktops" );
                try
                {
                    DesktopWrapper.PinWindow( action.Handle, false );
                }
                catch
                {
                    Logger.Error( $"[RULE.Action]PIN.Win {action.Handle:X2} Failed" );
                }

                continue; // <- if PinWindow, then MoveToDesktop is invalid
            }

            if ( action.MoveToDesktop < 0 )
            {
                continue;
            }

            try
            {
                Logger.Debug( $"[RULE.Action]MOVE.Win {action.Handle:X2} TO Desktop[{action.MoveToDesktop}]" );
                DesktopWrapper.MoveWindowToDesktop( action.Handle, action.MoveToDesktop );
                if ( action.FollowWindow )
                {
                    WindowTool.ActivateWindow( action.Handle, action.MoveToDesktop );
                }
            }
            catch
            {
                CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
                Logger.Error(
                    $"[RULE.Action]MOVE.Win {action.Handle:X2} TO Desktop[{action.MoveToDesktop}]",
                    new NotifyObject
                    {
                        Title   = Agent.Langs.GetString( "Error.Title" )!,
                        Message = string.Format( Agent.Langs.GetString( "Error.MoveWindowToDesktop" )!, action.WindowTitle, action.RuleName )
                    } );
            }
        }
    }

    public static async void Start()
    {
        WaitForAction();
        StartDaemon();
        if ( !ConfigManager.CurrentProfile.DaemonAutoStart )
        {
            return;
        }

        if ( ConfigManager.CurrentProfile.DaemonAutoStartDelay > 0 )
        {
            await Task.Delay( ConfigManager.CurrentProfile.DaemonAutoStartDelay * Const.OneSecond );
        }

        CanRun.Set();
    }

    public static void SetRunLevel( int i )
    {
        _runlevel = i < 1 ? 1 : i;
    }

    public static void Stop()
    {
        var task = _daemonTask;
        if ( task is null || task.IsCompleted )
        {
            return;
        }

        StopEvent.Set();
        CanRun.Set();

        try
        {
            if ( !task.Wait( TimeSpan.FromSeconds( 1 ) ) )
            {
                Logger.Warning( "Daemon shutdown timed out." );
            }
        }
        catch ( Exception ex )
        {
            Logger.Warning( $"Daemon shutdown failed: {ex.Message}" );
        }
    }

    private static void StartDaemon()
    {
        StopEvent.Reset();
        _daemonTask = Task.Factory.StartNew( () =>
        {
            var sw          = Stopwatch.StartNew();
            var waitHandles = new WaitHandle[] { StopEvent, CanRun };

            while ( true )
            {
                if ( WaitHandle.WaitAny( waitHandles ) == 0 )
                {
                    return;
                }

                if ( sw.ElapsedMilliseconds >= Const.OneMinute )
                {
                    _ = User32.EnumWindows( CleanIgnoreList, 0 );
                    Logger.Debug( "Daemon running normally in last minute." );
                    sw.Restart();
                }
                else
                {
                    _ = User32.EnumWindows( WindowRuleFilter, 0 );
                }

                if ( WaitHandle.WaitAny( new WaitHandle[] { StopEvent }, _runlevel * Const.OneSecond ) == 0 )
                {
                    return;
                }
            }
        }, TaskCreationOptions.LongRunning );
    }

    private static bool WindowRuleFilter( IntPtr hWnd, int lParam )
    {
        if ( Conditions.WndHandleIgnoreListByRule.Contains( hWnd ) ||
             Filters.WndHandleIgnoreListByError.Contains( hWnd ) ||
             !User32.IsWindowVisible( hWnd ) ||
             Filters.IsCloaked( hWnd ) )
        {
            return true;
        }

        var titleLen = User32.GetWindowText( hWnd, WinInfoBuffer, WinInfoBuffer.Length );
        var title    = titleLen <= 0 ? string.Empty : new string( WinInfoBuffer, 0, titleLen );
        if ( string.IsNullOrEmpty( title ) ||
             Filters.WndTitleIgnoreList.Contains( title ) )
        {
            return true;
        }

        var classLen  = User32.GetClassName( hWnd, WinInfoBuffer, WinInfoBuffer.Length );
        var classname = classLen <= 0 ? string.Empty : new string( WinInfoBuffer, 0, classLen );
        if ( Filters.WndClsIgnoreList.Contains( classname ) )
        {
            return true;
        }

        switch ( classname )
        {
            case "#32770" when WindowTool.IsModalWindow( hWnd ):
            case "Chrome_WidgetWin_1" or "MozillaDropShadowWindowClass" when WindowTool.IsPopupToolWindow( hWnd ):
                return true;
        }

        if ( classname != Const.WindowsUiCoreWindow )
        {
            SendToCheckingRule( hWnd, title, classname );
        }

        return true;
    }

    private static void SendToCheckingRule( IntPtr hWnd, string title, string classname )
    {
        VisibleWindowsProducer.Writer.TryWrite( new Window { Title = title, WndClass = classname, Handle = hWnd } );
    }

    /// <summary>
    ///     概要：非可靠的清理窗口规则忽略列表的办法，可满足日常使用。
    ///     原有问题：对于窗口规则重度使用场景，无论因何种原因导致已销毁窗口的 HWND 被新窗口复用，若该 HWND 在忽略列表中，将无法再进行窗口规则检查。
    ///     现有方案：尽量保持原有机制且不增加资源占用的前提下，大幅降低上述问题的概率
    ///     成立前提：借助 _daemonTask 中的 Const.OneMinute 心跳，期望的是“在某窗口被销毁的一分钟内，不会有相同 HWND 的窗口被创建出来”。
    ///     额外说明：最可靠的是 HWND+PID+TIMESTAMP 做键，但代价过于高昂，因此保留通过 HWND 判断的机制。
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lParam"></param>
    private static bool CleanIgnoreList( IntPtr hWnd, int lParam )
    {
        if ( User32.IsWindow( hWnd ) )
        {
            return true;
        }

        ImmutableInterlocked.Update( ref Conditions.WndHandleIgnoreListByRule, list => list.Remove( hWnd ) );
        ImmutableInterlocked.Update( ref Filters.WndHandleIgnoreListByError, list => list.Remove( hWnd ) );

        return true;
    }
}