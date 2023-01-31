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
        private void tv_mouse_AfterSelect( object sender, TreeViewEventArgs e )
        {
            tc_Mouse.Visible = false;

            var ma            = Manager.Configs.MouseActions;
            var mouseActionId = e.Node.Name;
            if ( !ma.ContainsKey( mouseActionId ) )
            {
                if ( Const.MouseAction.Info.ContainsKey( mouseActionId ) )
                {
                    ma[mouseActionId] = Const.MouseAction.Info[mouseActionId].Item2;
                }
                else
                {
                    return;
                }
            }

            lb_mouse_action.Text = e.Node.FullPath;
            tc_Mouse.Visible = true;

            if ( mouseActionId.StartsWith( "mouse_node_d" ) )
            {
                var items = new List<object>
                {
                    new {Value = Const.MouseAction.Action.DesktopVisibleAndCloseView, Text = Agent.Langs.GetString( "Mouse.Action.DesktopVisibleAndCloseView" )},
                    new {Value = Const.MouseAction.Action.DesktopVisibleOnly, Text = Agent.Langs.GetString( "Mouse.Action.DesktopVisibleOnly" )},
                    new {Value = Const.MouseAction.Action.ContextMenu, Text = Agent.Langs.GetString( "Mouse.Action.ContextMenu" )},
                    new {Value = Const.MouseAction.Action.DoNothing, Text = Agent.Langs.GetString( "Mouse.Action.DoNothing" )}
                };
                WinForms.SetComboBoxDataSource( cb_mouse_func, items );
            }
            else if ( mouseActionId.StartsWith( "mouse_node_w" ) )
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
                    new {Value = Const.MouseAction.Action.WindowClose, Text = Agent.Langs.GetString( "Mouse.Action.WindowClose" )},
                    new {Value = Const.MouseAction.Action.DoNothing, Text = Agent.Langs.GetString( "Mouse.Action.DoNothing" )}
                };
                WinForms.SetComboBoxDataSource( cb_mouse_func, items );
            }

            cb_mouse_func.SelectedValue = ma[mouseActionId];
        }

        private void btn_mouse_save_Click( object sender, EventArgs e )
        {
            var maId = tv_mouse.SelectedNode.Name;
            Manager.Configs.MouseActions[maId] = (Const.MouseAction.Action)cb_mouse_func.SelectedValue;
            Manager.Save();
        }

        private void MouseTopNodeExpand()
        {
            tv_mouse.Nodes[0].Expand();
        }
    }
}