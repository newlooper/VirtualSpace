﻿/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

extern alias VirtualDesktop10;
extern alias VirtualDesktop11;
using VirtualSpace.Helpers;
using VD10 = VirtualDesktop10::VirtualDesktop;
using VD11 = VirtualDesktop11::VirtualDesktop;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopWrapper
    {
        public static void Create()
        {
            if ( SysInfo.IsWin10 )
            {
                VD10.Desktop.Create();
            }
            else
            {
                var desk = VD11.Desktop.Create();
                var path = WinRegistry.GetDefaultWallpaperPath();
                if ( !string.IsNullOrEmpty( path ) )
                    desk.SetWallpaperPath( path );
            }
        }
    }
}