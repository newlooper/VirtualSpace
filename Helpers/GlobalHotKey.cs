/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class GlobalHotKey
    {
        [Flags]
        public enum KeyModifiers
        {
            None       = 0,
            Alt        = 1,
            Ctrl       = 2,
            Shift      = 4,
            WindowsKey = 8
        }

        private static          IntPtr    _handle = IntPtr.Zero;
        private static readonly List<int> Ids     = new();

        public static bool RegHotKey( IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk )
        {
            _handle = hWnd;
            Ids.Add( id );
            return RegisterHotKey( hWnd, id, fsModifiers, vk );
        }

        [DllImport( "user32.dll", SetLastError = true )]
        private static extern bool RegisterHotKey( IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool UnregisterHotKey( IntPtr hWnd, int id );

        public static void UnRegAllHotKey()
        {
            foreach ( var id in Ids )
            {
                UnregisterHotKey( _handle, id );
            }
        }
    }
}