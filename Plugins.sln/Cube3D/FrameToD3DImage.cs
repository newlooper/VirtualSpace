/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cube3D.Config;
using ScreenCapture;

namespace Cube3D
{
    public class FrameToD3DImage : FrameProcessor
    {
        private readonly Dictionary<string, D3DImageInfo> _d3DImageDict;
        private readonly Duration                         _duration = new( new TimeSpan( 0, 0, 0, 0, 5 ) );
        private          Action                           _animation;

        private FrameToD3DImage()
        {
        }

        public FrameToD3DImage( Dictionary<string, D3DImageInfo> d3dImageDict )
        {
            _d3DImageDict = d3dImageDict;
        }

        public void SetAction( Action action )
        {
            _animation = action;
        }

        public override void Proceed( IntPtr pointer, ulong frameNumber )
        {
            if ( frameNumber == 1 )
            {
                Paint( _d3DImageDict[Const.Front], pointer );
            }
            else
            {
                if ( frameNumber == 2 )
                {
                    _animation?.Invoke();
                }

                Paint( _d3DImageDict[Const.Others], pointer );
            }
        }

        private void Paint( D3DImageInfo dii, IntPtr pointer )
        {
            var useSoftRender = IsSoftRender();

            var d3DImage = dii.Image;

            if ( !useSoftRender && !d3DImage.IsFrontBufferAvailable ) return;

            if ( d3DImage.TryLock( _duration ) )
            {
                d3DImage.SetBackBuffer( D3DResourceType.IDirect3DSurface9, pointer, useSoftRender );
                d3DImage.AddDirtyRect( new Int32Rect( 0, 0, d3DImage.PixelWidth, d3DImage.PixelHeight ) );
            }

            d3DImage.Unlock();
        }

        private static bool IsSoftRender()
        {
            var level = RenderCapability.Tier >> 16;
            return level == 0;
        }
    }

    public class D3DImageInfo
    {
        public D3DImage Image { get; set; }
    }
}