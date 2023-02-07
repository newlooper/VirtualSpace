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
        private void InitUiConfig()
        {
            var ui = ConfigManager.CurrentProfile.UI;

            chb_show_vd_name.Checked = ui.ShowVdName;
            chb_show_vd_index.Checked = ui.ShowVdIndex;

            if ( ui.ShowVdIndexType == 0 )
            {
                rb_vd_index_0.Checked = true;
            }
            else
            {
                rb_vd_index_1.Checked = true;
            }

            chb_show_vd_name.CheckedChanged += chb_show_vd_name_CheckedChanged;
            chb_show_vd_index.CheckedChanged += chb_show_vd_index_CheckedChanged;

            rb_vd_index_0.CheckedChanged += rb_vd_index_0_CheckedChanged;
        }

        private void chb_show_vd_name_CheckedChanged( object? sender, EventArgs e )
        {
            var ui = ConfigManager.CurrentProfile.UI;
            ui.ShowVdName = chb_show_vd_name.Checked;
            ConfigManager.Save();
        }

        private void chb_show_vd_index_CheckedChanged( object? sender, EventArgs e )
        {
            var ui = ConfigManager.CurrentProfile.UI;
            ui.ShowVdIndex = chb_show_vd_index.Checked;
            ConfigManager.Save();
        }

        private void rb_vd_index_0_CheckedChanged( object? sender, EventArgs e )
        {
            var ui = ConfigManager.CurrentProfile.UI;
            ui.ShowVdIndexType = (byte)( rb_vd_index_0.Checked ? 0 : 1 );
            ConfigManager.Save();
        }
    }
}