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

namespace VirtualSpace.Commons
{
    public static class UserMessage
    {
        public const int RiseView          = 1000;
        public const int ShowAppController = 1001;
        public const int CloseView         = 1002;
        public const int SwitchDesktop     = 1003;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int    cbData;
        public IntPtr lpData;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct VirtualDesktopSwitchInfo
    {
        public IntPtr hostHandle;
        public int    vdCount;
        public int    fromIndex;
        public int    dir;
        public int    targetIndex;
    }
}