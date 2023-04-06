// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.Tools
{
    public static class TrayIcon
    {
        private static readonly NotifyIcon         Ti                 = new();
        private static readonly ContextMenuStrip   TiMenu             = new();
        private static readonly ResourceManager    ImageManager       = Agent.Images;
        private static readonly ToolStripMenuItem  TraySettings       = new();
        private static readonly ToolStripSeparator ToolStripSeparator = new();
        private static readonly ToolStripMenuItem  TrayQuit           = new();

        static TrayIcon()
        {
            SetLang();
            InitTrayIcon();

            TraySettings.Click += ( sender, args ) => { MainWindow.AcForm.BringToTop(); };
            TrayQuit.Click += ( sender,     args ) => { MainWindow.Quit(); };

            TiMenu.Items.AddRange( new ToolStripItem[] {TraySettings, ToolStripSeparator, TrayQuit} );
            Ti.ContextMenuStrip = TiMenu;
        }

        public static void InitTrayIcon()
        {
            Ti.Icon = Images.BytesToIcon( ImageManager.GetObject( "TrayIcon" ) );
        }

        public static void UpdateVDIndexOnTrayIcon( string index )
        {
            if ( ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon == 0 || index.Length > 2 )
            {
                PaintVdIndexWithLogo( index );
                return;
            }

            var backColor   = "TrayIconBack_White";
            var numberColor = "Black";
            switch ( ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon )
            {
                case 1:
                    backColor = "TrayIconBack_White";
                    numberColor = "Black";
                    break;
                case 2:
                    backColor = "TrayIconBack_Black";
                    numberColor = "White";
                    break;
            }

            using var bitmap = Images.BytesToBitmap( ImageManager.GetObject( $@"{backColor}" ) );
            if ( index.Length == 1 )
            {
                using var number = Images.BytesToBitmap( ImageManager.GetObject( $@"Big{index}{numberColor}" ) );
                using var gBack  = Graphics.FromImage( bitmap );
                gBack.CompositingMode = CompositingMode.SourceOver;
                number.MakeTransparent();
                gBack.DrawImage( number, new Point( 0, 0 ) );
            }
            else
            {
                using var number1 = Images.BytesToBitmap( ImageManager.GetObject( $@"Small{index[0]}{numberColor}" ) );
                using var number2 = Images.BytesToBitmap( ImageManager.GetObject( $@"Small{index[1]}{numberColor}" ) );
                number1.MakeTransparent();
                number2.MakeTransparent();
                using var gBack = Graphics.FromImage( bitmap );
                gBack.CompositingMode = CompositingMode.SourceOver;
                gBack.DrawImage( number1, new Point( 0, 0 ) );
                gBack.DrawImage( number2, new Point( bitmap.Width / 2, 0 ) );
            }

            Ti.Icon = Icon.FromHandle( bitmap.GetHicon() );
        }

        private static void PaintVdIndexWithLogo( string index )
        {
            using var bitmap = Images.BytesToBitmap( ImageManager.GetObject( "TrayIconBack_Default" ) );
            var       rectF  = new RectangleF( 0, 0, bitmap.Width, bitmap.Height );
            var textFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var fontSize   = 210;
            var borderSize = 10;

            switch ( index.Length )
            {
                case 1:
                    fontSize = 210;
                    borderSize = 20;
                    break;
                case 2:
                    fontSize = 160;
                    borderSize = 30;
                    break;
                case 3:
                    fontSize = 110;
                    borderSize = 30;
                    break;
            } // fontSize and borderSize based on TrayIconBack_Default's size is 256x256

            using var textFont  = new Font( "Comic Sans MS", fontSize, FontStyle.Bold, GraphicsUnit.Pixel );
            using var textBrush = new SolidBrush( ColorTranslator.FromHtml( "#FFFFFF" ) );
            using var borderPen = new Pen( ColorTranslator.FromHtml( "#FF0000" ), borderSize );
            borderPen.LineJoin = LineJoin.Round; // prevent "spikes" at the path

            using var gp = new GraphicsPath();
            gp.AddString( index, textFont.FontFamily, (int)textFont.Style, textFont.Size, rectF, textFormat );

            using var g = Graphics.FromImage( bitmap );
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            g.DrawPath( borderPen, gp );
            g.FillPath( textBrush, gp );

            g.Flush();

            Ti.Icon = Icon.FromHandle( bitmap.GetHicon() );
        }

        public static void SetLang()
        {
            CultureInfo.CurrentCulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
            CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
            TraySettings.Text = Agent.Langs.GetString( "Tray.Menu.Settings" );
            TrayQuit.Text = Agent.Langs.GetString( "Tray.Menu.Quit" );
        }

        public static void Show()
        {
            Ti.Visible = true;
        }
    }
}