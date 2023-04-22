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
using System.IO;
using System.Reflection;
using System.Text.Json;
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
        vm.BoxVisible = Visibility.Hidden;

        var selectedNode = e.NewValue as TreeViewItem;
        if ( selectedNode is null ) return;

        var kbInConfig = Manager.Configs.KeyBindings;
        var hotkeyId   = selectedNode.Name;

        if ( !kbInConfig.ContainsKey( hotkeyId ) )
        {
            var kb = Const.Hotkey.GetKeyBinding( hotkeyId );
            if ( kb.MessageId == 0 ) return;

            kbInConfig[hotkeyId] = kb;
        }

        vm.BoxVisible = Visibility.Visible;

        var stack = GetNodePath( selectedNode );

        var path = "";
        foreach ( var node in stack )
        {
            if ( string.IsNullOrEmpty( path ) )
                path = node.Header.ToString();
            else
            {
                path += " > " + node.Header;
            }
        }

        vm.Path = path;
        vm.Extra = Const.Hotkey.GetHotkeyExtra( hotkeyId );

        if ( kbInConfig[hotkeyId].GhkCode == "" )
        {
            vm.LWin = vm.Ctrl = vm.Alt = vm.Shift = false;
            vm.Key = Const.Hotkey.NONE;
            return;
        }

        var arr = kbInConfig[hotkeyId].GhkCode.Split( Const.Hotkey.SPLITTER );
        if ( arr.Length == 5 )
        {
            vm.LWin = arr[0] != Const.Hotkey.NONE;
            vm.Ctrl = arr[1] != Const.Hotkey.NONE;
            vm.Alt = arr[2] != Const.Hotkey.NONE;
            vm.Shift = arr[3] != Const.Hotkey.NONE;

            vm.Key = arr[4];
        }
    }

    private void LoadKeyboardTreeView()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name     = assembly.GetName().Name;

        using var stream = assembly.GetManifestResourceStream( $"{name}.Resources.Definitions.KeyboardTree.json" );
        using var reader = new StreamReader( stream );
        var       result = reader.ReadToEnd();

        KeyboardTreeView.Items.Clear();

        BuildTreeView( KeyboardTreeView, JsonDocument.Parse( result ), new ValueTuple<string, string, string, string>( "Name", "Header", "Tag", "Nodes" ) );

        var nodeDesktop       = KeyboardTreeView.Items[1] as TreeViewItem;
        var nodeDesktopSwitch = nodeDesktop.Items[0] as TreeViewItem;

        var nodeWindow              = KeyboardTreeView.Items[2] as TreeViewItem;
        var nodeWindowMove          = nodeWindow.Items[0] as TreeViewItem;
        var nodeWindowMoveAndFollow = nodeWindow.Items[1] as TreeViewItem;

        for ( var i = 1; i <= DesktopWrapper.Count; i++ )
        {
            var item = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.SVD" ) + i,
                Name = Const.Hotkey.SVD_TREE_NODE_PREFIX + i,
                Tag = "KB.Hotkey.SVD"
            };
            nodeDesktopSwitch.Items.Add( item );

            var item2 = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.MW" ) + i,
                Name = Const.Hotkey.MW_TREE_NODE_PREFIX + i,
                Tag = "KB.Hotkey.MW"
            };
            nodeWindowMove.Items.Add( item2 );

            var item3 = new TreeViewItem
            {
                Header = Agent.Langs.GetString( "KB.Hotkey.MWF" ) + i,
                Name = Const.Hotkey.MWF_TREE_NODE_PREFIX + i,
                Tag = "KB.Hotkey.MWF"
            };
            nodeWindowMoveAndFollow.Items.Add( item3 );
        }

        var item4 = new TreeViewItem
        {
            Header = Agent.Langs.GetString( "KB.Hotkey.SVD_BACK_LAST" ),
            Name = Const.Hotkey.SWITCH_BACK_LAST,
            Tag = "KB.Hotkey.SVD_BACK_LAST"
        };
        nodeDesktopSwitch.Items.Add( item4 );
    }

    private (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) GetGhk( KeyBindingModel kbm )
    {
        string ghkCode;
        var    km = GlobalHotKey.KeyModifiers.None;

        if ( string.IsNullOrEmpty( kbm.Key ) )
        {
            ghkCode = "";
        }
        else
        {
            ghkCode = ( kbm.LWin ? Const.Hotkey.WIN : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
            ghkCode += ( kbm.Ctrl ? Const.Hotkey.CTRL : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
            ghkCode += ( kbm.Alt ? Const.Hotkey.ALT : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;
            ghkCode += ( kbm.Shift ? Const.Hotkey.SHIFT : Const.Hotkey.NONE ) + Const.Hotkey.SPLITTER;

            km = kbm.LWin ? GlobalHotKey.KeyModifiers.WindowsKey : GlobalHotKey.KeyModifiers.None;
            km |= kbm.Ctrl ? GlobalHotKey.KeyModifiers.Ctrl : GlobalHotKey.KeyModifiers.None;
            km |= kbm.Alt ? GlobalHotKey.KeyModifiers.Alt : GlobalHotKey.KeyModifiers.None;
            km |= kbm.Shift ? GlobalHotKey.KeyModifiers.Shift : GlobalHotKey.KeyModifiers.None;

            ghkCode += kbm.Key;
        }

        return new ValueTuple<string, GlobalHotKey.KeyModifiers>( ghkCode, km );
    }

    private void SaveHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
    {
        var selectedItem = KeyboardTreeView.SelectedItem as TreeViewItem;
        var hotkeyId     = selectedItem.Name;
        var kb           = Const.Hotkey.GetKeyBinding( hotkeyId );
        kb.GhkCode = ghk.keyCode;
        Manager.Configs.KeyBindings[hotkeyId] = kb;
        Manager.Save( reason: kb.GhkCode.Replace( Const.Hotkey.NONE + Const.Hotkey.SPLITTER, "" ), reasonName: hotkeyId );
        ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.SettingsSaved" ) );
    }

    private void RegHotkey( (string keyCode, GlobalHotKey.KeyModifiers keyModifiers) ghk )
    {
        var selectedItem = KeyboardTreeView.SelectedItem as TreeViewItem;
        var hotkeyId     = selectedItem.Name;
        var msgId        = Const.Hotkey.GetKeyBinding( hotkeyId ).MessageId;
        GlobalHotKey.UnregisterHotKey( MainWindow.MainWindowHandle, msgId );

        var vm = KeyBindingBox.DataContext as KeyBindingModel;

        if ( string.IsNullOrEmpty( vm.Key ) || vm.Key == Const.Hotkey.NONE )
        {
            return;
        }

        if ( GlobalHotKey.RegHotKey( MainWindow.MainWindowHandle,
                msgId,
                ghk.keyModifiers,
                KeyInterop.VirtualKeyFromKey( Enum.Parse<Key>( vm.Key ) ) ) )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.Reg.Success" ) );
        }
        else
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.Reg.Fail" ) );
        }
    }

    private void RegAndSave_OnClick( object sender, RoutedEventArgs e )
    {
        var vm = KeyBindingBox.DataContext as KeyBindingModel;
        if ( vm == null ) return;

        if ( ( vm.LWin | vm.Ctrl | vm.Alt | vm.Shift ) == false )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.MKeyCheck" ) );
            return;
        }

        if ( string.IsNullOrEmpty( vm.Key ) || vm.Key == Const.Hotkey.NONE )
        {
            ShowTips( Snackbar, Agent.Langs.GetString( "KB.Hotkey.KeyCheck" ) );
            return;
        }

        var ghk = GetGhk( vm );
        RegHotkey( ghk );
        SaveHotkey( ghk );
    }

    private void ClearAndSave_OnClick( object sender, RoutedEventArgs e )
    {
        var selectedItem = KeyboardTreeView.SelectedItem as TreeViewItem;
        if ( selectedItem == null ) return;
        var hotkeyId = selectedItem.Name;

        var msgId = Const.Hotkey.GetKeyBinding( hotkeyId ).MessageId;
        GlobalHotKey.UnregisterHotKey( MainWindow.MainWindowHandle, msgId );
        var vm = KeyBindingBox.DataContext as KeyBindingModel;
        vm.Clear();
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