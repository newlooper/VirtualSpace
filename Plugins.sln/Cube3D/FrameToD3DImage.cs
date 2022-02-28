/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using ScreenCapture;

namespace Cube3D
{
    public class FrameToD3DImage : FrameProcessor
    {
        public readonly Dictionary<string, D3DImageInfo> D3DImages = new Dictionary<string, D3DImageInfo>();

        public override void Proceed( IntPtr pointer )
        {
            foreach ( var (_, value) in D3DImages )
            {
                if ( !value.Draw ) continue;
                var d3DImage = value.Image;

                if ( d3DImage.TryLock( new Duration( new TimeSpan( 0, 0, 0, 0, 16 ) ) ) )
                {
                    d3DImage.SetBackBuffer( D3DResourceType.IDirect3DSurface9, pointer );
                    d3DImage.AddDirtyRect( new Int32Rect( 0, 0, d3DImage.PixelWidth, d3DImage.PixelHeight ) );
                }

                d3DImage.Unlock();
            }
        }
    }

    public class D3DImageInfo
    {
        public D3DImage Image { get; set; }
        public bool     Draw  { get; set; }
    }
}