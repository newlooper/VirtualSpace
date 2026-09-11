/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop;

internal static class WallpaperImages
{
    private static readonly Dictionary<string, SharedEntry> Shared = new();
    private static readonly Lock                            Gate   = new();

    public static string Key( string path, int width, int height )
    {
        return $"{path}|{width}|{height}";
    }

    public static BitmapSource? Retain( string path, int width, int height, string cachePath, long quality )
    {
        var key = Key( path, width, height );
        lock ( Gate )
        {
            if ( Shared.TryGetValue( key, out var hit ) )
            {
                hit.Refs++;
                return hit.Source;
            }
        }

        EnsureCache( path, width, height, cachePath, quality );
        var (exists, file) = Wallpaper.CachedWallPaperInfo( path, cachePath, width, height );
        if ( !exists )
        {
            return null;
        }

        var source = LoadJpeg( file );
        lock ( Gate )
        {
            if ( Shared.TryGetValue( key, out var race ) )
            {
                race.Refs++;
                return race.Source;
            }

            Shared[key] = new SharedEntry { Source = source, Refs = 1 };
            return source;
        }
    }

    public static void Release( string? key )
    {
        if ( string.IsNullOrEmpty( key ) )
        {
            return;
        }

        lock ( Gate )
        {
            if ( !Shared.TryGetValue( key, out var entry ) )
            {
                return;
            }

            entry.Refs--;
            if ( entry.Refs <= 0 )
            {
                Shared.Remove( key );
            }
        }
    }

    public static void EnsureCache( string path, int width, int height, string cachePath, long quality )
    {
        var (exists, dest) = Wallpaper.CachedWallPaperInfo( path, cachePath, width, height );
        if ( exists )
        {
            return;
        }

        var temp = dest + "_" + Environment.CurrentManagedThreadId;
        try
        {
            RunSta( () =>
            {
                var scaled = DecodeScaled( path, width, height );
                var encoder = new JpegBitmapEncoder
                {
                    QualityLevel = (int)Math.Clamp( quality, 1, 100 )
                };
                encoder.Frames.Add( BitmapFrame.Create( scaled ) );
                using var output = File.Create( temp );
                encoder.Save( output );
            } );

            CommitCacheFile( temp, dest );
        }
        catch ( Exception ex )
        {
            Logger.Warning( "Wallpaper cache: " + ex.Message );
            TryDelete( temp );
        }
    }

    private static BitmapImage DecodeScaled( string file, int width, int height )
    {
        using var stream = new MemoryStream( File.ReadAllBytes( file ), false );
        var       bi     = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource      = stream;
        bi.DecodePixelWidth  = width;
        bi.DecodePixelHeight = height;
        bi.CacheOption       = BitmapCacheOption.OnLoad;
        bi.CreateOptions     = BitmapCreateOptions.IgnoreColorProfile;
        bi.EndInit();
        bi.Freeze();
        return bi;
    }

    private static BitmapImage LoadJpeg( string file )
    {
        using var stream = new MemoryStream( File.ReadAllBytes( file ), false );
        var       bi     = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource  = stream;
        bi.CacheOption   = BitmapCacheOption.OnLoad;
        bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
        bi.EndInit();
        bi.Freeze();
        return bi;
    }

    private static void CommitCacheFile( string temp, string dest )
    {
        try
        {
            if ( !File.Exists( dest ) )
            {
                File.Move( temp, dest );
            }
        }
        catch ( Exception ex )
        {
            Logger.Warning( "Commit wallpaper cache: " + ex.Message );
        }
        finally
        {
            TryDelete( temp );
        }
    }

    private static void TryDelete( string file )
    {
        try
        {
            if ( File.Exists( file ) )
            {
                File.Delete( file );
            }
        }
        catch
        {
            // ignored
        }
    }

    private static void RunSta( Action action )
    {
        if ( Thread.CurrentThread.GetApartmentState() == ApartmentState.STA )
        {
            action();
            return;
        }

        Exception? ex   = null;
        var        done = new ManualResetEventSlim( false );
        var thread = new Thread( () =>
        {
            try
            {
                action();
            }
            catch ( Exception e )
            {
                ex = e;
            }
            finally
            {
                done.Set();
            }
        } );
        thread.SetApartmentState( ApartmentState.STA );
        thread.IsBackground = true;
        thread.Start();
        done.Wait();
        if ( ex is not null )
        {
            throw ex;
        }
    }

    private sealed class SharedEntry
    {
        public          int          Refs;
        public required BitmapSource Source { get; init; }
    }
}