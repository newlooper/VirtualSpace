// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VirtualSpace.Plugin;

namespace Cube3D
{
    public static class ConfigManager
    {
        public static readonly PluginInfo PluginInfo = GetPluginInfo();
        public static readonly Settings   Settings   = GetSettings();

        private static PluginInfo GetPluginInfo()
        {
            var file = Path.Combine( GetAppFolder(), "plugin.json" );
            return PluginManager.LoadFromJson<PluginInfo>( file );
        }

        private static Settings GetSettings()
        {
            var file = Path.Combine( GetAppFolder(), "settings.json" );
            return PluginManager.LoadFromJson<Settings>( file );
        }

        public static string GetAppPath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        private static string GetAppFolder()
        {
            return Directory.GetParent( GetAppPath() ).FullName;
        }

        public static void SaveJson( string file = null )
        {
            file ??= Path.Combine( GetAppFolder(), "settings.json" );
            var contents = JsonSerializer.SerializeToUtf8Bytes( Settings, new JsonSerializerOptions {WriteIndented = true} );
            File.WriteAllBytesAsync( file, contents );
        }
    }
}