/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop.Api
{
    public static partial class DesktopManagerWrapper
    {
        private const  string WALLPAPER_REGISTRY_PREFIX = @"HKEY_CURRENT_USER\Control Panel\Desktop\";
        private const  string COLOR_REGISTRY_PREFIX     = @"HKEY_CURRENT_USER\Control Panel\Colors\";
        private static string _lastPath                 = "";
        private static string _lastColor                = "";

        private static void WatchWallpaperEvents()
        {
            Task.Factory.StartNew( () =>
            {
                var cachePath = ConfigManager.GetCachePath();
                while ( true )
                {
                    var path  = Registry.GetValue( WALLPAPER_REGISTRY_PREFIX, "Wallpaper", "" ).ToString();
                    var color = Registry.GetValue( COLOR_REGISTRY_PREFIX, "Background", "" ).ToString();
                    if ( _lastPath != path || _lastColor != color )
                    {
                        _lastPath = path;
                        _lastColor = color;
                        Parallel.ForEach( VirtualDesktopManager.GetAllVirtualDesktops(), ( vdw, _ ) =>
                        {
                            vdw.UpdateWallpaper();
                        } );
                    }

                    Thread.Sleep( Const.OneSecond );
                }
            }, TaskCreationOptions.LongRunning );
        }
    }
}