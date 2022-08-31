/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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

        public static MainWindow GetMainWindow()
        {
            return _instance;
        }

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

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            var source = PresentationSource.FromVisual( this ) as HwndSource;
            source?.AddHook( WndProc );
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            Bootstrap();

            VirtualDesktopManager.Bootstrap();

            if ( !( ConfigManager.CurrentProfile.HideOnStartup ||
                    ( (App)Application.Current ).HideOnStartup ) )
                VirtualDesktopManager.InitLayout();

            DesktopManagerWrapper.ListenVirtualDesktopEvents();
            DesktopManagerWrapper.RegisterVirtualDesktopEvents();
        }

        private void Bootstrap()
        {
            Handle = new WindowInteropHelper( this ).Handle;
            _acForm.SetMainWindowHandle( Handle );
            RegisterHotKey( Handle );
            FixStyle();
            EnableBlur();
            RegisterSystemMessages();
        }

        private void RegisterSystemMessages()
        {
            _taskbarCreatedMessage = User32.RegisterWindowMessage( Const.TaskbarCreated );
            foreach ( var strMsg in PluginHost.CareAboutMessages.Keys.ToList() )
            {
                PluginHost.CareAboutMessages[strMsg] = User32.RegisterWindowMessage( strMsg );
            }
        }

        private void Window_MouseDown( object sender, MouseButtonEventArgs e )
        {
            var profile = ConfigManager.CurrentProfile;
            if ( e.ChangedButton == MouseButton.Left )
            {
                switch ( profile.Mouse.LeftClickOnCanvas )
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
                switch ( profile.Mouse.RightClickOnCanvas )
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
                switch ( profile.Mouse.MiddleClickOnCanvas )
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
                ( (App)Application.Current ).ReleaseMutex();

                Thread.Sleep( 1000 );
                var startInfo = new ProcessStartInfo( ConfigManager.AppPath )
                {
                    Arguments = Const.Args.HIDE_ON_STARTUP
                };
                Process.Start( startInfo );

                Application.Current.Shutdown();
            }
            else
            {
                if ( PluginHost.CareAboutMessages.Values.Contains( (uint)msg ) )
                {
                    var (key, _) = PluginHost.CareAboutMessages.First( m => m.Value == msg );
                    foreach ( var plugin in PluginHost.Plugins.Where( plugin =>
                                 plugin.RestartPolicy?.Trigger == PolicyTrigger.WINDOWS_MESSAGE &&
                                 plugin.RestartPolicy.Enabled &&
                                 plugin.RestartPolicy.Values.Contains( key ) ) )
                    {
                        PluginHost.RestartPlugin( plugin );
                    }

                    foreach ( var plugin in PluginHost.Plugins.Where( plugin =>
                                 plugin.ClosePolicy?.Trigger == PolicyTrigger.WINDOWS_MESSAGE &&
                                 plugin.ClosePolicy.Enabled &&
                                 plugin.ClosePolicy.Values.Contains( key ) ) )
                    {
                        PluginHost.ClosePlugin( plugin );
                    }

                    goto RETURN;
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
                            if ( SwitchDesktopTimer.ElapsedMilliseconds > Const.SwitchDesktopInterval )
                            {
                                var desktopOrder = ConfigManager.CurrentProfile.DesktopOrder;
                                var currentDesktopMatrixIndex =
                                    VirtualDesktopManager.GetMatrixIndexByVdIndex( desktopOrder.IndexOf( DesktopWrapper.CurrentGuid ) );

                                var dir = lParam.ToInt32();
                                var targetIndex = Navigation.CalculateTargetIndex(
                                    DesktopWrapper.Count,
                                    currentDesktopMatrixIndex,
                                    (Keys)dir,
                                    ConfigManager.CurrentProfile.Navigation );

                                var vDsi = new VirtualDesktopSwitchInfo
                                {
                                    hostHandle = Handle,
                                    vdCount = DesktopWrapper.Count,
                                    fromIndex = currentDesktopMatrixIndex,
                                    dir = dir,
                                    targetIndex = targetIndex
                                };
                                var vDsiSize = Marshal.SizeOf( typeof( VirtualDesktopSwitchInfo ) );
                                var pVDsi    = Marshal.AllocHGlobal( vDsiSize );
                                Marshal.StructureToPtr( vDsi, pVDsi, true );

                                var cds = new COPYDATASTRUCT
                                {
                                    dwData = (IntPtr)WinApi.UM_SWITCHDESKTOP,
                                    cbData = vDsiSize,
                                    lpData = pVDsi
                                };
                                var pCds = Marshal.AllocHGlobal( Marshal.SizeOf( typeof( COPYDATASTRUCT ) ) );
                                Marshal.StructureToPtr( cds, pCds, true );

                                foreach ( var pluginInfo in PluginHost.Plugins.Where(
                                             p => p.Type == PluginType.VD_SWITCH_OBSERVER && User32.IsWindow( p.Handle ) ) )
                                {
                                    User32.SendMessage( pluginInfo.Handle, WinApi.WM_COPYDATA, 0, (ulong)pCds );
                                }

                                ////////////////////////////////////////////////////////////////////////////////////
                                // if none of plugins send back message after 100 ms, host will force switch desktop
                                Interlocked.Increment( ref _forceSwitchOnTimeout );
                                Task.Run( () =>
                                {
                                    Thread.Sleep( 100 );
                                    if ( _forceSwitchOnTimeout == 0 ) return;
                                    DesktopWrapper.MakeVisibleByGuid( desktopOrder[VirtualDesktopManager.GetVdIndexByMatrixIndex( targetIndex )] );
                                } );

                                Marshal.FreeHGlobal( pVDsi );
                                Marshal.FreeHGlobal( pCds );
                                SwitchDesktopTimer.Restart();
                            }

                            break;
                    }

                    break;
                case WinApi.UM_SWITCHDESKTOP:
                    var targetMatrixIndex = wParam.ToInt32();
                    if ( targetMatrixIndex >= 0 && targetMatrixIndex < DesktopWrapper.Count )
                    {
                        Interlocked.Exchange( ref _forceSwitchOnTimeout, 0 );
                        DesktopWrapper.MakeVisibleByGuid(
                            ConfigManager.CurrentProfile.DesktopOrder[VirtualDesktopManager.GetVdIndexByMatrixIndex( targetMatrixIndex )] );
                    }

                    break;
                // case WinMsg.WM_MOUSEACTIVATE:
                //     handled = true;
                //     return new IntPtr( WinMsg.MA_NOACTIVATE );
            }

            RETURN:
            return IntPtr.Zero;
        }

        public void SetAppController( IAppController ac )
        {
            _acForm = ac;
        }

        private static void BringToTop()
        {
            _instance.Left = 0;
            _instance.Top = 0;
            _instance.Show();

            VirtualDesktopManager.FixLayout();
            VirtualDesktopManager.ShowAllVirtualDesktops();
            VirtualDesktopManager.ShowVisibleWindowsForDesktops();
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
    }
}