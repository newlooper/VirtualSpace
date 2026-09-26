/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Threading;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private const double TOLERANCE = 0.0000000001;
    private       bool   _surfaceRefreshQueued;

    public void AddWindow( VisibleWindow wnd )
    {
        _visibleWindows.Add( wnd );
    }

    public void ClearVisibleWindows()
    {
        ReleaseThumbnails();
        _visibleWindows.Clear();
        ClearThumbCaptions();
    }

    private void SetSurfaceSize( double width, double height )
    {
        if ( Math.Abs( _surface.Width - width ) < TOLERANCE && Math.Abs( _surface.Height - height ) < TOLERANCE )
        {
            Logger.Verbose( $"Desktop[{VdIndex}] size not changed." );
            return;
        }

        _surface.Width  = width;
        _surface.Height = height;
        QueueRefreshSurface();
    }

    private void QueueRefreshSurface()
    {
        if ( _surfaceRefreshQueued )
        {
            return;
        }

        _surfaceRefreshQueued = true;
        _                     = _surface.Dispatcher.InvokeAsync( RefreshSurfaceCore, DispatcherPriority.Render );
    }

    private void RefreshSurfaceCore()
    {
        _surfaceRefreshQueued = false;

        Logger.Event( $"Refresh background/Repaint thumbs in Desktop[{VdIndex}] due to size changed." );

        RefreshWallpaper();

        ReleaseThumbnails();
        ShowThumbnails();
    }

    public void ShowThumbnails()
    {
        if ( !_surface.Dispatcher.CheckAccess() )
        {
            _surface.Dispatcher.Invoke( ShowThumbnails );
            return;
        }

        var wndCount = _visibleWindows.Count;

        if ( wndCount < 1 )
        {
            ClearThumbCaptions();
            return;
        }

        var captionMode    = VirtualDesktopManager.Ui.ThumbCaptionMode;
        var groupByMonitor = captionMode.GroupsByMonitor();
        var showCaption    = captionMode.ShowsCaption();
        if ( groupByMonitor )
        {
            SortByScreenThenTitle();
        }
        else
        {
            _visibleWindows.Sort( WindowTitleOrder() );
        }

        var rows = Math.Floor( Math.Sqrt( wndCount ) );
        var cols = Math.Ceiling( wndCount / rows );

        var marginH = VirtualDesktopManager.Ui.ThumbMargin!.Left;
        var marginV = VirtualDesktopManager.Ui.ThumbMargin.Top;

        //////////////////////////////////////
        // thumb container size
        var ox          = VdwSurface.ChromeInsetPixelX;
        var oy          = VdwSurface.ChromeInsetPixelY;
        var pixelWidth  = PixelWidth;
        var pixelHeight = PixelHeight;
        var thumbWidth  = ( pixelWidth - ( cols + 1 ) * marginH ) / cols;
        var thumbHeight = ( pixelHeight - ( rows + 1 ) * marginV ) / rows;

        //////////////////////////////////////
        // show thumbnails
        for ( int index = 0, row = 0; row < rows; row++ )
        {
            var topLeftY = oy + _vdwPadding + ( row + 1 ) * marginV + row * thumbHeight;

            for ( var col = 0; col < cols; col++ )
            {
                if ( index >= wndCount )
                {
                    break;
                }

                var topLeftX = ox + _vdwPadding + ( col + 1 ) * marginH + col * thumbWidth;

                var i = DwmApi.DwmRegisterThumbnail( Handle, _visibleWindows[index].Handle, out var thumb );

                if ( i == 0 )
                {
                    _visibleWindows[index].Thumb = thumb;

                    var card = new RECT(
                        (int)topLeftX,
                        (int)topLeftY,
                        (int)( topLeftX + thumbWidth ),
                        (int)( topLeftY + thumbHeight )
                    );
                    DWM_THUMBNAIL_PROPERTIES props;
                    if ( showCaption )
                    {
                        var image = CaptionImageRect( card );
                        props = ScaleCenter( thumb, image );
                        PinThumbnailUnderCaption( ref props, card, image );
                    }
                    else
                    {
                        props = ScaleCenter( thumb, card );
                    }

                    _visibleWindows[index].ThumbCell = card;
                    _visibleWindows[index].SetValidArea( props );
                    UpdateThumbnail( thumb, props );
                }

                index++;
            }
        }

        if ( showCaption )
        {
            RefreshThumbCaptions();
        }
        else
        {
            ClearThumbCaptions();
        }
    }

    private static DWM_THUMBNAIL_PROPERTIES ScaleCenter( IntPtr thumb, RECT rect )
    {
        var props = new DWM_THUMBNAIL_PROPERTIES
        {
            fVisible      = true,
            dwFlags       = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_OPACITY,
            opacity       = byte.MaxValue,
            rcDestination = rect
        };

        if ( thumb == IntPtr.Zero )
        {
            return props;
        }

        _ = DwmApi.DwmQueryThumbnailSourceSize( thumb, out var srcSize );

        var cellWidth       = rect.Right - rect.Left;
        var cellHeight      = rect.Bottom - rect.Top;
        var cellAspectRatio = cellWidth / (double)cellHeight;
        var srcAspectRatio  = srcSize.cx / (double)srcSize.cy;

        if ( cellAspectRatio > srcAspectRatio )
        {
            var scaleFactor = cellHeight / (double)srcSize.cy;
            var scaledX     = (int)( srcSize.cx * scaleFactor );
            var xOffset     = ( cellWidth - scaledX ) / 2;
            props.rcDestination.Left  += xOffset;
            props.rcDestination.Right -= xOffset;
        }
        else
        {
            var scaleFactor = cellWidth / (double)srcSize.cx;
            var scaledY     = (int)( srcSize.cy * scaleFactor );
            var yOffset     = ( cellHeight - scaledY ) / 2;
            props.rcDestination.Top    += yOffset;
            props.rcDestination.Bottom -= yOffset;
        }

        return props;
    }

    private static void UpdateThumbnail( IntPtr thumb, DWM_THUMBNAIL_PROPERTIES props )
    {
        _ = DwmApi.DwmUpdateThumbnailProperties( thumb, ref props );
    }

    private void ReleaseThumbnails()
    {
        foreach ( var window in _visibleWindows )
        {
            _ = DwmApi.DwmUnregisterThumbnail( window.Thumb );
        }
    }
}