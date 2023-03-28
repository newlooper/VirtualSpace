/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

extern alias VirtualDesktop10;
extern alias VirtualDesktop11;
using System.Threading.Channels;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using VD10 = VirtualDesktop10::VirtualDesktop;
using VD11 = VirtualDesktop11::VirtualDesktop;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopManagerWrapper
    {
        private static readonly Channel<VirtualDesktopNotification> VirtualDesktopNotifications = Channels.VirtualDesktopNotifications;

        public static void RegisterVirtualDesktopEvents( WallpaperChanged wc10, Action<Guid, string> wc11 )
        {
            if ( SysInfo.IsWin10 )
            {
                RegisterVirtualDesktopEvents10( wc10 );
            }
            else
            {
                RegisterVirtualDesktopEvents11( wc11 );
            }
        }

        private static void RegisterVirtualDesktopEvents10( WallpaperChanged wc )
        {
            VD10.DesktopManager.Created += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification {Type = VirtualDesktopNotificationType.CREATED} );
            };

            VD10.DesktopManager.Destroyed += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.DELETED,
                    NewId = e.Fallback.GetId()
                } );
            };

            VD10.DesktopManager.CurrentChanged += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.CURRENT_CHANGED,
                    NewId = e.NewDesktop.GetId(),
                    OldId = e.OldDesktop.GetId()
                } );
            };

            WatchWallpaperEvents( wc );
        }

        private static void RegisterVirtualDesktopEvents11( Action<Guid, string> wc11 )
        {
            VD11.DesktopManager.Created += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification {Type = VirtualDesktopNotificationType.CREATED} );
            };

            VD11.DesktopManager.Destroyed += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.DELETED,
                    NewId = e.Fallback.GetId()
                } );
            };

            VD11.DesktopManager.CurrentChanged += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.CURRENT_CHANGED,
                    NewId = e.NewDesktop.GetId(),
                    OldId = e.OldDesktop.GetId()
                } );
            };

            VD11.DesktopManager.WallpaperChanged += ( _, e ) =>
            {
                if ( string.IsNullOrEmpty( e.Path ) ) return;
                wc11( e.Desktop.GetId(), e.Path );
            };
        }

        public delegate void DesktopCreated();

        public delegate void DesktopDeleted( VirtualDesktopNotification vdn );

        public delegate void DesktopChanged( VirtualDesktopNotification vdn );

        public static event DesktopCreated DesktopCreatedEvent;
        public static event DesktopDeleted DesktopDeletedEvent;
        public static event DesktopChanged DesktopChangedEvent;

        public static async void ListenVirtualDesktopEvents()
        {
            while ( await VirtualDesktopNotifications.Reader.WaitToReadAsync() )
            {
                if ( VirtualDesktopNotifications.Reader.TryRead( out var vdn ) )
                {
                    switch ( vdn.Type )
                    {
                        case VirtualDesktopNotificationType.CREATED:
                            DesktopCreatedEvent();

                            break;
                        case VirtualDesktopNotificationType.DELETED:
                            DesktopDeletedEvent( vdn );

                            break;
                        case VirtualDesktopNotificationType.CURRENT_CHANGED:
                            DesktopChangedEvent( vdn );

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}