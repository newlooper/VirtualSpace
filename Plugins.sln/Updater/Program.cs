// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of Updater.
//
// Updater is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// Updater is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with Updater. If not, see <https://www.gnu.org/licenses/>.

using VirtualSpace.Plugin;

namespace Updater
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var pluginInfo = PluginManager.PluginInfo;
            if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Name ) )
            {
                MessageBox.Show( $"{PluginManager.PluginInfoFile} invalid." );
            }
            else
            {
                Application.Run( new MainForm() );
            }
        }
    }
}