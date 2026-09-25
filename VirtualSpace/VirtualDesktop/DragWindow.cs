/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop;

internal partial class DragWindow : Window
{
    private DragWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.Manual;
    }

    internal IntPtr Handle { get; private set; }

    internal IntPtr Thumb { get; set; }

    internal int PixelWidth => Math.Max( 0, (int)Math.Round( Width * SysInfo.Dpi.ScaleX ) );

    internal int PixelHeight => Math.Max( 0, (int)Math.Round( Height * SysInfo.Dpi.ScaleY ) );

    internal static DragWindow CreateAndShow( int width, int height )
    {
        var (scaleX, scaleY) = SysInfo.Dpi;
        var dw = new DragWindow
        {
            Title  = Const.Window.VD_DRAG_TITLE,
            Width  = Math.Max( 1, width ) / scaleX,
            Height = Math.Max( 1, height ) / scaleY
        };
        dw.EnsureCreated();
        dw.Show();
        return dw;
    }

    internal void MoveCenterToScreenPixel( int x, int y )
    {
        var (scaleX, scaleY) = SysInfo.Dpi;
        Left                 = x / scaleX - Width / 2;
        Top                  = y / scaleY - Height / 2;
    }

    private void EnsureCreated()
    {
        new WindowInteropHelper( this ).EnsureHandle();
    }

    protected override void OnSourceInitialized( EventArgs e )
    {
        base.OnSourceInitialized( e );
        Handle = new WindowInteropHelper( this ).EnsureHandle();
        VirtualDesktopManager.IgnoreWindowHandle( Handle );
        FixStyle();
    }

    private void FixStyle()
    {
        var style = User32.GetWindowLong( Handle, (int)GetWindowLongFields.GWL_STYLE );
        style = unchecked( style | (int)0x80000000 ); // WS_POPUP
        User32.SetWindowLongPtr( new HandleRef( this, Handle ), (int)GetWindowLongFields.GWL_STYLE, style );

        var exStyle = User32.GetWindowLong( Handle, (int)GetWindowLongFields.GWL_EXSTYLE );
        exStyle |= 0x00000080; // WS_EX_TOOLWINDOW
        exStyle |= 0x08000000; // WS_EX_NOACTIVATE
        exStyle &= ~0x00040000; // WS_EX_APPWINDOW
        User32.SetWindowLongPtr( new HandleRef( this, Handle ), (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
    }
}