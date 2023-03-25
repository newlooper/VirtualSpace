/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cube3D.Config;
using Cube3D.Effects;
using ScreenCapture;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;

#pragma warning disable CA1416

namespace Cube3D
{
    public partial class MainWindow
    {
        private static   SettingsWindow _sw;
        private readonly StringBuilder  _sbWinInfo = new( 1024 );
        private          bool           _isTopmost;

        private void FakeHide( bool stopCapture = false )
        {
            Left = Const.FakeHideX;
            Top = Const.FakeHideY;

            if ( stopCapture ) StopCapture();
        }

        private void StopCapture()
        {
            _capture?.StopCaptureSession();
        }

        private bool WindowFilter( IntPtr hWnd, int lParam )
        {
            if ( !User32.IsWindowVisible( hWnd ) )
                return true;

            _sbWinInfo.Clear();
            _ = User32.GetWindowText( hWnd, _sbWinInfo, _sbWinInfo.Capacity );
            if ( _sbWinInfo.Length == 0 )
                return true;

            _isTopmost = Handle == hWnd; // if the first visible non-empty title window is Cube3D, then Cube3D is on the top.

            return false;
        }

        private void RealShow( bool forceTop = false )
        {
            if ( forceTop )
            {
                _ = User32.EnumWindows( WindowFilter, 0 );
                if ( !_isTopmost )
                {
                    User32.SetWindowPos( Handle, User32.SpecialWindowHandles.HWND_TOP, 0, 0, 0, 0,
                        User32.SetWindowPosFlags.SWP_NOSIZE |
                        User32.SetWindowPosFlags.SWP_NOMOVE |
                        User32.SetWindowPosFlags.SWP_NOACTIVATE |
                        User32.SetWindowPosFlags.SWP_NOREDRAW |
                        User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                        User32.SetWindowPosFlags.SWP_DEFERERASE |
                        User32.SetWindowPosFlags.SWP_NOSENDCHANGING
                    );
                }
            }

            var dpi = GetDpiForMonitor( _monitorInfo.Hmon );

            Left = _monitorInfo.WorkArea.Left / dpi.ScaleX;
            Top = _monitorInfo.WorkArea.Top / dpi.ScaleY;
            Width = _monitorInfo.ScreenSize.X / dpi.ScaleX;
            Height = _monitorInfo.ScreenSize.Y / dpi.ScaleY;
        }

        private IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch ( msg )
            {
                case WinMsg.WM_SYSCOMMAND:
                    var wP = wParam.ToInt32();
                    if ( wP is WinMsg.SC_RESTORE or WinMsg.SC_MINIMIZE or WinMsg.SC_MAXIMIZE )
                        handled = true;
                    break;

                case WinApi.WM_COPYDATA:
                    var copyDataStruct = (COPYDATASTRUCT)Marshal.PtrToStructure( lParam, typeof( COPYDATASTRUCT ) );
                    switch ( copyDataStruct.dwData.ToInt32() )
                    {
                        case WinApi.UM_SWITCHDESKTOP:
                            if ( _mainWindowRunningAnimationCount == 0 )
                            {
                                var vdSwitchInfo =
                                    (VirtualDesktopSwitchInfo)Marshal.PtrToStructure( copyDataStruct.lpData, typeof( VirtualDesktopSwitchInfo ) );

                                Task.Run( () => { Dispatcher.Invoke( () => PerformAnimationPrimary( vdSwitchInfo ) ); } );
                                foreach ( var other in OtherScreens )
                                {
                                    other.PerformAnimationOthers( vdSwitchInfo );
                                }
                            }

                            break;
                    }

                    break;

                case WinApi.UM_PLUGINSETTINGS:
                    if ( _sw is null || PresentationSource.FromVisual( _sw ) == null )
                    {
                        _sw = new SettingsWindow();
                        _sw.SetMainWindow( this );
                        _sw.ShowDialog();
                    }
                    else
                    {
                        _sw.Activate();
                    }

                    break;
                case WinApi.UM_OTHERSCREENS:
                    switch ( wParam.ToInt32() )
                    {
                        case 0:
                            ClearOtherScreens();
                            break;
                        case 1:
                            CreateOtherScreens();
                            break;
                    }

                    break;

                case WinMsg.WM_DISPLAYCHANGE:
                    try
                    {
                        _capture?.UpdateCapturePrimaryMonitor();
                    }
                    catch
                    {
                        App.Restart();
                    }

                    break;

                case WinMsg.WM_MOUSEACTIVATE:
                    handled = true;
                    return new IntPtr( WinMsg.MA_NOACTIVATE );
            }

            return IntPtr.Zero;
        }

        private void PerformAnimationPrimary( VirtualDesktopSwitchInfo vdSwitchInfo )
        {
            var mi = ( from m in MonitorEnumerationHelper.GetMonitors() where m.IsPrimary select m ).First();
            _capture = D3D9ShareCapture.Create( mi, _frameProcessor );
            _capture?.StartCaptureSession();

            if ( ( SettingsManager.Settings.TransitionType & TransitionType.NotificationGridOnly ) > 0 )
                NotificationGridLayout( vdSwitchInfo.vdCount );

            var em = EaseFactory.GetEaseModeByName( SettingsManager.Settings.EaseMode );
            var ef = EaseFactory.GetEaseByName( SettingsManager.Settings.EaseType, em );

            _frameProcessor.SetAction( () =>
            {
                //////////////////////////////////////////////////////
                // trigger action only after first frame be proceeded
                // see FrameToD3DImage.Proceed() for detail.
                RealShow( true );

                if ( ( SettingsManager.Settings.TransitionType & TransitionType.NotificationGridOnly ) > 0 )
                {
                    NotificationGridAnimation( vdSwitchInfo.fromIndex, vdSwitchInfo.targetIndex, vdSwitchInfo.vdCount, ef );
                    Interlocked.Increment( ref _mainWindowRunningAnimationCount );
                }

                if ( vdSwitchInfo.targetIndex != vdSwitchInfo.fromIndex &&
                     ( SettingsManager.Settings.TransitionType & TransitionType.AnimationOnly ) > 0 )
                {
                    _effect.AnimationInDirection( (KeyCode)vdSwitchInfo.dir, MainModel3DGroup, ef );
                    Interlocked.Increment( ref _mainWindowRunningAnimationCount );
                }

                WinApi.PostMessage( vdSwitchInfo.hostHandle, WinApi.UM_SWITCHDESKTOP, (uint)vdSwitchInfo.targetIndex, 0 );
            } );
        }

        private void PerformAnimationOthers( VirtualDesktopSwitchInfo vdSwitchInfo )
        {
            if ( ( SettingsManager.Settings.TransitionType & TransitionType.NotificationGridOnly ) == 0 ) return;

            NotificationGridLayout( vdSwitchInfo.vdCount );

            var em = EaseFactory.GetEaseModeByName( SettingsManager.Settings.EaseMode );
            var ef = EaseFactory.GetEaseByName( SettingsManager.Settings.EaseType, em );

            RealShow();

            NotificationGridAnimation( vdSwitchInfo.fromIndex, vdSwitchInfo.targetIndex, vdSwitchInfo.vdCount, ef );
        }
    }
}

#pragma warning restore CA1416