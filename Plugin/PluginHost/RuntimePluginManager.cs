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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using VirtualSpace.PluginContracts;

namespace VirtualSpace.Plugin
{
    public sealed class RuntimePluginManager
    {
        public static RuntimePluginManager Instance { get; } = new();

        public List<PluginInfo> Plugins     { get; } = new();
        public HostContext      HostContext { get; } = new();

        public event Action? PluginsChanged;

        public void Initialize( string? legacyPluginsPath = null )
        {
            HostContext.HostVersion = GetHostVersion();
            ScanDllPlugins( PluginPaths.GetHostPluginsDirectory(), disturbLoaded: false );

            if ( !string.IsNullOrEmpty( legacyPluginsPath ) )
                ScanExePlugins( legacyPluginsPath );

            ScanExePlugins( PluginPaths.GetHostPluginsDirectory() );
            PluginsChanged?.Invoke();
        }

        public void Refresh()
        {
            ScanDllPlugins( PluginPaths.GetHostPluginsDirectory(), disturbLoaded: false );
            PluginsChanged?.Invoke();
        }

        public void AutoStart( AutoStartTiming timing )
        {
            foreach ( var pi in Plugins.Where( p => p.AutoStart && p.AutoStartTiming == timing && !p.IsLoaded ).ToList() )
                Start( pi );
        }

        public async Task AutoStartAsync( AutoStartTiming timing )
        {
            foreach ( var pi in Plugins.Where( p => p.AutoStart && p.AutoStartTiming == timing && !p.IsLoaded ).ToList() )
            {
                if ( pi.Kind == PluginKind.InProcess )
                    await PluginLoader.LoadAsync( pi, HostContext ).ConfigureAwait( true );
                else
                    Start( pi );
            }

            PluginsChanged?.Invoke();
        }

        public void Start( PluginInfo pluginInfo )
        {
            if ( pluginInfo.Kind == PluginKind.InProcess )
            {
                PluginLoader.Load( pluginInfo, HostContext );
                return;
            }

            if ( !PluginManager.CheckRequirements( pluginInfo.Requirements ) ) return;
            Logger.Info( $"[PLUGIN.Start] {pluginInfo.Display}" );
            StartExe( Path.Combine( pluginInfo.Folder, pluginInfo.Entry ) );
        }

        public void Close( PluginInfo pluginInfo )
        {
            if ( pluginInfo.Kind == PluginKind.InProcess )
            {
                PluginLoader.Unload( pluginInfo, HostContext );
                return;
            }

            if ( !string.IsNullOrEmpty( pluginInfo.Display ) )
                Logger.Info( $"[PLUGIN.Close] {pluginInfo.Display}" );
            User32.PostMessage( pluginInfo.Handle, WinMsg.WM_CLOSE, 0, 0 );
        }

        public void Restart( PluginInfo pluginInfo )
        {
            if ( pluginInfo.Kind == PluginKind.InProcess )
            {
                PluginLoader.Unload( pluginInfo, HostContext );
                PluginLoader.Load( pluginInfo, HostContext );
                Logger.Info( $"[PLUGIN] {pluginInfo.Display} Restarted." );
                return;
            }

            try
            {
                User32.PostMessage( pluginInfo.Handle, WinMsg.UM_RESTART, 0, PluginConst.RestartDelay );
                Logger.Info( $"[PLUGIN] {pluginInfo.Display} Restarted." );
            }
            catch ( Exception ex )
            {
                Logger.Warning( "Failed Restart Plugin, Abort Operation." );
                Logger.Warning( ex.Message );
            }
        }

        public void ShowSettings( PluginInfo pluginInfo )
        {
            if ( pluginInfo.Kind == PluginKind.InProcess )
            {
                PluginLoader.GetInstance( pluginInfo.Name )?.ShowSettings();
                return;
            }

            User32.PostMessage( pluginInfo.Handle, WinMsg.UM_PLUGINSETTINGS, 0, 0 );
        }

        public void CloseAll()
        {
            foreach ( var pluginInfo in Plugins.ToList() )
                Close( pluginInfo );
        }

        public void Publish( string eventName, object payload )
        {
            HostContext.Publish( eventName, payload );
        }

        private void ScanDllPlugins( string pluginsRoot, bool disturbLoaded )
        {
            var discovered = PluginLoader.ScanMetadata( pluginsRoot );
            var hostVer    = HostContext.HostVersion;

            foreach ( var existing in Plugins.Where( p => p.Kind == PluginKind.InProcess ).ToList() )
            {
                var match = discovered.FirstOrDefault( d => d.Name == existing.Name );
                if ( match is null )
                {
                    existing.LoadStatus = PluginLoadStatus.Missing;
                    continue;
                }

                if ( existing.IsLoaded && !disturbLoaded )
                {
                    existing.FileHash     = match.FileHash;
                    existing.AssemblyPath = match.AssemblyPath;
                    existing.Folder       = match.Folder;
                    continue;
                }

                CopyDiscoveredMetadata( existing, match );
            }

            foreach ( var info in discovered )
            {
                if ( Plugins.Any( p => p.Name == info.Name ) ) continue;
                if ( info.Requirements?.HostVersion != null && info.Requirements.HostVersion > hostVer )
                {
                    Logger.Warning( $"[PLUGIN] {info.Display} not satisfy the host version" );
                    continue;
                }

                Plugins.Add( info );
                Logger.Info( $"[PLUGIN] {info.Display} Registered." );
            }
        }

        private static void CopyDiscoveredMetadata( PluginInfo target, PluginInfo source )
        {
            target.Display      = source.Display;
            target.Version      = source.Version;
            target.Description  = source.Description;
            target.Author       = source.Author;
            target.Email        = source.Email;
            target.Type         = source.Type;
            target.Folder       = source.Folder;
            target.AssemblyPath = source.AssemblyPath;
            target.Entry        = source.Entry;
            target.FileHash     = source.FileHash;
            target.Kind         = PluginKind.InProcess;
            if ( !target.IsLoaded )
                target.LoadStatus = PluginLoadStatus.Available;
        }

        private void ScanExePlugins( string pluginsPath )
        {
            if ( !Directory.Exists( pluginsPath ) ) return;

            foreach ( var path in Directory.GetDirectories( pluginsPath ) )
            {
                var infoFile = Path.Combine( path, PluginManager.PluginInfoFile );
                if ( !File.Exists( infoFile ) ) continue;

                var pluginInfo = PluginManager.LoadFromJson<PluginInfo>( infoFile );
                if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Entry ) ) continue;
                if ( !pluginInfo.Entry.EndsWith( ".exe", StringComparison.OrdinalIgnoreCase ) ) continue;
                if ( Plugins.Any( p => p.Name == pluginInfo.Name ) ) continue;

                if ( pluginInfo.Requirements is null )
                {
                    Logger.Warning( $"[PLUGIN] {pluginInfo.Display} has no 'Requirements' info" );
                    continue;
                }

                if ( pluginInfo.Requirements.HostVersion == null ||
                     pluginInfo.Requirements.HostVersion > HostContext.HostVersion )
                {
                    Logger.Warning( $"[PLUGIN] {pluginInfo.Display} not satisfy the host version" );
                    continue;
                }

                pluginInfo.Folder     = path;
                pluginInfo.Kind       = PluginKind.ExternalProcess;
                pluginInfo.LoadStatus = PluginLoadStatus.Available;
                OverlayPersisted( pluginInfo );
                Plugins.Add( pluginInfo );
                Logger.Info( $"[PLUGIN] {pluginInfo.Display} Registered." );
            }
        }

        private static void OverlayPersisted( PluginInfo info )
        {
            var persisted = PluginManager.LoadPersistedPluginInfo( info.Name );
            if ( persisted is null ) return;
            info.AutoStart       = persisted.AutoStart;
            info.AutoStartTiming = persisted.AutoStartTiming;
            if ( persisted.RestartPolicy is not null ) info.RestartPolicy = persisted.RestartPolicy;
            if ( persisted.ClosePolicy is not null ) info.ClosePolicy = persisted.ClosePolicy;
        }

        private static void StartExe( string exe )
        {
            System.Threading.Tasks.Task.Run( () => System.Diagnostics.Process.Start( exe ) );
        }

        private static Version GetHostVersion()
        {
            var fileVersion = ( (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly()!,
                typeof( AssemblyFileVersionAttribute ),
                false )! ).Version;
            return new Version( fileVersion );
        }
    }
}
