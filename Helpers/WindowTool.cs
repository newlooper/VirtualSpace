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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualSpace.Helpers
{
    public static class WindowTool
    {
        private static void MoveWindowToScreen( IntPtr hWnd, Screen destScreen )
        {
            var srcScreen = Screen.FromHandle( hWnd );
            if ( srcScreen.DeviceName == destScreen.DeviceName ) return;

            var wp = new WINDOWPLACEMENT();
            wp.Length = Marshal.SizeOf( wp );
            if ( !User32.GetWindowPlacement( hWnd, ref wp ) ) return;

            var rect         = wp.NormalPosition;
            var targetX      = destScreen.WorkingArea.X + rect.Left - srcScreen.WorkingArea.Left;
            var targetY      = destScreen.WorkingArea.Y + rect.Top - srcScreen.WorkingArea.Top;
            var targetWidth  = rect.Right - rect.Left;
            var targetHeight = rect.Bottom - rect.Top;

            switch ( wp.ShowCmd )
            {
                case ShowState.SW_SHOWMAXIMIZED:
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_RESTORE );
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_MAXIMIZE );
                    break;
                case ShowState.SW_MINIMIZE:
                case ShowState.SW_SHOWMINIMIZED:
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_RESTORE );
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    // User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_SHOWMINIMIZED );
                    break;
                case ShowState.SW_NORMAL:
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    break;
            }
        }

        public static void MoveWindowToScreen( IntPtr hWnd, int index )
        {
            var allScreens = Screen.AllScreens;

            if ( index < 0 || index > allScreens.Length ) return;

            MoveWindowToScreen( hWnd, allScreens[index] );
        }

        public static void MoveWindowToScreen( IntPtr hWnd, string deviceName )
        {
            var allScreens = Screen.AllScreens;
            var index      = -1;

            for ( var i = 0; i < allScreens.Length; i++ )
            {
                if ( deviceName == allScreens[i].DeviceName )
                {
                    index = i;
                    break;
                }
            }

            if ( index < 0 ) return;

            MoveWindowToScreen( hWnd, allScreens[index] );
        }

        public static int GetZOrderByHandle( IntPtr hWnd )
        {
            var index = 0;
            _ = User32.EnumWindows( ( wnd, param ) =>
            {
                index++;
                return hWnd != wnd;
            }, 0 );

            return index;
        }

        public static void ActiveWindow( IntPtr hWnd )
        {
            if ( User32.IsIconic( hWnd ) )
            {
                _ = User32.ShowWindow( hWnd, (short)ShowState.SW_RESTORE );
            }

            User32.SetForegroundWindow( hWnd );
            User32.BringWindowToTop( hWnd );
        }

        public static bool IsModalWindow( IntPtr hWnd )
        {
            // child windows cannot have owners
            var style = User32.GetWindowLong( hWnd, (int)GetWindowLongFields.GWL_STYLE );
            if ( ( style & (int)WindowStyles.WS_CHILD ) > 0 ) return false;

            var hWndOwner = User32.GetWindow( hWnd, GetWindowType.GW_OWNER );
            if ( hWndOwner == IntPtr.Zero ) return false; // not an owned window
            if ( User32.IsWindowEnabled( hWndOwner ) ) return false; // owner is enabled
            return true; // an owned window whose owner is disabled
        }
    }
}