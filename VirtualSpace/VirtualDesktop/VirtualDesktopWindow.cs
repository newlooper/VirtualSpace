/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using DrawingColor = System.Drawing.Color;
using DrawingPoint = System.Drawing.Point;
using Point = System.Windows.Point;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private static   List<VirtualDesktopWindow>? _virtualDesktops;
    private readonly VdwSurface                  _surface;
    private readonly int                         _vdwPadding;
    private readonly List<VisibleWindow>         _visibleWindows = [];
    public           Guid                        VdId;
    private          bool                        _allowClose;
    private          string                      _desktopName = "";

    private VirtualDesktopWindow( int vdwPadding )
    {
        _vdwPadding               =  vdwPadding;
        _surface                  =  new VdwSurface( WndProc );
        _surface.Closing          += Surface_Closing;
        _surface.PreviewMouseDown += VirtualDesktopWindow_MouseDown;
        _surface.PreviewMouseMove += VirtualDesktopWindow_MouseMove;
        _surface.PreviewMouseUp   += VirtualDesktopWindow_MouseUp;
        _surface.Loaded           += Surface_Loaded;
    }

    public int VdIndex
    {
        get => _surface.VdIndex;
        set => _surface.VdIndex = value;
    }

    internal IntPtr Handle => _surface.Handle;

    public double Opacity
    {
        get => _surface.Opacity;
        set => _surface.Opacity = value;
    }

    private double Left
    {
        get => _surface.Left;
        set => _surface.Left = value;
    }

    private double Top
    {
        get => _surface.Top;
        set => _surface.Top = value;
    }

    private int PixelWidth => Math.Max( 0, (int)Math.Round( ContentWidthDip * SysInfo.Dpi.ScaleX ) );

    private int PixelHeight => Math.Max( 0, (int)Math.Round( ContentHeightDip * SysInfo.Dpi.ScaleY ) );

    private double ContentWidthDip => Math.Max( 0, _surface.Width - 2 * VdwSurface.ChromeInsetDip );

    private double ContentHeightDip => Math.Max( 0, _surface.Height - 2 * VdwSurface.ChromeInsetDip );

    private Rectangle PixelBoundsOnScreen
    {
        get
        {
            if ( PresentationSource.FromVisual( _surface ) is null )
            {
                return Rectangle.Empty;
            }

            var topLeft     = _surface.PointToScreen( new Point( 0, 0 ) );
            var bottomRight = _surface.PointToScreen( new Point( _surface.ActualWidth, _surface.ActualHeight ) );
            return Rectangle.FromLTRB(
                (int)Math.Round( topLeft.X ),
                (int)Math.Round( topLeft.Y ),
                (int)Math.Round( bottomRight.X ),
                (int)Math.Round( bottomRight.Y ) );
        }
    }

    private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
    {
        switch ( msg )
        {
            case WinMsg.WM_QUERYENDSESSION:
                handled = true;
                return new IntPtr( 1 );
            case WinMsg.WM_HOTKEY:
                switch ( wParam.ToInt32() )
                {
                    case UserMessage.ShowVdw:
                        ShowByVdIndex();
                        handled = true;
                        return IntPtr.Zero;
                    case UserMessage.RefreshVdw:
                        RefreshWallpaper();
                        handled = true;
                        return IntPtr.Zero;
                    case UserMessage.ShowThumbsOfVdw:
                        ShowThumbnails();
                        handled = true;
                        return IntPtr.Zero;
                    case UserMessage.RefreshVdLabel:
                        RefreshDesktopLabel();
                        handled = true;
                        return IntPtr.Zero;
                }

                break;
        }

        return IntPtr.Zero;
    }

    public static VirtualDesktopWindow Create( int index, Guid guid, DrawingColor defaultBackColor, int vdwPadding )
    {
        var dispatcher = MainWindow.GetMainWindow().Dispatcher;
        if ( !dispatcher.CheckAccess() )
        {
            return dispatcher.Invoke( () => Create( index, guid, defaultBackColor, vdwPadding ) );
        }

        var vdw = new VirtualDesktopWindow( vdwPadding )
        {
            VdId    = guid,
            VdIndex = index,
            _surface =
            {
                Title  = Const.Window.VD_CONTAINER_TITLE,
                Width  = 0,
                Height = 0
            }
        };

        vdw._surface.SetBackColor( defaultBackColor );
        vdw._surface.EnsureCreated();
        vdw.SetOwner( MainWindow.GetMainWindow() );
        return vdw;
    }

    private void SetOwner( MainWindow owner )
    {
        if ( owner.Dispatcher.CheckAccess() )
        {
            DoSetOwner();
        }
        else
        {
            owner.Dispatcher.Invoke( DoSetOwner );
        }

        return;

        void DoSetOwner()
        {
            User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                (int)GetWindowLongFields.GWL_HWNDPARENT,
                owner.Handle.ToInt32()
            );
        }
    }

    private void Surface_Closing( object? sender, CancelEventArgs e )
    {
        if ( !_allowClose )
        {
            e.Cancel = true;
        }
    }

    private void Surface_Loaded( object sender, RoutedEventArgs e )
    {
        RefreshWallpaper();
        _desktopName = DesktopWrapper.DesktopNameFromGuid( VdId );
        RefreshDesktopLabel();
    }

    public void Hide()
    {
        _surface.Hide();
    }

    public void RealClose()
    {
        _allowClose = true;
        ClearVisibleWindows();
        ReleaseWallpaperResources();
        _surface.Close();
    }

    public DrawingPoint ClientPixelToScreen( DrawingPoint client )
    {
        var (scaleX, scaleY) = SysInfo.Dpi;
        var screen = _surface.PointToScreen( new Point( client.X / scaleX, client.Y / scaleY ) );
        return new DrawingPoint( (int)Math.Round( screen.X ), (int)Math.Round( screen.Y ) );
    }

    private void ShowByVdIndex()
    {
        PlaceInCell();
    }

    internal void PlaceInCell()
    {
        if ( VirtualDesktopManager.TryGetOnlyOneInMainView( out var onlyOne ) && onlyOne != this )
        {
            Hide();
            return;
        }

        var ui = VirtualDesktopManager.Ui;
        var (scaleX, scaleY) = SysInfo.Dpi;

        var matrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex( VdIndex );
        if ( matrixIndex < 0 )
        {
            Hide();
            return;
        }

        var location = MainWindow.GetCellLocationByMatrixIndex( matrixIndex );
        var size     = MainWindow.GetCellSizeByMatrixIndex( matrixIndex );
        var maxS     = VdwShadow.MaxDip;
        var contentW = size.Width - 2 * ui.VDWBorderSize;
        var contentH = size.Height - 2 * ui.VDWBorderSize;
        var vdwWidthPx   = contentW * scaleX;
        var vdwHeightPx  = contentH * scaleY;

        ////////////////////////////////////////////////////////////////
        // 虚拟桌面容器的宽/高下限，宽/高任意一个低于此值，虚拟桌面直接不显示
        // 由于主次布局的次要窗口会大比例拉低宽高下限，因此需要给她一个比自动布局的更小的值
        var sideLengthFloor = Manager.CurrentProfile.IsPsLayout
            ? Const.VirtualDesktop.DefaultLayoutVdwSideLengthFloor * Const.VirtualDesktop.PsLayoutVdwSideLengthFloorToleranceRatio
            : Const.VirtualDesktop.DefaultLayoutVdwSideLengthFloor;
        if ( vdwWidthPx < sideLengthFloor || vdwHeightPx < sideLengthFloor )
        {
            Logger.Warning( $"Desktop[{VdIndex}] hit sideLengthFloor, just hide." );
            Hide();
            return;
        }

        SetSurfaceSize( size.Width + 2 * maxS, size.Height + 2 * maxS );
        Left = location.X - maxS;
        Top  = location.Y - maxS;
        _surface.ApplyChrome( VdId == DesktopWrapper.CurrentGuid );

        if ( !_surface.IsVisible )
        {
            _surface.Show();
        }
    }

    public void RefreshChrome( bool hover = false )
    {
        if ( !_surface.Dispatcher.CheckAccess() )
        {
            _surface.Dispatcher.Invoke( () => RefreshChrome( hover ) );
            return;
        }

        _surface.ApplyChrome( VdId == DesktopWrapper.CurrentGuid, hover );
    }
}