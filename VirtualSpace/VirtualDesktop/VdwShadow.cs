/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.Entity;
using Color = System.Drawing.Color;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

/// <summary>
///     Shared 9-slice shadow atlas for <see cref="VdwSurface" /> chrome.
///     Layout ring is always <see cref="MaxDip" />; displayed thickness can be thinner.
/// </summary>
internal static class VdwShadow
{
    private const int ATLAS_SIZE  = 64;
    private const int ATLAS_INSET = 16;

    private static ShadowSlices _default;
    private static ShadowSlices _current;

    internal static double DefaultDip => Math.Max( 2, ConfigManager.CurrentProfile.UI.VDWShadowSize );
    internal static double CurrentDip => Math.Max( 8, ConfigManager.CurrentProfile.UI.VDWCurrentShadowSize );
    internal static double MaxDip     => Math.Max( DefaultDip, CurrentDip );

    internal static ShadowSlices DefaultSlices
    {
        get
        {
            Ensure();
            return _default;
        }
    }

    internal static ShadowSlices CurrentSlices
    {
        get
        {
            Ensure();
            return _current;
        }
    }

    private static void Ensure()
    {
        if ( _default.Tl != null )
        {
            return;
        }

        Logger.Verbose( "Ensure Shadow Slices" );
        var ui = ConfigManager.CurrentProfile.UI;
        var d  = ui.VDWDefaultShadowColor ?? new ShadowColour { R = 20, G            = 20, B            = 20 };
        var c  = ui.VDWCurrentShadowColor ?? new ShadowColour { R = Color.White.R, G = Color.White.G, B = Color.White.B };
        _default = CreateSlices( d.R, d.G, d.B, ui.VDWDefaultShadowOpacity );
        _current = CreateSlices( c.R, c.G, c.B, ui.VDWCurrentShadowOpacity );
    }

    private static ShadowSlices CreateSlices( byte r, byte g, byte b, byte maxAlpha )
    {
        var       src = CreateAtlas( r, g, b, maxAlpha );
        const int m   = ATLAS_SIZE - 2 * ATLAS_INSET;
        return new ShadowSlices
        {
            Tl = Crop( src, 0, 0, ATLAS_INSET, ATLAS_INSET ),
            T  = Crop( src, ATLAS_INSET, 0, m, ATLAS_INSET ),
            Tr = Crop( src, ATLAS_INSET + m, 0, ATLAS_INSET, ATLAS_INSET ),
            L  = Crop( src, 0, ATLAS_INSET, ATLAS_INSET, m ),
            R  = Crop( src, ATLAS_INSET + m, ATLAS_INSET, ATLAS_INSET, m ),
            Bl = Crop( src, 0, ATLAS_INSET + m, ATLAS_INSET, ATLAS_INSET ),
            B  = Crop( src, ATLAS_INSET, ATLAS_INSET + m, m, ATLAS_INSET ),
            Br = Crop( src, ATLAS_INSET + m, ATLAS_INSET + m, ATLAS_INSET, ATLAS_INSET )
        };
    }

    private static WriteableBitmap CreateAtlas( byte r, byte g, byte b, byte maxAlpha )
    {
        var       bmp    = new WriteableBitmap( ATLAS_SIZE, ATLAS_SIZE, 96, 96, PixelFormats.Bgra32, null );
        var       pixels = new byte[ATLAS_SIZE * ATLAS_SIZE * 4];
        const int inner1 = ATLAS_SIZE - ATLAS_INSET;

        for ( var y = 0; y < ATLAS_SIZE; y++ )
        for ( var x = 0; x < ATLAS_SIZE; x++ )
        {
            var cx = x < ATLAS_INSET ? ATLAS_INSET : x >= inner1 ? inner1 - 1 : x;
            var cy = y < ATLAS_INSET ? ATLAS_INSET : y >= inner1 ? inner1 - 1 : y;
            if ( x is >= ATLAS_INSET and < inner1 && y is >= ATLAS_INSET and < inner1 )
            {
                continue;
            }

            var dist = Math.Sqrt( ( x - cx ) * ( x - cx ) + ( y - cy ) * ( y - cy ) );
            var t    = Math.Min( 1, dist / ATLAS_INSET );
            var a    = (byte)( maxAlpha * ( 1 - t ) * ( 1 - t ) );
            var o    = ( y * ATLAS_SIZE + x ) * 4;
            pixels[o]     = b;
            pixels[o + 1] = g;
            pixels[o + 2] = r;
            pixels[o + 3] = a;
        }

        bmp.WritePixels( new Int32Rect( 0, 0, ATLAS_SIZE, ATLAS_SIZE ), pixels, ATLAS_SIZE * 4, 0 );
        bmp.Freeze();
        return bmp;
    }

    private static CroppedBitmap Crop( BitmapSource src, int x, int y, int w, int h )
    {
        var crop = new CroppedBitmap( src, new Int32Rect( x, y, w, h ) );
        crop.Freeze();
        return crop;
    }

    internal readonly struct ShadowSlices
    {
        public ImageSource? Tl { get; init; }
        public ImageSource? T  { get; init; }
        public ImageSource? Tr { get; init; }
        public ImageSource? L  { get; init; }
        public ImageSource? R  { get; init; }
        public ImageSource? Bl { get; init; }
        public ImageSource? B  { get; init; }
        public ImageSource? Br { get; init; }
    }
}