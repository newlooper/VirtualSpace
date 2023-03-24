/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cube3D.Config;
using ScreenCapture;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;

#pragma warning disable CA1416

namespace Cube3D
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr _handle;

        public static IntPtr MainWindowHandle { get; private set; }

        private readonly MonitorInfo _monitorInfo;

        private static (double ScaleX, double ScaleY) GetDpiForMonitor( IntPtr hMon )
        {
            _ = User32.GetDpiForMonitor( hMon, User32.MonitorDpiType.MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY );
            return new ValueTuple<double, double>( dpiX / 96.0, dpiY / 96.0 );
        }

        public MainWindow()
        {
            InitializeComponent();

            _monitorInfo = ( from m in MonitorEnumerationHelper.GetMonitors() where m.IsPrimary select m ).First();

            var dpi = GetDpiForMonitor( _monitorInfo.Hmon );
            Left = 0;
            Top = 0;
            Width = _monitorInfo.ScreenSize.X / dpi.ScaleX;
            Height = _monitorInfo.ScreenSize.Y / dpi.ScaleY;

            Topmost = true;
            ShowActivated = false;

            new WindowInteropHelper( this ).EnsureHandle();
        }

        private MainWindow( MonitorInfo mi )
        {
            InitializeComponent();

            _monitorInfo = mi;

            var dpi = GetDpiForMonitor( _monitorInfo.Hmon );
            Left = _monitorInfo.WorkArea.Left / dpi.ScaleX;
            Top = _monitorInfo.WorkArea.Top / dpi.ScaleY;
            Width = _monitorInfo.ScreenSize.X / dpi.ScaleX;
            Height = _monitorInfo.ScreenSize.Y / dpi.ScaleY;

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

            if ( _monitorInfo.IsPrimary ) MainWindowHandle = _handle;
        }

        private void Register()
        {
            if ( !_monitorInfo.IsPrimary ) return;

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
#if DEBUG
            var interval = 1;
#else
            var interval = SettingsManager.Settings.CheckAliveInterval;
#endif
            IpcPipeClient.CheckAlive( pipeMessage.Name, pipeMessage.Handle, pipeMessage.ProcessId, interval, Exit );
        }

        private void Bootstrap()
        {
            Register();

            FixStyle();

            CameraPosition( _monitorInfo );

            _animationNotifyGrid.Completed += AnimationCompleted;
        }

        private void FixStyle()
        {
            _ = User32.SetWindowDisplayAffinity( _handle, User32.WDA_EXCLUDEFROMCAPTURE ); // self exclude from screen capture

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
            FakeHide();

            SetTransitionType();

            Bootstrap();

            if ( _monitorInfo.IsPrimary )
            {
                Build3D();

                await StartPrimaryMonitorCapture();

                CreateOtherScreens();
            }
        }

        public void SetTransitionType()
        {
            if ( SettingsManager.Settings.TransitionType == TransitionType.NotificationGridOnly || !_monitorInfo.IsPrimary )
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

            NotifyContainer.Visibility = ( SettingsManager.Settings.TransitionType & TransitionType.NotificationGridOnly ) > 0 || !_monitorInfo.IsPrimary
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private static readonly List<MainWindow> OtherScreens = new();

        private void CreateOtherScreens()
        {
            if ( ( SettingsManager.Settings.TransitionType & TransitionType.NotificationGridOnly ) == 0 ||
                 !SettingsManager.Settings.ShowNotificationGridOnAllScreens ) return;

            ClearOtherScreens();
            var others = ( from m in MonitorEnumerationHelper.GetMonitors()
                where !m.IsPrimary
                select m ).ToList();
            foreach ( var ow in from mi in others select new MainWindow( mi ) )
            {
                OtherScreens.Add( ow );
                User32.SetWindowLongPtr( new HandleRef( ow, ow._handle ),
                    (int)GetWindowLongFields.GWL_HWNDPARENT,
                    _handle.ToInt32()
                );
                ow.Show();
            }
        }

        private static void ClearOtherScreens()
        {
            foreach ( var ow in OtherScreens )
            {
                ow.Close();
            }

            OtherScreens.Clear();
        }
    }
}

#pragma warning restore CA1416