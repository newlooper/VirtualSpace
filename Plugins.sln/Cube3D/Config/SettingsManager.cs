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

namespace Cube3D.Config
{
    public class SettingsManager
    {
        private const  string    PluginSettingFile = "settings.json";
        private static string    _dataDirectory    = string.Empty;
        public static  Settings  Settings          { get; private set; } = new();

        public static void Initialize( string dataDirectory )
        {
            _dataDirectory = dataDirectory;
            Directory.CreateDirectory( _dataDirectory );

            var dest = Path.Combine( _dataDirectory, PluginSettingFile );
            if ( !File.Exists( dest ) )
            {
                var bundled = Path.Combine( Path.GetDirectoryName( typeof( SettingsManager ).Assembly.Location ) ?? string.Empty, PluginSettingFile );
                if ( File.Exists( bundled ) )
                    File.Copy( bundled, dest );
                else
                    SaveJson( dest );
            }

            Settings = LoadFromJson( dest ) ?? new Settings();
        }

        private static Settings LoadFromJson( string file )
        {
            using var fs     = new FileStream( file, FileMode.Open, FileAccess.Read );
            var       buffer = new byte[fs.Length];
            _ = fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );
            return JsonSerializer.Deserialize<Settings>( ref utf8Reader );
        }

        public static void SaveJson( string file = null )
        {
            file ??= Path.Combine( _dataDirectory, PluginSettingFile );
            Directory.CreateDirectory( Path.GetDirectoryName( file )! );
            var contents = JsonSerializer.SerializeToUtf8Bytes( Settings, new JsonSerializerOptions { WriteIndented = true } );
            File.WriteAllBytes( file, contents );
        }
    }
}
