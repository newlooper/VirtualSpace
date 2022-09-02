﻿/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow : Form
    {
        private static   List<VirtualDesktopWindow>? _virtualDesktops;
        private static   int                         _hoverVdIndex;
        private static   Point                       _startPoint;
        private static   bool                        _dragging;
        private static   Rectangle                   _dragBounds = Rectangle.Empty;
        private static   VisibleWindow?              _selectedWindow;
        private static   DragWindow?                 _dw;
        private static   WindowInteropHelper         _windowInteropHelper;
        private readonly List<VisibleWindow>         _visibleWindows = new();
        private          string                      _desktopName;
        private          Point                       _fixedPosition;
        public           Guid                        VdId;
        public           int                         VdIndex;

        public VirtualDesktopWindow()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.Style = unchecked(cp.Style | (int)0x80000000); // WS_POPUP
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;

        public static VirtualDesktopWindow Create( int index, Guid guid, Color defaultBackColor, Size commonSize, int VDWPadding )
        {
            var vdw = new VirtualDesktopWindow
            {
                StartPosition = FormStartPosition.Manual,
                TabStop = false,
                TopLevel = true,
                TopMost = true,
                Name = "vdw_" + index,
                VdId = guid,
                VdIndex = index,
                BackColor = defaultBackColor,
                Size = commonSize,
                Padding = new Padding( VDWPadding )
            };
            vdw.SetOwner( MainWindow.GetMainWindow() );
            return vdw;
        }

        private void SetOwner( Window owner )
        {
            _windowInteropHelper ??= new WindowInteropHelper( owner );
            if ( owner.Dispatcher.CheckAccess() )
            {
                User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                    (int)GetWindowLongFields.GWL_HWNDPARENT,
                    _windowInteropHelper.Handle.ToInt32()
                );
            }
            else
            {
                owner.Dispatcher.Invoke( () =>
                    User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                        (int)GetWindowLongFields.GWL_HWNDPARENT,
                        _windowInteropHelper.Handle.ToInt32()
                    )
                );
            }
        }

        public void SetBackground( Wallpaper wp )
        {
            pbWallpaper.Image?.Dispose();
            pbWallpaper.Image = null;
            if ( wp.Image != null )
            {
                var cloneImage = new Bitmap( wp.Image );
                pbWallpaper.Image = cloneImage;
                wp.Release();
            }
            else
            {
                pbWallpaper.BackColor = wp.Color;
            }
        }

        public void AddWindow( VisibleWindow wnd )
        {
            _visibleWindows.Add( wnd );
        }

        public void ClearWindows()
        {
            ReleaseThumbnails();
            _visibleWindows.Clear();
        }

        public void ShowThumbnails()
        {
            var wndCount = _visibleWindows.Count;
            if ( wndCount < 1 ) return;

            _visibleWindows.Sort( ( x, y ) => x.Title.CompareTo( y.Title ) );

            var rows = Math.Floor( Math.Sqrt( wndCount ) );
            var cols = Math.Ceiling( wndCount / rows );

            var marginH = VirtualDesktopManager.Ui.ThumbMargin.Left;
            var marginV = VirtualDesktopManager.Ui.ThumbMargin.Top;

            //////////////////////////////////////
            // thumb container size
            var thumbWidth  = ( pbWallpaper.Width - ( cols + 1 ) * marginH ) / cols;
            var thumbHeight = ( pbWallpaper.Height - ( rows + 1 ) * marginV ) / rows;

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

        private DWM_THUMBNAIL_PROPERTIES ScaleCenter( IntPtr thumb, RECT rect )
        {
            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_OPACITY,
                opacity = 255,
                rcDestination = rect
            };

            if ( thumb != IntPtr.Zero )
            {
                DwmApi.DwmQueryThumbnailSourceSize( thumb, out var srcSize );

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
            }

            return props;
        }

        private void UpdateThumbnail( IntPtr thumb, DWM_THUMBNAIL_PROPERTIES props )
        {
            DwmApi.DwmUpdateThumbnailProperties( thumb, ref props );
        }

        private void ReleaseThumbnails()
        {
            foreach ( var window in _visibleWindows ) DwmApi.DwmUnregisterThumbnail( window.Thumb );
        }

        private void VirtualDesktopWindow_Closing( object? sender, FormClosingEventArgs e )
        {
            e.Cancel = true;
        }

        public void RealClose()
        {
            FormClosing -= VirtualDesktopWindow_Closing;
            ClearWindows();
            Close();
        }

        public void ShowByVdIndex()
        {
            var ui       = VirtualDesktopManager.Ui;
            var dpi      = SysInfo.Dpi;
            var location = MainWindow.GetCellLocation( VirtualDesktopManager.GetMatrixIndexByVdIndex( VdIndex ) );
            var point    = new Point( (int)( ( location.X + ui.VDWBorderSize ) * dpi[0] ), (int)( ( location.Y + ui.VDWBorderSize ) * dpi[1] ) );
            Location = point;
            _fixedPosition = point;
            _desktopName = DesktopWrapper.DesktopNameFromGuid( VdId );
            RefreshWallpaper();
            Show();
        }

        private void pbWallpaper_Paint( object sender, PaintEventArgs e )
        {
            using var font = new Font( "Segoe UI emoji", 10 );
            e.Graphics.DrawString(
                $"{_desktopName}[{VdIndex}]",
                font,
                Brushes.Beige,
                new Point( 2, Height - 30 )
            );
        }

        public void UpdateDesktopName( string name )
        {
            _desktopName = name;
            RefreshWallpaper();
        }

        public void RefreshWallpaper()
        {
            pbWallpaper.Refresh();
        }
    }
}