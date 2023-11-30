/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        public static int GetMatrixIndexByVdIndex( int vdIndex )
        {
            var rowsCols    = (int)Math.Ceiling( Math.Sqrt( DesktopWrapper.Count ) );
            var rc          = Navigation.RowColFromIndex( rowsCols, vdIndex, ConfigManager.CurrentProfile.UI.DesktopArrangement );
            var matrixIndex = Navigation.IndexFromRowCol( rowsCols, rc, 0 );

            return matrixIndex;
        }

        public static int GetVdIndexByMatrixIndex( int matrixIndex )
        {
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( DesktopWrapper.Count ) );
            var rc       = Navigation.RowColFromIndex( rowsCols, matrixIndex, 0 );
            var vdIndex  = Navigation.IndexFromRowCol( rowsCols, rc, ConfigManager.CurrentProfile.UI.DesktopArrangement );

            return vdIndex;
        }
    }
}