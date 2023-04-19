/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.Converter;
using VirtualSpace.Config.DataAnnotations;
using VirtualSpace.Config.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Config.Profiles;
using Settings = VirtualSpace.Config.Const.Settings;

namespace VirtualSpace.Config
{
    public static class Manager
    {
        public static ConfigTemplate Configs;
        public static string         AppPath;
        public static string         AppRootFolder;
        public static string         ProfileFolder;
        public static string         CacheFolder;
        public static string         PluginsFolder;
        public static string         ConfigRootFolder;
        public static string         ConfigFilePath;

        public static Profile CurrentProfile => Configs.Profiles[Configs.CurrentProfileName];

        public static bool Init()
        {
            try
            {
                AppPath = Environment.ProcessPath!;
                AppRootFolder = Directory.GetParent( AppPath )!.FullName;

                ConfigRootFolder = GetConfigRoot();
                ConfigFilePath = Path.Combine( ConfigRootFolder, Settings.SettingsFile );

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
                var cluster = ReadCluster();
                if ( cluster is not null )
                {
                    Configs.Cluster = cluster;
                }

                PropertyProtector.Walk( Configs );

                if ( Configs.MouseActions.Count == 0 )
                {
                    Logger.Info( "Missing MouseActions, Try find old version from configs." );
                    if ( Configs.MouseAction is null || Configs.MouseAction.Count == 0 )
                    {
                        Logger.Info( "Old version MouseActions not found, Using native default." );
                        Configs.MouseActions = MouseAction.Info;
                    }
                    else
                    {
                        Logger.Info( "Old version MouseActions found, try to convert to new version." );
                        try
                        {
                            EntityConverter.ConvertMouseAction( Configs.MouseAction, Configs.MouseActions );
                        }
                        catch
                        {
                            Logger.Info( "Convert MouseAction failed, Using native default." );
                            Configs.MouseActions = MouseAction.Info;
                        }
                    }
                }

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
                    },
                    MouseActions = MouseAction.Info
                };
                Save( filePath, "init", "Setting File" );
                SaveCluster( Configs.Cluster );
            }
        }

        public static async void Save( string? filePath = null, object? reason = null, [CallerArgumentExpression( "reason" )] string reasonName = "" )
        {
            filePath ??= ConfigFilePath;
            try
            {
                var contents = JsonSerializer.SerializeToUtf8Bytes( Configs, JsonWriteOptions );
                await File.WriteAllBytesAsync( filePath, contents ).ConfigureAwait( false );
                Logger.Info( $"Settings Saved [{reasonName}: {reason}]." );

                if ( reasonName.Contains( ".Configs.Cluster" ) )
                    SaveCluster( Configs.Cluster );
            }
            catch ( Exception ex )
            {
                Logger.Error( "Failed to save Settings: " + ex.Message );
            }
        }

        public static async void SwitchProfile( string name )
        {
            Configs.CurrentProfileName = name;
            var cluster = ReadCluster(); // after CurrentProfileName changed
            if ( cluster is not null )
            {
                Configs.Cluster = cluster;
            }

            Conditions.SwitchRuleProfile(); // after CurrentProfileName changed

            try
            {
                var contents = JsonSerializer.SerializeToUtf8Bytes( Configs, JsonWriteOptions );
                await File.WriteAllBytesAsync( ConfigFilePath, contents ).ConfigureAwait( false );
                Logger.Info( $"[Profile]Switch: {name}" );
            }
            catch ( Exception ex )
            {
                Logger.Error( "Failed to save Settings: " + ex.Message );
            }

            ProfileChanged?.Invoke( null, null );
        }

        public static event EventHandler<EventArgs>? ProfileChanged;

        public static void SaveCluster( Cluster cluster )
        {
            SaveProfile( Path.Combine( ProfileFolder, Configs.CurrentProfileName + Settings.ClusterFileExt ), cluster );
        }

        private static Cluster? ReadCluster()
        {
            return ReadProfile<Cluster>( Path.Combine( ProfileFolder, Configs.CurrentProfileName + Settings.ClusterFileExt ) );
        }

        private static async void SaveProfile<T>( string path, T p )
        {
            try
            {
                var content = JsonSerializer.SerializeToUtf8Bytes( p, JsonWriteOptions );
                await File.WriteAllBytesAsync( path, content );

                Logger.Info( $"[Profile]{typeof( T ).Name}.{Configs.CurrentProfileName} Saved." );
            }
            catch ( Exception ex )
            {
                Logger.Error( $"Failed to save profile of {typeof( T ).Name}: {ex.Message}" );
            }
        }

        private static T? ReadProfile<T>( string path )
        {
            if ( !File.Exists( path ) )
                return default;
            try
            {
                using var fs     = new FileStream( path, FileMode.Open, FileAccess.Read );
                var       buffer = new byte[fs.Length];
                _ = fs.Read( buffer, 0, (int)fs.Length );
                var utf8Reader = new Utf8JsonReader( buffer );
                return JsonSerializer.Deserialize<T>( ref utf8Reader );
            }
            catch ( Exception ex )
            {
                Logger.Error( $"Failed to read profile of {typeof( T ).Name}: {ex.Message}" );
                return default;
            }
        }

        public static async void DeleteFilesOfProfile( string profileName )
        {
            var dir = new DirectoryInfo( ProfileFolder );
            await Task.Run( () =>
            {
                try
                {
                    foreach ( var file in dir.EnumerateFiles( profileName + ".*" ) ) // such violent
                    {
                        file.Delete();
                    }
                }
                catch ( Exception ex )
                {
                    Logger.Error( $"Failed to delete related files of profile {profileName}: {ex.Message}" );
                }
            } );
        }

        public static void SetConfigRoot( string path )
        {
            using var vsReg = Registry.CurrentUser.CreateSubKey( Const.Reg.RegKeyApp );
            vsReg.SetValue( Const.Reg.RegKeyConfigRoot, path );
            ConfigRootFolder = path;
        }

        private static string GetConfigRoot()
        {
            using var vsReg         = Registry.CurrentUser.CreateSubKey( Const.Reg.RegKeyApp );
            var       configRootReg = vsReg.GetValue( Const.Reg.RegKeyConfigRoot );
            if ( configRootReg is null || !Directory.Exists( configRootReg.ToString() ) )
            {
                return AppRootFolder;
            }

            return configRootReg.ToString();
        }

        private static void CheckFolders()
        {
            ProfileFolder = Path.Combine( ConfigRootFolder, Settings.ProfilesFolder );
            Directory.CreateDirectory( ProfileFolder );

            CacheFolder = Path.Combine( AppRootFolder, Settings.CacheFolder );
            Directory.CreateDirectory( CacheFolder );

            PluginsFolder = Path.Combine( AppRootFolder, Settings.PluginsFolder );
            Directory.CreateDirectory( PluginsFolder );
        }

        public static string GetRuleFilePath( string? profile = null )
        {
            CheckFolders();
            return string.IsNullOrEmpty( profile )
                ? Path.Combine( ProfileFolder, Configs.CurrentProfileName + Settings.RuleFileExt )
                : Path.Combine( ProfileFolder, profile + Settings.RuleFileExt );
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

        private static readonly JsonSerializerOptions JsonWriteOptions = new() {WriteIndented = true};
    }
}