/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;
using System.Windows.Input;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using KeyBinding = VirtualSpace.Config.Entity.KeyBinding;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void tv_keyboard_AfterSelect( object sender, TreeViewEventArgs e )
        {
            tb_hk_tip.Clear();

            cb_hk_win.Checked = false;
            cb_hk_ctrl.Checked = false;
            cb_hk_alt.Checked = false;
            cb_hk_shift.Checked = false;
            cb_hk_key.SelectedIndex = -1;

            tc_Keyboard.Visible = false;

            var kb       = Manager.Configs.KeyBindings;
            var hotkeyId = e.Node.Name;

            if ( !kb.ContainsKey( hotkeyId ) )
            {
                if ( Const.Hotkey.Info.ContainsKey( hotkeyId ) )
                {
                    kb[hotkeyId] = new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[hotkeyId].Item2};
                }
                else
                {
                    return;
                }
            }

            lb_hk_func.Text = e.Node.FullPath;
            tc_Keyboard.Visible = true;
            lb_hk_extra.Text = Const.Hotkey.Info[hotkeyId].Item3;

            var keyBinding = kb[hotkeyId];
            if ( keyBinding.GhkCode == "" ) return;

            var arr = keyBinding.GhkCode.Split( Const.Hotkey.SPLITTER );
            if ( arr.Length == 5 )
            {
                cb_hk_win.Checked = arr[0] != Const.Hotkey.NONE;
                cb_hk_ctrl.Checked = arr[1] != Const.Hotkey.NONE;
                cb_hk_alt.Checked = arr[2] != Const.Hotkey.NONE;
                cb_hk_shift.Checked = arr[3] != Const.Hotkey.NONE;

                cb_hk_key.Text = arr[4];
            }
        }

        private void tssb_hk_save_reg_ButtonClick( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
            if ( !Check() ) return;

            var ghk = GetGhk();


            RegHotkey( ghk );
            SaveHotkey( ghk );
        }

        private void tssb_hk_save_reg_DropDownItemClicked( object sender, ToolStripItemClickedEventArgs e )
        {
            var index = tssb_hk_save_reg.DropDownItems.IndexOf( e.ClickedItem );
            switch ( index )
            {
                case 0: // save only

                    tb_hk_tip.Clear();
                    if ( !Check() ) return;
                    SaveHotkey( GetGhk() );
                    break;
                case 1: // register only

                    tb_hk_tip.Clear();
                    if ( !Check() ) return;
                    RegHotkey( GetGhk() );
                    break;
                case 2: // clear
                    tb_hk_tip.Clear();
                    GlobalHotKey.UnregisterHotKey( _mainWindowHandle, Manager.Configs.KeyBindings[tv_keyboard.SelectedNode.Name].MessageId );
                    ClearHotkey();
                    SaveHotkey( GetGhk() );
                    break;
            }
        }

        private bool Check()
        {
            if ( cb_hk_key.SelectedItem is not null ) return true;

            tb_hk_tip.Text = Agent.Langs.GetString( "KB.Hotkey.KeyCheck" );
            return false;
        }

        private Tuple<string, GlobalHotKey.KeyModifiers> GetGhk()
        {
            var ghkCode = "";
            var km      = GlobalHotKey.KeyModifiers.None;

            if ( cb_hk_key.SelectedIndex == 0 || cb_hk_key.SelectedItem is null )
            {
                ghkCode = "";
            }
            else
            {
                ghkCode = ( cb_hk_win.Checked ? Const.Hotkey.WIN : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
                ghkCode += ( cb_hk_ctrl.Checked ? Const.Hotkey.CTRL : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
                ghkCode += ( cb_hk_alt.Checked ? Const.Hotkey.ALT : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
                ghkCode += ( cb_hk_shift.Checked ? Const.Hotkey.SHIFT : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;

                km = cb_hk_win.Checked ? GlobalHotKey.KeyModifiers.WindowsKey : GlobalHotKey.KeyModifiers.None;
                km |= cb_hk_ctrl.Checked ? GlobalHotKey.KeyModifiers.Ctrl : GlobalHotKey.KeyModifiers.None;
                km |= cb_hk_alt.Checked ? GlobalHotKey.KeyModifiers.Alt : GlobalHotKey.KeyModifiers.None;
                km |= cb_hk_shift.Checked ? GlobalHotKey.KeyModifiers.Shift : GlobalHotKey.KeyModifiers.None;

                ghkCode += cb_hk_key.SelectedItem.ToString();
            }

            return new Tuple<string, GlobalHotKey.KeyModifiers>( ghkCode, km );
        }

        private void SaveHotkey( Tuple<string, GlobalHotKey.KeyModifiers> ghk )
        {
            var hotkeyId = tv_keyboard.SelectedNode.Name;
            Manager.Configs.KeyBindings[hotkeyId].GhkCode = ghk.Item1;
            Manager.Save();
            tb_hk_tip.Text += Agent.Langs.GetString( "KB.Hotkey.SettingsSaved" ) + Const.WindowsCRLF;
        }

        private void RegHotkey( Tuple<string, GlobalHotKey.KeyModifiers> ghk )
        {
            var hotkeyId = tv_keyboard.SelectedNode.Name;
            GlobalHotKey.UnregisterHotKey( _mainWindowHandle, Manager.Configs.KeyBindings[hotkeyId].MessageId );

            if ( cb_hk_key.SelectedIndex == 0 )
            {
                ClearHotkey();
                return;
            }

            if ( GlobalHotKey.RegHotKey( _mainWindowHandle,
                    Manager.Configs.KeyBindings[hotkeyId].MessageId,
                    ghk.Item2,
                    KeyInterop.VirtualKeyFromKey( Enum.Parse<Key>( cb_hk_key.SelectedItem.ToString() ) ) ) )
            {
                tb_hk_tip.Text += Agent.Langs.GetString( "KB.Hotkey.Reg.Success" ) + Const.WindowsCRLF;
            }
            else
            {
                tb_hk_tip.Text += Agent.Langs.GetString( "KB.Hotkey.Reg.Fail" ) + Const.WindowsCRLF;
            }
        }

        private void ClearHotkey()
        {
            cb_hk_win.Checked = false;
            cb_hk_ctrl.Checked = false;
            cb_hk_alt.Checked = false;
            cb_hk_shift.Checked = false;
            cb_hk_key.SelectedIndex = -1;
            tb_hk_tip.Text += Agent.Langs.GetString( "KB.Hotkey.Cleared" ) + Const.WindowsCRLF;
        }

        private void cb_hk_key_SelectedIndexChanged( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
        }

        private void cb_hk_win_CheckedChanged( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
        }

        private void cb_hk_ctrl_CheckedChanged( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
        }

        private void cb_hk_alt_CheckedChanged( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
        }

        private void cb_hk_shift_CheckedChanged( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
        }

        private void KeyboardTopNodeExpand()
        {
            tv_keyboard.Nodes[0].Expand();
            tv_keyboard.Nodes[1].Expand();
        }
    }
}