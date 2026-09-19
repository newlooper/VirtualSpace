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

    public void WallpaperChangedHandler()
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
        RefreshDesktopLabel();
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

    private void AttachWallpaper( string path, int width, int height, string key )
    {
        if ( _wallpaperSource != null && _wallpaperKey == key )
        {
            _surface.SetWallpaper( _wallpaperSource );
            return;
        }

        var source = WallpaperImages.Retain( path, width, height, ConfigManager.GetCachePath(), WallpaperQuality );
        if ( source is null )
        {
            RequestDiskCache( path, width, height, key );
            return;
        }

        WallpaperImages.Release( _wallpaperKey );
        _wallpaperKey    = key;
        _wallpaperSource = source;
        _surface.SetWallpaper( source );
    }

    private void RequestDiskCache( string path, int width, int height, string key )
    {
        if ( _cacheBuildKey == key )
        {
            return;
        }

        _cacheBuildKey = key;
        var hWnd      = Handle;
        var cachePath = ConfigManager.GetCachePath();
        var quality   = WallpaperQuality;
        
        Logger.Verbose( $"Create cache image({PixelWidth}*{PixelHeight}) for Desktop[{VdIndex}]" );
        
        Task.Run( () =>
        {
            WallpaperImages.EnsureCache( path, width, height, cachePath, quality );
            User32.PostMessage( hWnd, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
        } );
    }

    private void ApplyWallpaper( Color color )
    {
        Logger.Verbose( "Apply Wallpaper: Pure Color." );
        _surface.SetBackColor( color );
        DetachWallpaper();
    }

    private void ApplyWallpaper( bool isCached, string path, string key )
    {
        if ( isCached )
        {
            Logger.Verbose( $"Apply Wallpaper: cached={isCached}, key={key}" );
            AttachWallpaper( path, PixelWidth, PixelHeight, key );
            return;
        }

        RequestDiskCache( path, PixelWidth, PixelHeight, key );
    }

    private void RefreshDesktopLabel()
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
            ApplyWallpaper( WinRegistry.GetBackColor() );
            return;
        }

        var key = WallpaperImages.Key( wpPath, PixelWidth, PixelHeight );
        if ( _wallpaperSource != null && _wallpaperKey == key )
        {
            Logger.Verbose( $"Reuse Wallpaper: key={key}" );
            _surface.SetWallpaper( _wallpaperSource );
            return;
        }

        var (exists, _) = Wallpaper.CachedWallPaperInfo( wpPath, ConfigManager.GetCachePath(), PixelWidth, PixelHeight );
        ApplyWallpaper( exists, wpPath, key );
    }
}