/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static class Daemon
    {
        private static          int               _runlevel              = 1;
        private static readonly ManualResetEvent  CanRun                 = new( false );
        private static readonly StringBuilder     SbWinInfo              = new( Const.WindowTitleMaxLength );
        private static readonly Channel<Behavior> ActionConsumer         = Channels.ActionChannel;
        private static readonly Channel<Window>   VisibleWindowsProducer = Channels.VisibleWindowsChannel;

        private static async void WaitForAction()
        {
            while ( await ActionConsumer.Reader.WaitToReadAsync() )
            {
                if ( !ActionConsumer.Reader.TryRead( out var action ) ) continue;

                if ( action.HideFromView )
                {
                    Logger.Debug( $"[RULE.Action]HIDE.Win {action.Handle.ToString( "X2" )}" );
                    Filters.WndHandleIgnoreListByManual.TryAdd( action.Handle, 0 );
                }

                if ( action.MoveToScreen >= 0 )
                {
                    Logger.Debug( $"[RULE.Action]MOVE_TO_SCREEN.Win {action.Handle.ToString( "X2" )} TO Screen[{action.MoveToScreen.ToString()}]" );
                    WindowTool.MoveWindowToScreen( action.Handle, action.MoveToScreen );
                }

                if ( action.PinApp )
                {
                    Logger.Debug( $"[RULE.Action]PIN.App of {action.Handle.ToString( "X2" )} TO All Desktops" );
                    try
                    {
                        DesktopWrapper.PinApp( action.Handle, false );
                    }
                    catch
                    {
                        Logger.Error( $"[RULE.Action]PIN.App {action.Handle.ToString( "X2" )} Failed" );
                    }

                    continue; // <- if PinApp, then PinWindow & MoveToDesktop is invalid
                }

                if ( action.PinWindow )
                {
                    Logger.Debug( $"[RULE.Action]PIN.Win {action.Handle.ToString( "X2" )} TO All Desktops" );
                    try
                    {
                        DesktopWrapper.PinWindow( action.Handle, false );
                    }
                    catch
                    {
                        Logger.Error( $"[RULE.Action]PIN.Win {action.Handle.ToString( "X2" )} Failed" );
                    }

                    continue; // <- if PinWindow, then MoveToDesktop is invalid
                }

                if ( action.MoveToDesktop >= 0 )
                {
                    try
                    {
                        DesktopWrapper.MoveWindowToDesktop( action.Handle, action.MoveToDesktop );
                        Logger.Debug( $"[RULE.Action]MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop.ToString()}]" );
                        if ( action.FollowWindow )
                        {
                            Logger.Debug( $"[RULE.Action]CHANGE CURRENT DESKTOP TO Desktop[{action.MoveToDesktop.ToString()}]" );
                            DesktopWrapper.MakeVisibleByIndex( action.MoveToDesktop );
                            WindowTool.ActiveWindow( action.Handle );
                        }
                    }
                    catch
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
                        Logger.Error(
                            $"[RULE.Action]MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop.ToString()}]",
                            new NotifyObject
                            {
                                Title = Agent.Langs.GetString( "Error.Title" ),
                                Message = string.Format( Agent.Langs.GetString( "Error.MoveWindowToDesktop" ), action.WindowTitle, action.RuleName )
                            } );
                    }
                }
            }
        }

        public static void Start()
        {
            WaitForAction();
            StartDaemon();
            if ( ConfigManager.CurrentProfile.DaemonAutoStart )
                CanRun.Set();
        }

        public static void SetCanRun( bool isCanRun )
        {
            if ( isCanRun )
            {
                CanRun.Set();
            }
            else
            {
                CanRun.Reset();
            }
        }

        public static void SetRunLevel( int i )
        {
            _runlevel = i < 1 ? 1 : i;
        }

        private static void StartDaemon()
        {
            Task.Factory.StartNew( () =>
            {
                var sw = Stopwatch.StartNew();
                while ( true )
                {
                    CanRun.WaitOne();
                    _ = User32.EnumWindows( WindowHandleFilter, 0 );
                    if ( sw.ElapsedMilliseconds >= Const.OneMinute )
                    {
                        Logger.Debug( "Daemon running normally in last minute." );
                        sw.Restart();
                    }

                    Thread.Sleep( _runlevel * Const.OneSecond );
                }
            }, TaskCreationOptions.LongRunning );
        }

        private static bool WindowHandleFilter( IntPtr hWnd, int lParam )
        {
            if ( Conditions.WndHandleIgnoreListByRule.Contains( hWnd ) ||
                 Filters.WndHandleIgnoreListByError.Contains( hWnd ) ||
                 !User32.IsWindowVisible( hWnd ) ||
                 Filters.IsCloaked( hWnd ) )
                return true;

            _ = User32.GetWindowText( hWnd, SbWinInfo, SbWinInfo.Capacity );
            var title = SbWinInfo.ToString();
            if ( string.IsNullOrEmpty( title ) ||
                 Filters.WndTitleIgnoreList.Contains( title ) )
                return true;

            _ = User32.GetClassName( hWnd, SbWinInfo, SbWinInfo.Capacity );
            var classname = SbWinInfo.ToString();
            if ( Filters.WndClsIgnoreList.Contains( classname ) )
                return true;

            if ( classname != Const.WindowsUiCoreWindow )
            {
                SendToCheckingRule( hWnd, title, classname );
            }

            return true;
        }

        private static void SendToCheckingRule( IntPtr hWnd, string title, string classname )
        {
            VisibleWindowsProducer.Writer.TryWrite( new Window {Title = title, WndClass = classname, Handle = hWnd} );
        }
    }
}