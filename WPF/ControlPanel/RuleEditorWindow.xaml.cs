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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using ControlPanel.ViewModels;
using VirtualSpace.Config;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Helpers;
using Button = System.Windows.Controls.Button;

namespace ControlPanel;

public partial class RuleEditorWindow
{
    private static RuleEditorWindow? _instance;

    private RuleEditorWindow()
    {
        InitializeComponent();
        AddHandler( Button.ClickEvent, new RoutedEventHandler( ClickEventFromSubControl ) );
    }

    public static RuleEditorWindow Create( IntPtr handle )
    {
        _instance ??= new RuleEditorWindow();

        _instance.RuleEditor.DataContext = new RuleTemplate
        {
            Id = Guid.Empty,
            Enabled = true,
            Action = new Behavior()
        };

        var sbTitle = new StringBuilder( Const.WindowTitleMaxLength );
        _ = User32.GetWindowText( handle, sbTitle, sbTitle.Capacity );
        _instance.RuleEditor.chb_Title.IsChecked = true;
        _instance.RuleEditor.tb_Title.Text = sbTitle.ToString();

        _ = User32.GetWindowThreadProcessId( handle, out var pId );
        var process = Process.GetProcessById( pId );
        _instance.RuleEditor.chb_ProcessName.IsChecked = true;
        _instance.RuleEditor.tb_ProcessName.Text = process.ProcessName;

        try
        {
            _instance.RuleEditor.tb_ProcessPath.Text = process.MainModule?.FileName;
        }
        catch ( Exception ex )
        {
            _instance.RuleEditor.chb_ProcessPath.IsChecked = false;
            _instance.RuleEditor.tb_ProcessPath.Text = ex.Message;
        }

        try
        {
            _instance.RuleEditor.tb_CommandLine.Text = process.GetCommandLineArgs();
        }
        catch ( Exception ex )
        {
            _instance.RuleEditor.chb_CommandLine.IsChecked = false;
            _instance.RuleEditor.tb_CommandLine.Text = ex.Message;
        }

        var sbCName = new StringBuilder( Const.WindowClassMaxLength );
        _ = User32.GetClassName( handle, sbCName, sbCName.Capacity );
        _instance.RuleEditor.tb_WndClass.Text = sbCName.ToString();

        var allScreens = Screen.AllScreens;
        var screen     = Screen.FromHandle( handle );
        for ( var i = 0; i < allScreens.Length; i++ )
        {
            if ( screen.DeviceName == allScreens[i].DeviceName )
            {
                _instance.RuleEditor.cbb_WinInScreen.SelectedValue = i;
                break;
            }
        }

        _instance.RuleEditor.RuleListItemsSource = RulesViewModel.Instance.Rules;
        _instance.RuleEditor.RuleDate.Visibility = Visibility.Hidden;

        return _instance;
    }

    private void ClickEventFromSubControl( object sender, RoutedEventArgs e )
    {
        if ( e.OriginalSource is Button btn )
        {
            switch ( btn.Name )
            {
                case "btnSave":
                case "btnCloseDefBox":

                    e.Handled = true;
                    Close();
                    break;
            }
        }
    }

    private void RuleEditorWindow_OnClosing( object? sender, CancelEventArgs e )
    {
        e.Cancel = true;
        Hide();
    }
}