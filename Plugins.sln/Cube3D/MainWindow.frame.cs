/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Linq;
using System.Threading.Tasks;
using Cube3D.Config;
using ScreenCapture;

namespace Cube3D
{
    public partial class MainWindow
    {
        private D3D9ShareCapture _capture;
        private FrameToD3DImage  _frameProcessor;

        private Task StartPrimaryMonitorCapture()
        {
            var monitor = ( from m in MonitorEnumerationHelper.GetMonitors()
                where m.IsPrimary
                select m ).First();
            return StartMonitorCapture( monitor );
        }

        private async Task StartMonitorCapture( MonitorInfo mi )
        {
            _frameProcessor = new FrameToD3DImage( D3DImages.D3DImages.D3DImageDict );
            try
            {
                _capture = D3D9ShareCapture.Create( mi, _frameProcessor );
            }
            catch
            {
                await Task.Delay( 1000 );
                App.Restart();
                return;
            }

            if ( _capture != null )
            {
                _capture.StartCaptureSession();
                await Task.Delay( Const.CaptureInitTimer );
                RecreateCapture();
            }
        }
    }
}