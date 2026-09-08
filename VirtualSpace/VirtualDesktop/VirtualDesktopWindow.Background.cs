/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private string? _cacheBuildKey;
    private Size    _initSize = Size.Empty;
    private Font?   _nameFont;
    private Bitmap? _wallpaperBmp;
    private string? _wallpaperKey;

    private static long WallpaperQuality => ConfigManager.Configs.Cluster.VdwWallpaperQuality;

    private static string WallpaperKey( string path, int width, int height, long quality )
    {
        return $"{path}|{width}|{height}|{quality}";
    }

    public void UpdateWallpaper()
    {
        void InvalidateAndRefresh()
        {
            _wallpaperKey  = null;
            _cacheBuildKey = null;
            Refresh();
        }

        if ( InvokeRequired )
        {
            Invoke( (MethodInvoker)InvalidateAndRefresh );
        }
        else
        {
            InvalidateAndRefresh();
        }
    }

    public void UpdateDesktopName( string name )
    {
        _desktopName = name;
        Invalidate( new Rectangle( 0, Math.Max( 0, Height - 30 ), Width, 30 ) );
    }

    private void ReleaseWallpaperResources()
    {
        _wallpaperBmp?.Dispose();
        _wallpaperBmp  = null;
        _wallpaperKey  = null;
        _cacheBuildKey = null;
        _nameFont?.Dispose();
        _nameFont = null;
    }

    private void AdoptWallpaper( Wallpaper wp, string key )
    {
        var image = wp.Image;
        wp.Image = null;
        wp.Release();

        if ( image is null )
        {
            return;
        }

        _wallpaperBmp?.Dispose();
        _wallpaperBmp = image;
        _wallpaperKey = key;
    }

    private void DrawWallpaper( Graphics g )
    {
        if ( _wallpaperBmp is null )
        {
            return;
        }

        if ( _wallpaperBmp.Width == Width && _wallpaperBmp.Height == Height )
        {
            g.DrawImageUnscaled( _wallpaperBmp, 0, 0 );
            return;
        }

        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode   = PixelOffsetMode.Half;
        g.DrawImage( _wallpaperBmp, 0, 0, Width, Height );
    }

    private void RequestDiskCache( string path, int width, int height )
    {
        var key = WallpaperKey( path, width, height, WallpaperQuality );
        if ( _cacheBuildKey == key )
        {
            return;
        }

        _cacheBuildKey = key;
        var hWnd      = Handle;
        var cachePath = ConfigManager.GetCachePath();
        var quality   = WallpaperQuality;
        Task.Run( () =>
        {
            WinRegistry.GetWallpaperByPath( path, width, height, cachePath, quality ).Release();
            User32.PostMessage( hWnd, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
        } );
    }

    private void PaintWallpaper( PaintEventArgs e, (bool isCached, string path, Color? color) wpInfo )
    {
        if ( wpInfo.color != null )
        {
            BackColor = (Color)wpInfo.color;
            if ( _wallpaperBmp is null )
            {
                return;
            }

            _wallpaperBmp.Dispose();
            _wallpaperBmp = null;
            _wallpaperKey = null;
            return;
        }

        if ( Width < 1 || Height < 1 )
        {
            return;
        }

        var key = WallpaperKey( wpInfo.path, Width, Height, WallpaperQuality );
        if ( _wallpaperBmp != null && _wallpaperKey == key )
        {
            DrawWallpaper( e.Graphics );
            return;
        }

        if ( wpInfo.isCached || VirtualDesktopManager.IsBatchCreate )
        {
            AdoptWallpaper(
                WinRegistry.GetWallpaperByPath( wpInfo.path, Width, Height,
                    ConfigManager.GetCachePath(), WallpaperQuality ),
                key );
            DrawWallpaper( e.Graphics );
            return;
        }

        Logger.Event( $"Create cache image({Width}*{Height}) for Desktop[{VdIndex}]" );
        RequestDiskCache( wpInfo.path, Width, Height );
        DrawWallpaper( e.Graphics );
    }

    private void PaintDesktopLabel( PaintEventArgs e )
    {
        var ui  = ConfigManager.CurrentProfile.UI;
        var str = "";

        if ( ui.ShowVdName )
        {
            str += _desktopName;
        }

        if ( ui.ShowVdIndex )
        {
            str += ui.ShowVdIndexType == 0 ? $"[{VdIndex}]" : $"[{VdIndex + 1}]";
        }

        if ( str == "" )
        {
            return;
        }

        _nameFont ??= new Font( "Segoe UI emoji", 10 );
        e.Graphics.DrawString( str, _nameFont, Brushes.Beige, new Point( 2, Height - 30 ) );
    }

    private void Background_Paint( object sender, PaintEventArgs e )
    {
        if ( _initSize == Size.Empty )
        {
            Logger.Event( $"Init Desktop[{VdIndex}] background." );
            _initSize =  Size;
            Resize    += RefreshThumbs;
        }

        var wpPath = WinRegistry.GetWallPaperPathByGuid( VdId );
        if ( wpPath is null )
        {
            PaintWallpaper( e, ( false, "", WinRegistry.GetBackColor() ) );
            PaintDesktopLabel( e );
            return;
        }

        var key = WallpaperKey( wpPath, Width, Height, WallpaperQuality );
        if ( _wallpaperBmp != null && _wallpaperKey == key )
        {
            DrawWallpaper( e.Graphics );
            PaintDesktopLabel( e );
            return;
        }

        var (exists, _) = Wallpaper.CachedWallPaperInfo( wpPath, ConfigManager.GetCachePath(), Width, Height );
        PaintWallpaper( e, ( exists, wpPath, null ) );
        PaintDesktopLabel( e );
    }
}