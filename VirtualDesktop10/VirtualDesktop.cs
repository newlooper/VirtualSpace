// Author: Markus Scholtes, 2021
// Version 1.9, 2021-10-08
// Version for Windows 10 1809 to 21H1
// Compile with:
// C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe VirtualDesktop.cs
// Based on http://stackoverflow.com/a/32417530, Windows 10 SDK, github project Grabacr07/VirtualDesktop and own research

/////////////////////////////////////////////////
// Dylan Cheng (https://github.com/newlooper)
// added Notifications about desktop
// added conditional compile for C#/WinRT breaking change on .NET 5.0+

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
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
            return new Desktop( DesktopManager.VirtualDesktopManagerInternal.FindDesktop( ref id ) );
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
            var guid = desktop._ivd.GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name",
                    null );
            }
            catch
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
            var guid = DesktopManager.GetDesktop( index ).GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name",
                    null );
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
            var guid = DesktopManager.GetDesktop( index ).GetId();

            // read desktop name in registry
            string desktopName = null;
            try
            {
                desktopName = (string)Registry.GetValue(
                    "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{" + guid.ToString() + "}", "Name",
                    null );
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

        public void SetName( string name )
        {
            // set name for desktop, empty string removes name
            if ( DesktopManager.VirtualDesktopManagerInternal2 != null ) // only if interface to set name is present
            {
#if NET5_0_OR_GREATER
                var newName = MarshalString.CreateMarshaler( name );
                DesktopManager.VirtualDesktopManagerInternal2.SetName( _ivd, MarshalString.GetAbi( newName ) );
#else
                DesktopManager.VirtualDesktopManagerInternal2.SetName( _ivd, name );
#endif
            }
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
            try
            {
                return new Desktop( DesktopManager.VirtualDesktopManagerInternal.FindDesktop( ref guid ) );
            }
            catch
            {
                return null;
            }
        }
    }
}