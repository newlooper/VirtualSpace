/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;

namespace VirtualSpace.Commons
{
    public static class IpcPipe
    {
        private const  string       PIPE_NAME                     = "VIRTUAL_SPACE_IPC_PIPE";
        private const  string       PIPE_SERVER                   = ".";
        private static bool         _isRunning                    = true;
        public const   int          Power                         = 1000;
        public static  List<IntPtr> VirtualDesktopSwitchObservers = new();
        public static  IntPtr       MainWindowHandle { get; set; }

        public static void AsServer()
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
                                Logger.Info( "Virtual Desktop Switch Observer Plugin Registered." );
                                var handle = (IntPtr)int.Parse( msg.Value );
                                if ( !VirtualDesktopSwitchObservers.Contains( handle ) )
                                    VirtualDesktopSwitchObservers.Add( handle );
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

            foreach ( var hWnd in VirtualDesktopSwitchObservers )
            {
                WinApi.PostMessage( hWnd, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0 );
                WinApi.PostMessage( hWnd, WinApi.WM_CLOSE, 0, 0 );
                WinApi.PostMessage( hWnd, WinApi.WM_QUIT, 0, 0 );
                WinApi.PostMessage( hWnd, WinApi.WM_DESTROY, 0, 0 );
            }
        }

        public static bool RegisterVdSwitchObserver( string handle )
        {
            using var client = new NamedPipeClientStream( PIPE_SERVER, PIPE_NAME, PipeDirection.InOut, PipeOptions.None );
            try
            {
                client.Connect( 1000 );
            }
            catch
            {
                return false;
            }

            if ( client.IsConnected )
            {
                using var writer = new StreamWriter( client );
                var       msg    = new PipeMessage {Type = PipeMessageType.PLUGIN_VD_SWITCH_OBSERVER, Value = handle};
                writer.WriteLine( JsonSerializer.Serialize( msg ) );
                writer.Flush();
                return true;
            }

            return false;
        }
    }

    public static class WinApi
    {
        public const int WM_SYSCOMMAND    = 0x0112;
        public const int SC_MAXIMIZE      = 0xF030;
        public const int SC_MINIMIZE      = 0xF020;
        public const int SC_RESTORE       = 0xF120;
        public const int SC_SIZE          = 0xF000;
        public const int SC_MOVE          = 0xF010;
        public const int SC_CLOSE         = 0xF060;
        public const int WM_HOTKEY        = 0x0312;
        public const int WM_CLOSE         = 0x0010;
        public const int WM_QUIT          = 0x0012;
        public const int WM_DESTROY       = 0x0002;
        public const int WM_MOUSEACTIVATE = 0x0021;
        public const int MA_NOACTIVATE    = 0x3;

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool PostMessage( IntPtr hWnd, int msg, uint wParam, uint lParam );
    }
}