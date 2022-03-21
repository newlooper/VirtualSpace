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
using System.Runtime.InteropServices;

namespace VirtualSpace.Plugin
{
    public static class WinApi
    {
        public const int WM_SYSCOMMAND    = 0x0112;
        public const int SC_MAXIMIZE      = 0xF030;
        public const int SC_MINIMIZE      = 0xF020;
        public const int SC_RESTORE       = 0xF120;
        public const int SC_SIZE          = 0xF000;
        public const int SC_MOVE          = 0xF010;
        public const int SC_CLOSE         = 0xF060;
        public const int WM_HOTKEY        = 0x0312;
        public const int WM_CLOSE         = 0x0010;
        public const int WM_QUIT          = 0x0012;
        public const int WM_DESTROY       = 0x0002;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int MA_NOACTIVATE    = 0x3;
        public const int WM_COPYDATA      = 0x004A;

        private const int WM_USER           = 0x0400;
        public const  int UM_SWITCHDESKTOP  = WM_USER + 1;
        public const  int UM_PLUGINSETTINGS = WM_USER + 2;

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool PostMessage( IntPtr hWnd, int msg, uint wParam, uint lParam );
    }
}