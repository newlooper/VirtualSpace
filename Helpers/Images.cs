/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using VirtualSpace.AppLogs;

namespace VirtualSpace.Helpers
{
    internal static class Images
    {
        public static Bitmap GetScaledBitmap( int width, int height, string path, ref Wallpaper wp, string cachePath )
        {
            var cached = Wallpaper.CachedWallPaper( path, cachePath );

            if ( cached != null ) return cached;
            using ( var src = new Bitmap( path ) )
            {
                var dest = new Bitmap( width, height, PixelFormat.Format32bppPArgb );
                using ( var gr = Graphics.FromImage( dest ) )
                {
                    gr.DrawImage( src, new Rectangle( Point.Empty, dest.Size ) );
                }

                var md5Path = Wallpaper.Md5Hash( path );
                var file = Path.Combine( cachePath, md5Path[1], md5Path[2],
                    md5Path[0] + "_" + Environment.CurrentManagedThreadId );
                dest.Save( file, ImageFormat.Jpeg );
                wp.Fullpath = file;

                return dest;
            }
        }
    }

    public class Wallpaper
    {
        public Bitmap? Image    { get; set; }
        public Color   Color    { get; set; }
        public string? Fullpath { get; set; }

        public static Bitmap? CachedWallPaper( string path, string cachePath )
        {
            var md5Path    = Md5Hash( path );
            var targetPath = Path.Combine( cachePath, md5Path[1], md5Path[2] );
            Directory.CreateDirectory( targetPath );
            var file = Path.Combine( targetPath, md5Path[0] );
            if ( File.Exists( file ) )
            {
                return new Bitmap( file );
            }

            return null;
        }

        public static string[] Md5Hash( string input )
        {
            var md5        = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes( input );
            var hashBytes  = md5.ComputeHash( inputBytes );

            var sb = new StringBuilder();
            foreach ( var b in hashBytes )
            {
                sb.Append( b.ToString( "x2" ) );
            }

            var md5Str = sb.ToString();
            var result = new string[3];

            result[0] = md5Str;
            result[1] = md5Str.Substring( 0, 1 );
            result[2] = md5Str.Substring( 1, 1 );

            return result;
        }

        public void Release()
        {
            Image?.Dispose();
            if ( string.IsNullOrEmpty( Fullpath ) ) return;
            try
            {
                var file = Regex.Replace( Fullpath, @"(.*?)_\d+$", "$1" );
                File.Move( Fullpath, file );
            }
            catch ( Exception ex )
            {
                File.Delete( Fullpath );
                Logger.Warning( "Delete cache file: " + ex.Message );
            }
        }
    }
}