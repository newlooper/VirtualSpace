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
using System.Threading.Tasks;
using System.Windows;
using Cube3D.Config;
using Cube3D.Effects;
using ScreenCapture;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using VirtualSpace.Plugin;

namespace Cube3D
{
    public partial class MainWindow
    {
        private static readonly StringBuilder  SbWinInfo = new( 1024 );
        private static          bool           _isTopmost;
        private static          SettingsWindow sw;

        private void FakeHide( bool recreateCapture = false )
        {
            Left = Const.FakeHideX;
            Top = Const.FakeHideY;

            if ( recreateCapture ) RecreateCapture();
        }

        private void RecreateCapture()
        {
            _capture?.StopCaptureSession();
        }

        private static bool WindowFilter( IntPtr hWnd, int lParam )
        {
            if ( !User32.IsWindowVisible( hWnd ) )
                return true;

            SbWinInfo.Clear();
            _ = User32.GetWindowText( hWnd, SbWinInfo, SbWinInfo.Capacity );
            if ( SbWinInfo.Length == 0 )
                return true;

            _isTopmost = _handle == hWnd; // if the first visible non-empty title window is Cube3D, then Cube3D is on the top.

            return false;
        }

        private void RealShow()
        {
            _ = User32.EnumWindows( WindowFilter, 0 );
            if ( !_isTopmost )
            {
                User32.SetWindowPos( _handle, User32.SpecialWindowHandles.HWND_TOP, 0, 0, 0, 0,
                    User32.SetWindowPosFlags.SWP_NOSIZE |
                    User32.SetWindowPosFlags.SWP_NOMOVE |
                    User32.SetWindowPosFlags.SWP_NOACTIVATE |
                    User32.SetWindowPosFlags.SWP_NOREDRAW |
                    User32.SetWindowPosFlags.SWP_NOCOPYBITS |
                    User32.SetWindowPosFlags.SWP_DEFERERASE |
                    User32.SetWindowPosFlags.SWP_NOSENDCHANGING
                );
            }

            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch ( msg )
            {
                case WinMsg.WM_SYSCOMMAND:
                    var wP = wParam.ToInt32();
                    if ( wP == WinMsg.SC_RESTORE || wP == WinMsg.SC_MINIMIZE || wP == WinMsg.SC_MAXIMIZE )
                        handled = true;
                    break;

                case WinApi.WM_COPYDATA:
                    var copyDataStruct = (COPYDATASTRUCT)Marshal.PtrToStructure( lParam, typeof( COPYDATASTRUCT ) );
                    switch ( copyDataStruct.dwData.ToInt32() )
                    {
                        case WinApi.UM_SWITCHDESKTOP:
                            if ( RunningAnimationCount == 0 )
                            {
                                var vdSwitchInfo =
                                    (VirtualDesktopSwitchInfo)Marshal.PtrToStructure( copyDataStruct.lpData, typeof( VirtualDesktopSwitchInfo ) );
                                var vdCount     = vdSwitchInfo.vdCount;
                                var fromIndex   = vdSwitchInfo.fromIndex;
                                var dir         = vdSwitchInfo.dir;
                                var targetIndex = vdSwitchInfo.targetIndex;
                                Task.Run( () =>
                                {
                                    Dispatcher.Invoke( () =>
                                    {
                                        var mi = ( from m in MonitorEnumerationHelper.GetMonitors()
                                            where m.IsPrimary
                                            select m ).First();
                                        _capture = D3D9ShareCapture.Create( mi, _frameProcessor );
                                        _capture?.StartCaptureSession();
                                        NotificationGridLayout( vdCount );

                                        var em = EaseFactory.GetEaseModeByName( SettingsManager.Settings.EaseMode );
                                        var ef = EaseFactory.GetEaseByName( SettingsManager.Settings.EaseType, em );

                                        _frameProcessor.SetAction( () =>
                                        {
                                            //////////////////////////////////////////////////////
                                            // trigger action only after first frame be proceeded
                                            // see FrameToD3DImage.Proceed() for detail.
                                            RealShow();
                                            NotificationGridAnimation( fromIndex, targetIndex, vdCount, ef );
                                            if ( targetIndex != fromIndex )
                                            {
                                                _effect.AnimationInDirection( (KeyCode)dir, MainModel3DGroup, ef );
                                                WinApi.PostMessage( vdSwitchInfo.hostHandle, WinApi.UM_SWITCHDESKTOP, (uint)targetIndex, 0 );
                                            }
                                        } );
                                    } );
                                } );
                            }

                            break;
                    }

                    break;

                case WinApi.UM_PLUGINSETTINGS:
                    if ( sw is null || PresentationSource.FromVisual( sw ) == null )
                    {
                        sw = new SettingsWindow();
                        sw.SetMainWindow( this );
                        sw.ShowDialog();
                    }
                    else
                    {
                        sw.Activate();
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
    }
}