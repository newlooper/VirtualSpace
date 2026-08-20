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
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace VirtualSpace.Plugin
{
    internal sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly string                     _pluginDir;

        public PluginLoadContext( string pluginPath ) : base( isCollectible: true )
        {
            _resolver  = new AssemblyDependencyResolver( pluginPath );
            _pluginDir = Path.GetDirectoryName( pluginPath ) ?? string.Empty;
        }

        protected override Assembly? Load( AssemblyName assemblyName )
        {
            if ( assemblyName.Name is null ) return null;

            if ( IsSharedWithHost( assemblyName.Name ) )
            {
                EnsureLoadedInDefaultContext( assemblyName );
                return null;
            }

            foreach ( var assembly in Default.Assemblies )
            {
                if ( string.Equals( assembly.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase ) )
                    return null;
            }

            var path = _resolver.ResolveAssemblyToPath( assemblyName ) ?? Path.Combine( _pluginDir, assemblyName.Name + ".dll" );
            return File.Exists( path ) ? LoadFromAssemblyPath( path ) : null;
        }

        private static bool IsSharedWithHost( string assemblyName )
        {
            return string.Equals( assemblyName, "VirtualSpace.PluginContracts", StringComparison.OrdinalIgnoreCase )
                   || string.Equals( assemblyName, "Microsoft.Windows.SDK.NET", StringComparison.OrdinalIgnoreCase )
                   || string.Equals( assemblyName, "WinRT.Runtime", StringComparison.OrdinalIgnoreCase )
                   || string.Equals( assemblyName, "MaterialDesignThemes.Wpf", StringComparison.OrdinalIgnoreCase )
                   || string.Equals( assemblyName, "MaterialDesignColors", StringComparison.OrdinalIgnoreCase )
                   || string.Equals( assemblyName, "Microsoft.Xaml.Behaviors", StringComparison.OrdinalIgnoreCase );
        }

        private void EnsureLoadedInDefaultContext( AssemblyName assemblyName )
        {
            if ( string.Equals( assemblyName.Name, "Microsoft.Windows.SDK.NET", StringComparison.OrdinalIgnoreCase ) )
                EnsureLoadedInDefaultContext( new AssemblyName( "WinRT.Runtime" ) );

            if ( string.Equals( assemblyName.Name, "MaterialDesignThemes.Wpf", StringComparison.OrdinalIgnoreCase ) )
            {
                EnsureLoadedInDefaultContext( new AssemblyName( "MaterialDesignColors" ) );
                EnsureLoadedInDefaultContext( new AssemblyName( "Microsoft.Xaml.Behaviors" ) );
            }

            foreach ( var assembly in Default.Assemblies )
            {
                if ( string.Equals( assembly.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase ) )
                    return;
            }

            var hostPath = Path.Combine( AppContext.BaseDirectory, assemblyName.Name + ".dll" );
            if ( File.Exists( hostPath ) )
            {
                Default.LoadFromAssemblyPath( hostPath );
                return;
            }

            var path = _resolver.ResolveAssemblyToPath( assemblyName );
            if ( path is null ) return;

            Default.LoadFromAssemblyPath( path );
        }

        protected override IntPtr LoadUnmanagedDll( string unmanagedDllName )
        {
            var path = _resolver.ResolveUnmanagedDllToPath( unmanagedDllName )
                       ?? Path.Combine( _pluginDir, unmanagedDllName );
            return File.Exists( path ) ? LoadUnmanagedDllFromPath( path ) : IntPtr.Zero;
        }
    }
}
