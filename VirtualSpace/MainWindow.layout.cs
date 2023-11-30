/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static int              _desktopCount;
        private static UserInterface    Ui       => Manager.CurrentProfile.UI;
        private static int              RowsCols { get; set; }
        private static DropShadowEffect _borderShadowDefault;
        private static DropShadowEffect _borderShadowCurrent;

        public static void ResetMainGrid()
        {
            var vdCount = DesktopWrapper.Count;
            if ( vdCount == _desktopCount ) return;
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );

            var mainGrid = _instance.MainGrid;

            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            if ( RowsCols != rowsCols )
            {
                _instance.Dispatcher.Invoke( new Action( () => { } ), DispatcherPriority.ContextIdle, null );
            }

            var borderBrushDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};

            for ( var r = 0; r < rowsCols; r++ )
            {
                mainGrid.RowDefinitions.Add( new RowDefinition() );
                mainGrid.ColumnDefinitions.Add( new ColumnDefinition() );
                for ( var c = 0; c < rowsCols; c++ )
                {
                    var border = new Border
                    {
                        Margin = new Thickness( Ui.VDWMargin ),
                        BorderThickness = new Thickness( Ui.VDWBorderSize ),
                        BorderBrush = borderBrushDefault,
                        Effect = _borderShadowDefault,
                        Background = Brushes.Transparent
                    };
                    Grid.SetRow( border, r );
                    Grid.SetColumn( border, c );
                    mainGrid.Children.Add( border );
                }
            }

            _desktopCount = vdCount; // remember last count
            RowsCols = rowsCols;
            _instance.UpdateLayout();
        }

        public static void ResetMainGridForSingleDesktop( int vdIndex )
        {
            var vdCount  = DesktopWrapper.Count;
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );

            vdIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex( vdIndex );

            var bigRow          = vdIndex / rowsCols;
            var bigCol          = vdIndex % rowsCols;
            var bigGridLength   = new GridLength( 1, GridUnitType.Star );
            var smallGridLength = new GridLength( 0 );

            var mainGrid = _instance.MainGrid;

            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            _instance.Dispatcher.Invoke( new Action( () => { } ), DispatcherPriority.ContextIdle, null );

            var borderBrushDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};

            for ( var r = 0; r < rowsCols; r++ )
            {
                var height = bigRow == r ? bigGridLength : smallGridLength;
                mainGrid.RowDefinitions.Add( new RowDefinition {Height = height} );

                for ( var c = 0; c < rowsCols; c++ )
                {
                    if ( mainGrid.ColumnDefinitions.Count < rowsCols )
                    {
                        var width = bigCol == c ? bigGridLength : smallGridLength;
                        mainGrid.ColumnDefinitions.Add( new ColumnDefinition {Width = width} );
                    }

                    var border = mainGrid.ColumnDefinitions[c].Width == smallGridLength
                        ? new Border()
                        : new Border
                        {
                            Margin = new Thickness( Ui.VDWMargin ),
                            BorderThickness = new Thickness( Ui.VDWBorderSize ),
                            BorderBrush = borderBrushDefault,
                            Effect = _borderShadowDefault,
                            Background = Brushes.Transparent
                        };
                    Grid.SetRow( border, r );
                    Grid.SetColumn( border, c );
                    mainGrid.Children.Add( border );
                }
            }

            _desktopCount = 1; // single, single, single
            RowsCols = rowsCols;
            _instance.UpdateLayout();
        }

        public static void UpdateHoverBorder( int hover )
        {
            var borderColorHover = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWHighlightBackColor.R, Ui.VDWHighlightBackColor.G, Ui.VDWHighlightBackColor.B )};
            var borderColorDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};

            for ( var i = 0; i < _desktopCount; i++ )
            {
                var border = (Border)_instance.MainGrid.Children[i];
                border.BorderBrush = i == hover ? borderColorHover : borderColorDefault;
            }
        }

        public static void RenderCellBorder()
        {
            var currentMatrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex( VirtualDesktopManager.GetVdIndexByGuid( DesktopWrapper.CurrentGuid ) );

            var borderColorHover = Color.FromRgb( Ui.VDWHighlightBackColor.R, Ui.VDWHighlightBackColor.G, Ui.VDWHighlightBackColor.B );

            var borderBrushDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};
            var borderBrushCurrent = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWCurrentBackColor.R, Ui.VDWCurrentBackColor.G, Ui.VDWCurrentBackColor.B )};

            for ( var i = 0; i < Math.Pow( RowsCols, 2 ); i++ )
            {
                var border = (Border)_instance.MainGrid.Children[i];
                if ( i == currentMatrixIndex )
                {
                    border.Effect = _borderShadowCurrent;
                    border.BorderBrush = borderBrushCurrent;
                }
                else
                {
                    var effect = border.Effect as DropShadowEffect;
                    var brush  = border.BorderBrush as SolidColorBrush;
                    if ( effect?.Color == Colors.White
                         || brush?.Color == borderColorHover )
                    {
                        border.Effect = _borderShadowDefault;
                        border.BorderBrush = borderBrushDefault;
                    }
                }
            }
        }

        public static Point GetCellLocationByMatrixIndex( int index )
        {
            if ( _instance.Dispatcher.CheckAccess() )
            {
                return _instance.MainGrid.Children[index].TranslatePoint( new Point(), _instance );
            }

            return _instance.Dispatcher.Invoke( () => _instance.MainGrid.Children[index].TranslatePoint( new Point(), _instance ) );
        }

        public static Size GetCellSizeByMatrixIndex( int index )
        {
            return _instance.MainGrid.Children[index].RenderSize;
        }

        public static int InCell( Point p )
        {
            var cells = _instance.MainGrid.Children;
            var index = -1;
            var dpi   = SysInfo.Dpi;
            for ( var i = 0; i < cells.Count; i++ )
            {
                var topLeft = cells[i].TranslatePoint( new Point(), _instance );
                topLeft = new Point( topLeft.X * dpi.ScaleX, topLeft.Y * dpi.ScaleY );
                var bottomRight = new Point( topLeft.X + cells[i].RenderSize.Width * dpi.ScaleX, topLeft.Y + cells[i].RenderSize.Height * dpi.ScaleY );
                var rect        = new Rect( topLeft, bottomRight );
                if ( rect.Contains( p ) )
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void InitCellBorderShadowEffect()
        {
            _borderShadowDefault = Resources["VdwShadowDefault"] as DropShadowEffect;
            _borderShadowCurrent = Resources["VdwShadowCurrent"] as DropShadowEffect;
        }
    }
}