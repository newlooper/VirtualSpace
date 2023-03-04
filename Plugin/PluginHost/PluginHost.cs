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
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;

namespace VirtualSpace.Plugin
{
    public static class PluginHost
    {
        public static readonly List<PluginInfo> Plugins = new();

        public static readonly Dictionary<string, uint> CareAboutMessages = new()
        {
            {PluginConst.DirectInputNotificationMsgString, 0},
            {PluginConst.HotPlugDetected, 0}
        };

        /// <summary>
        /// 此处注册的是插件的静态信息；
        /// 插件的运行时信息，则在插件启动后通过 IPC 自行报道给宿主
        /// </summary>
        /// <param name="pluginsPath"></param>
        public static void RegisterPlugins( string pluginsPath )
        {
            var pluginFolders = Directory.GetDirectories( pluginsPath );
            foreach ( var path in pluginFolders )
            {
                var infoFile = Path.Combine( path, PluginManager.PluginInfoFile );
                if ( !File.Exists( infoFile ) )
                {
                    Logger.Warning( $"[PLUGIN] missing {PluginManager.PluginInfoFile} in {path}" );
                    continue;
                }

                var pluginInfo = PluginManager.LoadFromJson<PluginInfo>( infoFile );
                if ( pluginInfo == null )
                {
                    Logger.Warning( $"[PLUGIN] invalid {PluginManager.PluginInfoFile}" );
                    continue;
                }

                var alreadyLoaded = Plugins.Find( p => p.Name == pluginInfo.Name );
                if ( alreadyLoaded != null ) continue;

                if ( pluginInfo.Requirements is null )
                {
                    Logger.Warning( $"[PLUGIN] {pluginInfo.Display} has no 'Requirements' info" );
                    continue;
                }

                if ( pluginInfo.Requirements.HostVersion == null ||
                     pluginInfo.Requirements.HostVersion > GetHostVersion() )
                {
                    Logger.Warning( $"[PLUGIN] {pluginInfo.Display} not satisfy the host version" );
                    continue;
                }

                pluginInfo.Folder = path;

                Plugins.Add( pluginInfo );
                Logger.Info( $"[PLUGIN] {pluginInfo.Display} Registered." );

                if ( pluginInfo is {AutoStart: true, AutoStartTiming: AutoStartTiming.AppStart} )
                    StartPlugin( pluginInfo );
            }
        }

        public static void AutoStartAfterMainWindowLoaded()
        {
            foreach ( var pi in Plugins.Where( pi => pi is {AutoStart: true, AutoStartTiming: AutoStartTiming.MainWindowLoaded} ) )
            {
                StartPlugin( pi );
            }
        }

        private static void StartExe( string exe )
        {
            Task.Run( () => Process.Start( exe ) );
        }

        public static void PluginSettings( PluginInfo pluginInfo )
        {
            WinApi.PostMessage( pluginInfo.Handle, WinApi.UM_PLUGINSETTINGS, 0, 0 );
        }

        public static void StartPlugin( PluginInfo pluginInfo )
        {
            if ( !PluginManager.CheckRequirements( pluginInfo.Requirements ) ) return;
            Logger.Info( $"[PLUGIN.Start] {pluginInfo.Display}" );
            StartExe( Path.Combine( pluginInfo.Folder, pluginInfo.Entry ) );
        }

        public static void ClosePlugin( PluginInfo pluginInfo )
        {
            if ( !string.IsNullOrEmpty( pluginInfo.Display ) )
                Logger.Info( $"[PLUGIN.Close] {pluginInfo.Display}" );
            WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0 );
            // WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_CLOSE, 0, 0 );
            // WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_QUIT, 0, 0 );
            // WinApi.PostMessage( pluginInfo.Handle, WinApi.WM_DESTROY, 0, 0 );
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
                    Thread.Sleep( PluginConst.RestartDelay );
                    Process.Start( exe );
                    Logger.Info( $"[PLUGIN] {pluginInfo.Display} Restarted." );
                } );
            }
            catch
            {
                Logger.Warning( "Failed Restart Plugin, Abort Operation." );
            }
        }

        private static Version GetHostVersion()
        {
            var fileVersion = ( (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetEntryAssembly(),
                typeof( AssemblyFileVersionAttribute ),
                false ) ).Version;
            return new Version( fileVersion );
        }
    }
}