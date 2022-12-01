/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using VirtualDesktop;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopWrapper
    {
        public static Desktop Create()
        {
            var desk = Desktop.Create();
            var path = WinRegistry.GetDefaultWallpaperPath();
            if ( !string.IsNullOrEmpty( path ) )
                SetWallpaper( desk, path );
            return desk;
        }

        private static void SetWallpaper( Desktop d, string path )
        {
            d.SetWallpaperPath( path );
        }
    }
}