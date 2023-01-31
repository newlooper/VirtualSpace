/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VirtualSpace.Plugin;

namespace VirtualSpace
{
    public partial class AppController
    {
        private static readonly List<PluginInfo> PluginInfos = PluginHost.Plugins;

        private void InitPluginListView()
        {
            ///////////////////////////////////////
            // fix no ColumnHeader name issue
            lvc_PluginName.Name = nameof( lvc_PluginName );
            lvc_PluginVersion.Name = nameof( lvc_PluginVersion );
            lvc_PluginAuthor.Name = nameof( lvc_PluginAuthor );
            lvc_PluginEmail.Name = nameof( lvc_PluginEmail );
        }

        private void mainTabs_SelectedIndexChanged( object sender, EventArgs e )
        {
            var s = sender as TabControl;
            if ( s?.SelectedTab != MT_Plugins ) return;

            lv_Plugins.ItemCheck -= lv_Plugins_ItemCheck;

            lv_Plugins.Items.Clear();
            foreach ( var pluginInfo in PluginInfos )
            {
                lv_Plugins.Items.Add( LviByPlugin( pluginInfo ) );
            }

            lv_Plugins.ItemCheck += lv_Plugins_ItemCheck;
        }

        private static ListViewItem LviByPlugin( PluginInfo plugin )
        {
            var item = new ListViewItem( plugin.Display );
            item.SubItems.Add( plugin.Version );
            item.SubItems.Add( plugin.Author );
            item.SubItems.Add( plugin.Email );
            item.Checked = plugin.AutoStart;
            return item;
        }

        private void lv_Plugins_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( lv_Plugins.SelectedItems.Count > 0 )
            {
                btn_PluginSettings.Enabled = true;
            }
            else
            {
                btn_PluginSettings.Enabled = false;
            }
        }

        private void btn_PluginSettings_Click( object sender, EventArgs e )
        {
            if ( lv_Plugins.SelectedItems.Count <= 0 ) return;
            var p = PluginInfos[lv_Plugins.SelectedIndices[0]];
            PluginHost.PluginSettings( p );
        }

        private void lv_Plugins_ItemCheck( object? sender, ItemCheckEventArgs e )
        {
            var selectedIndex = e.Index;
            if ( e.NewValue == CheckState.Checked )
            {
                PluginInfos[selectedIndex].AutoStart = true;
                PluginHost.StartPlugin( PluginInfos[selectedIndex] );
            }
            else
            {
                PluginInfos[selectedIndex].AutoStart = false;
                PluginHost.ClosePlugin( PluginInfos[selectedIndex] );
            }

            PluginManager.SavePluginInfo( PluginInfos[selectedIndex] );
        }
    }
}