// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualDesktop;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopWrapper
    {
        public static int  CurrentIndex => Desktop.FromDesktop( Desktop.Current );
        public static int  Count        => Desktop.Count;
        public static Guid CurrentGuid  => Desktop.Current.Guid;

        public static bool RemoveDesktopByIndex( int vdIndex )
        {
            if ( Count <= 1 ) return false;
            try
            {
                var desktop = FromIndex( vdIndex );
                desktop.Remove( null );
                return true;
            }
            catch ( Exception e )
            {
                Logger.Error( "Remove Desktop: " + e.Message );
                return false;
            }
        }

        public static bool RemoveDesktopByGuid( Guid guid )
        {
            if ( Count <= 1 ) return false;
            try
            {
                var desktop = DesktopFromId( guid );
                desktop.Remove( null );
                return true;
            }
            catch ( Exception e )
            {
                Logger.Error( "Remove Desktop: " + e.Message );
                return false;
            }
        }

        public static void PinWindow( IntPtr handle, bool isPinned )
        {
            if ( isPinned )
                Desktop.UnpinWindow( handle );
            else
                Desktop.PinWindow( handle );
        }

        public static void PinApp( IntPtr handle, bool isPinned )
        {
            if ( isPinned )
                Desktop.UnpinApplication( handle );
            else
                Desktop.PinApplication( handle );
        }

        public static int DesktopIndexByWindow( IntPtr handle )
        {
            var desktop = Desktop.FromWindow( handle );
            return Desktop.FromDesktop( desktop );
        }

        public static bool IsWindowPinned( IntPtr handle )
        {
            return Desktop.IsWindowPinned( handle );
        }

        public static bool IsApplicationPinned( IntPtr handle )
        {
            return Desktop.IsApplicationPinned( handle );
        }

        public static string DesktopNameFromIndex( int vdIndex )
        {
            return Desktop.DesktopNameFromIndex( vdIndex );
        }

        public static string DesktopNameFromGuid( Guid guid )
        {
            var desktop = Desktop.GetDesktopById( guid );
            if ( desktop == null )
            {
                return "";
            }

            return Desktop.DesktopNameFromDesktop( desktop );
        }

        public static Desktop? DesktopFromId( Guid guid )
        {
            return Desktop.GetDesktopById( guid );
        }

        public static int IndexFromGuid( Guid guid )
        {
            return Desktop.FromDesktop( DesktopFromId( guid ) );
        }

        public static void MoveWindowToDesktop( IntPtr handle, int vdIndex )
        {
            var desktop = FromIndex( vdIndex );
            desktop.MoveWindow( handle );
        }

        public static void MakeVisibleByIndex( int vdIndex )
        {
            var desktop = FromIndex( vdIndex );
            desktop.MakeVisible();
        }

        public static void MakeVisibleByGuid( Guid guid, bool? forceFocusForegroundWindow = null )
        {
            var desktop = DesktopFromId( guid );
            if ( desktop is null ) return;

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

                    if ( SysInfo.IsAdministrator() )
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
        }

        public static void SetNameByIndex( int vdIndex, string name )
        {
            var desktop = FromIndex( vdIndex );
            desktop.SetName( name );
        }

        public static void SetNameByGuid( Guid guid, string name )
        {
            var desktop = DesktopFromId( guid );
            desktop.SetName( name );
        }

        private static Desktop FromIndex( int vdIndex )
        {
            return Desktop.FromIndex( vdIndex );
        }

        public static Desktop FromWindow( IntPtr handle )
        {
            return Desktop.FromWindow( handle );
        }
    }
}