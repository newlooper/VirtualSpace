/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

namespace Cube3D
{
    public partial class MainWindow
    {
        private const uint WDA_NONE               = 0;
        private const uint WDA_MONITOR            = 1;
        private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport( "user32.dll" )]
        public static extern uint SetWindowDisplayAffinity( IntPtr hWnd, uint dwAffinity );

        [DllImport( "gdi32.dll", EntryPoint = "DeleteObject" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool DeleteObject( [In] IntPtr hObject );

        private static Bitmap Screenshot()
        {
            var       screenBounds = Screen.GetBounds( Point.Empty );
            using var src          = new Bitmap( screenBounds.Width, screenBounds.Height );
            using var g            = Graphics.FromImage( src );
            g.CopyFromScreen( Point.Empty, Point.Empty, screenBounds.Size );

            var       dest = new Bitmap( screenBounds.Width * 1 / 2, screenBounds.Height * 1 / 2, PixelFormat.Format32bppPArgb );
            using var gr   = Graphics.FromImage( dest );
            gr.DrawImage( src, new Rectangle( Point.Empty, dest.Size ) );

            return dest;
        }

        private static ImageSource ImageSourceFromBitmap( Bitmap bmp )
        {
            var hBitmap = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap( hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
            }
            finally
            {
                DeleteObject( hBitmap );
                // GC.Collect();
            }
        }

        private static ImageBrush ImageBrushFromBitmap( Bitmap bmp )
        {
            return new ImageBrush( ImageSourceFromBitmap( bmp ) );
        }
    }
}