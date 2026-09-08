// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace ControlPanel.Pages;

public partial class Control
{
    private void KeyboardTreeView_OnSelectedItemChanged( object sender, RoutedPropertyChangedEventArgs<object> e )
    {
        var vm = KeyBindingBox.DataContext as KeyBindingModel;
        vm!.BoxVisible = Visibility.Hidden;

        if ( e.NewValue is not TreeViewItem selectedNode )
        {
            return;
        }

        var kbInConfig = Manager.Configs.KeyBindings;
        var hotkeyId   = selectedNode.Name;

        if ( !kbInConfig!.TryGetValue( hotkeyId, out var value ) )
        {
            var kb = Const.Hotkey.GetKeyBinding( hotkeyId );
            if ( kb.MessageId == 0 )
            {
                return;
            }

            value                = kb;
            kbInConfig[hotkeyId] = value;
        }

        vm.BoxVisible = Visibility.Visible;

        var stack = GetNodePath( selectedNode );

        var path = "";
        foreach ( var node in stack )
        {
            if ( string.IsNullOrEmpty( path ) )
            {
                path = node.Header.ToString();
            }
            else
            {
                path += " > " + node.Header;
            }
        }

        vm.Path  = path!;
        vm.Extra = Const.Hotkey.GetHotkeyExtra( hotkeyId );

        if ( value.GhkCode == "" )
        {
            vm.LWin = vm.Ctrl = vm.Alt = vm.Shift = false;
            vm.Key  = Const.Hotkey.NONE;
            return;
        }

        var arr = value.GhkCode.Split( Const.Hotkey.SPLITTER );
        if ( arr.Length == 5 )
        {
            vm.LWin  = arr[0] != Const.Hotkey.NONE;
            vm.Ctrl  = arr[1] != Const.Hotkey.NONE;
            vm.Alt   = arr[2] != Const.Hotkey.NONE;
            vm.Shift = arr[3] != Const.Hotkey.NONE;

            vm.Key = arr[4];
        }
    }

    private static TreeViewItem CreateKbNode( string key, bool hidden = false )
    {
        return new TreeViewItem
        {
            Name       = key,
            Header     = Agent.Langs.GetString( key ),
            IsExpanded = true,
            Visibility = hidden ? Visibility.Collapsed : Visibility.Visible
        };
    }

    private static void AddKbChildren( TreeViewItem parent, params TreeViewItem[] children )
    {
        foreach ( var child in children )
        {
            parent.Items.Add( child );
        }
    }

    private void LoadKeyboardTreeView()
    {
        KeyboardTreeView.Items.Clear();

        var nodeGeneral = CreateKbNode( "K_G" );
        AddKbChildren( nodeGeneral,
            CreateKbNode( Const.Hotkey.RISE_VIEW ),
            CreateKbNode( Const.Hotkey.SHOW_APP_CONTROLLER ),
            CreateKbNode( Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP ),
            CreateKbNode( Const.Hotkey.RISE_VIEW_FOR_CURRENT_VD ),
            CreateKbNode( Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP_IN_CURRENT_VD ),
            CreateKbNode( Const.Hotkey.TOGGLE_WINDOW_FILTER, hidden: true ) );

        var nodeDesktopSwitch = CreateKbNode( "K_D_S" );
        var nodeDesktopNav    = CreateKbNode( "K_D_N" );
        AddKbChildren( nodeDesktopNav,
            CreateKbNode( Const.Hotkey.NAV_LEFT ),
            CreateKbNode( Const.Hotkey.NAV_RIGHT ),
            CreateKbNode( Const.Hotkey.NAV_UP ),
            CreateKbNode( Const.Hotkey.NAV_DOWN ) );

        var nodeDesktop = CreateKbNode( "K_D" );
        AddKbChildren( nodeDesktop, nodeDesktopSwitch, nodeDesktopNav );

        var nodeWindowMove          = CreateKbNode( "K_W_M" );
        var nodeWindowMoveAndFollow = CreateKbNode( "K_W_MF" );
        var nodeWindow              = CreateKbNode( "K_W" );
        AddKbChildren( nodeWindow, nodeWindowMove, nodeWindowMoveAndFollow );

        KeyboardTreeView.Items.Add( nodeGeneral );
        KeyboardTreeView.Items.Add( nodeDesktop );
        KeyboardTreeView.Items.Add( nodeWindow );

        for ( var i = 1; i <= DesktopWrapper.Count; i++ )
        {
            var item = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.SVD" ) + i,
                Name   = Const.Hotkey.SVD_TREE_NODE_PREFIX + i,
                Tag    = "KB.Hotkey.SVD"
            };
            nodeDesktopSwitch.Items.Add( item );

            var item2 = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.MW" ) + i,
                Name   = Const.Hotkey.MW_TREE_NODE_PREFIX + i,
                Tag    = "KB.Hotkey.MW"
            };
            nodeWindowMove.Items.Add( item2 );

            var item3 = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.MWF" ) + i,
                Name   = Const.Hotkey.MWF_TREE_NODE_PREFIX + i,
                Tag    = "KB.Hotkey.MWF"
            };
            nodeWindowMoveAndFollow.Items.Add( item3 );
        }

        var item4 = new TreeViewItem
        {
            Header = Agent.Langs.GetString( "KB.Hotkey.SVD_BACK_LAST" ),
            Name   = Const.Hotkey.SWITCH_BACK_LAST,
            Tag    = "KB.Hotkey.SVD_BACK_LAST"
        };
        nodeDesktopSwitch.Items.Add( item4 );
    }

    private static (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) GetGhk( KeyBindingModel kbm )
    {
        string ghkCode;
        var    kms = GlobalHotKey.KeyModifiers.None;

        string GenGhkCode( bool @checked, string code )
        {
            return ( @checked ? code : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
        }

        GlobalHotKey.KeyModifiers GenKm( bool @checked, GlobalHotKey.KeyModifiers km )
        {
            return @checked ? km : GlobalHotKey.KeyModifiers.None;
        }

        if ( string.IsNullOrEmpty( kbm.Key ) )
        {
            ghkCode = "";
        }
        else
        {
            ghkCode =  GenGhkCode( kbm.LWin, Const.Hotkey.WIN );
            ghkCode += GenGhkCode( kbm.Ctrl, Const.Hotkey.CTRL );
            ghkCode += GenGhkCode( kbm.Alt, Const.Hotkey.ALT );
            ghkCode += GenGhkCode( kbm.Shift, Const.Hotkey.SHIFT );

            kms =  GenKm( kbm.LWin, GlobalHotKey.KeyModifiers.WindowsKey );
            kms |= GenKm( kbm.Ctrl, GlobalHotKey.KeyModifiers.Ctrl );
            kms |= GenKm( kbm.Alt, GlobalHotKey.KeyModifiers.Alt );
            kms |= GenKm( kbm.Shift, GlobalHotKey.KeyModifiers.Shift );

            ghkCode += kbm.Key;
        }

        return new ValueTuple<string, GlobalHotKey.KeyModifiers>( ghkCode, kms );
    }

    private void SaveHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
    {
        var selectedItem = KeyboardTreeView.SelectedItem as TreeViewItem;
        var hotkeyId     = selectedItem!.Name;
        var kb           = Const.Hotkey.GetKeyBinding( hotkeyId );
        kb.GhkCode                             = ghk.keyCode;
        Manager.Configs.KeyBindings![hotkeyId] = kb;
        Manager.Save( reason: kb.GhkCode.Replace( Const.Hotkey.NONE + Const.Hotkey.SPLITTER, "" ), reasonName: hotkeyId );
        ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.SettingsSaved" )! );
    }

    private void RegHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
    {
        var selectedItem = KeyboardTreeView.SelectedItem as TreeViewItem;
        var hotkeyId     = selectedItem!.Name;
        var msgId        = Const.Hotkey.GetKeyBinding( hotkeyId ).MessageId;
        GlobalHotKey.UnregisterHotKey( MainWindow.MainWindowHandle, msgId );

        var vm = KeyBindingBox.DataContext as KeyBindingModel;

        if ( string.IsNullOrEmpty( vm?.Key ) || vm.Key == Const.Hotkey.NONE )
        {
            return;
        }

        if ( GlobalHotKey.RegHotKey( MainWindow.MainWindowHandle,
                msgId,
                ghk.keyModifiers,
                KeyInterop.VirtualKeyFromKey( Enum.Parse<Key>( vm.Key ) ) ) )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.Reg.Success" )! );
        }
        else
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.Reg.Fail" )! );
        }
    }

    private void RegAndSave_OnClick( object sender, RoutedEventArgs e )
    {
        if ( KeyBindingBox.DataContext is not KeyBindingModel vm )
        {
            return;
        }

        if ( !( vm.LWin | vm.Ctrl | vm.Alt | vm.Shift ) )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.MKeyCheck" )! );
            return;
        }

        if ( string.IsNullOrEmpty( vm.Key ) || vm.Key == Const.Hotkey.NONE )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.KeyCheck" )! );
            return;
        }

        var ghk = GetGhk( vm );
        RegHotkey( ghk );
        SaveHotkey( ghk );
    }

    private void ClearAndSave_OnClick( object sender, RoutedEventArgs e )
    {
        if ( KeyboardTreeView.SelectedItem is not TreeViewItem selectedItem )
        {
            return;
        }

        var hotkeyId = selectedItem.Name;

        var msgId = Const.Hotkey.GetKeyBinding( hotkeyId ).MessageId;
        GlobalHotKey.UnregisterHotKey( MainWindow.MainWindowHandle, msgId );
        var vm = KeyBindingBox.DataContext as KeyBindingModel;
        vm?.Clear();
        Manager.Configs.KeyBindings!.Remove( hotkeyId );
        Manager.Save( reason: "clear", reasonName: hotkeyId );
    }

    private void ShowTips( Snackbar sb, string msg, int seconds = 1 )
    {
        sb.MessageQueue?.Enqueue(
            msg,
            null,
            null,
            null,
            false,
            true,
            TimeSpan.FromSeconds( seconds ) );
    }
}