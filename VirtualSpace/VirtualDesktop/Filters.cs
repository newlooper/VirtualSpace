/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop
{
    public static class Filters
    {
        public static readonly string[] WndClsIgnoreList =
        {
            "Progman",
            "RainmeterMeterWindow",
            "SysDragImage"
        };

        public static readonly string[] WndTitleIgnoreList =
        {
            "__VirtualDesktopFrame!",
            "__VirtualDesktopWindow!",
            "__VirtualDesktopDragWindow!",
            "[Virtual Space Controller]",
            "WinFormsDesigner"
        };

        public static readonly List<IntPtr>                      WndHandleIgnoreListByError  = new();
        public static readonly ConcurrentDictionary<IntPtr, int> WndHandleIgnoreListByManual = new();

        public static bool IsCloaked( IntPtr handle )
        {
            var HRESULT = DwmApi.DwmGetWindowAttribute( handle,
                (uint)DwmApi.DwmWindowAttribute.DWMWA_CLOAKED,
                out var cloaked,
                sizeof( uint ) );

            return cloaked > 2;
        }
    }
}