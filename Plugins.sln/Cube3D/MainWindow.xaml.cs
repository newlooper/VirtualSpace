/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;

namespace Cube3D
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static IntPtr _handle;

        public MainWindow()
        {
            InitializeComponent();

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Topmost = true;
        }

        private void Bootstrap()
        {
            var pluginInfoFile = Path.Combine( GetAppFolder(), "plugin.json" );
            var pluginInfo     = PluginManager.LoadFromJson( pluginInfoFile );
            if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Name ) )
            {
                MessageBox.Show( "plugin.json invalid." );
                Application.Current.Shutdown();
            }

            if ( !PluginManager.CheckRequirements( pluginInfo.Requirements ) )
            {
                MessageBox.Show( "The system does not meet the Requirements." );
                Application.Current.Shutdown();
            }

            _handle = new WindowInteropHelper( this ).Handle;
            var pId = Process.GetCurrentProcess().Id;
            if ( !IpcPipe.RegisterVdSwitchObserver( pluginInfo.Name, _handle, pId ) )
            {
                MessageBox.Show( "This Program require VirtualSpace running first." );
                Application.Current.Shutdown();
            }

            SetWindowDisplayAffinity( _handle, WDA_EXCLUDEFROMCAPTURE ); // self exclude from screen capture
            FixStyle();

            CameraPosition( 0 );

            _animationCube.Completed += AnimationCompleted;
            _animationNotifyGrid.Completed += AnimationCompleted;
        }

        private static void FixStyle()
        {
            var style = User32.GetWindowLong( _handle, (int)GetWindowLongFields.GWL_STYLE );
            style = unchecked(style | (int)0x80000000); // WS_POPUP
            User32.SetWindowLong( _handle, (int)GetWindowLongFields.GWL_STYLE, style );

            var exStyle = User32.GetWindowLong( _handle, (int)GetWindowLongFields.GWL_EXSTYLE );
            exStyle |= 0x08000000; // WS_EX_NOACTIVATE
            exStyle &= ~0x00040000; // WS_EX_APPWINDOW
            User32.SetWindowLong( _handle, (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            Bootstrap();

            BuildCube();

            StartPrimaryMonitorCapture();

            ShowHide();
        }

        private string GetAppFolder()
        {
            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            return Directory.GetParent( appPath ).FullName;
        }
    }
}