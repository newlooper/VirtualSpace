// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of Cube3D.
//
// Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Cube3D.Config;

namespace Cube3D.D3DImages
{
    public static class D3DImages
    {
        public static D3DImage FrontD3DImage  { get; private set; }
        public static D3DImage OthersD3DImage { get; private set; }

        public static Dictionary<string, D3DImageInfo> D3DImageDict { get; private set; } = new();

        public static void Initialize( ResourceDictionary resources )
        {
            FrontD3DImage  = (D3DImage)resources[Const.Front];
            OthersD3DImage = (D3DImage)resources[Const.Others];
            D3DImageDict = new Dictionary<string, D3DImageInfo>
            {
                { Const.Front, new D3DImageInfo { Image = FrontD3DImage } },
                { Const.Others, new D3DImageInfo { Image = OthersD3DImage } }
            };
        }

        public static void Reset()
        {
            ClearBackBuffer( FrontD3DImage );
            ClearBackBuffer( OthersD3DImage );
            FrontD3DImage  = null;
            OthersD3DImage = null;
            D3DImageDict   = new Dictionary<string, D3DImageInfo>();
        }

        private static void ClearBackBuffer( D3DImage image )
        {
            if ( image == null ) return;
            if ( !image.TryLock( TimeSpan.FromMilliseconds( 200 ) ) ) return;
            try
            {
                image.SetBackBuffer( D3DResourceType.IDirect3DSurface9, IntPtr.Zero );
            }
            finally
            {
                image.Unlock();
            }
        }
    }
}
