/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using Color = System.Drawing.Color;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

/// <summary>
///     HWND shell for a virtual-desktop cell. Domain state lives on <see cref="VirtualDesktopWindow" />.
/// </summary>
internal partial class VdwSurface : Window
{
    private static readonly DropShadowEffect LabelOutline = CreateLabelOutline();

    private readonly HwndSourceHook _hook;
    private          TextBlock?     _desktopLabel;

    internal VdwSurface( HwndSourceHook hook )
    {
        _hook = hook;
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.Manual;
        IsVisibleChanged     += OnIsVisibleChanged;
    }

    internal int VdIndex { get; set; }

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

    private void OnIsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
    {
        if ( ConfigManager.Configs.Cluster.ForceRefreshVDWBackground && e.NewValue is true )
        {
            ScheduleWallpaperInvalidate();
        }
    }

    private async void ScheduleWallpaperInvalidate()
    {
        await Task.Delay( 50 );
        if ( !IsVisible )
        {
            return;
        }

        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Render,
            new Action( () => WallpaperImage.InvalidateVisual() ) );
    }

    internal void SetDesktopLabel( string text, bool visible )
    {
        EnsureDesktopLabel();
        ApplyLabelChrome( _desktopLabel );
        SetLabelText( text );
        _desktopLabel?.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
    }

    private void EnsureDesktopLabel()
    {
        var useColorEmoji = ConfigManager.CurrentProfile.UI.EnableVdNameColorEmoji;
        if ( _desktopLabel != null && _desktopLabel is Emoji.Wpf.TextBlock == useColorEmoji )
        {
            return;
        }

        if ( _desktopLabel != null )
        {
            Root.Children.Remove( _desktopLabel );
        }

        _desktopLabel = useColorEmoji
            ? new Emoji.Wpf.TextBlock()
            : new TextBlock();
        _desktopLabel.FontSize         = 13;
        _desktopLabel.Foreground       = Brushes.Beige;
        _desktopLabel.FontFamily       = new FontFamily( "Segoe UI, Segoe UI Emoji" );
        _desktopLabel.IsHitTestVisible = false;
        _desktopLabel.Effect           = LabelOutline;
        Root.Children.Add( _desktopLabel );
    }

    private static DropShadowEffect CreateLabelOutline()
    {
        var effect = new DropShadowEffect
        {
            Color         = Colors.Black,
            ShadowDepth   = 1,
            BlurRadius    = 2,
            Opacity       = 1,
            RenderingBias = RenderingBias.Performance
        };
        effect.Freeze();
        return effect;
    }

    private static void ApplyLabelChrome( FrameworkElement? label )
    {
        if ( label == null ) return;

        switch ( ConfigManager.CurrentProfile.UI.ShowVdNamePosition )
        {
            case VdNamePosition.TopLeft:
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment   = VerticalAlignment.Top;
                break;
            case VdNamePosition.TopRight:
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignment   = VerticalAlignment.Top;
                break;
            case VdNamePosition.BottomLeft:
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment   = VerticalAlignment.Bottom;
                break;
            case VdNamePosition.BottomRight:
            default:
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignment   = VerticalAlignment.Bottom;
                break;
        }

        label.Margin = new Thickness( 2, 2, 5, 2 );
    }

    private void SetLabelText( string text )
    {
        if ( _desktopLabel is Emoji.Wpf.TextBlock emoji )
        {
            emoji.Text = text;
        }
        else
        {
            _desktopLabel?.Text = text;
        }
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