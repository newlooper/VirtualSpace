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
        var key    = Key( path, width, height );
        var source = LoadOrBuild( path, width, height, cachePath, quality );
        if ( source is null )
        {
            return null;
        }

        lock ( Gate )
        {
            if ( Shared.TryGetValue( key, out var entry ) )
            {
                entry.Refs++;
                return entry.Source;
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
        _ = LoadOrBuild( path, width, height, cachePath, quality );
    }

    private static BitmapSource? LoadOrBuild( string path, int width, int height, string cachePath, long quality )
    {
        var key = Key( path, width, height );
        lock ( Gate )
        {
            if ( Shared.TryGetValue( key, out var hit ) )
            {
                return hit.Source;
            }
        }

        var (exists, dest) = Wallpaper.CachedWallPaperInfo( path, cachePath, width, height );
        var source = exists ? LoadJpeg( dest ) : BuildCache( path, width, height, dest, quality );
        if ( source is null )
        {
            return null;
        }

        lock ( Gate )
        {
            if ( Shared.TryGetValue( key, out var race ) )
            {
                return race.Source;
            }

            Shared[key] = new SharedEntry { Source = source, Refs = 0 };
            return source;
        }
    }

    private static BitmapSource? BuildCache( string path, int width, int height, string dest, long quality )
    {
        var temp = dest + "_" + Environment.CurrentManagedThreadId;
        try
        {
            BitmapSource? scaled = null;
            RunSta( () =>
            {
                scaled = DecodeScaled( path, width, height );
                var encoder = new JpegBitmapEncoder
                {
                    QualityLevel = (int)Math.Clamp( quality, 1, 100 )
                };
                encoder.Frames.Add( BitmapFrame.Create( scaled ) );
                using var output = File.Create( temp );
                encoder.Save( output );
            } );

            CommitCacheFile( temp, dest );
            return scaled;
        }
        catch ( Exception ex )
        {
            Logger.Warning( "Wallpaper cache: " + ex.Message );
            TryDelete( temp );
            return null;
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