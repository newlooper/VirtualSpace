/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.DataAnnotations;
using VirtualSpace.Config.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Config.Profiles;
using Settings = VirtualSpace.Config.Const.Settings;

namespace VirtualSpace.Config;

public static class Manager
{
    private static readonly JsonSerializerOptions JsonWriteOptions = new() { WriteIndented = true };
    private static          string                _cacheFolder     = string.Empty;
    private static          string                _pluginsFolder   = string.Empty;
    private static          string                _configFilePath  = string.Empty;
    public static           ConfigTemplate        Configs          { get; private set; } = null!;
    public static           string                AppPath          => Environment.ProcessPath!;
    public static           string                ProfileFolder    { get; private set; } = string.Empty;
    public static           string                ConfigRootFolder { get; private set; } = string.Empty;
    public static           Profile               CurrentProfile   => Configs.Profiles[Configs.CurrentProfileName];

    public static bool Init()
    {
        try
        {
            _configFilePath = Path.Combine( ConfigRootFolder, Settings.SettingsFile );

            InitConfig( _configFilePath );
            
            CheckFolders();

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
        EnsureProfileFolder();

        if ( File.Exists( filePath ) )
        {
            using var fs     = new FileStream( filePath, FileMode.Open, FileAccess.Read );
            var       buffer = new byte[fs.Length];
            _ = fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );
            Configs = JsonSerializer.Deserialize<ConfigTemplate>( ref utf8Reader ) ?? throw new Exception( $"Failed to deserialize ConfigTemplate from {filePath}" );
            var cluster = ReadCluster();
            if ( cluster is not null )
            {
                Configs.Cluster = cluster;
            }
            else if ( TryReadLegacyCluster( buffer ) is { } legacyCluster )
            {
                Configs.Cluster = legacyCluster;
                SaveCluster( Configs.Cluster );
            }

            PropertyProtector.Walk( Configs );

            ConfigTemplate.ConvertOldMouseAction( Configs );

            Logger.Info( $"Settings File Loaded, Version: {Configs.Version}, Current Profile: {Configs.CurrentProfileName}" );
        }
        else
        {
            Logger.Info( "Settings File Not Found, Create From Default Template." );
            var language = global::VirtualSpace.Agent.ResolveUiLanguage();
            Configs = new ConfigTemplate
            {
                CurrentProfileName = nameof( Default ),
                Version            = Settings.DefaultVersion,
                LogConfig          = new LogConfig { LogLevel = Settings.DefaultLogLevel },
                Profiles = new Dictionary<string, Profile>
                {
                    { nameof( Default ), new Default() }
                },
                MouseActions = MouseAction.Info
            };
            Configs.Profiles[nameof( Default )].UI.Language = language;
            Logger.Info( $"UI Language resolved from system ({CultureInfo.CurrentUICulture.Name}): {language}" );

            PropertyProtector.Walk( Configs );

            Save( filePath, "init", "Setting File", LogLevel.Information );
            SaveCluster( Configs.Cluster );
        }
    }

    public static async void Save( string?            filePath   = null,
        object?                                       reason     = null,
        [CallerArgumentExpression( "reason" )] string reasonName = "",
        LogLevel                                      logLevel   = LogLevel.Debug )
    {
        try
        {
            filePath ??= _configFilePath;
            var contents = JsonSerializer.SerializeToUtf8Bytes( Configs, JsonWriteOptions );
            await File.WriteAllBytesAsync( filePath, contents ).ConfigureAwait( false );

            switch ( logLevel )
            {
                case LogLevel.Information:
                    Logger.Info( $"Settings Saved [{reasonName}: {reason}]." );
                    break;
                case LogLevel.Debug:
                    Logger.Debug( $"Settings Saved [{reasonName}: {reason}]." );
                    break;
                case LogLevel.Trace:
                    Logger.Verbose( $"Settings Saved [{reasonName}: {reason}]." );
                    break;
                case LogLevel.Warning:
                    Logger.Warning( $"Settings Saved [{reasonName}: {reason}]." );
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                case LogLevel.None:
                default:
                    break;
            }
        }
        catch ( Exception ex )
        {
            Logger.Error( "Failed to save Settings: " + ex.Message );
        }
    }

    public static async void SwitchProfile( string name )
    {
        try
        {
            Configs.CurrentProfileName = name;
            var cluster = ReadCluster(); // after CurrentProfileName changed
            if ( cluster is not null )
            {
                Configs.Cluster = cluster;
            }

            Conditions.SwitchRuleProfile(); // after CurrentProfileName changed
            var contents = JsonSerializer.SerializeToUtf8Bytes( Configs, JsonWriteOptions );
            await File.WriteAllBytesAsync( _configFilePath, contents ).ConfigureAwait( false );
            Logger.Info( $"[Profile]Switch: {name}" );

            ProfileChanged?.Invoke( null, EventArgs.Empty );
        }
        catch ( Exception ex )
        {
            Logger.Error( "Failed to save Settings: " + ex.Message );
        }
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

    private static Cluster? TryReadLegacyCluster( byte[] settingsUtf8 )
    {
        try
        {
            using var doc = JsonDocument.Parse( settingsUtf8 );
            if ( !doc.RootElement.TryGetProperty( nameof( ConfigTemplate.Cluster ), out var clusterEl ) )
            {
                return null;
            }

            return JsonSerializer.Deserialize<Cluster>( clusterEl.GetRawText() );
        }
        catch ( Exception ex )
        {
            Logger.Error( $"Failed to migrate Cluster from settings.json: {ex.Message}" );
            return null;
        }
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
        {
            return default;
        }

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

    public static string GetConfigRoot()
    {
        ConfigRootFolder = Path.Combine(
            Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
            Const.OrganizationName,
            Const.AppName
        );

        using var vsReg = Registry.CurrentUser.OpenSubKey( Const.Reg.RegKeyApp );
        if ( vsReg is null )
        {
            return ConfigRootFolder;
        }

        var configRootReg = vsReg.GetValue( Const.Reg.RegKeyConfigRoot ) as string ?? string.Empty;
        if ( configRootReg == string.Empty || !Directory.Exists( configRootReg ) )
        {
            return ConfigRootFolder;
        }

        ConfigRootFolder = configRootReg;

        return ConfigRootFolder;
    }

    private static void EnsureProfileFolder()
    {
        ProfileFolder = Path.Combine( ConfigRootFolder, Settings.ProfilesFolder );
        Directory.CreateDirectory( ProfileFolder );
    }

    private static void CheckFolders()
    {
        EnsureProfileFolder();

        _cacheFolder = Configs.Cluster.UseSystemTempForCache
            ? Path.Combine( Path.GetTempPath(), Const.AppName, Settings.CacheFolder )
            : Path.Combine( ConfigRootFolder, Settings.CacheFolder );
        Directory.CreateDirectory( _cacheFolder );

        _pluginsFolder = Path.Combine( ConfigRootFolder, Settings.PluginsFolder );
        Directory.CreateDirectory( _pluginsFolder );
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
        return _cacheFolder;
    }

    public static string GetPluginsPath()
    {
        CheckFolders();
        return _pluginsFolder;
    }
}