// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of VirtualSpace.
//
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using VirtualSpace.PluginContracts;

namespace VirtualSpace.Plugin
{
    public static class PluginHost
    {
        public static readonly Dictionary<string, uint> CareAboutMessages = new()
        {
            { PluginConst.DirectInputNotificationMsgString, 0 },
            { PluginConst.HotPlugDetected, 0 }
        };

        public static List<PluginInfo> Plugins => RuntimePluginManager.Instance.Plugins;

        public static HostContext HostContext => RuntimePluginManager.Instance.HostContext;

        public static void RegisterPlugins( string pluginsPath )
        {
            RuntimePluginManager.Instance.Initialize( pluginsPath );
            RuntimePluginManager.Instance.AutoStart( AutoStartTiming.AppStart );
        }

        public static void RefreshPlugins()
        {
            RuntimePluginManager.Instance.Refresh();
        }

        public static void AutoStartAfterMainWindowLoaded()
        {
            RuntimePluginManager.Instance.AutoStart( AutoStartTiming.MainWindowLoaded );
        }

        public static void PluginSettings( PluginInfo pluginInfo )
        {
            RuntimePluginManager.Instance.ShowSettings( pluginInfo );
        }

        public static void StartPlugin( PluginInfo pluginInfo )
        {
            RuntimePluginManager.Instance.Start( pluginInfo );
        }

        public static void ClosePlugin( PluginInfo pluginInfo )
        {
            RuntimePluginManager.Instance.Close( pluginInfo );
        }

        public static void RestartPlugin( PluginInfo pluginInfo )
        {
            RuntimePluginManager.Instance.Restart( pluginInfo );
        }

        public static void CloseAllPlugins()
        {
            RuntimePluginManager.Instance.CloseAll();
        }

        public static void Publish( string eventName, object payload )
        {
            RuntimePluginManager.Instance.Publish( eventName, payload );
        }
    }
}
