// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using VirtualSpace;
#if NET5_0_OR_GREATER
using WinRT;
#endif

namespace VirtualDesktop
{
    public class Desktop : IDesktop
    {
        private readonly IVirtualDesktop _ivd;

        private Desktop( IVirtualDesktop desktop )
        {
            _ivd = desktop;
        }

        public Guid Guid => _ivd.GetId();

        public static int Count =>
            // return the number of desktops
            DesktopManager.GetDesktopCount();

        public static Desktop Current
        {
            get
            {
                // returns current desktop
                try
                {
                    return new Desktop( DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop() );
                }
                catch
                {
                    DesktopManager.ResetDesktopManager();
                    return new Desktop( DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop() );
                }
            }
        }

        public bool IsVisible =>
            // return true if this desktop is the current displayed one
            ReferenceEquals( _ivd, DesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop() );

        public Desktop Left
        {
            // return desktop at the left of this one, null if none
            get
            {
                var hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop( _ivd, 3, out var desktop ); // 3 = LeftDirection
                if ( hr == 0 )
                    return new Desktop( desktop );
                else
                    return null;
            }
        }

        public Desktop Right
        {
            // return desktop at the right of this one, null if none
            get
            {
                var hr = DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop( _ivd, 4, out var desktop ); // 4 = RightDirection
                if ( hr == 0 )
                    return new Desktop( desktop );
                else
                    return null;
            }
        }

        // get process id to window handle
        [DllImport( "user32.dll" )]
        private static extern int GetWindowThreadProcessId( IntPtr hWnd, out int lpdwProcessId );

        // get handle of active window
        [DllImport( "user32.dll" )]
        private static extern IntPtr GetForegroundWindow();

        public override int GetHashCode()
        {
            // get hash
            return _ivd.GetHashCode();
        }

        public override bool Equals( object? obj )
        {
            // compare with object
            return obj is Desktop desk && ReferenceEquals( _ivd, desk._ivd );
        }

        public static Desktop FromIndex( int index )
        {
            // return desktop object from index (-> index = 0..Count-1)
            return new Desktop( DesktopManager.GetDesktop( index ) );
        }

        public static Desktop FromWindow( IntPtr hWnd )
        {
            // return desktop object to desktop on which window <hWnd> is displayed
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            var id = DesktopManager.VirtualDesktopManager.GetWindowDesktopId( hWnd );

            return GetDesktopByGuid( id );
        }

        public static int SysIndexFromDesktop( Desktop desktop )
        {
            // return index of desktop object or -1 if not found
            if ( desktop == null ) return -1;
            return DesktopManager.GetDesktopIndex( desktop._ivd );
        }

        public static string DesktopNameFromDesktop( Desktop desktop )
        {
            // return name of desktop or "Desktop n" if it has no name

            // get desktop name
            string desktopName = null;
            try
            {
#if NET5_0_OR_GREATER
                desktop._ivd.GetString( out var hstr );
                desktopName = MarshalString.FromAbi( hstr );
#else
                desktopName = desktop._ivd.GetName();
#endif
            }
            catch ( Exception )
            {
            }

            // no name found, generate generic name
            if ( string.IsNullOrEmpty( desktopName ) )
            {
                // create name "Desktop n" (n = number starting with 1)
                desktopName = "Desktop " + ( DesktopManager.GetDesktopIndex( desktop._ivd ) + 1 ).ToString();
            }

            return desktopName;
        }

        public static string DesktopNameFromIndex( int index )
        {
            // return name of desktop from index (-> index = 0..Count-1) or "Desktop n" if it has no name

            // get desktop name
            string desktopName = null;
            try
            {
#if NET5_0_OR_GREATER
                DesktopManager.GetDesktop( index ).GetString( out var hstr );
                desktopName = MarshalString.FromAbi( hstr );
#else
                desktopName = DesktopManager.GetDesktop( index ).GetName();
#endif
            }
            catch
            {
            }

            // no name found, generate generic name
            if ( string.IsNullOrEmpty( desktopName ) )
            {
                // create name "Desktop n" (n = number starting with 1)
                desktopName = "Desktop " + ( index + 1 ).ToString();
            }

            return desktopName;
        }

        public static bool HasDesktopNameFromIndex( int index )
        {
            // return true is desktop is named or false if it has no name

            // read desktop name in registry
            string desktopName = null;
            try
            {
#if NET5_0_OR_GREATER
                DesktopManager.GetDesktop( index ).GetString( out var hstr );
                desktopName = MarshalString.FromAbi( hstr );
#else
                desktopName = DesktopManager.GetDesktop( index ).GetName();
#endif
            }
            catch
            {
            }

            // name found?
            if ( string.IsNullOrEmpty( desktopName ) )
                return false;
            else
                return true;
        }

        public static string DesktopWallpaperFromIndex( int index )
        {
            // return name of desktop wallpaper from index (-> index = 0..Count-1)

            // get desktop name
            var desktopWpPath = "";
            try
            {
                desktopWpPath = DesktopManager.GetDesktop( index ).GetWallpaperPath();
            }
            catch
            {
            }

            return desktopWpPath;
        }

        public static int SearchDesktop( string partialName )
        {
            // get index of desktop with partial name, return -1 if no desktop found
            var index = -1;

            for ( var i = 0; i < DesktopManager.GetDesktopCount(); i++ )
            {
                // loop through all virtual desktops and compare partial name to desktop name
                if ( DesktopNameFromIndex( i ).ToUpper().IndexOf( partialName.ToUpper() ) >= 0 )
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static Desktop Create()
        {
            // create a new desktop
            return new Desktop( DesktopManager.VirtualDesktopManagerInternal.CreateDesktop() );
        }

        public void Remove( Desktop? fallback = null )
        {
            // destroy desktop and switch to <fallback>
            IVirtualDesktop fallbackDesktop;
            if ( fallback == null )
            {
                // if no fallback is given use desktop to the left except for desktop 0.
                var dtToCheck = new Desktop( DesktopManager.GetDesktop( 0 ) );
                if ( Equals( dtToCheck ) )
                {
                    // desktop 0: set fallback to second desktop (= "right" desktop)
                    DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop( _ivd, 4, out fallbackDesktop ); // 4 = RightDirection
                }
                else
                {
                    // set fallback to "left" desktop
                    DesktopManager.VirtualDesktopManagerInternal.GetAdjacentDesktop( _ivd, 3, out fallbackDesktop ); // 3 = LeftDirection
                }
            }
            else
                // set fallback desktop
                fallbackDesktop = fallback._ivd;

            DesktopManager.VirtualDesktopManagerInternal.RemoveDesktop( _ivd, fallbackDesktop );
        }

        public static void RemoveAll()
        {
            // remove all desktops but visible
            // DesktopManager.VirtualDesktopManagerInternal.SetDesktopIsPerMonitor( true );
        }

        public void Move( int index )
        {
            // move current desktop to desktop in index (-> index = 0..Count-1)
            DesktopManager.VirtualDesktopManagerInternal.MoveDesktop( _ivd, index );
        }

        public void SetName( string name )
        {
            // set name for desktop, empty string removes name
#if NET5_0_OR_GREATER
            var newName = MarshalString.CreateMarshaler( name );
            DesktopManager.VirtualDesktopManagerInternal.SetName( _ivd, MarshalString.GetAbi( newName ) );
#else
            DesktopManager.VirtualDesktopManagerInternal.SetDesktopName( _ivd, name );
#endif
        }

        public void SetWallpaperPath( string path )
        {
            // set path for wallpaper, empty string removes path
            if ( string.IsNullOrEmpty( path ) ) throw new ArgumentNullException();

#if NET5_0_OR_GREATER
            var newPath = MarshalString.CreateMarshaler( path );
            DesktopManager.VirtualDesktopManagerInternal.SetWallpaper( _ivd, MarshalString.GetAbi( newPath ) );
#else
            DesktopManager.VirtualDesktopManagerInternal.SetDesktopWallpaper( _ivd, path );
#endif
        }

        public static void SetAllWallpaperPaths( string path )
        {
            // set wallpaper path for all desktops
            if ( string.IsNullOrEmpty( path ) ) throw new ArgumentNullException();
            DesktopManager.VirtualDesktopManagerInternal.UpdateWallpaperPathForAllDesktops( path );
        }

        public void MakeVisible()
        {
            // make this desktop visible
            DesktopManager.VirtualDesktopManagerInternal.SwitchDesktop( _ivd );
        }

        public void MoveWindow( IntPtr hWnd )
        {
            // move window to this desktop
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            _ = GetWindowThreadProcessId( hWnd, out var processId );

            if ( Process.GetCurrentProcess().Id == processId )
            {
                // window of process
                try // the easy way (if we are owner)
                {
                    DesktopManager.VirtualDesktopManager.MoveWindowToDesktop( hWnd, _ivd.GetId() );
                }
                catch // window of process, but we are not the owner
                {
                    DesktopManager.ApplicationViewCollection.GetViewForHWnd( hWnd, out var view );
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop( view, _ivd );
                }
            }
            else
            {
                // window of other process
                DesktopManager.ApplicationViewCollection.GetViewForHWnd( hWnd, out var view );
                try
                {
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop( view, _ivd );
                }
                catch
                {
                    // could not move active window, try main window (or whatever windows thinks is the main window)
                    DesktopManager.ApplicationViewCollection.GetViewForHWnd(
                        Process.GetProcessById( processId ).MainWindowHandle,
                        out view );
                    DesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop( view, _ivd );
                }
            }
        }

        public void MoveActiveWindow()
        {
            // move active window to this desktop
            MoveWindow( GetForegroundWindow() );
        }

        public bool HasWindow( IntPtr hWnd )
        {
            // return true if window is on this desktop
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            return _ivd.GetId() == DesktopManager.VirtualDesktopManager.GetWindowDesktopId( hWnd );
        }

        public static bool IsWindowPinned( IntPtr hWnd )
        {
            // return true if window is pinned to all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            return DesktopManager.VirtualDesktopPinnedApps.IsViewPinned( hWnd.GetApplicationView() );
        }

        public static void PinWindow( IntPtr hWnd )
        {
            // pin window to all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            var view = hWnd.GetApplicationView();
            if ( !DesktopManager.VirtualDesktopPinnedApps.IsViewPinned( view ) )
            {
                // pin only if not already pinned
                DesktopManager.VirtualDesktopPinnedApps.PinView( view );
            }
        }

        public static void UnpinWindow( IntPtr hWnd )
        {
            // unpin window from all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            var view = hWnd.GetApplicationView();
            if ( DesktopManager.VirtualDesktopPinnedApps.IsViewPinned( view ) )
            {
                // unpin only if not already unpinned
                DesktopManager.VirtualDesktopPinnedApps.UnpinView( view );
            }
        }

        public static bool IsApplicationPinned( IntPtr hWnd )
        {
            // return true if application for window is pinned to all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            return DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned( DesktopManager.GetAppId( hWnd ) );
        }

        public static void PinApplication( IntPtr hWnd )
        {
            // pin application for window to all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            var appId = DesktopManager.GetAppId( hWnd );
            if ( !DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned( appId ) )
            {
                // pin only if not already pinned
                DesktopManager.VirtualDesktopPinnedApps.PinAppID( appId );
            }
        }

        public static void UnpinApplication( IntPtr hWnd )
        {
            // unpin application for window from all desktops
            if ( hWnd == IntPtr.Zero ) throw new ArgumentNullException();
            var view  = hWnd.GetApplicationView();
            var appId = DesktopManager.GetAppId( hWnd );
            if ( DesktopManager.VirtualDesktopPinnedApps.IsAppIdPinned( appId ) )
            {
                // unpin only if pinned
                DesktopManager.VirtualDesktopPinnedApps.UnpinAppID( appId );
            }
        }

        public static Desktop? FromId( Guid guid )
        {
            return GetDesktopByGuid( guid );
        }

        private static Desktop? GetDesktopByGuid( Guid guid )
        {
            Desktop? desktop = null;

            try
            {
                var count = DesktopManager.GetDesktopCount();
                DesktopManager.VirtualDesktopManagerInternal.GetDesktops( out var desktops );

                for ( var i = 0; i < count; i++ )
                {
                    desktops.GetAt( i, typeof( IVirtualDesktop ).GUID, out var objDesktop );
                    if ( guid.CompareTo( ( (IVirtualDesktop)objDesktop ).GetId() ) != 0 ) continue;
                    desktop = new Desktop( (IVirtualDesktop)objDesktop );
                }

                Marshal.ReleaseComObject( desktops );
                return desktop;
            }
            catch
            {
                return desktop;
            }
        }
    }
}