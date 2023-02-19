// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using VirtualDesktop;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private uint _taskbarCreatedMessage;

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
            var profile = Manager.CurrentProfile;
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
                Logger.Warning( "explorer.exe restarted, reset DesktopManager and restart all Plugins." );
                DesktopManager.ResetDesktopManager();
                foreach ( var plugin in PluginHost.Plugins )
                {
                    PluginHost.RestartPlugin( plugin );
                }

                goto RETURN;
            }

            if ( PluginHost.CareAboutMessages.Values.Contains( (uint)msg ) )
            {
                var (key, _) = PluginHost.CareAboutMessages.First( m => m.Value == msg );
                foreach ( var plugin in PluginHost.Plugins.Where( plugin =>
                             plugin.RestartPolicy?.Trigger == PolicyTrigger.WINDOWS_MESSAGE &&
                             plugin.RestartPolicy.Enabled &&
                             plugin.RestartPolicy.Values.Contains( key ) ) )
                {
                    Logger.Info( $"Restart Plugin {plugin.Display} because {key}" );
                    PluginHost.RestartPlugin( plugin );
                }

                foreach ( var plugin in PluginHost.Plugins.Where( plugin =>
                             plugin.ClosePolicy?.Trigger == PolicyTrigger.WINDOWS_MESSAGE &&
                             plugin.ClosePolicy.Enabled &&
                             plugin.ClosePolicy.Values.Contains( key ) ) )
                {
                    Logger.Info( $"Close Plugin {plugin.Display} because {key}" );
                    PluginHost.ClosePlugin( plugin );
                }

                goto RETURN;
            }

            void SwitchByIndex( int index )
            {
                if ( Manager.CurrentProfile.DesktopOrder.Count > index )
                    DesktopWrapper.MakeVisibleByGuid( Manager.CurrentProfile.DesktopOrder[index] );
            }

            switch ( msg )
            {
                case WinMsg.WM_SYSCOMMAND:
                    var wP = wParam.ToInt32();
                    if ( wP is WinMsg.SC_RESTORE or WinMsg.SC_MINIMIZE or WinMsg.SC_MAXIMIZE )
                        handled = true;
                    break;
                case WinMsg.WM_HOTKEY:
                    switch ( wParam.ToInt32() )
                    {
                        case UserMessage.RiseView:
                            if ( Manager.Configs.Cluster.HideMainViewIfItsShown && IsShowing() )
                            {
                                HideAll();
                            }
                            else if ( RiseViewTimer.ElapsedMilliseconds > Const.RiseViewInterval )
                            {
                                BringToTop();
                                RiseViewTimer.Restart();
                            }

                            break;
                        case UserMessage.RiseViewForActiveApp:
                            var hFWin = User32.GetForegroundWindow();
                            User32.GetWindowThreadProcessId( hFWin, out var processId );
                            if ( Manager.Configs.Cluster.HideMainViewIfItsShown && IsShowing() )
                            {
                                HideAll();
                            }
                            else if ( RiseViewTimer.ElapsedMilliseconds > Const.RiseViewInterval )
                            {
                                BringToTop( processId );
                                RiseViewTimer.Restart();
                            }

                            break;
                        case UserMessage.ShowAppController:
                            _acForm.BringToTop();
                            break;
                        case UserMessage.SwitchDesktop:
                            if ( SwitchDesktopTimer.ElapsedMilliseconds > Const.SwitchDesktopInterval )
                            {
                                var desktopOrder = Manager.CurrentProfile.DesktopOrder;
                                var currentDesktopMatrixIndex =
                                    VirtualDesktopManager.GetMatrixIndexByVdIndex( desktopOrder.IndexOf( DesktopWrapper.CurrentGuid ) );

                                var dir = lParam.ToInt32();
                                var targetIndex = Navigation.CalculateTargetIndex(
                                    DesktopWrapper.Count,
                                    currentDesktopMatrixIndex,
                                    (Keys)dir,
                                    Manager.CurrentProfile.Navigation );

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
                        case UserMessage.DesktopArrangement:

                            VirtualDesktopManager.FixLayout();
                            VirtualDesktopManager.RebuildMatrixMap( RowsCols );

                            if ( IsShowing() )
                            {
                                VirtualDesktopManager.ShowAllVirtualDesktops();
                                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                            }

                            break;
                        case UserMessage.RunAsAdministrator:
                            TryRunAsAdmin();
                            goto RETURN;
                        case UserMessage.RestartApp:
                            RestartApp();
                            goto RETURN;
                        case UserMessage.EnableMouseHook:
                            EnableMouseHook();
                            goto RETURN;
                        case UserMessage.DisableMouseHook:
                            DisableMouseHook();
                            goto RETURN;
                        case UserMessage.SVD1:
                            SwitchByIndex( 0 );
                            break;
                        case UserMessage.SVD2:
                            SwitchByIndex( 1 );
                            break;
                        case UserMessage.SVD3:
                            SwitchByIndex( 2 );
                            break;
                        case UserMessage.SVD4:
                            SwitchByIndex( 3 );
                            break;
                        case UserMessage.SVD5:
                            SwitchByIndex( 4 );
                            break;
                        case UserMessage.SVD6:
                            SwitchByIndex( 5 );
                            break;
                        case UserMessage.SVD7:
                            SwitchByIndex( 6 );
                            break;
                        case UserMessage.SVD8:
                            SwitchByIndex( 7 );
                            break;
                        case UserMessage.SVD9:
                            SwitchByIndex( 8 );
                            break;
                        case UserMessage.NavLeft:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)Keys.Left );
                            break;
                        case UserMessage.NavRight:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)Keys.Right );
                            break;
                        case UserMessage.NavUp:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)Keys.Up );
                            break;
                        case UserMessage.NavDown:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)Keys.Down );
                            break;
                    }

                    break;
                case WinApi.UM_SWITCHDESKTOP:
                    var targetMatrixIndex = wParam.ToInt32();
                    if ( targetMatrixIndex >= 0 && targetMatrixIndex < DesktopWrapper.Count )
                    {
                        Interlocked.Exchange( ref _forceSwitchOnTimeout, 0 );
                        DesktopWrapper.MakeVisibleByGuid(
                            Manager.CurrentProfile.DesktopOrder[VirtualDesktopManager.GetVdIndexByMatrixIndex( targetMatrixIndex )] );
                    }

                    break;
                // case WinMsg.WM_MOUSEACTIVATE:
                //     handled = true;
                //     return new IntPtr( WinMsg.MA_NOACTIVATE );
            }

            RETURN:
            return IntPtr.Zero;
        }
    }
}