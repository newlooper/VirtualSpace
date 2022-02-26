/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;

namespace VirtualSpace.Commons
{
    public static class IpcPipe
    {
        private const  string PIPE_NAME   = "VIRTUAL_SPACE_IPC_PIPE";
        private const  string PIPE_SERVER = ".";
        private static bool   _isRunning  = true;

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
                                MainWindow.DelegateBringToTop();
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
        }
    }
}