// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace Cube3D
{
    public static class D3DImages
    {
        public const           string   Front          = nameof( Front );
        public const           string   Others         = nameof( Others );
        public static readonly D3DImage FrontD3DImage  = Application.Current.Resources[Front] as D3DImage;
        public static readonly D3DImage OthersD3DImage = Application.Current.Resources[Others] as D3DImage;

        public static readonly Dictionary<string, D3DImageInfo> D3DImageDict = new()
        {
            {
                Front, new D3DImageInfo
                {
                    Draw = true,
                    Image = FrontD3DImage
                }
            },
            {
                Others, new D3DImageInfo
                {
                    Draw = true,
                    Image = OthersD3DImage
                }
            }
        };
    }
}