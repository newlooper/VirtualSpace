/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VirtualSpace.VirtualDesktop;

internal readonly struct ThumbCaptionBand
{
    internal double X          { get; init; }
    internal double Y          { get; init; }
    internal double Width      { get; init; }
    internal double Height     { get; init; }
    internal string Index      { get; init; }
    internal string Title      { get; init; }
    internal Brush  Background { get; init; }
    internal Brush  Foreground { get; init; }
    internal double FontSize   { get; init; }
}

internal partial class VdwSurface
{
    internal const double CAPTION_RADIUS_DIP        = 4;
    private const  double CAPTION_TITLE_WIDTH_RATIO = 0.85;

    internal void SetThumbCaptions( IReadOnlyList<ThumbCaptionBand> bands )
    {
        if ( !Dispatcher.CheckAccess() )
        {
            Dispatcher.Invoke( () => SetThumbCaptions( bands ) );
            return;
        }

        ThumbCaptions.Children.Clear();
        foreach ( var band in bands )
        {
            if ( band.Width <= 0 || band.Height <= 0 )
            {
                continue;
            }

            var radius = Math.Min( CAPTION_RADIUS_DIP, band.Height / 2 );
            var bar = new Border
            {
                Width               = band.Width,
                Height              = band.Height,
                CornerRadius        = new CornerRadius( radius ),
                Background          = band.Background,
                IsHitTestVisible    = false,
                SnapsToDevicePixels = true,
                ClipToBounds        = true,
                Padding             = new Thickness( 4, 0, 4, radius ),
                Child               = CaptionContent( band )
            };
            Canvas.SetLeft( bar, band.X );
            Canvas.SetTop( bar, band.Y );
            ThumbCaptions.Children.Add( bar );
        }
    }

    private static FrameworkElement CaptionContent( ThumbCaptionBand band )
    {
        var title = CaptionRun( band, band.Title, TextAlignment.Left );
        if ( string.IsNullOrEmpty( band.Index ) )
        {
            return title;
        }

        var grid = new Grid();
        grid.ColumnDefinitions.Add( new ColumnDefinition { Width = new GridLength( CAPTION_TITLE_WIDTH_RATIO, GridUnitType.Star ) } );
        grid.ColumnDefinitions.Add( new ColumnDefinition { Width = new GridLength( 1 - CAPTION_TITLE_WIDTH_RATIO, GridUnitType.Star ) } );
        grid.Children.Add( title );

        var index = CaptionRun( band, band.Index, TextAlignment.Right );
        Grid.SetColumn( index, 1 );
        grid.Children.Add( index );
        return grid;
    }

    private static TextBlock CaptionRun( ThumbCaptionBand band, string text, TextAlignment align )
    {
        return new TextBlock
        {
            Text              = text,
            Foreground        = band.Foreground,
            FontFamily        = SystemFonts.MessageFontFamily,
            FontSize          = band.FontSize,
            FontWeight        = FontWeights.Bold,
            TextAlignment     = align,
            TextTrimming      = TextTrimming.CharacterEllipsis,
            TextWrapping      = TextWrapping.NoWrap,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible  = false
        };
    }
}