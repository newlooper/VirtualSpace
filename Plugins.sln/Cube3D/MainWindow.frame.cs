/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Diagnostics;
using System.Threading.Tasks;
using ScreenCapture;

namespace Cube3D
{
    public partial class MainWindow
    {
        private D3D9ShareCapture? _capture;
        private FrameToD3DImage?  _frameProcessor;

        private Task StartPrimaryMonitorCapture()
        {
            var monitor = TryGetPrimaryMonitor();
            return monitor == null ? Task.CompletedTask : StartMonitorCapture( monitor, _windowLoadGeneration );
        }

        private async Task StartMonitorCapture( MonitorInfo mi, int loadGeneration )
        {
            if ( !await WarmupMonitorCaptureAsync( loadGeneration, mi ).ConfigureAwait( true ) )
                ScheduleCaptureRetry();
        }

        private async Task<bool> WarmupMonitorCaptureAsync( int loadGeneration, MonitorInfo? monitor = null )
        {
            monitor ??= TryGetPrimaryMonitor();
            if ( monitor == null ) return false;

            _frameProcessor ??= new FrameToD3DImage( D3DImages.D3DImages.D3DImageDict );

            if ( !TryStartCapture( monitor ) )
                return false;

#if DEBUG
            await Task.Delay( 50 ).ConfigureAwait( true );
#endif
            if ( !IsLoadCurrent( loadGeneration ) || !IsLoaded )
            {
                StopCapture();
                return true;
            }

            StopCapture();
            return true;
        }

        private bool TryStartCapture( MonitorInfo mi )
        {
            StopCapture();

            _frameProcessor ??= new FrameToD3DImage( D3DImages.D3DImages.D3DImageDict );
            _capture = D3D9ShareCapture.Create( mi, _frameProcessor );
            if ( _capture == null )
            {
                Trace.WriteLine( "[Cube3D.Error] capture create failed." );
                return false;
            }

            if ( _capture.StartCaptureSession() ) return true;

            Trace.WriteLine( "[Cube3D.Error] capture session failed." );
            StopCapture();
            return false;
        }
    }
}
