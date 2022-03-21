/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Cube3D.Config;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;

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

            Left = Const.FakeHideX;
            Top = Const.FakeHideY;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Topmost = true;
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            var source = PresentationSource.FromVisual( this ) as HwndSource;
            source?.AddHook( WndProc );
        }

        private void Bootstrap()
        {
            _handle = new WindowInteropHelper( this ).Handle;

            CheckHost();
            _ = User32.SetWindowDisplayAffinity( _handle, User32.WDA_EXCLUDEFROMCAPTURE ); // self exclude from screen capture

            FixStyle();
            CameraPosition();
            _animationNotifyGrid.Completed += AnimationCompleted;
        }

        private void FixStyle()
        {
            var style = User32.GetWindowLong( _handle, (int)GetWindowLongFields.GWL_STYLE );
            style = unchecked(style | (int)0x80000000); // WS_POPUP
            User32.SetWindowLongPtr( new HandleRef( this, _handle ), (int)GetWindowLongFields.GWL_STYLE, style );

            var exStyle = User32.GetWindowLong( _handle, (int)GetWindowLongFields.GWL_EXSTYLE );
            exStyle |= 0x08000000; // WS_EX_NOACTIVATE
            exStyle &= ~0x00040000; // WS_EX_APPWINDOW
            User32.SetWindowLongPtr( new HandleRef( this, _handle ), (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
        }

        private async void Window_Loaded( object sender, RoutedEventArgs e )
        {
            Bootstrap();

            Build3D();

            await StartPrimaryMonitorCapture();

            FakeHide();
        }

        private static void CheckHost()
        {
            var pluginInfo = ConfigManager.PluginInfo;
            var pId        = Process.GetCurrentProcess().Id;
            if ( !IpcPipeClient.RegisterVdSwitchObserver( pluginInfo.Name, _handle, pId ) )
            {
                MessageBox.Show( "This Program require VirtualSpace running first." );
                Application.Current.Shutdown();
            }

            CheckAlive( pluginInfo.Name, _handle, pId );
        }

        private static async void CheckAlive( string name, IntPtr handle, int pId )
        {
            while ( IpcPipeClient.AskAlive( name, handle, pId ) )
            {
                await Task.Delay( ConfigManager.Settings.CheckAliveInterval * 1000 );
            }

            Application.Current.Shutdown();
        }
    }
}