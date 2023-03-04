/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Runtime.InteropServices;
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
        private void FakeHide( bool recreateCapture = false )
        {
            Left = Const.FakeHideX;
            Top = Const.FakeHideY;
            // Width = SystemParameters.PrimaryScreenWidth / 10;
            // Height = SystemParameters.PrimaryScreenHeight / 10;
            // Width = 0;
            // Height = 0;
            if ( recreateCapture ) RecreateCapture();
        }

        private void RecreateCapture()
        {
            _capture?.StopCaptureSession();
        }

        private void RealShow()
        {
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            //////////////////////////////////////////////////
            // since HostProgram's MainWindow as Cube3D's Owner
            // this is no need do to.
            // if ( SettingsManager.Settings.ForceOnTop )
            //     Activate();
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

                                        _frameProcessor.SetAction( () =>
                                        {
                                            //////////////////////////////////////////////////////
                                            // trigger action only after first frame be proceeded
                                            // see FrameToD3DImage.Proceed() for detail.
                                            RealShow();
                                            NotificationGridAnimation( fromIndex, targetIndex, vdCount );
                                            if ( targetIndex != fromIndex )
                                            {
                                                _effect.AnimationInDirection( (KeyCode)dir, MainModel3DGroup );
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
                    var sw = new SettingsWindow();
                    sw.ShowDialog();
                    break;

                case WinMsg.WM_DISPLAYCHANGE:
                    _capture?.UpdateCapturePrimaryMonitor();
                    break;

                case WinMsg.WM_MOUSEACTIVATE:
                    handled = true;
                    return new IntPtr( WinMsg.MA_NOACTIVATE );
            }

            return IntPtr.Zero;
        }
    }
}