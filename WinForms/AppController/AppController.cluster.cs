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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using AppController.WinTaskScheduler;
using VirtualSpace.Config;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private readonly System.Resources.ResourceManager _rm = new( typeof( global::AppController.Properties.Resources ) );

        public void UpdateVDIndexOnTrayIcon( string index )
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

            using var bitmap = (Bitmap)_rm.GetObject( $@"{backColor}" );
            if ( index.Length == 1 )
            {
                using var number = (Bitmap)_rm.GetObject( $@"Big{index}{numberColor}" );
                using var gBack  = Graphics.FromImage( bitmap );
                gBack.CompositingMode = CompositingMode.SourceOver;
                number.MakeTransparent();
                gBack.DrawImage( number, new Point( 0, 0 ) );
            }
            else
            {
                using var number1 = (Bitmap)_rm.GetObject( $@"Small{index[0]}{numberColor}" );
                using var number2 = (Bitmap)_rm.GetObject( $@"Small{index[1]}{numberColor}" );
                number1.MakeTransparent();
                number2.MakeTransparent();
                using var gBack = Graphics.FromImage( bitmap );
                gBack.CompositingMode = CompositingMode.SourceOver;
                gBack.DrawImage( number1, new Point( 0, 0 ) );
                gBack.DrawImage( number2, new Point( bitmap.Width / 2, 0 ) );
            }

            niTray.Icon = Icon.FromHandle( bitmap.GetHicon() );
        }

        private void PaintVdIndexWithLogo( string index )
        {
            using var bitmap = (Bitmap)_rm.GetObject( "TrayIconBack_Default" );
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

            niTray.Icon = Icon.FromHandle( bitmap.GetHicon() );
        }

        private void InitClusterConfig()
        {
            chb_HideMainViewIfItsShown.Checked = ConfigManager.Configs.Cluster.HideMainViewIfItsShown;
            chb_notify_vd_changed.Checked = ConfigManager.Configs.Cluster.NotificationOnVdChanged;
            chb_showVDIndexOnTrayIcon.Checked = ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon;
            chb_HideOnStart.Checked = ConfigManager.Configs.Cluster.HideOnStart;

            chb_HideMainViewIfItsShown.CheckedChanged += chb_HideMainViewIfItsShown_CheckedChanged;
            chb_notify_vd_changed.CheckedChanged += chb_notify_vd_changed_CheckedChanged;
            chb_showVDIndexOnTrayIcon.CheckedChanged += chb_showVDIndexOnTrayIcon_CheckedChanged;
            chb_HideOnStart.CheckedChanged += chb_HideOnStart_CheckedChanged;

            switch ( ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon )
            {
                case 0:
                    rb_vdi_on_tray_style_0.Checked = true;
                    break;
                case 1:
                    rb_vdi_on_tray_style_1.Checked = true;
                    break;
                case 2:
                    rb_vdi_on_tray_style_2.Checked = true;
                    break;
                default:
                    rb_vdi_on_tray_style_0.Checked = true;
                    break;
            }

            rb_vdi_on_tray_style_0.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
            rb_vdi_on_tray_style_1.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
            rb_vdi_on_tray_style_2.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
        }

        private void rb_vdi_on_tray_style_0_CheckedChanged( object? sender, EventArgs e )
        {
            if ( !( (RadioButton)sender ).Checked ) return;
            if ( rb_vdi_on_tray_style_0.Checked )
            {
                ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon = 0;
            }
            else if ( rb_vdi_on_tray_style_1.Checked )
            {
                ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon = 1;
            }
            else if ( rb_vdi_on_tray_style_2.Checked )
            {
                ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon = 2;
            }

            ConfigManager.Save( reason: ConfigManager.Configs.Cluster.StyleOfVDIndexOnTrayIcon );
        }

        private void chb_HideMainViewIfItsShown_CheckedChanged( object? sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.HideMainViewIfItsShown = chb_HideMainViewIfItsShown.Checked;
            ConfigManager.Save( reason: ConfigManager.Configs.Cluster.HideMainViewIfItsShown );
        }

        private void chb_notify_vd_changed_CheckedChanged( object? sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.NotificationOnVdChanged = chb_notify_vd_changed.Checked;
            ConfigManager.Save( reason: ConfigManager.Configs.Cluster.NotificationOnVdChanged );
        }

        private void chb_showVDIndexOnTrayIcon_CheckedChanged( object? sender, EventArgs e )
        {
            if ( !chb_showVDIndexOnTrayIcon.Checked )
            {
                var bitmap = (Bitmap)_rm.GetObject( "AboutLogo_2" );
                niTray.Icon = Icon.FromHandle( bitmap.GetHicon() );
            }

            ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon = chb_showVDIndexOnTrayIcon.Checked;
            ConfigManager.Save( reason: ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon );
        }

        private void chb_HideOnStart_CheckedChanged( object? sender, EventArgs e )
        {
            ConfigManager.Configs.Cluster.HideOnStart = chb_HideOnStart.Checked;
            ConfigManager.Save( reason: ConfigManager.Configs.Cluster.HideOnStart );
        }

        private void chb_RunOnStartup_CheckedChanged( object sender, EventArgs e )
        {
            if ( chb_RunOnStartup.Checked )
            {
                if ( TaskSchedulerHelper.IsTaskExistsByName( Const.AppName ) ) return;
                if ( !TaskSchedulerHelper.CreateTask() )
                    chb_RunOnStartup.Checked = false;
            }
            else
            {
                if ( !TaskSchedulerHelper.IsTaskExistsByName( Const.AppName ) ) return;
                if ( !TaskSchedulerHelper.DeleteTaskByName( Const.AppName ) )
                    chb_RunOnStartup.Checked = true;
            }
        }

        private void chb_RunOnStartup_VisibleChanged( object sender, EventArgs e )
        {
            chb_RunOnStartup.Checked = TaskSchedulerHelper.IsTaskExistsByName( Const.AppName );
        }

        private void llb_TaskScheduler_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            var psi = new ProcessStartInfo
            {
                FileName = "taskschd.msc",
                UseShellExecute = true
            };
            Process.Start( psi );
        }
    }
}