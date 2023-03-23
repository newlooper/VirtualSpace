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
using System.Windows.Forms;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void InitClusterConfig( bool resetEventHandlers = true )
        {
            if ( resetEventHandlers )
            {
                chb_HideMainViewIfItsShown.CheckedChanged -= chb_HideMainViewIfItsShown_CheckedChanged;
                chb_notify_vd_changed.CheckedChanged -= chb_notify_vd_changed_CheckedChanged;
                chb_showVDIndexOnTrayIcon.CheckedChanged -= chb_showVDIndexOnTrayIcon_CheckedChanged;
                chb_HideOnStart.CheckedChanged -= chb_HideOnStart_CheckedChanged;
                rb_vdi_on_tray_style_0.CheckedChanged -= rb_vdi_on_tray_style_0_CheckedChanged;
                rb_vdi_on_tray_style_1.CheckedChanged -= rb_vdi_on_tray_style_0_CheckedChanged;
                rb_vdi_on_tray_style_2.CheckedChanged -= rb_vdi_on_tray_style_0_CheckedChanged;
            }

            chb_HideMainViewIfItsShown.Checked = ConfigManager.Configs.Cluster.HideMainViewIfItsShown;
            chb_notify_vd_changed.Checked = ConfigManager.Configs.Cluster.NotificationOnVdChanged;
            chb_showVDIndexOnTrayIcon.Checked = ConfigManager.Configs.Cluster.ShowVDIndexOnTrayIcon;
            chb_HideOnStart.Checked = ConfigManager.Configs.Cluster.HideOnStart;

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

            if ( resetEventHandlers )
            {
                chb_HideMainViewIfItsShown.CheckedChanged += chb_HideMainViewIfItsShown_CheckedChanged;
                chb_notify_vd_changed.CheckedChanged += chb_notify_vd_changed_CheckedChanged;
                chb_showVDIndexOnTrayIcon.CheckedChanged += chb_showVDIndexOnTrayIcon_CheckedChanged;
                chb_HideOnStart.CheckedChanged += chb_HideOnStart_CheckedChanged;
                rb_vdi_on_tray_style_0.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
                rb_vdi_on_tray_style_1.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
                rb_vdi_on_tray_style_2.CheckedChanged += rb_vdi_on_tray_style_0_CheckedChanged;
            }
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

            NotifyHostRefreshTrayIcon();
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
            NotifyHostRefreshTrayIcon();

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
                if ( TaskSchedulerHelper.IsTaskExistsByName( Const.AppName, Const.AppName ) ) return;
                try
                {
                    TaskSchedulerHelper.CreateAutoRunTask( Const.AppName, Manager.AppPath, Const.AppName );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( Agent.Langs.GetString( ex.Message ) );
                    chb_RunOnStartup.Checked = false;
                }
            }
            else
            {
                if ( !TaskSchedulerHelper.IsTaskExistsByName( Const.AppName, Const.AppName ) ) return;
                try
                {
                    TaskSchedulerHelper.DeleteTaskByName( Const.AppName, Const.AppName );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( Agent.Langs.GetString( ex.Message ) );
                    chb_RunOnStartup.Checked = true;
                }
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

        private void NotifyHostRefreshTrayIcon()
        {
            User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RefreshTrayIcon, 0 );
        }
    }
}