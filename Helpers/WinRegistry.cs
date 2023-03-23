/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Management;
using System.Security.Principal;
using Microsoft.Win32;
using VirtualSpace.AppLogs;

namespace VirtualSpace.Helpers
{
    public static class WinRegistry
    {
        private const string PATH_VD_WALLPAPER_REGISTRY = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\";
        private const string PATH_WALLPAPER_REGISTRY    = @"HKEY_CURRENT_USER\Control Panel\Desktop\";
        private const string PATH_COLOR_REGISTRY        = @"HKEY_CURRENT_USER\Control Panel\Colors\";
        private const string PATH_APP_USE_LIGHT_THEME   = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\";

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
            return Registry.GetValue( PATH_WALLPAPER_REGISTRY, "Wallpaper", "" ).ToString();
        }

        public static string? GetWallPaperPathByGuid( Guid guid )
        {
            var path = Registry.GetValue( PATH_VD_WALLPAPER_REGISTRY + "{" + guid + "}", "Wallpaper", "" )?.ToString();

            if ( string.IsNullOrEmpty( path ) )
                path = GetDefaultWallpaperPath();

            return string.IsNullOrEmpty( path ) ? null : path;
        }

        public static Color GetBackColor()
        {
            var color    = Registry.GetValue( PATH_COLOR_REGISTRY, "Background", "" ).ToString();
            var strColor = color.Split( ' ' );
            return Color.FromArgb( int.Parse( strColor[0] ), int.Parse( strColor[1] ), int.Parse( strColor[2] ) );
        }

        public static bool AppThemeIsLight()
        {
            return Registry.GetValue( PATH_APP_USE_LIGHT_THEME, "AppsUseLightTheme", "1" ).ToString() == "1";
        }
    }

    public class RegValueMonitor : IDisposable
    {
        private readonly ManagementEventWatcher? _watcher;

        public RegValueMonitor( string hive, string keyPath, string valueName )
        {
            var currentUser = WindowsIdentity.GetCurrent();
            var sid         = currentUser.User.Value;
            var q = $"SELECT * FROM RegistryValueChangeEvent WHERE Hive='{hive}' " +
                    @$"AND KeyPath='{sid}\\{keyPath}' AND ValueName='{valueName}'";

            var query = new WqlEventQuery( q );
            try
            {
                _watcher = new ManagementEventWatcher( query );
                _watcher.EventArrived += HandleEvent;
                _watcher.Start();
            }
            catch ( ManagementException managementException )
            {
                Logger.Error( "[Registry]: " + managementException.Message );
            }
        }

        private static void HandleEvent( object sender, EventArrivedEventArgs e )
        {
            var keyPath   = e.NewEvent.Properties["Hive"].Value + @"\" + e.NewEvent.Properties["KeyPath"].Value;
            var valueName = e.NewEvent.Properties["ValueName"].Value.ToString();

            var v = Registry.GetValue( keyPath, valueName, "" );
            OnRegValueChanged?.Invoke( null, new RegValueChangedEventArgs( v.ToString() ) );
        }

        public void Dispose()
        {
            _watcher?.Stop();
        }

        public static event EventHandler<RegValueChangedEventArgs>? OnRegValueChanged;

        public class RegValueChangedEventArgs : EventArgs
        {
            public string Value { get; set; }

            public RegValueChangedEventArgs( string value )
            {
                Value = value;
            }
        }
    }
}