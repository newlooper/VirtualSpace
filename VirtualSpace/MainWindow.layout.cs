/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static int           _desktopCount;
        private static int           _rowsCols;
        private static UserInterface Ui => Manager.GetCurrentProfile().UI;

        public static void UpdateHoverBorder( int hover )
        {
            var borderColorHover = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWHighlightBackColor.R, Ui.VDWHighlightBackColor.G, Ui.VDWHighlightBackColor.B )};
            var borderColorDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};

            for ( var i = 0; i < _desktopCount; i++ )
            {
                var border = (Border)_instance.MainGrid.Children[i];
                if ( i == hover )
                    border.BorderBrush = borderColorHover;
                else
                    border.BorderBrush = borderColorDefault;
            }
        }

        public static void ResetAllBorder()
        {
            var currentVdIndex = VirtualDesktopManager.CurrentDesktopIndex();

            var borderColorHover = Color.FromRgb( Ui.VDWHighlightBackColor.R, Ui.VDWHighlightBackColor.G, Ui.VDWHighlightBackColor.B );

            var borderBrushDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};
            var borderBrushCurrent = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWCurrentBackColor.R, Ui.VDWCurrentBackColor.G, Ui.VDWCurrentBackColor.B )};

            var borderShadowDefault = _instance.Resources["VdwShadowDefault"] as DropShadowEffect;
            var borderShadowCurrent = _instance.Resources["VdwShadowCurrent"] as DropShadowEffect;

            for ( var i = 0; i < _desktopCount; i++ )
            {
                var border = (Border)_instance.MainGrid.Children[i];
                if ( i == currentVdIndex )
                {
                    border.Effect = borderShadowCurrent;
                    border.BorderBrush = borderBrushCurrent;
                }
                else
                {
                    var effect = border.Effect as DropShadowEffect;
                    var brush  = border.BorderBrush as SolidColorBrush;
                    if ( effect?.Color == Colors.White
                         || brush?.Color == borderColorHover )
                    {
                        border.Effect = borderShadowDefault;
                        border.BorderBrush = borderBrushDefault;
                    }
                }
            }
        }

        public static void ResetMainGrid()
        {
            var vdCount  = DesktopWrapper.Count;
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
            if ( vdCount == _desktopCount ) return;

            _instance.MainGrid.Children.Clear();

            if ( _rowsCols != rowsCols )
            {
                VirtualDesktopManager.NeedRepaintThumbs = true;
                _instance.Dispatcher.Invoke( new Action( () => { } ), DispatcherPriority.ContextIdle, null );
            }

            var borderBrushDefault = new SolidColorBrush
                {Color = Color.FromRgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B )};
            var borderShadowDefault = _instance.Resources["VdwShadowDefault"] as DropShadowEffect;

            for ( var i = 0; i < vdCount; i++ )
            {
                var border = new Border
                {
                    Margin = new Thickness( Ui.VDWMargin ),
                    BorderThickness = new Thickness( Ui.VDWBorderSize ),
                    BorderBrush = borderBrushDefault,
                    Effect = borderShadowDefault,
                    Background = Brushes.Transparent
                };
                _instance.MainGrid.Children.Add( border );
            }

            _desktopCount = vdCount; // remember last count
            _rowsCols = rowsCols;
            _instance.UpdateLayout();
        }

        public static Point GetCellLocation( int index )
        {
            if ( _instance.Dispatcher.CheckAccess() )
            {
                return _instance.MainGrid.Children[index].TranslatePoint( new Point(), _instance );
            }

            return _instance.Dispatcher.Invoke( () => _instance.MainGrid.Children[index].TranslatePoint( new Point(), _instance ) );
        }

        public static int InCell( Point p )
        {
            var cells = _instance.MainGrid.Children;
            var index = -1;
            var dpi   = Agent.Dpi;
            for ( var i = 0; i < cells.Count; i++ )
            {
                var topLeft = cells[i].TranslatePoint( new Point(), _instance );
                topLeft = new Point( topLeft.X * dpi[0], topLeft.Y * dpi[1] );
                var bottomRight = new Point( topLeft.X + cells[i].RenderSize.Width * dpi[0], topLeft.Y + cells[i].RenderSize.Height * dpi[1] );
                var rect        = new Rect( topLeft, bottomRight );
                if ( rect.Contains( p ) )
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static UniformGrid GetMainGrid()
        {
            return _instance.MainGrid;
        }

        public static Size MainGridCellSize => _instance.MainGrid.Children[0].RenderSize;
    }
}