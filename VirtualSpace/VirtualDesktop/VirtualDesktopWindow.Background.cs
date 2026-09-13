/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private string?       _cacheBuildKey;
    private string?       _wallpaperKey;
    private BitmapSource? _wallpaperSource;

    private static long WallpaperQuality => ConfigManager.Configs.Cluster.VdwWallpaperQuality;

    public void UpdateWallpaper()
    {
        if ( !_surface.Dispatcher.CheckAccess() )
        {
            _surface.Dispatcher.Invoke( InvalidateAndRefresh );
        }
        else
        {
            InvalidateAndRefresh();
        }

        return;

        void InvalidateAndRefresh()
        {
            DetachWallpaper();
            _cacheBuildKey = null;
            RefreshWallpaper();
        }
    }

    public void UpdateDesktopName( string name )
    {
        _desktopName = name;
        UpdateDesktopLabel();
    }

    private void ReleaseWallpaperResources()
    {
        DetachWallpaper();
        _cacheBuildKey = null;
    }

    private void DetachWallpaper()
    {
        WallpaperImages.Release( _wallpaperKey );
        _wallpaperKey    = null;
        _wallpaperSource = null;
        _surface.SetWallpaper( null );
    }

    private void AttachWallpaper( string path, int width, int height )
    {
        var key = WallpaperImages.Key( path, width, height );
        if ( _wallpaperSource != null && _wallpaperKey == key )
        {
            _surface.SetWallpaper( _wallpaperSource );
            return;
        }

        var source = WallpaperImages.Retain( path, width, height, ConfigManager.GetCachePath(), WallpaperQuality );
        if ( source is null )
        {
            RequestDiskCache( path, width, height );
            return;
        }

        WallpaperImages.Release( _wallpaperKey );
        _wallpaperKey    = key;
        _wallpaperSource = source;
        _surface.SetWallpaper( source );
    }

    private void RequestDiskCache( string path, int width, int height )
    {
        var key = WallpaperImages.Key( path, width, height );
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
            WallpaperImages.EnsureCache( path, width, height, cachePath, quality );
            User32.PostMessage( hWnd, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
        } );
    }

    private void ApplyWallpaper( (bool isCached, string path, Color? color) wpInfo )
    {
        if ( wpInfo.color != null )
        {
            _surface.SetBackColor( (Color)wpInfo.color );
            DetachWallpaper();
            return;
        }

        if ( PixelWidth < 1 || PixelHeight < 1 )
        {
            return;
        }

        var key = WallpaperImages.Key( wpInfo.path, PixelWidth, PixelHeight );
        if ( _wallpaperSource != null && _wallpaperKey == key )
        {
            _surface.SetWallpaper( _wallpaperSource );
            return;
        }

        if ( wpInfo.isCached || VirtualDesktopManager.IsBatchCreate )
        {
            AttachWallpaper( wpInfo.path, PixelWidth, PixelHeight );
            if ( _wallpaperSource is null )
            {
                RequestDiskCache( wpInfo.path, PixelWidth, PixelHeight );
            }

            return;
        }

        Logger.Verbose( $"Create cache image({PixelWidth}*{PixelHeight}) for Desktop[{VdIndex}]" );
        RequestDiskCache( wpInfo.path, PixelWidth, PixelHeight );
        if ( _wallpaperSource != null )
        {
            _surface.SetWallpaper( _wallpaperSource );
        }
    }

    private void UpdateDesktopLabel()
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

        _surface.SetDesktopLabel( str, str != "" );
    }

    private void RefreshWallpaper()
    {
        if ( PixelWidth < 1 || PixelHeight < 1 )
        {
            return;
        }

        var wpPath = WinRegistry.GetWallPaperPathByGuid( VdId );
        if ( wpPath is null )
        {
            ApplyWallpaper( ( false, "", WinRegistry.GetBackColor() ) );
            UpdateDesktopLabel();
            return;
        }

        var key = WallpaperImages.Key( wpPath, PixelWidth, PixelHeight );
        if ( _wallpaperSource != null && _wallpaperKey == key )
        {
            Logger.Verbose( $"Reuse Wallpaper: {key}" );
            _surface.SetWallpaper( _wallpaperSource );
            UpdateDesktopLabel();
            return;
        }

        var (exists, _) = Wallpaper.CachedWallPaperInfo( wpPath, ConfigManager.GetCachePath(), PixelWidth, PixelHeight );
        Logger.Verbose( "Applied Wallpaper src=" + ( exists ? wpPath : "pending" ) + " key=" + key );
        ApplyWallpaper( ( exists, wpPath, null ) );
        UpdateDesktopLabel();
    }
}