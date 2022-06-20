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
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void ReadNavConfig()
        {
            cb_nav_circle_h_type.Items.Clear();
            cb_nav_circle_h_type.Items.Add( Agent.Langs.GetString( "Nav.CircleHType.NextRow" ) );
            cb_nav_circle_h_type.Items.Add( Agent.Langs.GetString( "Nav.CircleHType.SameRow" ) );

            cb_nav_circle_h.Checked = ConfigManager.CurrentProfile.Navigation.CirculationH;
            cb_nav_circle_h_type.SelectedIndex = ConfigManager.CurrentProfile.Navigation.CirculationHType;
            cb_nav_circle_v.Checked = ConfigManager.CurrentProfile.Navigation.CirculationV;
        }

        private void cb_nav_circle_h_CheckedChanged( object sender, EventArgs e )
        {
            ConfigManager.CurrentProfile.Navigation.CirculationH = cb_nav_circle_h.Checked;
            ConfigManager.Save();
        }

        private void cb_nav_circle_v_CheckedChanged( object sender, EventArgs e )
        {
            ConfigManager.CurrentProfile.Navigation.CirculationV = cb_nav_circle_v.Checked;
            ConfigManager.Save();
        }

        private void cb_nav_circle_h_type_SelectedIndexChanged( object sender, EventArgs e )
        {
            ConfigManager.CurrentProfile.Navigation.CirculationHType = cb_nav_circle_h_type.SelectedIndex;
            ConfigManager.Save();
        }
    }
}