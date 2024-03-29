﻿/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

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
using VirtualSpace.VirtualDesktop.Api;

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

            var kbInConfig = Manager.Configs.KeyBindings;
            var hotkeyId   = e.Node.Name;

            if ( !kbInConfig.ContainsKey( hotkeyId ) )
            {
                var kb = Const.Hotkey.GetKeyBinding( hotkeyId );
                if ( kb.MessageId == 0 ) return;
                kbInConfig[hotkeyId] = kb;
            }

            lb_hk_func.Text = e.Node.FullPath;
            lb_hk_extra.Text = Const.Hotkey.GetHotkeyExtra( hotkeyId );
            tc_Keyboard.Visible = true;

            if ( kbInConfig[hotkeyId].GhkCode == "" ) return;

            var arr = kbInConfig[hotkeyId].GhkCode.Split( Const.Hotkey.SPLITTER );
            if ( arr.Length == 5 )
            {
                cb_hk_win.Checked = arr[0] != Const.Hotkey.NONE;
                cb_hk_ctrl.Checked = arr[1] != Const.Hotkey.NONE;
                cb_hk_alt.Checked = arr[2] != Const.Hotkey.NONE;
                cb_hk_shift.Checked = arr[3] != Const.Hotkey.NONE;

                cb_hk_key.Text = arr[4];
            }
        }

        private void btn_hk_RegAndSave_Click( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
            if ( !Check() ) return;

            var ghk = GetGhk();

            RegHotkey( ghk );
            SaveHotkey( ghk );
        }

        private void btn_hk_ClearAndSave_Click( object sender, EventArgs e )
        {
            tb_hk_tip.Clear();
            var msgId = Const.Hotkey.GetKeyBinding( tv_keyboard.SelectedNode.Name ).MessageId;
            GlobalHotKey.UnregisterHotKey( _mainWindowHandle, msgId );
            ClearHotkey();
            SaveHotkey( GetGhk() );
        }

        private bool Check()
        {
            if ( cb_hk_key.SelectedItem is not null ) return true;

            tb_hk_tip.Text = Agent.Langs.GetString( "KB.Hotkey.KeyCheck" );
            return false;
        }

        private (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) GetGhk()
        {
            string ghkCode;
            var    km = GlobalHotKey.KeyModifiers.None;

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

            return new ValueTuple<string, GlobalHotKey.KeyModifiers>( ghkCode, km );
        }

        private void SaveHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
        {
            var hotkeyId = tv_keyboard.SelectedNode.Name;
            var kb       = Const.Hotkey.GetKeyBinding( hotkeyId );
            kb.GhkCode = ghk.keyCode;
            Manager.Configs.KeyBindings[hotkeyId] = kb;
            Manager.Save( reason: kb.GhkCode.Replace( Const.Hotkey.NONE + Const.Hotkey.SPLITTER, "" ), reasonName: hotkeyId );
            tb_hk_tip.Text += Agent.Langs.GetString( "KB.Hotkey.SettingsSaved" ) + Const.WindowsCRLF;
        }

        private void RegHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
        {
            var hotkeyId = tv_keyboard.SelectedNode.Name;
            var msgId    = Const.Hotkey.GetKeyBinding( hotkeyId ).MessageId;
            GlobalHotKey.UnregisterHotKey( _mainWindowHandle, msgId );

            if ( cb_hk_key.SelectedIndex == 0 )
            {
                ClearHotkey();
                return;
            }

            if ( GlobalHotKey.RegHotKey( _mainWindowHandle,
                    msgId,
                    ghk.keyModifiers,
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

        private void InitKeyboardNodes()
        {
            var nodeSvd = tv_keyboard.Nodes["hk_root_desktop"].Nodes["hk_parent_svd"];
            var nodeMw  = tv_keyboard.Nodes["hk_root_window"].Nodes["hk_parent_win_move"];
            var nodeMwf = tv_keyboard.Nodes["hk_root_window"].Nodes["hk_parent_win_move_follow"];
            nodeSvd.Nodes.Clear();
            nodeMw.Nodes.Clear();
            nodeMwf.Nodes.Clear();

            for ( var i = 1; i <= DesktopWrapper.Count; i++ )
            {
                nodeSvd.Nodes.Add( Const.Hotkey.SVD_TREE_NODE_PREFIX + i, Agent.Langs.GetString( "KB.Hotkey.SVD" ) + i );
                nodeMw.Nodes.Add( Const.Hotkey.MW_TREE_NODE_PREFIX + i, Agent.Langs.GetString( "KB.Hotkey.MW" ) + i );
                nodeMwf.Nodes.Add( Const.Hotkey.MWF_TREE_NODE_PREFIX + i, Agent.Langs.GetString( "KB.Hotkey.MWF" ) + i );
            }

            nodeSvd.Nodes.Add( Const.Hotkey.SWITCH_BACK_LAST, Agent.Langs.GetString( "KB.Hotkey.SVD_BACK_LAST" ) );

            tc_Keyboard.Visible = false;
            tv_keyboard.ExpandAll();
        }
    }
}