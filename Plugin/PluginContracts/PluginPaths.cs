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

namespace VirtualSpace.PluginContracts
{
    public static class PluginPaths
    {
        public static string GetPluginDataDirectory( string pluginName )
        {
            return Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                AppIdentity.OrganizationName,
                AppIdentity.AppName,
                AppIdentity.PluginsFolder,
                pluginName );
        }

        public static string GetHostPluginsDirectory()
        {
            var appDir = Path.GetDirectoryName( Environment.ProcessPath ) ?? AppContext.BaseDirectory;
            return Path.Combine( appDir, AppIdentity.PluginsFolder );
        }
    }
}
