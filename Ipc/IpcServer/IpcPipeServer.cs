﻿// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
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
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Plugin;

namespace VirtualSpace.Commons
{
    public static class IpcPipeServer
    {
        private const  string PIPE_NAME   = Config.PIPE_NAME;
        private const  string PIPE_SERVER = Config.PIPE_SERVER;
        private static bool   _isRunning  = true;
        public static  IntPtr MainWindowHandle { get; set; }

        public static void Start()
        {
            Task.Factory.StartNew( () =>
            {
                Logger.Info( "Ipc Pipe Server Wait For Connections." );
                while ( _isRunning )
                {
                    using var server = new NamedPipeServerStream( PIPE_NAME );
                    server.WaitForConnection();
                    using var reader = new StreamReader( server );
                    var       line   = reader.ReadLine();
                    if ( line != null )
                    {
                        var msg = JsonSerializer.Deserialize<PipeMessage>( line );
                        switch ( msg?.Type )
                        {
                            case PipeMessageType.INSTANCE:
                                Logger.Info( "Only single instance allowed, just bring to top." );
                                WinApi.PostMessage( MainWindowHandle, WinApi.WM_HOTKEY, UserMessage.RiseView, 0 );
                                break;
                            case PipeMessageType.PLUGIN_VD_SWITCH_OBSERVER:
                                foreach ( var p in PluginManagerServer.Plugins.Where( p => p.Name == msg.Name ) )
                                {
                                    Logger.Info( $"Virtual Desktop Switch Observer Plugin ({p.Display}) Registered." );
                                    p.Handle = (IntPtr)msg.Handle;
                                    p.ProcessId = msg.ProcessId;
                                    p.Type = PluginType.VD_SWITCH_OBSERVER;
                                    break;
                                }

                                break;
                            case PipeMessageType.PLUGIN_CHECK_ALIVE:
                                var runningPlugin = PluginManagerServer.Plugins.Find( p =>
                                    p.Name == msg.Name
                                    && p.Handle == (IntPtr)msg.Handle
                                    && p.ProcessId == msg.ProcessId );
                                if ( runningPlugin == null )
                                {
                                    PluginManagerServer.ClosePlugin( new PluginInfo {Handle = (IntPtr)msg.Handle} );
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    server.Close();
                }

                Logger.Info( "Ipc Pipe Server Shutdown." );
            }, TaskCreationOptions.LongRunning );
        }

        public static void AsClient()
        {
            using var client = new NamedPipeClientStream( PIPE_SERVER, PIPE_NAME, PipeDirection.InOut, PipeOptions.None );
            client.Connect( 3000 );
            using var writer = new StreamWriter( client );
            var       msg    = new PipeMessage {Type = PipeMessageType.INSTANCE};
            writer.WriteLine( JsonSerializer.Serialize( msg ) );
            writer.Flush();
        }

        public static void SimpleShutdown()
        {
            _isRunning = false;
            using var client = new NamedPipeClientStream( PIPE_SERVER, PIPE_NAME, PipeDirection.InOut, PipeOptions.None );
            client.Connect( 1000 );
            client.Close();

            foreach ( var pluginInfo in PluginManagerServer.Plugins )
            {
                PluginManagerServer.ClosePlugin( pluginInfo );
            }
        }
    }
}