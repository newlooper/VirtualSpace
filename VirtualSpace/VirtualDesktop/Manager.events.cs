/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Notification.Wpf;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        public static void RegisterVirtualDesktopEvents()
        {
            DesktopManagerWrapper.DesktopCreatedEvent += () =>
            {
                if ( !IsBatchCreate ) UpdateMainView();
            };
            DesktopManagerWrapper.DesktopDeletedEvent += vdn => { UpdateMainView( vdn: vdn ); };
            DesktopManagerWrapper.DesktopChangedEvent += vdn =>
            {
                LastDesktopId = vdn.OldId;
                if ( MainWindow.IsShowing() )
                    UpdateVdwBackground();

                if ( ConfigManager.Configs.Cluster.NotificationOnVdChanged )
                {
                    CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
                    Logger.Notify( new NotifyObject
                    {
                        Title = Agent.Langs.GetString( "Cluster.Notification.SVD.Current" ) + DesktopWrapper.DesktopNameFromGuid( vdn.NewId ),
                        Message = Agent.Langs.GetString( "Cluster.Notification.SVD.Last" ) + DesktopWrapper.DesktopNameFromGuid( vdn.OldId ),
                        Background = new SolidColorBrush( Colors.DarkSlateGray ),
                        Foreground = new SolidColorBrush( Colors.White ),
                        Type = NotificationType.Notification,
                        ExpTime = TimeSpan.FromSeconds( 3 )
                    } );
                }

                MainWindow.UpdateVDIndexOnTrayIcon( vdn.NewId );
            };

            DesktopManagerWrapper.RegisterVirtualDesktopEvents(
                () =>
                {
                    Logger.Event( $"Wallpaper Changed" );
                    Parallel.ForEach( GetAllVirtualDesktops(), ( vdw, _ ) => { vdw.UpdateWallpaper(); } );
                },
                ( guid, path ) =>
                {
                    var vdwList = GetAllVirtualDesktops();
                    var vd      = ( from vdw in vdwList where vdw.VdId == guid select vdw ).FirstOrDefault();
                    if ( vd is null ) return;
                    vd.UpdateWallpaper();
                    Logger.Event( $"Desktop[{vd.VdIndex.ToString()}] Wallpaper Changed: {path}" );
                }
            );

            DesktopWrapper.OnDesktopVisibleEvent += ( desktop, forceFocusForegroundWindow ) =>
            {
                if ( MainWindow.IsShowing() )
                {
                    desktop.MakeVisible();
                    return;
                }

                forceFocusForegroundWindow ??= Manager.Configs.Cluster.ForceFocusForegroundWindow;
                if ( (bool)forceFocusForegroundWindow )
                {
                    var hTaskBar = User32.FindWindow( Const.TaskbarWndClass, "" );
                    if ( hTaskBar == IntPtr.Zero )
                    {
                        Logger.Verbose( "Taskbar not found, switch desktop only." );
                        desktop.MakeVisible();
                        return;
                    }

                    if ( SysInfo.IsTaskbarVisible() )
                    {
                        User32.SetForegroundWindow( hTaskBar );
                        desktop.MakeVisible();

                        if ( User32.GetForegroundWindow() != hTaskBar )
                        {
                            Logger.Verbose( "Taskbar not active, switch desktop only." );
                            return;
                        }

                        if ( SysInfo.IsAdministrator )
                        {
                            Logger.Verbose( "Send [Alt+Esc]." );
                            LowLevelKeyboardHook.MultipleKeyPress( new List<Keys> {Keys.Menu, Keys.Escape} );
                        }
                        else
                        {
                            Logger.Verbose( "Force minimize taskbar." );
                            _ = User32.ShowWindow( hTaskBar, (short)ShowState.SW_FORCEMINIMIZE );
                        }
                    }
                    else
                    {
                        Logger.Verbose( "Taskbar is hiding, switch desktop only." );
                        desktop.MakeVisible();
                    }
                }
                else
                {
                    desktop.MakeVisible();
                }
            };
        }
    }
}