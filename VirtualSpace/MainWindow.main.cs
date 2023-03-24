/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;
using Application = System.Windows.Application;
using ConfigManager = VirtualSpace.Config.Manager;
using IpcConfig = VirtualSpace.Commons.Config;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static readonly Stopwatch      RiseViewTimer      = Stopwatch.StartNew();
        private static readonly Stopwatch      SwitchDesktopTimer = Stopwatch.StartNew();
        private static          MainWindow     _instance;
        private static          int            _forceSwitchOnTimeout;
        private                 IAppController _acForm;

        private MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _instance = this;

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Topmost = true;
            Title = Const.Window.VD_FRAME_TITLE;
        }

        public IntPtr Handle { get; private set; }

        public static MainWindow GetMainWindow()
        {
            return _instance;
        }

        public static MainWindow Create( IAppController ac )
        {
            var mw = new MainWindow
            {
                _acForm = ac,
                Background = new SolidColorBrush(
                    Color.FromArgb(
                        Ui.CanvasOpacity,
                        Ui.CanvasBackColor.R,
                        Ui.CanvasBackColor.G,
                        Ui.CanvasBackColor.B )
                ),
                BlurOpacity = Ui.CanvasOpacity,
                BlurBackgroundColor = Ui.CanvasBackColor.GetLongOfColor()
            };

            new WindowInteropHelper( mw ).EnsureHandle();

            mw.InitCellBorderShadowEffect();

            return mw;
        }

        public static void NotifyDesktopManagerReset()
        {
            User32.SendMessage( _instance.Handle, (int)_instance._taskbarCreatedMessage, 0, 0 );
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            Handle = new WindowInteropHelper( this ).EnsureHandle();
            var source = HwndSource.FromHwnd( Handle );
            source?.AddHook( WndProc );

            Bootstrap();
        }

        private async void Window_Loaded( object sender, RoutedEventArgs e )
        {
            VirtualDesktopManager.Bootstrap();
            await VirtualDesktopManager.InitLayout();

            if ( ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon )
                UpdateVDIndexOnTrayIcon( DesktopWrapper.CurrentGuid );

            DesktopManagerWrapper.ListenVirtualDesktopEvents();
            DesktopManagerWrapper.RegisterVirtualDesktopEvents();

            if ( ConfigManager.Configs.Cluster.HideOnStart ||
                 ( (App)Application.Current ).HideOnStart ) return;

            VirtualDesktopManager.ShowAllVirtualDesktops();
            VirtualDesktopManager.ShowVisibleWindowsForDesktops();
        }

        private void Bootstrap()
        {
            _acForm.SetMainWindowHandle( Handle );
            RegisterHotKey( Handle );
            FixStyle();
            EnableBlur();
            RegisterSystemMessages();
        }

        public void FakeHide()
        {
            Hide();
        }

        private static void BringToTop( int processId = 0 )
        {
            TopShow();

            VirtualDesktopManager.FixLayout();
            VirtualDesktopManager.ShowAllVirtualDesktops();

            if ( processId > 0 )
            {
                VirtualDesktopManager.ShowVisibleWindowsForDesktops( null, processId );
            }
            else
            {
                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            }
        }

        private static void BringToTopForCurrentVd( int processId = 0 )
        {
            TopShow();

            var cvd = VirtualDesktopManager.GetCurrentVdw();
            cvd.MakeTheOnlyOne( processId );
        }

        private static void TopShow()
        {
            CheckScreenArea();

            _instance.Left = 0;
            _instance.Top = 0;
            _instance.Show();
        }

        private static void CheckScreenArea()
        {
            if ( (int)_instance.Width == (int)SystemParameters.PrimaryScreenWidth &&
                 (int)_instance.Height == (int)SystemParameters.PrimaryScreenHeight ) return;
            _instance.Width = SystemParameters.PrimaryScreenWidth;
            _instance.Height = SystemParameters.PrimaryScreenHeight;
        }

        public static void HideAll()
        {
            _instance.Hide();
            VirtualDesktopManager.HideAllVirtualDesktops();
        }

        public static bool IsShowing()
        {
            return _instance.IsVisible;
        }

        public static void UpdateVDIndexOnTrayIcon( Guid guid )
        {
            var i     = ConfigManager.CurrentProfile.DesktopOrder.IndexOf( guid );
            var index = ConfigManager.CurrentProfile.UI.ShowVdIndexType == 0 ? i : i + 1;
            _instance._acForm.UpdateVDIndexOnTrayIcon( index.ToString() );
        }

        private static void TryRunAsAdmin()
        {
            RestartApp( true );
        }

        private static void RestartApp( bool runas = false )
        {
            var app = (App)Application.Current;
            var psi = new ProcessStartInfo
            {
                FileName = ConfigManager.AppPath,
                UseShellExecute = true
            };

            if ( runas ) psi.Verb = "runas";

            try
            {
                app.ReleaseMutex();
                Process.Start( psi );
                Application.Current.Shutdown();
            }
            catch
            {
                App.TryMutex();
            }
        }
    }
}