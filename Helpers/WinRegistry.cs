/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using Microsoft.Win32;

namespace VirtualSpace.Helpers
{
    public static class WinRegistry
    {
        private const string VD_WALLPAPER_REGISTRY_PREFIX = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\";
        private const string WALLPAPER_REGISTRY_PREFIX    = @"HKEY_CURRENT_USER\Control Panel\Desktop\";
        private const string COLOR_REGISTRY_PREFIX        = @"HKEY_CURRENT_USER\Control Panel\Colors\";

        public static Wallpaper GetWallpaperByDesktopGuid( Guid guid, int width, int height, string cachePath, long quality )
        {
            var wallpaper = new Wallpaper();

            var path = GetWallPaperPathByGuid( guid );

            if ( string.IsNullOrEmpty( path ) )
            {
                wallpaper.Color = GetBackColor();
            }
            else
            {
                wallpaper.Image = Images.GetScaledBitmap( width, height, path, ref wallpaper, cachePath, quality );
            }

            return wallpaper;
        }

        public static Wallpaper GetWallpaperByPath( string path, int width, int height, string cachePath, long quality )
        {
            var wallpaper = new Wallpaper();
            wallpaper.Image = Images.GetScaledBitmap( width, height, path, ref wallpaper, cachePath, quality );
            return wallpaper;
        }

        public static string? GetDefaultWallpaperPath()
        {
            return Registry.GetValue( WALLPAPER_REGISTRY_PREFIX, "Wallpaper", "" ).ToString();
        }

        public static string? GetWallPaperPathByGuid( Guid guid )
        {
            var path = Registry.GetValue( VD_WALLPAPER_REGISTRY_PREFIX + "{" + guid + "}", "Wallpaper", "" )?.ToString();

            if ( string.IsNullOrEmpty( path ) )
                path = GetDefaultWallpaperPath();

            return string.IsNullOrEmpty( path ) ? null : path;
        }

        public static Color GetBackColor()
        {
            var color    = Registry.GetValue( COLOR_REGISTRY_PREFIX, "Background", "" ).ToString();
            var strColor = color.Split( ' ' );
            return Color.FromArgb( int.Parse( strColor[0] ), int.Parse( strColor[1] ), int.Parse( strColor[2] ) );
        }
    }
}