/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;

namespace Cube3D
{
    public partial class MainWindow
    {
        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            var source = PresentationSource.FromVisual( this ) as HwndSource;
            source?.AddHook( WndProc );
        }

        private void ShowHide( bool show = false )
        {
            if ( show )
            {
                // _frameProcessor.Interval = 50;
                _frameProcessor.D3DImages[Front].Draw = false;
                _frameProcessor.D3DImages[Others].Draw = true;
                Left = 0;
                Top = 0;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
                Activate();
            }
            else
            {
                //Left = -10000.0;
                //Top = -10000.0;
                //Width = SystemParameters.PrimaryScreenWidth / 100;
                //Height = SystemParameters.PrimaryScreenHeight / 100;
                Width = 0;
                Height = 0;
                _frameProcessor.D3DImages[Front].Draw = true;
                _frameProcessor.D3DImages[Others].Draw = false;
                // _frameProcessor.Interval = 50;
            }
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
                case WinMsg.WM_HOTKEY:
                    if ( _runningAnimationCount > 0 ) break;

                    var n           = wParam.ToInt32();
                    var vdCount     = n % IpcPipe.Power;
                    var fromIndex   = n / IpcPipe.Power % IpcPipe.Power;
                    var dir         = (Keys)( n / IpcPipe.Power / IpcPipe.Power % 100 );
                    var targetIndex = lParam.ToInt32();

                    NotificationGridLayout( vdCount );
                    ShowHide( true );

                    NotificationGridAnimation( fromIndex, targetIndex, vdCount );
                    if ( targetIndex != fromIndex )
                    {
                        CubeAnimation( dir );
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