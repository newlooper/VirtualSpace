/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.Entity;
using VirtualSpace.Config.Profiles;
using Settings = VirtualSpace.Config.Const.Settings;

namespace VirtualSpace.Config
{
    public static class Manager
    {
        public static ConfigTemplate Configs;
        public static string         AppPath;
        public static string         AppFolder;
        public static string         ProfileFolder;
        public static string         CacheFolder;
        public static string         PluginsFolder;
        public static string         ConfigFilePath;

        public static Profile CurrentProfile => Configs.Profiles[Configs.CurrentProfileName];

        public static bool Init()
        {
            try
            {
                AppPath = Process.GetCurrentProcess().MainModule.FileName;
                AppFolder = Directory.GetParent( AppPath ).FullName;
                ConfigFilePath = Path.Combine( AppFolder, Settings.SettingsFile );

                CheckFolders();

                InitConfig( ConfigFilePath );

                LogManager.SetLogLevel( Configs.LogConfig.LogLevel );
            }
            catch ( Exception ex )
            {
                MessageBox.Show( "File Access Error.\n" + ex.Message, @"Error", MessageBoxButton.OK, MessageBoxImage.Error );
                return false;
            }

            return true;
        }

        private static void InitConfig( string filePath )
        {
            if ( File.Exists( filePath ) )
            {
                using var fs     = new FileStream( filePath, FileMode.Open, FileAccess.Read );
                var       buffer = new byte[fs.Length];
                _ = fs.Read( buffer, 0, (int)fs.Length );
                var utf8Reader = new Utf8JsonReader( buffer );
                Configs = JsonSerializer.Deserialize<ConfigTemplate>( ref utf8Reader );

                Logger.Info( $"Settings File Loaded, Version: {Configs.Version}, Current Profile: {Configs.CurrentProfileName}" );
            }
            else
            {
                Logger.Info( "Settings File Not Found, Create From Default Template." );
                Configs = new ConfigTemplate
                {
                    CurrentProfileName = nameof( Default ),
                    Version = Settings.DefaultVersion,
                    LogConfig = new LogConfig {LogLevel = Settings.DefaultLogLevel},
                    Profiles = new Dictionary<string, Profile>
                    {
                        {nameof( Default ), new Default()}
                    }
                };
                Save( filePath );
            }
        }

        public static async void Save( string? filePath = null )
        {
            filePath ??= ConfigFilePath;
            try
            {
                var contents = JsonSerializer.SerializeToUtf8Bytes( Configs, new JsonSerializerOptions {WriteIndented = true} );
                await File.WriteAllBytesAsync( filePath, contents );
                Logger.Info( "Settings Saved." );
            }
            catch ( Exception ex )
            {
                Logger.Error( "Failed to save Settings: " + ex.Message );
            }
        }

        private static void CheckFolders()
        {
            ProfileFolder = Path.Combine( AppFolder, Settings.ProfilesFolder );
            Directory.CreateDirectory( ProfileFolder );

            CacheFolder = Path.Combine( AppFolder, Settings.CacheFolder );
            Directory.CreateDirectory( CacheFolder );

            PluginsFolder = Path.Combine( AppFolder, Settings.PluginsFolder );
            Directory.CreateDirectory( PluginsFolder );
        }

        public static string GetRulesPath( string? path = null )
        {
            CheckFolders();
            return string.IsNullOrEmpty( path )
                ? Path.Combine( ProfileFolder, Configs.CurrentProfileName + Settings.RuleFileExt )
                : Path.Combine( ProfileFolder, path + Settings.RuleFileExt );
        }

        public static string GetCachePath()
        {
            CheckFolders();
            return CacheFolder;
        }

        public static string GetPluginsPath()
        {
            CheckFolders();
            return PluginsFolder;
        }
    }
}