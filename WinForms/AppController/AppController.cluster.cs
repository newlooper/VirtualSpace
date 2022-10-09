// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void ReadClusterConfig()
        {
            chb_HideMainViewIfItsShown.Checked = ConfigManager.Configs.Cluster.HideMainViewIfItsShown;
            chb_notify_vd_changed.Checked = ConfigManager.Configs.Cluster.NotificationOnVdChanged;
            chb_showVDIndexOnTrayIcon.Checked = ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon;
        }

        private void chb_HideMainViewIfItsShown_CheckedChanged( object sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.HideMainViewIfItsShown = chb_HideMainViewIfItsShown.Checked;
            ConfigManager.Save();
        }

        private void chb_notify_vd_changed_CheckedChanged( object sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.NotificationOnVdChanged = chb_notify_vd_changed.Checked;
            ConfigManager.Save();
        }

        private void chb_showVDIndexOnTrayIcon_CheckedChanged( object sender, EventArgs e )
        {
            if ( !chb_showVDIndexOnTrayIcon.Checked )
            {
                var bitmap = (Bitmap)Resources.GetObject( "pb_AboutLogo.Image" );
                niTray.Icon = Icon.FromHandle( bitmap.GetHicon() );
            }

            ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon = chb_showVDIndexOnTrayIcon.Checked;
            ConfigManager.Save();
        }

        public void UpdateVDIndexOnTrayIcon( string index )
        {
            var bitmap = (Bitmap)Resources.GetObject( "pb_AboutLogo.Image" );
            var rectF = new RectangleF( 0, 0, bitmap.Width, bitmap.Height );
            var textFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var fontSize = index.Length == 1 ? 180 : 150;

            using var textFont  = new Font( "Comic Sans MS", fontSize, FontStyle.Bold, GraphicsUnit.Pixel );
            using var textBrush = new SolidBrush( ColorTranslator.FromHtml( "#FFFFFF" ) );
            using var borderPen = new Pen( ColorTranslator.FromHtml( "#FF0000" ), 40 );
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

            niTray.Icon = Icon.FromHandle( bitmap.GetHicon() );
        }
    }
}