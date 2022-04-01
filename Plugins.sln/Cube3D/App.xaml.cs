/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Windows;
using Cube3D.Config;
using VirtualSpace.Plugin;

namespace Cube3D
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            var pluginInfo = ConfigManager.PluginInfo;
            if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Name ) )
            {
                MessageBox.Show( $"{Const.PluginInfoFile} invalid." );
                Current.Shutdown();
            }

            if ( !PluginManager.CheckRequirements( pluginInfo.Requirements ) )
            {
                MessageBox.Show( "Plugin Error.\nThe system does not meet the Requirements." );
                Current.Shutdown();
            }
        }
    }
}