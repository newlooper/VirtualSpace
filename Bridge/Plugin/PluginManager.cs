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
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;

namespace VirtualSpace.Plugin
{
    public static class PluginManager
    {
        public static readonly List<PluginInfo> Plugins = new();

        public static void RegisterPlugins( string pluginsPath, string pluginInfoFile )
        {
            var pluginFolders = Directory.GetDirectories( pluginsPath );
            foreach ( var path in pluginFolders )
            {
                var infoFile = Path.Combine( path, pluginInfoFile );
                if ( !File.Exists( infoFile ) ) continue;

                var pluginInfo = LoadFromJson( infoFile );
                if ( pluginInfo == null ) continue;

                var loaded = Plugins.Find( p => p.Name == pluginInfo.Name );
                if ( loaded != null ) continue;

                if ( CheckRequirements( pluginInfo.Requirements ) && pluginInfo.AutoStart )
                {
                    Logger.Info( $"Auto Start Plugin: {pluginInfo.Display}" );
                    var exe = Path.Combine( path, pluginInfo.Entry );
                    StartExe( exe );
                }

                Plugins.Add( pluginInfo );
            }
        }

        public static PluginInfo? LoadFromJson( string infoFile )
        {
            using var fs     = new FileStream( infoFile, FileMode.Open, FileAccess.ReadWrite );
            var       buffer = new byte[fs.Length];
            fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );
            return JsonSerializer.Deserialize<PluginInfo>( ref utf8Reader );
        }

        private static Process StartExe( string exe )
        {
            return Process.Start( exe );
        }

        public static void ClosePlugin( PluginInfo pluginInfo )
        {
            WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0 );
            WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_CLOSE, 0, 0 );
            WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_QUIT, 0, 0 );
            WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_DESTROY, 0, 0 );
        }

        public static void RestartPlugin( PluginInfo pluginInfo )
        {
            try
            {
                using var process = Process.GetProcessById( pluginInfo.ProcessId );
                var       exe     = process.MainModule?.FileName;
                ClosePlugin( pluginInfo );
                Task.Run( () =>
                {
                    Thread.Sleep( 5000 );
                    StartExe( exe );
                    Logger.Info( $"Plugin ({pluginInfo.Display}) Restarted." );
                } );
            }
            catch
            {
                Logger.Warning( "Failed Restart Plugin, Abort Operation." );
            }
        }

        public static bool CheckRequirements( Requirements? req )
        {
            var check   = false;
            var version = Environment.OSVersion.Version;

            if ( version.Major >= req?.WinVer.Min.Major && version.Build >= req.WinVer.Min.Build )
                check = true;

            if ( req?.WinVer.Max != null && ( version.Major > req.WinVer.Max.Major || version.Build > req.WinVer.Max.Build ) )
                check = false;

            return check;
        }
    }
}