/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Channels;
using System.Windows.Media;
using Notification.Wpf;
using VirtualDesktop;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopManagerWrapper
    {
        private static readonly Channel<VirtualDesktopNotification> VirtualDesktopNotifications = Channels.VirtualDesktopNotifications;

        public static void RegisterVirtualDesktopEvents()
        {
            DesktopManager.Created += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification {Type = VirtualDesktopNotificationType.CREATED} );
            };

            DesktopManager.Destroyed += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.DELETED,
                    NewId = e.Fallback.GetId()
                } );
            };

            DesktopManager.CurrentChanged += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.CURRENT_CHANGED,
                    NewId = e.NewDesktop.GetId(),
                    OldId = e.OldDesktop.GetId()
                } );
            };

            DesktopManager.WallpaperChanged += ( _, e ) =>
            {
                if ( string.IsNullOrEmpty( e.Path ) ) return;
                var guid    = e.Desktop.GetId();
                var vdwList = VirtualDesktopManager.GetAllVirtualDesktops();

                var vd = ( from vdw in vdwList where vdw.VdId == guid select vdw ).FirstOrDefault();
                if ( vd is null ) return;
                vd.SetBackground( WinRegistry.GetWallpaperByDesktopGuid( guid, vd.Width, vd.Height, ConfigManager.GetCachePath() ) );
                Logger.Event( $"Desktop[{vd.VdIndex.ToString()}] Wallpaper Changed: {e.Path}" );
            };
        }

        public static async void ListenVirtualDesktopEvents()
        {
            while ( await VirtualDesktopNotifications.Reader.WaitToReadAsync() )
            {
                if ( VirtualDesktopNotifications.Reader.TryRead( out var vdn ) )
                {
                    switch ( vdn.Type )
                    {
                        case VirtualDesktopNotificationType.CREATED:
                            if ( !VirtualDesktopManager.IsBatchCreate )
                            {
                                if ( MainWindow.IsShowing() )
                                    VirtualDesktopManager.UpdateMainView();
                            }

                            break;
                        case VirtualDesktopNotificationType.DELETED:
                            if ( MainWindow.IsShowing() )
                            {
                                VirtualDesktopManager.UpdateMainView( vdn );
                            }

                            break;
                        case VirtualDesktopNotificationType.CURRENT_CHANGED:
                            if ( MainWindow.IsShowing() )
                                VirtualDesktopManager.UpdateVdwBackground();

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

                            if ( ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon )
                            {
                                MainWindow.UpdateVDIndexOnTrayIcon( vdn.NewId );
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}