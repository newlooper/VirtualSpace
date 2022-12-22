// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System.IO;
using System.Text.Json;
using VirtualSpace.Plugin;

namespace Cube3D.Config
{
    public class SettingsManager
    {
        public static readonly Settings Settings          = GetSettings();
        private const          string   PluginSettingFile = "settings.json";

        private static Settings GetSettings()
        {
            var file = Path.Combine( PluginManager.GetAppFolder(), PluginSettingFile );
            return PluginManager.LoadFromJson<Settings>( file );
        }

        public static void SaveJson( string file = null )
        {
            file ??= Path.Combine( PluginManager.GetAppFolder(), PluginSettingFile );
            var contents = JsonSerializer.SerializeToUtf8Bytes( Settings, new JsonSerializerOptions {WriteIndented = true} );
            File.WriteAllBytesAsync( file, contents );
        }
    }
}