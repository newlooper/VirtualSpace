/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Globalization;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private static void SetAllLang( string lang )
        {
            CultureInfo.CurrentCulture = new CultureInfo( lang );
            CultureInfo.CurrentUICulture = new CultureInfo( lang );

            Logger.Info( "Change Language: " + CultureInfo.CurrentUICulture.DisplayName );

            void Invoker()
            {
                SetControlLang( _instance.logCMS, lang );

                _instance.tv_keyboard.Nodes.Clear();
                _instance.tv_keyboard.Nodes.AddRange( new[]
                {
                    (TreeNode)Resources.GetObject( "tv_keyboard.Nodes" ),
                    (TreeNode)Resources.GetObject( "tv_keyboard.Nodes1" ),
                    (TreeNode)Resources.GetObject( "tv_keyboard.Nodes2" )
                } );
                _instance.InitKeyboardNodes();

                _instance.tv_mouse.Nodes.Clear();
                _instance.tv_mouse.Nodes.AddRange( new[]
                {
                    (TreeNode)Resources.GetObject( "tv_mouse.Nodes" )
                } );
                _instance.InitMouseNodes();

                SetControlLang( _instance, lang );
            }

            if ( _instance.InvokeRequired )
            {
                _instance.Invoke( (MethodInvoker)Invoker );
            }
            else
            {
                Invoker();
            }
        }

        private static void SetControlLang( Control control, string lang )
        {
            var ci = new CultureInfo( lang );

            switch ( control )
            {
                case MenuStrip strip:
                {
                    Resources.ApplyResources( control, strip.Name, ci );
                    foreach ( var item in strip.Items )
                    {
                        if ( item is ToolStripMenuItem c )
                            SetMenuItemLang( c, lang );
                    }

                    break;
                }
                case ContextMenuStrip ms:
                {
                    Resources.ApplyResources( control, ms.Name, ci );
                    foreach ( var item in ms.Items )
                    {
                        if ( item is ToolStripMenuItem c )
                            SetMenuItemLang( c, lang );
                    }

                    break;
                }
                case ListView lv:
                {
                    foreach ( var item in lv.Columns )
                    {
                        if ( item is ColumnHeader ch )
                            Resources.ApplyResources( ch, ch.Name, ci );
                    }

                    break;
                }
                case ToolStrip ts:
                {
                    foreach ( var item in ts.Items )
                    {
                        switch ( item )
                        {
                            case ToolStripButton tsb:
                                SetToolStripButtonLang( tsb, lang );
                                break;
                            case ToolStripSplitButton tssb:
                                Resources.ApplyResources( tssb, tssb.Name, ci );
                                foreach ( var m in tssb.DropDownItems )
                                {
                                    SetMenuItemLang( m as ToolStripMenuItem, lang );
                                }

                                break;
                        }
                    }

                    break;
                }
            }

            foreach ( var item in control.Controls )
            {
                if ( item is Control c )
                {
                    Resources.ApplyResources( c, c.Name, ci );
                    SetControlLang( c, lang );
                }
            }
        }

        private static void SetMenuItemLang( ToolStripMenuItem item, string lang )
        {
            var ci = new CultureInfo( lang );
            Resources.ApplyResources( item, item.Name, ci );
            if ( item.DropDownItems.Count > 0 )
            {
                foreach ( ToolStripItem c in item.DropDownItems )
                {
                    if ( c is ToolStripMenuItem menuItem )
                    {
                        SetMenuItemLang( menuItem, lang );
                    }
                }
            }
        }

        private static void SetToolStripButtonLang( ToolStripButton item, string lang )
        {
            var ci = new CultureInfo( lang );
            Resources.ApplyResources( item, item.Name, ci );
        }

        private void optionsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            void UpdateCheckState( object? obj, EventArgs evt )
            {
                var l = (ToolStripMenuItem)obj;

                ConfigManager.CurrentProfile.UI.Language = l.Name;
                ConfigManager.Save( reason: ConfigManager.CurrentProfile.UI.Language );
                User32.PostMessage( _instance._mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.UpdateTrayLang, 0 );

                if ( Math.Abs( SysInfo.Dpi.ScaleX - 1.0f ) > 0 )
                {
                    _cancelTokenSourceForLog.Cancel();
                    User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RestartAppController, 0 );
                    return;
                }

                SetAllLang( l.Name );

                foreach ( ToolStripMenuItem lang in langToolStripMenuItem.DropDownItems )
                {
                    lang.Checked = lang.Name == l.Name;
                }

                ReadNavConfig();
            }

            langToolStripMenuItem.DropDownItems.Clear();
            foreach ( var (key, value) in Agent.ValidLangs )
            {
                var langItem = new ToolStripMenuItem
                {
                    Checked = CultureInfo.CurrentUICulture.Name == key,
                    Name = key,
                    Text = value
                };
                langItem.Click += UpdateCheckState;
                langToolStripMenuItem.DropDownItems.Add( langItem );
            }
        }
    }
}