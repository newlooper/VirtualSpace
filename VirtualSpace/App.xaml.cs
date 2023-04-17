/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Factory;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;
using VirtualSpace.Tools;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;
using Application = System.Windows.Application;
using ConfigManager = VirtualSpace.Config.Manager;
using Point = System.Drawing.Point;

namespace VirtualSpace
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex;
        public         bool   HideOnStart;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            LogManager.GorgeousDividingLine();

            if ( SystemTool.VersionCheck() &&
                 SingleInstanceCheck() &&
                 ConfigManager.Init() )
            {
                Bootstrap();

                if ( e.Args.Contains( Const.Args.HIDE_ON_START ) ) HideOnStart = true;

                var mw = CreateCanvas( e );
                Current.MainWindow = mw;

                IpcPipeServer.MainWindowHandle = mw.Handle;

                if ( ConfigManager.Configs.Cluster.HideOnStart || HideOnStart )
                {
                    mw.Left = Const.FakeHideX;
                    mw.Top = Const.FakeHideY;
                }

                mw.Show();

                if ( ConfigManager.Configs.Cluster.HideOnStart || HideOnStart )
                {
                    mw.FakeHide();
                }

                PluginHost.AutoStartAfterMainWindowLoaded();
            }
            else
            {
                Current.Shutdown();
            }
        }

        protected override void OnExit( ExitEventArgs e )
        {
            base.OnExit( e );

            ReleaseMutex();
            IpcPipeServer.SimpleShutdown();
        }

        public void ReleaseMutex()
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            _mutex = null;
        }

        private static bool SingleInstanceCheck()
        {
            var createdNew = TryMutex();
            if ( createdNew )
            {
                IpcPipeServer.Start();
            }
            else
            {
                IpcPipeServer.AsClient();
            }

            return createdNew;
        }

        public static bool TryMutex()
        {
            _mutex = new Mutex( true, "乱花渐欲迷人眼", out var createdNew );
            return createdNew;
        }

        private MainWindow CreateCanvas( StartupEventArgs args )
        {
            var canvas = VirtualSpace.MainWindow.Create( AppControllerFactory.Create( "WPF", this ) );
            return canvas;
        }

        private static void Bootstrap()
        {
            Logger.ShowLogsInGui = ConfigManager.Configs.LogConfig.ShowLogsInGui;

            BootInfo();

            TrayIcon.Show();

            Daemon.Start();

            PluginHost.RegisterPlugins( ConfigManager.GetPluginsPath() );
        }

        private static void BootInfo()
        {
            var screen = Screen.FromPoint( new Point() );
            var ar     = SysInfo.GetAspectRadioOfScreen();
            Logger.Info( $"Application Start Successfully: {ConfigManager.AppPath}" );
            Logger.Info( $"System Version: {SysInfo.OSVersion}" );
            Logger.Info( $"Total Screens: {Screen.AllScreens.Length}" );
            Logger.Info( $"Total VirtualDesktops: {DesktopWrapper.Count}" );
            Logger.Info( $"Start Screen: {screen.DeviceName} ({screen.DeviceFriendlyName()})" );
            Logger.Info( $"Start Screen Aspect Ratio: [{ar.W}:{ar.H}]" );
            Logger.Info( $"Start VirtualDesktop: Desktop[{DesktopWrapper.CurrentIndex}]" );
            Logger.Info( $"Start Position: [{Screen.PrimaryScreen.Bounds.Location.X}, {Screen.PrimaryScreen.Bounds.Location.Y}]" );
            Logger.Info( $"Start Size: {Screen.PrimaryScreen.Bounds.Width}*{Screen.PrimaryScreen.Bounds.Height}" );
            Logger.Info( $"Is Running As Administrator: {SysInfo.IsAdministrator}" );
            Logger.Info( $"Language Config: {ConfigManager.CurrentProfile.UI.Language}" );
        }
    }
}