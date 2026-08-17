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
using VirtualSpace.AppLogs;
using VirtualSpace.PluginContracts;

namespace VirtualSpace.Plugin
{
    public sealed class HostContext : IHostContext
    {
        private readonly Dictionary<string, List<(object? Owner, Action<object> Handler)>> _handlers = new();

        public IntPtr  MainWindowHandle { get; set; }
        public Version HostVersion      { get; set; } = new( 0, 0 );

        public event Action<int>? DesktopSwitchRequested;

        public string GetPluginDataPath( string pluginName )
        {
            return PluginPaths.GetPluginDataDirectory( pluginName );
        }

        public void Subscribe( string eventName, Action<object> handler )
        {
            if ( string.IsNullOrEmpty( eventName ) || handler is null ) return;

            if ( !_handlers.TryGetValue( eventName, out var list ) )
            {
                list = new List<(object? Owner, Action<object> Handler)>();
                _handlers[eventName] = list;
            }

            list.Add( ( PluginLoader.ActivePlugin, handler ) );
        }

        public void Unsubscribe( string eventName, Action<object> handler )
        {
            if ( !_handlers.TryGetValue( eventName, out var list ) ) return;
            list.RemoveAll( item => item.Handler == handler );
        }

        public void UnsubscribeAll( object plugin )
        {
            foreach ( var list in _handlers.Values )
                list.RemoveAll( item => ReferenceEquals( item.Owner, plugin ) );
        }

        public void RequestDesktopSwitch( int targetIndex )
        {
            DesktopSwitchRequested?.Invoke( targetIndex );
        }

        public bool HasSubscribers( string eventName )
        {
            return _handlers.TryGetValue( eventName, out var list ) && list.Count > 0;
        }

        public void Publish( string eventName, object payload )
        {
            if ( !_handlers.TryGetValue( eventName, out var list ) || list.Count == 0 ) return;

            foreach ( var item in list.ToArray() )
            {
                try
                {
                    item.Handler( payload );
                }
                catch ( Exception ex )
                {
                    Logger.Warning( $"[PLUGIN] handler for {eventName} failed: {ex.Message}" );
                }
            }
        }
    }
}
