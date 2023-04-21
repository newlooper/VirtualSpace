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
    private RuleEditorWindow()
    {
        InitializeComponent();
        AddHandler( Button.ClickEvent, new RoutedEventHandler( ClickEventFromSubControl ) );
    }

    public static RuleEditorWindow Create( IntPtr handle )
    {
        var w = new RuleEditorWindow();

        w.RuleEditor.DataContext = new RuleTemplate
        {
            Id = Guid.Empty,
            Enabled = true,
            Action = new Behavior()
        };

        var sbTitle = new StringBuilder( Const.WindowTitleMaxLength );
        _ = User32.GetWindowText( handle, sbTitle, sbTitle.Capacity );
        w.RuleEditor.chb_Title.IsChecked = true;
        w.RuleEditor.tb_Title.Text = sbTitle.ToString();

        _ = User32.GetWindowThreadProcessId( handle, out var pId );
        var process = Process.GetProcessById( pId );
        w.RuleEditor.chb_ProcessName.IsChecked = true;
        w.RuleEditor.tb_ProcessName.Text = process.ProcessName;

        try
        {
            w.RuleEditor.tb_ProcessPath.Text = process.MainModule?.FileName;
        }
        catch ( Exception ex )
        {
            w.RuleEditor.chb_ProcessPath.IsChecked = false;
            w.RuleEditor.tb_ProcessPath.Text = ex.Message;
        }

        try
        {
            w.RuleEditor.tb_CommandLine.Text = process.GetCommandLineArgs();
        }
        catch ( Exception ex )
        {
            w.RuleEditor.chb_CommandLine.IsChecked = false;
            w.RuleEditor.tb_CommandLine.Text = ex.Message;
        }

        var sbCName = new StringBuilder( Const.WindowClassMaxLength );
        _ = User32.GetClassName( handle, sbCName, sbCName.Capacity );
        w.RuleEditor.tb_WndClass.Text = sbCName.ToString();

        var allScreens = Screen.AllScreens;
        var screen     = Screen.FromHandle( handle );
        for ( var i = 0; i < allScreens.Length; i++ )
        {
            if ( screen.DeviceName == allScreens[i].DeviceName )
            {
                w.RuleEditor.cbb_WinInScreen.SelectedValue = i;
                break;
            }
        }

        w.RuleEditor.RuleListItemsSource = RulesViewModel.Instance.Rules;
        w.RuleEditor.RuleDate.Visibility = Visibility.Hidden;

        return w;
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
}