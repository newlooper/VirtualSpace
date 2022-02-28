/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using ScreenCapture;

namespace Cube3D
{
    public partial class MainWindow
    {
        private FrameToD3DImage _frameProcessor;

        private void StartPrimaryMonitorCapture()
        {
            var monitor = ( from m in MonitorEnumerationHelper.GetMonitors()
                where m.IsPrimary
                select m ).First();
            StartMonitorCapture( monitor.Hmon );
        }

        private void StartMonitorCapture( IntPtr hMon )
        {
            var item = CaptureHelper.CreateItemForMonitor( hMon );
            if ( item == null ) return;
            _frameProcessor = new FrameToD3DImage();
            foreach ( var surface in new List<string> {Front, Others} )
            {
                _frameProcessor.D3DImages[surface] = new D3DImageInfo
                {
                    Draw = true,
                    Image = Resources[surface] as D3DImage
                };
            }

            var capture = new D3D9ShareCapture( item, _frameProcessor );
            capture.StartCapture();
        }
    }
}