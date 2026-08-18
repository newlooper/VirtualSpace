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
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.PluginContracts;

namespace VirtualSpace.Plugin
{
    internal sealed class LoadedPlugin
    {
        public PluginInfo        Info     { get; init; } = null!;
        public IPlugin           Instance { get; set; }  = null!;
        public PluginLoadContext Context  { get; init; } = null!;
    }

    public static class PluginLoader
    {
        internal static IPlugin? ActivePlugin { get; private set; }

        private static readonly Dictionary<string, LoadedPlugin> Loaded = new();

        public static bool IsLoaded( string pluginName ) => Loaded.ContainsKey( pluginName );

        public static IPlugin? GetInstance( string pluginName )
        {
            return Loaded.TryGetValue( pluginName, out var loaded ) ? loaded.Instance : null;
        }

        public static string ComputeFileHash( string path )
        {
            using var sha = SHA256.Create();
            using var fs  = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete );
            return Convert.ToHexString( sha.ComputeHash( fs ) );
        }

        public static IReadOnlyList<PluginInfo> ScanMetadata( string pluginsRoot )
        {
            var result = new List<PluginInfo>();
            if ( !Directory.Exists( pluginsRoot ) ) return result;

            foreach ( var folder in Directory.GetDirectories( pluginsRoot ) )
            {
                try
                {
                    var info = TryScanFolder( folder );
                    if ( info is null ) continue;
                    result.Add( info );
                }
                catch ( Exception ex )
                {
                    Logger.Warning( $"[PLUGIN] skip {folder}: {ex.Message}" );
                }
            }

            return result;
        }

        private static PluginInfo? TryScanFolder( string folder )
        {
            foreach ( var dll in EnumerateCandidateDlls( folder ) )
            {
                var info = TryCloneLoadedMetadata( dll, folder ) ?? TryReadMetadata( dll );
                if ( info is null ) continue;

                info.Folder       = folder;
                info.AssemblyPath = dll;
                info.Entry        = Path.GetFileName( dll );
                info.Kind         = PluginKind.InProcess;
                info.FileHash     = ComputeFileHash( dll );
                if ( !info.IsLoaded )
                    info.LoadStatus = PluginLoadStatus.Available;
                OverlayPersistedSettings( info );
                PluginManager.EnsureDataFiles( info );
                return info;
            }

            return null;
        }

        private static IEnumerable<string> EnumerateCandidateDlls( string folder )
        {
            var preferred = Path.Combine( folder, Path.GetFileName( folder ) + ".dll" );
            if ( File.Exists( preferred ) )
                yield return preferred;

            foreach ( var dll in Directory.GetFiles( folder, "*.dll" ) )
            {
                if ( dll.EndsWith( ".resources.dll", StringComparison.OrdinalIgnoreCase ) ) continue;
                if ( string.Equals( dll, preferred, StringComparison.OrdinalIgnoreCase ) ) continue;
                yield return dll;
            }
        }

        private static PluginInfo? TryCloneLoadedMetadata( string dllPath, string folder )
        {
            foreach ( var loaded in Loaded.Values )
            {
                if ( !string.Equals( loaded.Info.AssemblyPath, dllPath, StringComparison.OrdinalIgnoreCase ) )
                    continue;

                return new PluginInfo
                {
                    Name            = loaded.Info.Name,
                    Display         = loaded.Info.Display,
                    Version         = loaded.Info.Version,
                    Description     = loaded.Info.Description,
                    Author          = loaded.Info.Author,
                    Email           = loaded.Info.Email,
                    Type            = loaded.Info.Type,
                    AutoStart       = loaded.Info.AutoStart,
                    AutoStartTiming = loaded.Info.AutoStartTiming,
                    Requirements    = loaded.Info.Requirements,
                    Folder          = folder,
                    AssemblyPath    = dllPath,
                    Entry           = Path.GetFileName( dllPath ),
                    Kind            = PluginKind.InProcess,
                    IsLoaded        = true,
                    LoadStatus      = PluginLoadStatus.Loaded
                };
            }

            return null;
        }

        private static PluginInfo? TryReadMetadata( string dllPath )
        {
            var alc = new MetadataScanLoadContext();
            try
            {
                Assembly assembly;
                using ( var fs = File.OpenRead( dllPath ) )
                {
                    assembly = alc.LoadFromStream( fs );
                }

                var attr = assembly.GetCustomAttribute<PluginMetadataAttribute>();
                if ( attr is null ) return null;

                return new PluginInfo
                {
                    Name            = attr.Name,
                    Display         = attr.Display,
                    Version         = attr.Version,
                    Description     = attr.Description,
                    Author          = attr.Author,
                    Email           = attr.Email,
                    Type            = attr.Type,
                    AutoStart       = attr.DefaultAutoStart,
                    AutoStartTiming = attr.DefaultAutoStartTiming,
                    Requirements    = attr.ToRequirements()
                };
            }
            catch ( Exception ex )
            {
                Logger.Warning( $"[PLUGIN] skip {dllPath}: {ex.Message}" );
                return null;
            }
            finally
            {
                alc.Unload();
            }
        }

        private static void OverlayPersistedSettings( PluginInfo info )
        {
            var persisted = PluginManager.LoadPersistedPluginInfo( info.Name );
            if ( persisted is null ) return;

            info.AutoStart       = persisted.AutoStart;
            info.AutoStartTiming = persisted.AutoStartTiming;
            if ( persisted.RestartPolicy is not null ) info.RestartPolicy = persisted.RestartPolicy;
            if ( persisted.ClosePolicy is not null ) info.ClosePolicy = persisted.ClosePolicy;
        }

        public static bool Load( PluginInfo info, IHostContext hostContext )
        {
            if ( info.Kind != PluginKind.InProcess ) return false;
            if ( Loaded.ContainsKey( info.Name ) ) return true;
            if ( string.IsNullOrEmpty( info.AssemblyPath ) || !File.Exists( info.AssemblyPath ) )
            {
                info.LoadStatus = PluginLoadStatus.Missing;
                return false;
            }

            if ( !PluginManager.CheckRequirements( info.Requirements ) )
            {
                Logger.Warning( $"[PLUGIN] {info.Display} does not meet OS requirements." );
                info.LoadStatus = PluginLoadStatus.Error;
                return false;
            }

            try
            {
                var prepared = Prepare( info );
                return CompleteLoad( info, hostContext, prepared );
            }
            catch ( Exception ex )
            {
                return FailLoad( info, ex, null );
            }
        }

        public static async Task<bool> LoadAsync( PluginInfo info, IHostContext hostContext )
        {
            if ( info.Kind != PluginKind.InProcess ) return false;
            if ( Loaded.ContainsKey( info.Name ) ) return true;
            if ( string.IsNullOrEmpty( info.AssemblyPath ) || !File.Exists( info.AssemblyPath ) )
            {
                info.LoadStatus = PluginLoadStatus.Missing;
                return false;
            }

            if ( !PluginManager.CheckRequirements( info.Requirements ) )
            {
                Logger.Warning( $"[PLUGIN] {info.Display} does not meet OS requirements." );
                info.LoadStatus = PluginLoadStatus.Error;
                return false;
            }

            PreparedPlugin prepared;
            try
            {
                prepared = await Task.Run( () => Prepare( info ) ).ConfigureAwait( true );
            }
            catch ( Exception ex )
            {
                return FailLoad( info, ex, null );
            }

            try
            {
                return CompleteLoad( info, hostContext, prepared );
            }
            catch ( Exception ex )
            {
                return FailLoad( info, ex, prepared.Context );
            }
        }

        private readonly struct PreparedPlugin
        {
            public IPlugin           Instance { get; init; }
            public PluginLoadContext Context  { get; init; }
        }

        private static PreparedPlugin Prepare( PluginInfo info )
        {
            var alc = new PluginLoadContext( info.AssemblyPath );
            try
            {
                var assembly = alc.LoadFromAssemblyPath( info.AssemblyPath );
                var pluginType = FindPluginType( assembly );
                if ( pluginType is null )
                    throw new InvalidOperationException( $"No {nameof( IPlugin )} implementation in {info.Entry}" );

                var instance = (IPlugin)Activator.CreateInstance( pluginType )!;
                return new PreparedPlugin { Instance = instance, Context = alc };
            }
            catch
            {
                alc.Unload();
                throw;
            }
        }

        private static bool CompleteLoad( PluginInfo info, IHostContext hostContext, PreparedPlugin prepared )
        {
            try
            {
                ActivePlugin = prepared.Instance;
                prepared.Instance.Initialize( hostContext );

                Loaded[info.Name] = new LoadedPlugin
                {
                    Info     = info,
                    Instance = prepared.Instance,
                    Context  = prepared.Context
                };
                info.IsLoaded   = true;
                info.LoadStatus = PluginLoadStatus.Loaded;
                info.Type       = prepared.Instance.Type;
                PluginManager.EnsureDataFiles( info );
                Logger.Info( $"[PLUGIN.Load] {info.Display}" );
                return true;
            }
            finally
            {
                ActivePlugin = null;
            }
        }

        private static bool FailLoad( PluginInfo info, Exception ex, PluginLoadContext? alc )
        {
            Logger.Warning( $"[PLUGIN] failed to load {info.Display}: {ex.Message}" );
            info.LoadStatus = PluginLoadStatus.Error;
            alc?.Unload();
            return false;
        }

        public static void Unload( PluginInfo info, HostContext hostContext )
        {
            if ( !Loaded.TryGetValue( info.Name, out var loaded ) ) return;

            try
            {
                loaded.Instance.Shutdown();
            }
            catch ( Exception ex )
            {
                Logger.Warning( $"[PLUGIN] Shutdown {info.Display} failed: {ex.Message}" );
            }

            hostContext.UnsubscribeAll( loaded.Instance );

            var alc = loaded.Context;
            loaded.Instance = null!;
            Loaded.Remove( info.Name );
            alc.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            info.IsLoaded   = false;
            info.LoadStatus = File.Exists( info.AssemblyPath ) ? PluginLoadStatus.Available : PluginLoadStatus.Missing;
            Logger.Info( $"[PLUGIN.Unload] {info.Display}" );
        }

        private static Type? FindPluginType( Assembly assembly )
        {
            try
            {
                return assembly.GetExportedTypes().FirstOrDefault( t =>
                    typeof( IPlugin ).IsAssignableFrom( t ) && t is { IsAbstract: false, IsInterface: false } );
            }
            catch ( ReflectionTypeLoadException ex )
            {
                return ex.Types.FirstOrDefault( t =>
                    t is not null && typeof( IPlugin ).IsAssignableFrom( t ) && t is { IsAbstract: false, IsInterface: false } );
            }
        }

        private sealed class MetadataScanLoadContext : AssemblyLoadContext
        {
            public MetadataScanLoadContext() : base( "PluginMetadataScan-" + Guid.NewGuid().ToString( "N" ), isCollectible: true )
            {
            }

            protected override Assembly? Load( AssemblyName assemblyName )
            {
                return null;
            }
        }
    }
}
