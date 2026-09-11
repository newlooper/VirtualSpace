/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using VirtualSpace.Helpers;
using Color = System.Drawing.Color;

namespace VirtualSpace.VirtualDesktop;

/// <summary>
///     HWND shell for a virtual-desktop cell. Domain state lives on <see cref="VirtualDesktopWindow" />.
/// </summary>
internal partial class VdwSurface : Window
{
    private readonly HwndSourceHook _hook;

    internal VdwSurface( HwndSourceHook hook )
    {
        _hook = hook;
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.Manual;
    }

    internal IntPtr Handle { get; private set; }

    internal void EnsureCreated()
    {
        new WindowInteropHelper( this ).EnsureHandle();
    }

    protected override void OnSourceInitialized( EventArgs e )
    {
        base.OnSourceInitialized( e );
        Handle = new WindowInteropHelper( this ).EnsureHandle();
        FixStyle();
        HwndSource.FromHwnd( Handle )?.AddHook( _hook );
    }

    internal void SetWallpaper( ImageSource? source )
    {
        WallpaperImage.Source = source;
    }

    internal void SetDesktopLabel( string text, bool visible )
    {
        DesktopLabel.Text       = text;
        DesktopLabel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }

    internal void SetBackColor( Color color )
    {
        Background = new SolidColorBrush( System.Windows.Media.Color.FromRgb( color.R, color.G, color.B ) );
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