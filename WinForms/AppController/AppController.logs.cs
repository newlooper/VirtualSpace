/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Threading.Channels;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private static readonly Channel<LogMessage> LogChannel = Logger.LogChannel;
        private                 Point               _logTabCursorPos;

        private async void PickLogAndWrite()
        {
            Logger.ShowLogsInGui = ConfigManager.Configs.LogConfig.ShowLogsInGui;
            showLogsInGuiToolStripMenuItem.Checked = ConfigManager.Configs.LogConfig.ShowLogsInGui;
            while ( await LogChannel.Reader.WaitToReadAsync() )
            {
                if ( LogChannel.Reader.TryRead( out var message ) )
                {
                    switch ( message.Type )
                    {
                        case "DEBUG":
                            AppendLog( tbDebug, message );
                            break;
                        case "EVENT":
                            AppendLog( tbEvent, message );
                            break;
                        case "INFO":
                            AppendLog( tbInfo, message );
                            break;
                        case "WARNING":
                            AppendLog( tbWarning, message );
                            break;
                        case "ERROR":
                            AppendLog( tbError, message );
                            break;
                        default:
                            AppendLog( tbError, message );
                            break;
                    }
                }
            }
        }

        private void AppendLog( TextBox tb, LogMessage message )
        {
            tb.AppendText( message.Message );
        }

        private void logTabs_Click( object sender, EventArgs e )
        {
            var e2 = (MouseEventArgs)e;
            if ( e2.Button == MouseButtons.Right )
            {
                _logTabCursorPos = Cursor.Position;
                logCMS.Show( logTabs, e2.Location );
            }
        }

        private void clearToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var cnt = logTabs.TabCount;
            for ( var i = 0; i < cnt; i++ )
            {
                var rect = logTabs.GetTabRect( i );
                if ( rect.Contains( logTabs.PointToClient( _logTabCursorPos ) ) )
                {
                    var dialogResult = MessageBox.Show(
                        $@"Do you want to clear [{logTabs.TabPages[i].Text}] logs?",
                        @"Confirm",
                        MessageBoxButtons.YesNo );
                    switch ( dialogResult )
                    {
                        case DialogResult.Yes:
                            var tb = logTabs.TabPages[i].Controls[0] as TextBox;
                            tb.Clear();
                            break;
                        case DialogResult.No:
                            break;
                    }

                    break;
                }
            }
        }

        private void showLogsInGuiToolStripMenuItem_CheckedChanged( object sender, EventArgs e )
        {
            var item = sender as ToolStripMenuItem;
            Logger.ShowLogsInGui = item.Checked;
            ConfigManager.Configs.LogConfig.ShowLogsInGui = item.Checked;
            ConfigManager.Save();
        }
    }
}