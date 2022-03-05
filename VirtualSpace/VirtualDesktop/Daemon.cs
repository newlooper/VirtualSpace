/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
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
        private static          bool              _initialized;
        private static          int               _runlevel = 1;
        private static          IntPtr            _isVisibleCoreWindow;
        private static readonly List<IntPtr>      WndHandleSnapshot = new();
        private static readonly ManualResetEvent  CanRun            = new( false );
        private static readonly StringBuilder     SbWinText         = new( Const.WindowTitleMaxLength );
        private static readonly StringBuilder     SbCName           = new( Const.WindowClassMaxLength );
        private static readonly Channel<Behavior> ActionChannel     = Channels.ActionChannel;

        private static async void WaitForAction()
        {
            while ( await ActionChannel.Reader.WaitToReadAsync() )
            {
                if ( ActionChannel.Reader.TryRead( out var action ) )
                {
                    if ( action.MoveToDesktop >= 0 )
                    {
                        try
                        {
                            DesktopWrapper.MoveWindowToDesktop( action.Handle, action.MoveToDesktop );
                            Logger.Debug( $"MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop}]" );
                            if ( action.FollowWindow )
                            {
                                Logger.Debug( $"CHANGE CURRENT DESKTOP TO Desktop[{action.MoveToDesktop}]" );
                                DesktopWrapper.MakeVisibleByIndex( action.MoveToDesktop );
                                User32.SwitchToThisWindow( action.Handle, true );
                            }
                        }
                        catch
                        {
                            CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.GetCurrentProfile().UI.Language );
                            Logger.Error(
                                $"ERROR.MOVE.Win {action.Handle.ToString( "X2" )} TO Desktop[{action.MoveToDesktop}]",
                                new NotifyObject
                                {
                                    Title = Agent.Langs.GetString( "Error.Title" ),
                                    Message = string.Format( Agent.Langs.GetString( "Error.MoveWindowToDesktop" ), action.WindowTitle, action.RuleName )
                                } );
                            continue;
                        }
                    }

                    if ( action.PinApp )
                    {
                        Logger.Debug( $"PIN.App of {action.Handle.ToString( "X2" )} TO All Desktops" );
                        DesktopWrapper.PinApp( action.Handle, false );
                        break;
                    }

                    if ( action.PinWindow )
                    {
                        Logger.Debug( $"PIN.Win {action.Handle.ToString( "X2" )} TO All Desktops" );
                        DesktopWrapper.PinWindow( action.Handle, false );
                    }
                }
            }
        }

        public static void Start()
        {
            WaitForAction();
            User32.EnumWindows( WindowHandleFilter, 0 );
            _initialized = true;
            StartDaemon();
            if ( ConfigManager.GetCurrentProfile().DaemonAutoStart )
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

        private static async void StartDaemon()
        {
            await Task.Run( () =>
                {
                    while ( true )
                    {
                        CanRun.WaitOne();
                        User32.EnumWindows( WindowHandleFilter, 0 );
                        Logger.Debug( "Daemon one turn done." );
                        Thread.Sleep( _runlevel * 1000 );
                    }
                }
            );
        }

        private static bool WindowHandleFilter( IntPtr hWnd, int lParam )
        {
            User32.GetWindowText( hWnd, SbWinText, SbWinText.Capacity );
            var title = SbWinText.ToString();

            User32.GetClassName( hWnd, SbCName, SbCName.Capacity );
            var classname = SbCName.ToString();

            if ( User32.IsWindowVisible( hWnd ) &&
                 !string.IsNullOrEmpty( title ) &&
                 !Filters.WndClsIgnoreList.Contains( classname ) &&
                 !Filters.WndTitleIgnoreList.Contains( title ) &&
                 !Filters.WndHandleIgnoreList.Contains( hWnd ) &&
                 !Filters.WndHandleManualIgnoreList.Contains( hWnd ) &&
                 !Filters.IsCloaked( hWnd )
               )
            {
                if ( classname == Const.ApplicationFrameWindow )
                {
                    User32.EnumChildWindows( hWnd, ChildWindowHandleFilter, 0 );
                    if ( _isVisibleCoreWindow != default )
                    {
                        _isVisibleCoreWindow = default;
                        AddToSnapshot( hWnd );
                    }
                }
                else
                {
                    AddToSnapshot( hWnd );
                }
            }

            return true;
        }

        private static bool ChildWindowHandleFilter( IntPtr hWnd, int lParam )
        {
            var sbCName = new StringBuilder( Const.WindowClassMaxLength );
            User32.GetClassName( hWnd, sbCName, sbCName.Capacity );
            var classname = sbCName.ToString();
            if ( classname == Const.WindowsUiCoreWindow && _isVisibleCoreWindow == default )
                _isVisibleCoreWindow = hWnd;
            return true;
        }

        private static void AddToSnapshot( IntPtr hWnd )
        {
            if ( WndHandleSnapshot.Contains( hWnd ) ) return;
            WndHandleSnapshot.Add( hWnd );
            if ( _initialized )
            {
                Conditions.CheckWindow( new Window {Handle = hWnd} );
            }
        }
    }
}