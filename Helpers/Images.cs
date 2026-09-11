/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VirtualSpace.Helpers;

internal static class PathInfo
{
    public const string FILE_NAME_SPLITTER = "_";

    public static string GenImageName( string fullString, int width, int height )
    {
        return fullString + FILE_NAME_SPLITTER +
               width.ToString( "x2" ) + "x" + height.ToString( "x2" );
    }

    public static (string FullString, string Str0, string Str1) Md5Hash( string input )
    {
        var inputBytes = Encoding.ASCII.GetBytes( input );
        var hashBytes  = MD5.HashData( inputBytes );

        var sb = new StringBuilder();
        foreach ( var b in hashBytes )
        {
            sb.Append( b.ToString( "x2" ) );
        }

        var md5Str = sb.ToString();

        return new ValueTuple<string, string, string>(
            md5Str,
            md5Str.Substring( 0, 1 ),
            md5Str.Substring( 1, 1 )
        );
    }
}

public static class Images
{
    public static Icon BytesToIcon( object bytes )
    {
        using var ms = new MemoryStream( (byte[])bytes );
        return new Icon( ms );
    }

    public static Bitmap BytesToBitmap( object bytes )
    {
        using var ms = new MemoryStream( (byte[])bytes );
        return new Bitmap( ms );
    }
}

public static class Wallpaper
{
    public static (bool Exists, string Path) CachedWallPaperInfo( string path, string cachePath, int width, int height )
    {
        var (fullString, str0, str1) = PathInfo.Md5Hash( path );
        var targetPath = Path.Combine( cachePath, str0, str1 );
        Directory.CreateDirectory( targetPath );
        var filepath = Path.Combine( targetPath, PathInfo.GenImageName( fullString, width, height ) );

        return new ValueTuple<bool, string>( File.Exists( filepath ), filepath );
    }
}
