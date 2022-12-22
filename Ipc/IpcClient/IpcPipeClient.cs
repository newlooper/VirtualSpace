/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading.Tasks;

namespace VirtualSpace.Commons
{
    public static class IpcPipeClient
    {
        private const string PIPE_NAME   = Config.PIPE_NAME;
        private const string PIPE_SERVER = Config.PIPE_SERVER;

        private static bool CheckIn( PipeMessageType pmt, string name, IntPtr handle, int pId )
        {
            using var client = new NamedPipeClientStream( PIPE_SERVER, PIPE_NAME, PipeDirection.InOut, PipeOptions.None );
            try
            {
                client.Connect( 1000 );
                if ( client.IsConnected )
                {
                    using var writer = new StreamWriter( client );
                    var       msg    = new PipeMessage {Type = pmt, Handle = handle.ToInt32(), ProcessId = pId, Name = name};
                    writer.WriteLine( JsonSerializer.Serialize( msg ) );
                    writer.Flush();
                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static bool AskAlive( string name, IntPtr handle, int pId )
        {
            using var client = new NamedPipeClientStream( PIPE_SERVER, PIPE_NAME, PipeDirection.InOut, PipeOptions.None );
            try
            {
                client.Connect( 1000 );
                if ( client.IsConnected )
                {
                    using var writer = new StreamWriter( client );
                    var       msg    = new PipeMessage {Type = PipeMessageType.PLUGIN_CHECK_ALIVE, Handle = handle.ToInt32(), ProcessId = pId, Name = name};
                    writer.WriteLine( JsonSerializer.Serialize( msg ) );
                    writer.Flush();
                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        private static async void CheckAlive( string name, IntPtr handle, int pId, int interval, Action exit )
        {
            while ( AskAlive( name, handle, pId ) )
            {
                await Task.Delay( interval * 1000 );
            }

            exit();
        }

        public static void PluginCheckIn( PipeMessageType pmt, string name, IntPtr handle, int interval, Action error, Action exit )
        {
            var pId = Process.GetCurrentProcess().Id;
            if ( !CheckIn( pmt, name, handle, pId ) )
            {
                error();
                exit();
            }

            CheckAlive( name, handle, pId, interval, exit );
        }
    }
}