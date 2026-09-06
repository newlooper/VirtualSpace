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
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private const  int           SHADOW_ATLAS_SIZE  = 64;
        private const  int           SHADOW_ATLAS_INSET = 16;
        private static int           _desktopCount;
        private static int           RowsCols { get; set; }
        private static UserInterface Ui       => Manager.CurrentProfile.UI;
        private static ShadowSlices  _shadowDefault;
        private static ShadowSlices  _shadowCurrent;

        private enum CellChrome
        {
            Default,
            Current,
            Hover
        }

        public static void ResetMainGrid()
        {
            var vdCount = DesktopWrapper.Count;
            if ( vdCount == _desktopCount ) return;
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );

            var mainGrid = _instance.MainGrid;

            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();

            if ( RowsCols != rowsCols ) _instance.Dispatcher.Invoke( new Action( () => { } ), DispatcherPriority.ContextIdle, null );

            for ( var r = 0; r < rowsCols; r++ )
            {
                mainGrid.RowDefinitions.Add( new RowDefinition() );
                mainGrid.ColumnDefinitions.Add( new ColumnDefinition() );
                for ( var c = 0; c < rowsCols; c++ )
                {
                    var border = CreateCellBorder();
                    Grid.SetRow( border, r );
                    Grid.SetColumn( border, c );
                    mainGrid.Children.Add( border );
                }
            }

            _desktopCount = vdCount; // remember last count
            RowsCols      = rowsCols;
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

            for ( var r = 0; r < rowsCols; r++ )
            {
                var height = bigRow == r ? bigGridLength : smallGridLength;
                mainGrid.RowDefinitions.Add( new RowDefinition { Height = height } );

                for ( var c = 0; c < rowsCols; c++ )
                {
                    if ( mainGrid.ColumnDefinitions.Count < rowsCols )
                    {
                        var width = bigCol == c ? bigGridLength : smallGridLength;
                        mainGrid.ColumnDefinitions.Add( new ColumnDefinition { Width = width } );
                    }

                    var border = mainGrid.ColumnDefinitions[c].Width == smallGridLength
                        ? new Border()
                        : CreateCellBorder();
                    Grid.SetRow( border, r );
                    Grid.SetColumn( border, c );
                    mainGrid.Children.Add( border );
                }
            }

            _desktopCount = 1; // single, single, single
            RowsCols      = rowsCols;
            _instance.UpdateLayout();
        }

        public static void UpdateHoverBorder( int hover )
        {
            var currentMatrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex(
                VirtualDesktopManager.GetVdIndexByGuid( DesktopWrapper.CurrentGuid ) );

            for ( var i = 0; i < _desktopCount; i++ )
            {
                var chrome = i == currentMatrixIndex
                    ? CellChrome.Current
                    : i == hover
                        ? CellChrome.Hover
                        : CellChrome.Default;
                ApplyCellChrome( (Border)_instance.MainGrid.Children[i], chrome );
            }
        }

        public static void RenderCellBorder()
        {
            var currentMatrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex(
                VirtualDesktopManager.GetVdIndexByGuid( DesktopWrapper.CurrentGuid ) );

            for ( var i = 0; i < Math.Pow( RowsCols, 2 ); i++ )
            {
                ApplyCellChrome(
                    (Border)_instance.MainGrid.Children[i],
                    i == currentMatrixIndex ? CellChrome.Current : CellChrome.Default );
            }
        }

        private static Border CreateCellBorder()
        {
            EnsureShadowSlices();

            Logger.Verbose( "Create Cell Border" );

            var inner = new Border { Background = Brushes.Transparent };
            var grid  = new Grid();
            grid.RowDefinitions.Add( new RowDefinition { Height      = GridLength.Auto } );
            grid.RowDefinitions.Add( new RowDefinition { Height      = new GridLength( 1, GridUnitType.Star ) } );
            grid.RowDefinitions.Add( new RowDefinition { Height      = GridLength.Auto } );
            grid.ColumnDefinitions.Add( new ColumnDefinition { Width = GridLength.Auto } );
            grid.ColumnDefinitions.Add( new ColumnDefinition { Width = new GridLength( 1, GridUnitType.Star ) } );
            grid.ColumnDefinitions.Add( new ColumnDefinition { Width = GridLength.Auto } );

            var images = new Image[8];
            var cells = new (int row, int col)[]
            {
                ( 0, 0 ), ( 0, 1 ), ( 0, 2 ),
                ( 1, 0 ), ( 1, 2 ),
                ( 2, 0 ), ( 2, 1 ), ( 2, 2 )
            };
            for ( var i = 0; i < cells.Length; i++ )
            {
                var img = new Image
                {
                    Stretch             = Stretch.Fill,
                    IsHitTestVisible    = false,
                    SnapsToDevicePixels = true
                };
                Grid.SetRow( img, cells[i].row );
                Grid.SetColumn( img, cells[i].col );
                grid.Children.Add( img );
                images[i] = img;
            }

            Grid.SetRow( inner, 1 );
            Grid.SetColumn( inner, 1 );
            grid.Children.Add( inner );

            var outer = new Border
            {
                Background = Brushes.Transparent,
                Child      = grid,
                Tag        = images
            };

            ApplyCellChrome( outer, CellChrome.Default );
            return outer;
        }

        private static Border GetCellLayoutBorder( int index )
        {
            var outer = (Border)_instance.MainGrid.Children[index];
            return GetInnerBorder( outer ) ?? outer;
        }

        private static void ApplyCellChrome( Border outer, CellChrome chrome )
        {
            var ring = Math.Max( 1, Ui.VDWShadowSize );
            if ( outer.Child is Grid { ColumnDefinitions: { Count: 3 } } grid )
            {
                var ringLen = new GridLength( ring );
                grid.ColumnDefinitions[0].Width = ringLen;
                grid.ColumnDefinitions[2].Width = ringLen;
                grid.RowDefinitions[0].Height   = ringLen;
                grid.RowDefinitions[2].Height   = ringLen;
            }

            outer.Margin          = new Thickness( Ui.VDWMargin - Ui.VDWShadowSize );
            outer.BorderThickness = new Thickness( 0 );
            outer.Effect          = null;

            if ( outer.Tag is Image[] images )
            {
                var slices = chrome == CellChrome.Current ? _shadowCurrent : _shadowDefault;
                images[0].Source = slices.Tl;
                images[1].Source = slices.T;
                images[2].Source = slices.Tr;
                images[3].Source = slices.L;
                images[4].Source = slices.R;
                images[5].Source = slices.Bl;
                images[6].Source = slices.B;
                images[7].Source = slices.Br;
            }

            if ( GetInnerBorder( outer ) is not { } inner ) return;

            inner.BorderThickness = new Thickness( Ui.VDWBorderSize );
            inner.BorderBrush = chrome switch
            {
                CellChrome.Current => new SolidColorBrush( Color.FromRgb(
                    Ui.VDWCurrentBackColor!.R, Ui.VDWCurrentBackColor.G, Ui.VDWCurrentBackColor.B ) ),
                CellChrome.Hover => new SolidColorBrush( Color.FromRgb(
                    Ui.VDWHighlightBackColor!.R, Ui.VDWHighlightBackColor.G, Ui.VDWHighlightBackColor.B ) ),
                _ => new SolidColorBrush( Color.FromRgb(
                    Ui.VDWDefaultBackColor!.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B ) )
            };
        }

        private static Border? GetInnerBorder( Border outer )
        {
            if ( outer.Child is Grid grid )
            {
                foreach ( UIElement child in grid.Children )
                {
                    if ( child is Border inner && Grid.GetRow( inner ) == 1 && Grid.GetColumn( inner ) == 1 )
                        return inner;
                }
            }

            return outer.Child as Border;
        }

        private static void EnsureShadowSlices()
        {
            if ( _shadowDefault.Tl != null ) return;
            Logger.Verbose( "Ensure Shadow Slices" );
            const byte d = 20;
            const byte c = 200;
            _shadowDefault = CreateShadowSlices( d, d, d, 100 );
            _shadowCurrent = CreateShadowSlices( c, c, c, 220 );
        }

        private static ShadowSlices CreateShadowSlices( byte r, byte g, byte b, byte maxAlpha )
        {
            var       src = CreateShadowAtlas( r, g, b, maxAlpha );
            const int m   = SHADOW_ATLAS_SIZE - 2 * SHADOW_ATLAS_INSET;
            return new ShadowSlices
            {
                Tl = Crop( src, 0, 0, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET ),
                T  = Crop( src, SHADOW_ATLAS_INSET, 0, m, SHADOW_ATLAS_INSET ),
                Tr = Crop( src, SHADOW_ATLAS_INSET + m, 0, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET ),
                L  = Crop( src, 0, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET, m ),
                R  = Crop( src, SHADOW_ATLAS_INSET + m, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET, m ),
                Bl = Crop( src, 0, SHADOW_ATLAS_INSET + m, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET ),
                B  = Crop( src, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET + m, m, SHADOW_ATLAS_INSET ),
                Br = Crop( src, SHADOW_ATLAS_INSET + m, SHADOW_ATLAS_INSET + m, SHADOW_ATLAS_INSET, SHADOW_ATLAS_INSET )
            };
        }

        private static WriteableBitmap CreateShadowAtlas( byte r, byte g, byte b, byte maxAlpha )
        {
            var       bmp    = new WriteableBitmap( SHADOW_ATLAS_SIZE, SHADOW_ATLAS_SIZE, 96, 96, PixelFormats.Bgra32, null );
            var       pixels = new byte[SHADOW_ATLAS_SIZE * SHADOW_ATLAS_SIZE * 4];
            const int inner1 = SHADOW_ATLAS_SIZE - SHADOW_ATLAS_INSET;

            for ( var y = 0; y < SHADOW_ATLAS_SIZE; y++ )
            for ( var x = 0; x < SHADOW_ATLAS_SIZE; x++ )
            {
                var cx = x < SHADOW_ATLAS_INSET ? SHADOW_ATLAS_INSET : x >= inner1 ? inner1 - 1 : x;
                var cy = y < SHADOW_ATLAS_INSET ? SHADOW_ATLAS_INSET : y >= inner1 ? inner1 - 1 : y;
                if ( x is >= SHADOW_ATLAS_INSET and < inner1 && y is >= SHADOW_ATLAS_INSET and < inner1 )
                    continue;

                var dist = Math.Sqrt( ( x - cx ) * ( x - cx ) + ( y - cy ) * ( y - cy ) );
                var t    = Math.Min( 1, dist / SHADOW_ATLAS_INSET );
                var a    = (byte)( maxAlpha * ( 1 - t ) * ( 1 - t ) );
                var o    = ( y * SHADOW_ATLAS_SIZE + x ) * 4;
                pixels[o]     = b;
                pixels[o + 1] = g;
                pixels[o + 2] = r;
                pixels[o + 3] = a;
            }

            bmp.WritePixels( new Int32Rect( 0, 0, SHADOW_ATLAS_SIZE, SHADOW_ATLAS_SIZE ), pixels, SHADOW_ATLAS_SIZE * 4, 0 );
            bmp.Freeze();
            return bmp;
        }

        private static CroppedBitmap Crop( BitmapSource src, int x, int y, int w, int h )
        {
            var crop = new CroppedBitmap( src, new Int32Rect( x, y, w, h ) );
            crop.Freeze();
            return crop;
        }

        private readonly struct ShadowSlices
        {
            public ImageSource? Tl { get; init; }
            public ImageSource? T  { get; init; }
            public ImageSource? Tr { get; init; }
            public ImageSource? L  { get; init; }
            public ImageSource? R  { get; init; }
            public ImageSource? Bl { get; init; }
            public ImageSource? B  { get; init; }
            public ImageSource? Br { get; init; }
        }

        public static Point GetCellLocationByMatrixIndex( int index )
        {
            Point Location() => GetCellLayoutBorder( index ).TranslatePoint( new Point(), _instance );

            return _instance.Dispatcher.CheckAccess()
                ? Location()
                : _instance.Dispatcher.Invoke( Location );
        }

        public static Size GetCellSizeByMatrixIndex( int index )
        {
            return GetCellLayoutBorder( index ).RenderSize;
        }

        public static int InCell( Point p )
        {
            var cells = _instance.MainGrid.Children;
            var index = -1;
            var (scaleX, scaleY) = SysInfo.Dpi;
            for ( var i = 0; i < cells.Count; i++ )
            {
                var topLeft = cells[i].TranslatePoint( new Point(), _instance );
                topLeft = new Point( topLeft.X * scaleX, topLeft.Y * scaleY );
                var bottomRight = new Point( topLeft.X + cells[i].RenderSize.Width * scaleX, topLeft.Y + cells[i].RenderSize.Height * scaleY );
                var rect        = new Rect( topLeft, bottomRight );
                if ( rect.Contains( p ) )
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}