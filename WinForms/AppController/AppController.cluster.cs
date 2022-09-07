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
        private void ReadClusterConfig()
        {
            chb_HideMainViewIfItsShown.Checked = ConfigManager.Configs.Cluster.HideMainViewIfItsShown;
        }

        private void chb_HideMainViewIfItsShown_CheckedChanged( object sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.HideMainViewIfItsShown = chb_HideMainViewIfItsShown.Checked;
            ConfigManager.Save();
        }
    }
}