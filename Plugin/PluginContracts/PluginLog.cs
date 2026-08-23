// Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of VirtualSpace.
//
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;

namespace VirtualSpace.PluginContracts
{
    /// <summary>
    /// Host binds these to file logging at startup.
    /// </summary>
    public static class PluginLog
    {
        private static Action<string>? _writeEvent;
        private static Action<string>? _writeError;

        public static void Bind( Action<string> writeEvent, Action<string> writeError )
        {
            _writeEvent = writeEvent;
            _writeError = writeError;
        }

        public static void Event( string source, string message ) =>
            _writeEvent?.Invoke( $"[Plugin.{source}] {message}" );

        public static void Error( string source, string message ) =>
            _writeError?.Invoke( $"[Plugin.{source}] {message}" );
    }
}