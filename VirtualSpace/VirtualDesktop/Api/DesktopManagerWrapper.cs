﻿/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using VirtualDesktop;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopManagerWrapper
    {
        public static Guid GetIdByIndex( int index )
        {
            try
            {
                var desktop = DesktopManager.GetDesktop( index );
                return desktop.GetId();
            }
            catch
            {
                return default;
            }
        }

        public static int GetViewCount()
        {
            return DesktopManager.GetViewCount();
        }
    }
}