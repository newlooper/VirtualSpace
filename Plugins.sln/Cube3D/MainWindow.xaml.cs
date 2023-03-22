/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cube3D.Config;
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

            Left = Const.FakeHideX;
            Top = Const.FakeHideY;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Topmost = true;
            ShowActivated = false;

            new WindowInteropHelper( this ).EnsureHandle();
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            _handle = new WindowInteropHelper( this ).EnsureHandle();
            var source = HwndSource.FromHwnd( _handle );
            source?.AddHook( WndProc );
        }

        private void Bootstrap()
        {
            var pipeMessage = new PipeMessage
            {
                Type = PipeMessageType.PLUGIN_VD_SWITCH_OBSERVER,
                Name = PluginManager.PluginInfo.Name,
                Handle = _handle.ToInt32()
            };

            void Exit()
            {
                Application.Current.Shutdown();
            }

            void SetOwner( HostInfo hostInfo )
            {
                var pluginInfo = PluginManager.PluginInfo;
                if ( pluginInfo.Requirements.HostVersion == null ||
                     pluginInfo.Requirements.HostVersion > hostInfo.Version )
                {
                    MessageBox.Show( "Plugin Error.\nThe host does not meet the Requirements." );
                    Exit();
                    return;
                }

                User32.SetWindowLongPtr( new HandleRef( this, _handle ),
                    (int)GetWindowLongFields.GWL_HWNDPARENT,
                    hostInfo.MainWindowHandle
                );
            }

            IpcPipeClient.PluginCheckIn<HostInfo>(
                pipeMessage,
                () => { MessageBox.Show( "This Program require VirtualSpace running first." ); },
                Exit,
                SetOwner
            );

            IpcPipeClient.CheckAlive( pipeMessage.Name, pipeMessage.Handle, pipeMessage.ProcessId, SettingsManager.Settings.CheckAliveInterval, Exit );

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

            SetTransitionType();

            await StartPrimaryMonitorCapture();

            FakeHide();
        }

        public void SetTransitionType()
        {
            if ( SettingsManager.Settings.TransitionType == TransitionType.NotificationGridOnly )
            {
                Background = (Brush)Application.Current.Resources["BackgroundTrans"];
                WinChrome.GlassFrameThickness = new Thickness( -1 );
                Vp3D.Visibility = Visibility.Hidden;
            }
            else
            {
                Background = (Brush)Application.Current.Resources["BackgroundLgb"];
                WinChrome.GlassFrameThickness = new Thickness( 0 );
                Vp3D.Visibility = Visibility.Visible;
            }

            NotifyContainer.Visibility = SettingsManager.Settings.TransitionType > 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}