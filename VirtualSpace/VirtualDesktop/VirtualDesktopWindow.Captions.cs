/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Media;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private const double CAPTION_FACE_DIP = 22;
    private const double CAPTION_FONT_DIP = 12;
    private const double CAPTION_BAR_DIP  = CAPTION_FACE_DIP + VdwSurface.CAPTION_RADIUS_DIP;

    private const int TITLE_GROUP_DIGIT = 0;
    private const int TITLE_GROUP_LATIN = 1;
    private const int TITLE_GROUP_HAN   = 2;
    private const int TITLE_GROUP_OTHER = 3;

    // 大都来自 Windows 设置 \ 个性化 \ 颜色 \ 主题色里的深色。
    // 下标对应 Screen.AllScreens 序号，顺序固定。
    private static readonly Color[] AccentColors =
    [
        Color.FromRgb( 0x00, 0x63, 0xB1 ),
        Color.FromRgb( 0xC3, 0x00, 0x52 ),
        Color.FromRgb( 0x10, 0x7C, 0x10 ),
        Color.FromRgb( 0xCA, 0x50, 0x10 ),
        Color.FromRgb( 0x88, 0x17, 0x98 ),
        Color.FromRgb( 0x01, 0x85, 0x74 ),
        Color.FromRgb( 0x53, 0x6B, 0xC6 ),
        Color.FromRgb( 0x49, 0x82, 0x05 ),
        Color.FromRgb( 0x48, 0x68, 0x60 ),
        Color.FromRgb( 0x70, 0x3C, 0x00 )
    ];

    private static readonly SolidColorBrush[] AccentBackgrounds = BuildBackgrounds();

    // 只展示标题、不按显示器分组时，所有标题栏用同一种深灰。
    private static readonly SolidColorBrush CaptionPlainBackground = FrozenBrush( Color.FromRgb( 0x50, 0x50, 0x50 ) );

    private static readonly CompareInfo PinyinTitleOrder = CultureInfo.GetCultureInfo( "zh-Hans" ).CompareInfo;

    private readonly List<ThumbCaptionBand> _thumbCaptions = [];
    private          bool                   _thumbCaptionBold;

    internal bool ScreenOwnershipChanged()
    {
        foreach ( var window in _visibleWindows )
        {
            var slot = ScreenOf( window.Handle );
            if ( slot.Index != window.ScreenIndex || slot.Name != window.ScreenDevice )
            {
                return true;
            }
        }

        return false;
    }

    internal void RelayoutThumbnails()
    {
        if ( !_surface.Dispatcher.CheckAccess() )
        {
            _surface.Dispatcher.Invoke( RelayoutThumbnails );
            return;
        }

        ReleaseThumbnails();
        ShowThumbnails();
    }

    private void SortByScreenThenTitle()
    {
        _thumbCaptions.Clear();
        foreach ( var window in _visibleWindows )
        {
            var slot = ScreenOf( window.Handle );
            window.ScreenIndex  = slot.Index;
            window.ScreenDevice = slot.Name;
        }

        var byTitle = WindowTitleOrder();
        _visibleWindows.Sort( ( a, b ) =>
        {
            var byScreen = a.ScreenIndex.CompareTo( b.ScreenIndex );
            if ( byScreen != 0 )
            {
                return byScreen;
            }

            return byTitle( a, b );
        } );
    }

    private void ClearThumbCaptions()
    {
        _thumbCaptions.Clear();
        _surface.SetThumbCaptions( _thumbCaptions );
    }

    internal void RefreshThumbCaptions()
    {
        _thumbCaptionBold = VdId == DesktopWrapper.CurrentGuid;
        _thumbCaptions.Clear();
        foreach ( var window in _visibleWindows )
        {
            var thumb = window.DTP.rcDestination;
            if ( window.Thumb == IntPtr.Zero || thumb.Right <= thumb.Left || thumb.Bottom <= thumb.Top )
            {
                continue;
            }

            NoteThumbCaption( window, window.ThumbCell, thumb );
        }

        PresentThumbCaptions();
    }

    private void NoteThumbCaption( VisibleWindow window, RECT card, RECT thumb )
    {
        var (scaleX, scaleY) = SysInfo.Dpi;
        var barPx = CaptionBarPixels( card.Bottom - card.Top );
        if ( barPx <= 0 || scaleX <= 0 || scaleY <= 0 )
        {
            return;
        }

        var grouped = VirtualDesktopManager.Ui.ThumbCaptionMode.GroupsByMonitor();
        _thumbCaptions.Add( new ThumbCaptionBand
        {
            X          = ( thumb.Left - VdwSurface.ChromeInsetPixelX ) / (double)scaleX,
            Y          = ( card.Top - VdwSurface.ChromeInsetPixelY ) / (double)scaleY,
            Width      = ( thumb.Right - thumb.Left ) / (double)scaleX,
            Height     = barPx / (double)scaleY,
            Index      = grouped ? CaptionIndex( window.ScreenIndex ) : "",
            Title      = window.Title,
            Background = grouped ? CaptionBackground( window.ScreenIndex ) : CaptionPlainBackground,
            Foreground = Brushes.White,
            FontSize   = CaptionFontSize( barPx / (double)scaleY ),
            Bold       = _thumbCaptionBold
        } );
    }

    private void PresentThumbCaptions()
    {
        _surface.SetThumbCaptions( _thumbCaptions );
    }

    private void RefreshChromeCaptionWeight()
    {
        var current = VdId == DesktopWrapper.CurrentGuid;
        if ( _thumbCaptionBold == current || !VirtualDesktopManager.Ui.ThumbCaptionMode.ShowsCaption() )
        {
            return;
        }

        RefreshThumbCaptions();
    }

    private static bool CapturesPoint( VisibleWindow window, Point point )
    {
        if ( window.Rect.Contains( point ) || !VirtualDesktopManager.Ui.ThumbCaptionMode.ShowsCaption() )
        {
            return window.Rect.Contains( point );
        }

        var thumb = window.DTP.rcDestination;
        var bar   = CaptionBarPixels( window.ThumbCell.Bottom - window.ThumbCell.Top );
        if ( bar <= 0 || thumb.Right <= thumb.Left )
        {
            return false;
        }

        var strip = new Rectangle(
            thumb.Left,
            window.ThumbCell.Top,
            thumb.Right - thumb.Left,
            bar );
        return strip.Contains( point );
    }

    private static RECT CaptionImageRect( RECT card )
    {
        var bar = CaptionBarPixels( card.Bottom - card.Top );
        card.Top += bar;
        if ( card.Top >= card.Bottom )
        {
            card.Top = card.Bottom - 1;
        }

        return card;
    }

    private static void PinThumbnailUnderCaption( ref DWM_THUMBNAIL_PROPERTIES props, RECT card, RECT image )
    {
        var shift = props.rcDestination.Top - image.Top;
        if ( shift > 0 )
        {
            props.rcDestination.Top    -= shift;
            props.rcDestination.Bottom -= shift;
        }

        var barPx = image.Top - card.Top;
        if ( barPx <= 1 || props.rcDestination.Bottom - props.rcDestination.Top <= 1 )
        {
            return;
        }

        var radiusDip = Math.Min( VdwSurface.CAPTION_RADIUS_DIP, barPx / (double)SysInfo.Dpi.ScaleY / 2 );
        var cover     = Math.Min( barPx - 1, Math.Max( 1, (int)Math.Round( radiusDip * SysInfo.Dpi.ScaleY ) ) );
        if ( props.rcDestination.Bottom - props.rcDestination.Top <= cover )
        {
            return;
        }

        props.rcDestination.Top    -= cover;
        props.rcDestination.Bottom -= cover;
    }

    private double CaptionFontSize( double barHeightDip )
    {
        var size = Math.Max( 8, Math.Min( CAPTION_FONT_DIP, barHeightDip - 4 ) );
        if ( Manager.CurrentProfile.IsPsLayout && !VirtualDesktopManager.IsPsPrimaryDesktop( VdId ) )
        {
            size *= Const.VirtualDesktop.PsLayoutVdwTitleFontSizeRatio;
        }

        return size;
    }

    private static int CaptionBarPixels( int cardHeight )
    {
        if ( cardHeight <= 1 )
        {
            return 0;
        }

        var bar = Math.Max( 1, (int)Math.Round( CAPTION_BAR_DIP * SysInfo.Dpi.ScaleY ) );
        return Math.Min( bar, Math.Max( 1, cardHeight / 2 ) );
    }

    private static Comparison<VisibleWindow> WindowTitleOrder()
    {
        var language = VirtualDesktopManager.Ui.Language;
        if ( language.StartsWith( "zh", StringComparison.OrdinalIgnoreCase ) )
        {
            return CompareTitleByScript;
        }

        return ( a, b ) => string.Compare( a.Title, b.Title, StringComparison.Ordinal );
    }

    private static int CompareTitleByScript( VisibleWindow a, VisibleWindow b )
    {
        var left      = a.Title ?? "";
        var right     = b.Title ?? "";
        var leftGroup = TitleGroup( left );
        var byGroup   = leftGroup - TitleGroup( right );
        if ( byGroup != 0 )
        {
            return byGroup;
        }

        return leftGroup == TITLE_GROUP_HAN
            ? PinyinTitleOrder.Compare( left, right, CompareOptions.IgnoreCase )
            : string.Compare( left, right, StringComparison.OrdinalIgnoreCase );
    }

    private static int TitleGroup( string title )
    {
        for ( var i = 0; i < title.Length; i++ )
        {
            var c = title[i];
            if ( char.IsWhiteSpace( c ) || char.IsPunctuation( c ) || char.IsSymbol( c ) )
            {
                continue;
            }

            if ( char.IsDigit( c ) )
            {
                return TITLE_GROUP_DIGIT;
            }

            if ( IsLatinLetter( c ) )
            {
                return TITLE_GROUP_LATIN;
            }

            if ( IsHan( title, i ) )
            {
                return TITLE_GROUP_HAN;
            }

            return TITLE_GROUP_OTHER;
        }

        return TITLE_GROUP_OTHER;
    }

    private static bool IsLatinLetter( char c )
    {
        return c is >= 'A' and <= 'Z' or >= 'a' and <= 'z'
            or >= '\uFF21' and <= '\uFF3A' or >= '\uFF41' and <= '\uFF5A';
    }

    private static bool IsHan( string title, int index )
    {
        var c = title[index];
        if ( char.IsHighSurrogate( c ) )
        {
            if ( index + 1 >= title.Length || !char.IsLowSurrogate( title[index + 1] ) )
            {
                return false;
            }

            var codePoint = char.ConvertToUtf32( c, title[index + 1] );
            return codePoint is >= 0x20000 and <= 0x2A6DF
                or >= 0x2A700 and <= 0x2B73F
                or >= 0x2B740 and <= 0x2B81F
                or >= 0x2B820 and <= 0x2CEAF
                or >= 0x2CEB0 and <= 0x2EBEF
                or >= 0x30000 and <= 0x3134F
                or >= 0x2F800 and <= 0x2FA1F;
        }

        return c is >= '\u3400' and <= '\u4DBF' or >= '\u4E00' and <= '\u9FFF' or >= '\uF900' and <= '\uFAFF';
    }

    private static string CaptionIndex( int screenIndex )
    {
        return screenIndex is < 0 or int.MaxValue ? "" : ( screenIndex + 1 ).ToString();
    }

    private static SolidColorBrush CaptionBackground( int screenIndex )
    {
        return (uint)screenIndex >= (uint)AccentBackgrounds.Length ? Brushes.Black : AccentBackgrounds[screenIndex];
    }

    private static ScreenSlot ScreenOf( IntPtr handle )
    {
        var screen = Screen.FromHandle( handle );
        var name   = screen.DeviceName ?? "";
        var all    = Screen.AllScreens;
        for ( var i = 0; i < all.Length; i++ )
        {
            if ( string.Equals( all[i].DeviceName, name, StringComparison.Ordinal ) )
            {
                return new ScreenSlot( i, name );
            }
        }

        return new ScreenSlot( int.MaxValue, name );
    }

    private static SolidColorBrush[] BuildBackgrounds()
    {
        var brushes = new SolidColorBrush[AccentColors.Length];
        for ( var i = 0; i < AccentColors.Length; i++ )
        {
            brushes[i] = FrozenBrush( AccentColors[i] );
        }

        return brushes;
    }

    private static SolidColorBrush FrozenBrush( Color color )
    {
        var brush = new SolidColorBrush( color );
        brush.Freeze();
        return brush;
    }

    private readonly record struct ScreenSlot( int Index, string Name );
}

internal static partial class VirtualDesktopManager
{
    internal static void RelayoutThumbnails()
    {
        if ( VirtualDesktopWindow.IsDragInProgress )
        {
            return;
        }

        foreach ( var vdw in GetAllVirtualDesktops() )
        {
            vdw.RelayoutThumbnails();
        }
    }

    private static void RelayoutChangedScreens( List<VirtualDesktopWindow> targets )
    {
        if ( !Ui.ThumbCaptionMode.GroupsByMonitor() )
        {
            return;
        }

        foreach ( var vdw in targets )
        {
            if ( vdw.ScreenOwnershipChanged() )
            {
                vdw.RelayoutThumbnails();
            }
        }
    }
}