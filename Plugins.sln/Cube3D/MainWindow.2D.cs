/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using VirtualSpace.PluginContracts;

namespace Cube3D;

public partial class MainWindow
{
    private void NotificationGridLayout( VirtualDesktopSwitchInfo info )
    {
        ////////////////////////////////////////////////////////////////
        // position and size
        var screenW = SystemParameters.PrimaryScreenWidth;
        var screenH = SystemParameters.PrimaryScreenHeight;
        var centerX = screenW / 2;
        var centerY = screenH * 5 / 16;
        NotifyContainer.Width  = screenW / 6;
        NotifyContainer.Height = NotifyContainer.Width * 3 / 4;
        NotifyContainer.Margin = new Thickness
        {
            Top = centerY + NotifyContainer.Height
        };

        ApplyNotifyGrid( info );
        UpdateLayout();

        var n = Math.Max( NotifyGrid.RowDefinitions.Count, 1 );
        var buttonMargin = new Thickness
        {
            Left = NotifyGrid.ActualWidth / n / 10.0,
            Top  = NotifyGrid.ActualHeight / n / 10.0
        };

        Resources["NotifyButtonMargin"] = buttonMargin;
        NotifyBorder.Padding            = new Thickness { Right = buttonMargin.Left, Bottom = buttonMargin.Top };

        UpdateLayout();
        PlaceIndicatorOnCell( info.FromIndex );
    }

    private void ApplyNotifyGrid( VirtualDesktopSwitchInfo info )
    {
        var cells = info.Cells is { Length: > 0 } ? info.Cells : FallbackEqualCells( info.VdCount );
        var n     = info.BoardN > 0 ? info.BoardN : FallbackBoardN( cells.Length );

        if ( NotifyGrid.RowDefinitions.Count != n )
        {
            NotifyGrid.RowDefinitions.Clear();
            NotifyGrid.ColumnDefinitions.Clear();
            var star = new GridLength( 1, GridUnitType.Star );
            for ( var i = 0; i < n; i++ )
            {
                NotifyGrid.RowDefinitions.Add( new RowDefinition { Height      = star } );
                NotifyGrid.ColumnDefinitions.Add( new ColumnDefinition { Width = star } );
            }
        }

        if ( NotifyGrid.Children.Count != cells.Length )
        {
            NotifyGrid.Children.Clear();
            for ( var i = 0; i < cells.Length; i++ )
            {
                NotifyGrid.Children.Add( new Button() );
            }
        }

        for ( var i = 0; i < cells.Length; i++ )
        {
            var cell = cells[i];
            var btn  = (Button)NotifyGrid.Children[i];
            btn.Tag      = cell.Index;
            btn.Template = Resources[cell.Occupied ? "NotifyButtonTemplate" : "NotifyButtonTemplateEmpty"] as ControlTemplate;
            Grid.SetRow( btn, cell.Row );
            Grid.SetColumn( btn, cell.Col );
            Grid.SetRowSpan( btn, Math.Max( 1, cell.RowSpan ) );
            Grid.SetColumnSpan( btn, Math.Max( 1, cell.ColSpan ) );
        }
    }

    private void PlaceIndicatorOnCell( int index )
    {
        var el = FindNotifyCell( index );
        if ( el == null )
        {
            return;
        }

        var pos = el.TranslatePoint( new Point( 0, 0 ), NotifyContainer );
        CurrentIndicator.BeginAnimation( MarginProperty, null );
        CurrentIndicator.BeginAnimation( WidthProperty, null );
        CurrentIndicator.BeginAnimation( HeightProperty, null );
        CurrentIndicator.Width  = el.ActualWidth;
        CurrentIndicator.Height = el.ActualHeight;
        CurrentIndicator.Margin = new Thickness( pos.X, pos.Y, 0, 0 );
    }

    private FrameworkElement? FindNotifyCell( int index )
    {
        foreach ( Button btn in NotifyGrid.Children )
        {
            if ( btn.Tag is int tag && tag == index )
            {
                return btn;
            }
        }

        return index >= 0 && index < NotifyGrid.Children.Count
            ? (FrameworkElement)NotifyGrid.Children[index]
            : null;
    }

    private static int FallbackBoardN( int cellCount )
    {
        return Math.Max( 1, (int)Math.Ceiling( Math.Sqrt( Math.Max( cellCount, 1 ) ) ) );
    }

    private static VirtualDesktopCell[] FallbackEqualCells( int vdCount )
    {
        var n     = FallbackBoardN( vdCount );
        var cells = new VirtualDesktopCell[n * n];
        for ( var i = 0; i < cells.Length; i++ )
        {
            cells[i] = new VirtualDesktopCell
            {
                Index    = i,
                Row      = i / n,
                Col      = i % n,
                RowSpan  = 1,
                ColSpan  = 1,
                Occupied = i < vdCount
            };
        }

        return cells;
    }
}