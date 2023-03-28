// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

extern alias VirtualDesktop10;
extern alias VirtualDesktop11;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using VD10 = VirtualDesktop10::VirtualDesktop;
using VD11 = VirtualDesktop11::VirtualDesktop;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopWrapper
    {
        public delegate void OnDesktopVisible( IDesktop desktop, bool? forceFocusForegroundWindow = null );

        public static int CurrentIndex => SysInfo.IsWin10 ? VD10.Desktop.SysIndexFromDesktop( VD10.Desktop.Current ) : VD11.Desktop.SysIndexFromDesktop( VD11.Desktop.Current );

        public static int  Count       => SysInfo.IsWin10 ? VD10.Desktop.Count : VD11.Desktop.Count;
        public static Guid CurrentGuid => SysInfo.IsWin10 ? VD10.Desktop.Current.Guid : VD11.Desktop.Current.Guid;

        public static bool RemoveDesktopByGuid( Guid guid )
        {
            if ( Count <= 1 ) return false;
            try
            {
                if ( SysInfo.IsWin10 )
                {
                    var desktop = VD10.Desktop.FromId( guid );
                    desktop.Remove( null );
                }
                else
                {
                    var desktop = VD11.Desktop.FromId( guid );
                    desktop.Remove( null );
                }

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
            if ( SysInfo.IsWin10 )
            {
                if ( isPinned )
                    VD10.Desktop.UnpinWindow( handle );
                else
                    VD10.Desktop.PinWindow( handle );
            }
            else
            {
                if ( isPinned )
                    VD11.Desktop.UnpinWindow( handle );
                else
                    VD11.Desktop.PinWindow( handle );
            }
        }

        public static void PinApp( IntPtr handle, bool isPinned )
        {
            if ( SysInfo.IsWin10 )
            {
                if ( isPinned )
                    VD10.Desktop.UnpinApplication( handle );
                else
                    VD10.Desktop.PinApplication( handle );
            }
            else
            {
                if ( isPinned )
                    VD11.Desktop.UnpinApplication( handle );
                else
                    VD11.Desktop.PinApplication( handle );
            }
        }

        public static bool IsWindowPinned( IntPtr handle )
        {
            return SysInfo.IsWin10 ? VD10.Desktop.IsWindowPinned( handle ) : VD11.Desktop.IsWindowPinned( handle );
        }

        public static bool IsApplicationPinned( IntPtr handle )
        {
            return SysInfo.IsWin10 ? VD10.Desktop.IsApplicationPinned( handle ) : VD11.Desktop.IsApplicationPinned( handle );
        }

        public static string DesktopNameFromIndex( int sysIndex )
        {
            return SysInfo.IsWin10 ? VD10.Desktop.DesktopNameFromIndex( sysIndex ) : VD11.Desktop.DesktopNameFromIndex( sysIndex );
        }

        public static string DesktopNameFromGuid( Guid guid )
        {
            if ( SysInfo.IsWin10 )
            {
                var desktop = VD10.Desktop.FromId( guid );
                return desktop == null ? "" : VD10.Desktop.DesktopNameFromDesktop( desktop );
            }
            else
            {
                var desktop = VD11.Desktop.FromId( guid );
                return desktop == null ? "" : VD11.Desktop.DesktopNameFromDesktop( desktop );
            }
        }

        public static int IndexFromGuid( Guid guid )
        {
            if ( SysInfo.IsWin10 )
            {
                var desktop = VD10.Desktop.FromId( guid );
                return VD10.Desktop.SysIndexFromDesktop( desktop );
            }
            else
            {
                var desktop = VD11.Desktop.FromId( guid );
                return VD11.Desktop.SysIndexFromDesktop( desktop );
            }
        }

        public static void MoveWindowToDesktop( IntPtr handle, int sysIndex )
        {
            if ( SysInfo.IsWin10 )
            {
                var desktop = VD10.Desktop.FromIndex( sysIndex );
                desktop.MoveWindow( handle );
            }
            else
            {
                var desktop = VD11.Desktop.FromIndex( sysIndex );
                desktop.MoveWindow( handle );
            }
        }

        public static void MakeVisibleByIndex( int sysIndex )
        {
            if ( SysInfo.IsWin10 )
            {
                VD10.Desktop.FromIndex( sysIndex ).MakeVisible();
            }
            else
            {
                VD11.Desktop.FromIndex( sysIndex ).MakeVisible();
            }
        }

        public static void MakeVisibleByGuid( Guid guid, bool? forceFocusForegroundWindow = null )
        {
            IDesktop? desktop;

            if ( SysInfo.IsWin10 )
            {
                desktop = VD10.Desktop.FromId( guid );
            }
            else
            {
                desktop = VD11.Desktop.FromId( guid );
            }

            if ( desktop is null ) return;

            OnDesktopVisibleEvent( desktop, forceFocusForegroundWindow );
        }

        public static void SetNameByGuid( Guid guid, string name )
        {
            if ( SysInfo.IsWin10 )
            {
                VD10.Desktop.FromId( guid )?.SetName( name );
            }
            else
            {
                VD11.Desktop.FromId( guid )?.SetName( name );
            }
        }

        public static Guid GuidFromWindow( IntPtr handle )
        {
            if ( SysInfo.IsWin10 )
            {
                return VD10.Desktop.FromWindow( handle ).Guid;
            }

            return VD11.Desktop.FromWindow( handle ).Guid;
        }

        public static event OnDesktopVisible OnDesktopVisibleEvent;
    }
}