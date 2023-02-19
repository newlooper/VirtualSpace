// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

namespace VirtualSpace.Helpers
{
    public static class WinMsg
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
    }

    public static class UserMessage
    {
        public const int RiseView             = 1000;
        public const int ShowAppController    = 1001;
        public const int CloseView            = 1002;
        public const int SwitchDesktop        = 1003;
        public const int DesktopArrangement   = 1004;
        public const int RunAsAdministrator   = 1005;
        public const int RestartApp           = 1006;
        public const int EnableMouseHook      = 1007;
        public const int DisableMouseHook     = 1008;
        public const int RiseViewForActiveApp = 1009;

        public const int SVD1 = 1101;
        public const int SVD2 = 1102;
        public const int SVD3 = 1103;
        public const int SVD4 = 1104;
        public const int SVD5 = 1105;
        public const int SVD6 = 1106;
        public const int SVD7 = 1107;
        public const int SVD8 = 1108;
        public const int SVD9 = 1109;

        public const int NavLeft  = 1201;
        public const int NavRight = 1202;
        public const int NavUp    = 1203;
        public const int NavDown  = 1204;
    }
}