/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow
    {
        public void AddWindow( VisibleWindow wnd )
        {
            _visibleWindows.Add( wnd );
        }

        public void ClearVisibleWindows()
        {
            ReleaseThumbnails();
            _visibleWindows.Clear();
        }

        public void ShowThumbnails()
        {
            var wndCount = _visibleWindows.Count;
            if ( !string.IsNullOrEmpty( WindowFilter.Keyword ) )
            {
                _visibleWindows.RemoveAll( wnd => !wnd.Title.ToLower().Contains( WindowFilter.Keyword.ToLower() ) );
                wndCount = _visibleWindows.Count;
            }

            if ( wndCount < 1 ) return;
            _visibleWindows.Sort( ( x, y ) => x.Title.CompareTo( y.Title ) );

            var rows = Math.Floor( Math.Sqrt( wndCount ) );
            var cols = Math.Ceiling( wndCount / rows );

            var marginH = VirtualDesktopManager.Ui.ThumbMargin.Left;
            var marginV = VirtualDesktopManager.Ui.ThumbMargin.Top;

            //////////////////////////////////////
            // thumb container size
            var thumbWidth  = ( Width - ( cols + 1 ) * marginH ) / cols;
            var thumbHeight = ( Height - ( rows + 1 ) * marginV ) / rows;

            //////////////////////////////////////
            // show thumbnails
            for ( int index = 0, row = 0; row < rows; row++ )
            {
                var topLeftY = Padding.Top + ( row + 1 ) * marginV + row * thumbHeight;

                for ( var col = 0; col < cols; col++ )
                {
                    if ( index >= wndCount ) break;
                    var topLeftX = Padding.Left + ( col + 1 ) * marginH + col * thumbWidth;

                    var i     = -1;
                    var thumb = IntPtr.Zero;
                    if ( InvokeRequired )
                    {
                        var idx = index;

                        void Invoker()
                        {
                            i = DwmApi.DwmRegisterThumbnail( Handle, _visibleWindows[idx].Handle, out thumb );
                        }

                        Invoke( (MethodInvoker)Invoker );
                    }
                    else
                    {
                        i = DwmApi.DwmRegisterThumbnail( Handle, _visibleWindows[index].Handle, out thumb );
                    }

                    if ( i == 0 )
                    {
                        _visibleWindows[index].Thumb = thumb;

                        var props = ScaleCenter(
                            thumb,
                            new RECT(
                                (int)topLeftX,
                                (int)topLeftY,
                                (int)( topLeftX + thumbWidth ),
                                (int)( topLeftY + thumbHeight )
                            )
                        );

                        _visibleWindows[index].SetValidArea( props );
                        UpdateThumbnail( thumb, props );
                    }

                    index++;
                }
            }
        }

        private static DWM_THUMBNAIL_PROPERTIES ScaleCenter( IntPtr thumb, RECT rect )
        {
            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_OPACITY,
                opacity = 255,
                rcDestination = rect
            };

            if ( thumb == IntPtr.Zero ) return props;

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
                props.rcDestination.Left += xOffset;
                props.rcDestination.Right -= xOffset;
            }
            else
            {
                var scaleFactor = cellWidth / (double)srcSize.cx;
                var scaledY     = (int)( srcSize.cy * scaleFactor );
                var yOffset     = ( cellHeight - scaledY ) / 2;
                props.rcDestination.Top += yOffset;
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
            foreach ( var window in _visibleWindows ) _ = DwmApi.DwmUnregisterThumbnail( window.Thumb );
        }
    }
}