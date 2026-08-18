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

namespace VirtualSpace.PluginContracts
{
    public static class PluginPaths
    {
        private static string? _dataRoot;

        public static void SetDataRoot( string path )
        {
            _dataRoot = string.IsNullOrWhiteSpace( path ) ? null : path;
        }

        public static string GetPluginDataDirectory( string pluginName )
        {
            return Path.Combine( GetDataRoot(), pluginName );
        }

        private static string GetHostPluginsDirectory()
        {
            return Path.Combine( GetInstallDirectory(), AppIdentity.PluginsFolder );
        }

        public static IReadOnlyList<string> GetBundledPluginDirectories()
        {
            var roots      = new List<string>();
            var besideHost = GetHostPluginsDirectory();
            if ( Directory.Exists( besideHost ) )
                roots.Add( besideHost );

            if ( roots.Count == 0 )
                roots.Add( besideHost );

            return roots;
        }

        private static string GetInstallDirectory()
        {
            var fromExe = Path.GetDirectoryName( Environment.ProcessPath );
            return !string.IsNullOrEmpty( fromExe )
                ? fromExe
                : AppContext.BaseDirectory.TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
        }

        private static string GetDataRoot()
        {
            if ( !string.IsNullOrEmpty( _dataRoot ) )
                return _dataRoot;

            return Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                AppIdentity.OrganizationName,
                AppIdentity.AppName,
                AppIdentity.PluginsFolder );
        }
    }
}