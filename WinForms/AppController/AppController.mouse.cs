/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void InitMouseConfig()
        {
            chb_MouseOnTaskbarSwitchDesktop.Checked = Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar;
            chb_MouseOnTaskbarSwitchDesktop.CheckedChanged += chb_MouseOnTaskbarSwitchDesktop_CheckedChanged;
        }

        private void chb_MouseOnTaskbarSwitchDesktop_CheckedChanged( object? sender, EventArgs e )
        {
            Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar = chb_MouseOnTaskbarSwitchDesktop.Checked;
            var msg = Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar ? UserMessage.EnableMouseHook : UserMessage.DisableMouseHook;
            User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, (ulong)msg, 0 );
            Manager.Save();
        }

        private void tv_mouse_AfterSelect( object sender, TreeViewEventArgs e )
        {
            tc_Mouse.Visible = false;

            var ma            = Manager.Configs.MouseAction;
            var mouseActionId = e.Node.Name;
            if ( !ma.ContainsKey( mouseActionId ) )
            {
                if ( Const.MouseAction.Info.ContainsKey( mouseActionId ) )
                {
                    ma[mouseActionId] = Const.MouseAction.Info[mouseActionId];
                }
                else
                {
                    return;
                }
            }

            lb_mouse_action.Text = e.Node.FullPath;
            tc_Mouse.Visible = true;

            if ( mouseActionId.StartsWith( Const.MouseAction.MOUSE_NODE_DESKTOP_PREFIX ) )
            {
                var items = new List<object>
                {
                    new {Value = Const.MouseAction.Action.DesktopVisibleAndCloseView, Text = Agent.Langs.GetString( "Mouse.Action.DesktopVisibleAndCloseView" )},
                    new {Value = Const.MouseAction.Action.DesktopVisibleOnly, Text = Agent.Langs.GetString( "Mouse.Action.DesktopVisibleOnly" )},
                    new {Value = Const.MouseAction.Action.ContextMenu, Text = Agent.Langs.GetString( "Mouse.Action.ContextMenu" )},
                    new {Value = Const.MouseAction.Action.DesktopShowForSelectedDesktop, Text = Agent.Langs.GetString( "Mouse.Action.DesktopShowForSelectedDesktop" )},
                    new {Value = Const.MouseAction.Action.DoNothing, Text = Agent.Langs.GetString( "Mouse.Action.DoNothing" )}
                };
                WinForms.SetComboBoxDataSource( cb_mouse_func, items );
            }
            else if ( mouseActionId.StartsWith( Const.MouseAction.MOUSE_NODE_WINDOW_PREFIX ) )
            {
                var items = new List<object>
                {
                    new
                    {
                        Value = Const.MouseAction.Action.WindowActiveDesktopVisibleAndCloseView,
                        Text = Agent.Langs.GetString( "Mouse.Action.WindowActiveDesktopVisibleAndCloseView" )
                    },
                    new
                    {
                        Value = Const.MouseAction.Action.WindowActiveDesktopVisibleOnly,
                        Text = Agent.Langs.GetString( "Mouse.Action.WindowActiveDesktopVisibleOnly" )
                    },
                    new {Value = Const.MouseAction.Action.ContextMenu, Text = Agent.Langs.GetString( "Mouse.Action.ContextMenu" )},
                    new {Value = Const.MouseAction.Action.WindowHideFromView, Text = Agent.Langs.GetString( "VDW.CTM.Window.HideFromView" )},
                    new {Value = Const.MouseAction.Action.WindowClose, Text = Agent.Langs.GetString( "Mouse.Action.WindowClose" )},
                    new {Value = Const.MouseAction.Action.WindowShowForSelectedProcessOnly, Text = Agent.Langs.GetString( "Mouse.Action.WindowShowForSelectedProcessOnly" )},
                    new
                    {
                        Value = Const.MouseAction.Action.WindowShowForSelectedProcessInSelectedDesktop,
                        Text = Agent.Langs.GetString( "Mouse.Action.WindowShowForSelectedProcessInSelectedDesktop" )
                    },
                    new {Value = Const.MouseAction.Action.DoNothing, Text = Agent.Langs.GetString( "Mouse.Action.DoNothing" )}
                };
                WinForms.SetComboBoxDataSource( cb_mouse_func, items );
            }

            cb_mouse_func.SelectedValue = ma[mouseActionId];
        }

        private void btn_mouse_save_Click( object sender, EventArgs e )
        {
            var maId = tv_mouse.SelectedNode.Name;
            Manager.Configs.MouseAction[maId] = (Const.MouseAction.Action)cb_mouse_func.SelectedValue;
            Manager.Save();
        }

        private void InitMouseNodes()
        {
            var nodeDesktop = tv_mouse.Nodes["mouse_root_mainview"].Nodes["mouse_parent_desktop"];
            var nodeWindow  = tv_mouse.Nodes["mouse_root_mainview"].Nodes["mouse_parent_window"];
            nodeDesktop.Nodes.Clear();
            nodeWindow.Nodes.Clear();

            string GetLocaleText( string text )
            {
                text = text
                    .Replace( "Left", Agent.Langs.GetString( "Keys.Left" ) )
                    .Replace( "Middle", Agent.Langs.GetString( "Keys.Middle" ) )
                    .Replace( "Right", Agent.Langs.GetString( "Keys.Right" ) );
                return text;
            }

            foreach ( var kv in Const.MouseAction.Info )
            {
                if ( kv.Key.StartsWith( Const.MouseAction.MOUSE_NODE_DESKTOP_PREFIX ) )
                {
                    nodeDesktop.Nodes.Add( kv.Key, GetLocaleText( kv.Key.Replace( Const.MouseAction.MOUSE_NODE_DESKTOP_PREFIX, "" ) ) );
                }
                else if ( kv.Key.StartsWith( Const.MouseAction.MOUSE_NODE_WINDOW_PREFIX ) )
                {
                    nodeWindow.Nodes.Add( kv.Key, GetLocaleText( kv.Key.Replace( Const.MouseAction.MOUSE_NODE_WINDOW_PREFIX, "" ) ) );
                }
            }

            tc_Mouse.Visible = false;
            tv_mouse.ExpandAll();
        }
    }
}