/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading.Channels;
using VirtualDesktop;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;

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
                    TargetId = e.Fallback.GetId()
                } );
            };

            DesktopManager.CurrentChanged += ( _, e ) =>
            {
                VirtualDesktopNotifications.Writer.TryWrite( new VirtualDesktopNotification
                {
                    Type = VirtualDesktopNotificationType.CURRENT_CHANGED,
                    TargetId = e.NewDesktop.GetId()
                } );
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
                            if ( MainWindow.IsShowing() )
                            {
                                if ( !VirtualDesktopManager.IsBatchCreate ) VirtualDesktopManager.ResetLayout();
                            }

                            break;
                        case VirtualDesktopNotificationType.DELETED:
                            if ( MainWindow.IsShowing() )
                            {
                                VirtualDesktopManager.FixLayout();
                                VirtualDesktopManager.ShowAllVirtualDesktops();
                                if ( VirtualDesktopManager.NeedRepaintThumbs )
                                {
                                    VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                                    VirtualDesktopManager.NeedRepaintThumbs = false;
                                }
                                else
                                {
                                    var vdwList = VirtualDesktopManager.GetAllVirtualDesktops();
                                    try
                                    {
                                        var fallback = vdwList[DesktopWrapper.IndexFromGuid( vdn.TargetId )];
                                        VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {fallback} );
                                    }
                                    catch ( Exception e )
                                    {
                                        Logger.Warning( e.StackTrace );
                                    }
                                }
                            }

                            break;
                        case VirtualDesktopNotificationType.CURRENT_CHANGED:
                            if ( MainWindow.IsShowing() )
                                VirtualDesktopManager.ResetAllBackground();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}