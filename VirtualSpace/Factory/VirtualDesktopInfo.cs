// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

extern alias VirtualDesktop10;
extern alias VirtualDesktop11;
using VirtualSpace.Helpers;

namespace VirtualSpace.Factory
{
    public class VirtualDesktopInfo : IVirtualDesktopInfo
    {
        public int GetDesktopCount()
        {
            if ( SysInfo.IsWin10 )
            {
                return VirtualDesktop10::VirtualDesktop.Desktop.Count;
            }

            return VirtualDesktop11::VirtualDesktop.Desktop.Count;
        }

        public string DesktopNameFromIndex( int index )
        {
            if ( SysInfo.IsWin10 )
            {
                return VirtualDesktop10::VirtualDesktop.Desktop.DesktopNameFromIndex( index );
            }

            return VirtualDesktop11::VirtualDesktop.Desktop.DesktopNameFromIndex( index );
        }
    }
}