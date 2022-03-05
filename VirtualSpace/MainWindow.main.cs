/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;
using Application = System.Windows.Forms.Application;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static readonly Stopwatch      RiseViewTimer = Stopwatch.StartNew();
        private static          MainWindow     _instance;
        private                 IAppController _acForm;
        private                 uint           _hotplugDetected;
        private                 uint           _taskbarCreatedMessage;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _instance = this;

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Topmost = true;
        }

        public IntPtr Handle { get; private set; }

        public static MainWindow Create()
        {
            return new MainWindow
            {
                Background = new SolidColorBrush(
                    Color.FromArgb(
                        Ui.CanvasOpacity,
                        Ui.CanvasBackColor.R,
                        Ui.CanvasBackColor.G,
                        Ui.CanvasBackColor.B )
                ),
                BlurOpacity = Ui.CanvasOpacity,
                BlurBackGroundColor = Ui.CanvasBackColor.GetLongOfColor()
            };
        }

        public void SetAppController( IAppController ac )
        {
            _acForm = ac;
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            var source = PresentationSource.FromVisual( this ) as HwndSource;
            source?.AddHook( WndProc );
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            Bootstrap();
            VirtualDesktopManager.InitLayout();
            DesktopManagerWrapper.ListenVirtualDesktopEvents();
            DesktopManagerWrapper.RegisterVirtualDesktopEvents();
        }

        private void Bootstrap()
        {
            Handle = new WindowInteropHelper( this ).Handle;
            RegisterHotKey( Handle );
            FixStyle();
            EnableBlur();
            RegisterSystemMessages();
        }

        private void RegisterSystemMessages()
        {
            _taskbarCreatedMessage = User32.RegisterWindowMessage( Const.TaskbarCreated );
            _hotplugDetected = User32.RegisterWindowMessage( Const.HotplugDetected );
        }

        private void Window_MouseDown( object sender, MouseButtonEventArgs e )
        {
            var profile = ConfigManager.GetCurrentProfile();
            if ( e.ChangedButton == MouseButton.Left )
            {
                switch ( profile.LeftClickOnCanvas )
                {
                    case 0:
                        break;
                    case 1:
                        HideAll();
                        break;
                    default:
                        HideAll();
                        break;
                }
            }
            else if ( e.ChangedButton == MouseButton.Right )
            {
                switch ( profile.RightClickOnCanvas )
                {
                    case 0:
                        break;
                    case 1:
                        HideAll();
                        break;
                    default:
                        HideAll();
                        break;
                }
            }
            else if ( e.ChangedButton == MouseButton.Middle )
            {
                switch ( profile.MiddleClickOnCanvas )
                {
                    case 0:
                        break;
                    case 1:
                        HideAll();
                        break;
                    default:
                        HideAll();
                        break;
                }
            }
        }

        private IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            if ( msg == _taskbarCreatedMessage )
            {
                Logger.Warning( "explorer.exe restarted, program will restart to handle it." );
                Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }

            if ( msg == _hotplugDetected )
            {
                foreach ( var plugin in PluginManagerServer.Plugins )
                {
                    if ( plugin.RestartPolicy?.Type == RestartPolicyType.WINDOWS_MESSAGE
                         && plugin.RestartPolicy.Value == Const.HotplugDetected )
                    {
                        PluginManagerServer.RestartPlugin( plugin );
                    }
                }
            }

            switch ( msg )
            {
                case WinMsg.WM_SYSCOMMAND:
                    var wP = wParam.ToInt32();
                    if ( wP == WinMsg.SC_RESTORE || wP == WinMsg.SC_MINIMIZE || wP == WinMsg.SC_MAXIMIZE )
                        handled = true;
                    break;
                case WinMsg.WM_HOTKEY:
                    switch ( wParam.ToInt32() )
                    {
                        case UserMessage.RiseView:
                            if ( RiseViewTimer.ElapsedMilliseconds > Const.RiseViewInterval )
                            {
                                BringToTop();
                                RiseViewTimer.Restart();
                            }

                            break;
                        case UserMessage.ShowAppController:
                            _acForm.BringToTop();
                            break;
                        case UserMessage.SwitchDesktop:
                            if ( RiseViewTimer.ElapsedMilliseconds > Const.RiseViewInterval )
                            {
                                var dir         = lParam.ToInt32();
                                var targetIndex = Navigation.CalculateTargetIndex( DesktopWrapper.Count, DesktopWrapper.CurrentIndex, (Keys)dir );

                                foreach ( var pluginInfo in PluginManagerServer.Plugins.Where(
                                             p => p.Type == PluginType.VD_SWITCH_OBSERVER && User32.IsWindow( p.Handle ) ) )
                                {
                                    var w = DesktopWrapper.Count;
                                    w += DesktopWrapper.CurrentIndex * IpcPipeServer.Power;
                                    w += dir * IpcPipeServer.Power * IpcPipeServer.Power;
                                    User32.SendMessage( pluginInfo.Handle, WinMsg.WM_HOTKEY, (uint)w, (uint)targetIndex );
                                }

                                DesktopWrapper.MakeVisibleByIndex( targetIndex );
                            }

                            break;
                    }

                    break;
                case WinMsg.WM_MOUSEACTIVATE:
                    handled = true;
                    return new IntPtr( WinMsg.MA_NOACTIVATE );
            }

            return IntPtr.Zero;
        }

        public static void BringToTop()
        {
            _instance.Show();

            VirtualDesktopManager.FixLayout();
            VirtualDesktopManager.ShowAllVirtualDesktops();
            VirtualDesktopManager.ShowVisibleWindowsForDesktops();
        }

        public static void DelegateBringToTop()
        {
            _instance.Dispatcher.Invoke( BringToTop );
        }

        public static void HideAll()
        {
            _instance.Hide();
            VirtualDesktopManager.HideAllVirtualDesktops();
        }

        public static void Quit()
        {
            _instance.Close();
        }

        public static bool IsShowing()
        {
            return _instance.IsVisible;
        }
    }
}