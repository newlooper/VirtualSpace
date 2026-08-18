// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of VirtualSpace.
//
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text.Json;
using VirtualSpace.PluginContracts;

namespace VirtualSpace.Plugin
{
    public static class PluginManager
    {
        public const string PluginInfoFile = "plugin.json";
        public const string SettingsFile   = "settings.json";

        public static T? LoadFromJson<T>( string infoFile )
        {
            using var fs     = new FileStream( infoFile, FileMode.Open, FileAccess.Read );
            var       buffer = new byte[fs.Length];
            _ = fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );
            return JsonSerializer.Deserialize<T>( ref utf8Reader );
        }

        public static bool CheckRequirements( Requirements? req )
        {
            if ( req?.WinVer?.Min is null ) return true;

            var check   = false;
            var version = Environment.OSVersion.Version;

            if ( version.Major >= req.WinVer.Min.Major && version.Build >= req.WinVer.Min.Build )
                check = true;

            if ( req.WinVer.Max != null && ( version.Major > req.WinVer.Max.Major || version.Build > req.WinVer.Max.Build ) )
                check = false;

            return check;
        }

        public static void SavePluginInfo( PluginInfo pi )
        {
            var dir = PluginPaths.GetPluginDataDirectory( pi.Name );
            Directory.CreateDirectory( dir );
            var file     = Path.Combine( dir, PluginInfoFile );
            var contents = JsonSerializer.SerializeToUtf8Bytes( pi, new JsonSerializerOptions { WriteIndented = true } );
            File.WriteAllBytes( file, contents );
        }

        public static void EnsureDataFiles( PluginInfo info )
        {
            if ( string.IsNullOrEmpty( info.Name ) ) return;

            var dir = PluginPaths.GetPluginDataDirectory( info.Name );
            Directory.CreateDirectory( dir );

            var pluginFile = Path.Combine( dir, PluginInfoFile );
            if ( !File.Exists( pluginFile ) )
                SavePluginInfo( info );

            var settingsFile = Path.Combine( dir, SettingsFile );
            if ( File.Exists( settingsFile ) || string.IsNullOrEmpty( info.Folder ) ) return;

            var bundled = Path.Combine( info.Folder, SettingsFile );
            if ( File.Exists( bundled ) )
                File.Copy( bundled, settingsFile );
        }

        public static PluginInfo? LoadPersistedPluginInfo( string pluginName )
        {
            var file = Path.Combine( PluginPaths.GetPluginDataDirectory( pluginName ), PluginInfoFile );
            if ( !File.Exists( file ) ) return null;
            try
            {
                return LoadFromJson<PluginInfo>( file );
            }
            catch
            {
                return null;
            }
        }
    }
}
