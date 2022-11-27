/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
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
            if ( e.Args.Contains( Const.Args.HIDE_ON_START ) ) HideOnStart = true;

            LogManager.GorgeousDividingLine();
            SingleInstanceCheck();

            if ( _mutex != null &&
                 AssemblyLoader.VersionCheck() &&
                 ConfigManager.Init() )
            {
                Current.MainWindow = CreateCanvas( e );
                if ( ConfigManager.Configs.Cluster.HideOnStart || HideOnStart )
                {
                    Current.MainWindow.Left = Const.FakeHideX;
                    Current.MainWindow.Top = Const.FakeHideY;
                }

                Bootstrap();
                Current.MainWindow.Show();

                if ( ConfigManager.Configs.Cluster.HideOnStart || HideOnStart )
                {
                    Current.MainWindow.Hide();
                }
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

        private static void SingleInstanceCheck()
        {
            if ( TryMutex() )
            {
                IpcPipeServer.Start();
            }

            IpcPipeServer.AsClient();
        }

        public static bool TryMutex()
        {
            _mutex = new Mutex( true, "乱花渐欲迷人眼", out var createdNew );
            return createdNew;
        }

        private static MainWindow CreateCanvas( StartupEventArgs args )
        {
            var canvas = VirtualSpace.MainWindow.Create();
            canvas.SetAppController( AppControllerFactory.Create( "WinForm" ) );
            return canvas;
        }

        private static void Bootstrap()
        {
            BootInfo();
            // ProcessWatcher.Start();
            // WindowWatcher.Start();

            Daemon.Start();

            PluginHost.RegisterPlugins( ConfigManager.GetPluginsPath(), Const.PluginInfoFile );
        }

        private static void BootInfo()
        {
            Logger.Info( "Application Start Successfully: " + ConfigManager.AppPath );
            Logger.Info( "System Version: " + Environment.OSVersion );
            Logger.Info( "Total Screens: " + Screen.AllScreens.Length );
            Logger.Info( "Total Virtual Desktops: " + DesktopWrapper.Count );
            Logger.Info( "Start Screen: " + Screen.FromPoint( new Point() ).DeviceName );
            Logger.Info( "Start Virtual Desktop: Desktop[" + DesktopWrapper.CurrentIndex + "]" );
            Logger.Info( "Start Position: " +
                         Screen.PrimaryScreen.Bounds.Location.X + ", " +
                         Screen.PrimaryScreen.Bounds.Location.Y );
            Logger.Info( "Start Size: " +
                         Screen.PrimaryScreen.Bounds.Width + "*" +
                         Screen.PrimaryScreen.Bounds.Height );
            Logger.Info( "Is Running As Administrator: " + SysInfo.IsAdministrator() );
            Logger.Info( "Language Setting In Profile: " + ConfigManager.CurrentProfile.UI.Language );
        }
    }
}