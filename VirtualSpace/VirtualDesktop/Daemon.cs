/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Globalization;
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
        private static          int               _runlevel      = 1;
        private static readonly ManualResetEvent  CanRun         = new( false );
        private static readonly StringBuilder     SbWinText      = new( Const.WindowTitleMaxLength );
        private static readonly StringBuilder     SbCName        = new( Const.WindowClassMaxLength );
        private static readonly Channel<Behavior> ActionChannel  = Channels.ActionChannel;
        private static readonly Channel<Window>   VisibleWindows = Conditions.VisibleWindows;

        private static async void WaitForAction()
        {
            while ( await ActionChannel.Reader.WaitToReadAsync() )
            {
                if ( !ActionChannel.Reader.TryRead( out var action ) ) continue;

                if ( action.HideFromView )
                {
                    Logger.Debug( $"[RULE]HIDE.Win {action.Handle.ToString( "X2" )}" );
                    Filters.WndHandleIgnoreListByManual.Add( action.Handle );
                }

                if ( action.MoveToScreen >= 0 )
                {
                    Logger.Debug( $"[RULE]MOVE_TO_SCREEN.Win {action.Handle.ToString( "X2" )} TO Screen[{action.MoveToScreen.ToString()}]" );
                    WindowTool.MoveWindowToScreen( action.Handle, action.MoveToScreen );
                }

                if ( action.PinApp )
                {
                    Logger.Debug( $"[RULE]PIN.App of {action.Handle.ToString( "X2" )} TO All Desktops" );
                    try
                    {
                        DesktopWrapper.PinApp( action.Handle, false );
                    }
                    catch
                    {
                        Logger.Error( $"[RULE]PIN.Win {action.Handle.ToString( "X2" )} Failed" );
                    }

                    continue; // <- if PinApp, then PinWindow & MoveToDesktop is invalid
                }

                if ( action.PinWindow )
                {
                    Logger.Debug( $"[RULE]PIN.Win {action.Handle.ToString( "X2" )} TO All Desktops" );
                    try
                    {
                        DesktopWrapper.PinWindow( action.Handle, false );
                    }
                    catch
                    {
                        Logger.Error( $"[RULE]PIN.Win {action.Handle.ToString( "X2" )} Failed" );
                    }

                    continue; // <- if PinWindow, then MoveToDesktop is invalid
                }

                if ( action.MoveToDesktop >= 0 )
                {
                    try
                    {
                        DesktopWrapper.MoveWindowToDesktop( action.Handle, action.MoveToDesktop );
                        Logger.Debug( $"[RULE]MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop.ToString()}]" );
                        if ( action.FollowWindow )
                        {
                            Logger.Debug( $"[RULE]CHANGE CURRENT DESKTOP TO Desktop[{action.MoveToDesktop.ToString()}]" );
                            DesktopWrapper.MakeVisibleByIndex( action.MoveToDesktop );
                            User32.SwitchToThisWindow( action.Handle, true );
                        }
                    }
                    catch
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
                        Logger.Error(
                            $"[RULE]ERROR.MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop.ToString()}]",
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
                while ( true )
                {
                    CanRun.WaitOne();
                    _ = User32.EnumWindows( WindowHandleFilter, 0 );
                    Logger.Debug( "Daemon one turn done." );
                    Thread.Sleep( _runlevel * 1000 );
                }
            }, TaskCreationOptions.LongRunning );
        }

        private static bool WindowHandleFilter( IntPtr hWnd, int lParam )
        {
            _ = User32.GetWindowText( hWnd, SbWinText, SbWinText.Capacity );
            var title = SbWinText.ToString();

            _ = User32.GetClassName( hWnd, SbCName, SbCName.Capacity );
            var classname = SbCName.ToString();

            if ( User32.IsWindowVisible( hWnd ) &&
                 !string.IsNullOrEmpty( title ) &&
                 !Filters.WndClsIgnoreList.Contains( classname ) &&
                 !Filters.WndTitleIgnoreList.Contains( title ) &&
                 !Filters.WndHandleIgnoreListByError.Contains( hWnd ) &&
                 !Conditions.WndHandleIgnoreListByRule.Contains( hWnd ) &&
                 !Filters.IsCloaked( hWnd )
               )
            {
                if ( classname != Const.WindowsUiCoreWindow )
                {
                    AddToSnapshot( hWnd, title, classname );
                }
            }

            return true;
        }

        private static void AddToSnapshot( IntPtr hWnd, string title, string classname )
        {
            VisibleWindows.Writer.TryWrite( new Window {Title = title, WndClass = classname, Handle = hWnd} );
        }
    }
}