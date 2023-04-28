/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ControlPanel.Factories;
using ControlPanel.Pages;
using VirtualSpace;
using VirtualSpace.Config;
using VirtualSpace.VirtualDesktop.Api;
using WPFLocalizeExtension.Engine;

namespace ControlPanel;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IAppController
{
    private static MainWindow _instance;
    private static IntPtr     _handle;

    private IntPtr _mainWindowHandle;

    public MainWindow()
    {
        InitializeComponent();

        _instance = this;

        Title = Const.Window.VS_CONTROLLER_TITLE;
        Topmost = true;

        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        LocalizeDictionary.Instance.Culture = new CultureInfo( Manager.CurrentProfile.UI.Language );
        NavBarItem.InitNavBar( NavBar );

        new WindowInteropHelper( this ).EnsureHandle();
    }

    public void ForceLoad()
    {
        ShowInTaskbar = false;
        Left = Const.FakeHideX;
        Top = Const.FakeHideY;
        Show();
        Hide();
        ShowInTaskbar = true;
    }

    protected override void OnSourceInitialized( EventArgs e )
    {
        base.OnSourceInitialized( e );
        _handle = new WindowInteropHelper( this ).EnsureHandle();
    }

    public static IntPtr MainWindowHandle => _instance._mainWindowHandle;

    public void BringToTop()
    {
        Left = ( SystemParameters.PrimaryScreenWidth - Width ) / 2;
        Top = ( SystemParameters.PrimaryScreenHeight - Height ) / 2;

        if ( WindowState == WindowState.Minimized )
        {
            WindowState = WindowState.Normal;
        }

        if ( IsVisible )
        {
            DesktopWrapper.MoveWindowToDesktop( _handle, DesktopWrapper.CurrentIndex );
        }
        else
        {
            Show();
        }

        Topmost = false;
        Topmost = true;
    }

    public void SetMainWindowHandle( IntPtr handle )
    {
        _mainWindowHandle = handle;
    }

    public void Quit()
    {
        Closing -= MainWindow_OnClosing;
        Close();
    }

    public void RenderDesktopArrangementButtons( string selectedDa )
    {
        // throw new NotImplementedException();
    }

    public void CreateRuleFromWindowHandle( IntPtr handle )
    {
        RuleEditorWindow.Create( handle ).ShowDialog();
    }

    private void MainWindow_OnLoaded( object sender, RoutedEventArgs e )
    {
        InitTheme();
        PickLogAndWrite( _cancelTokenSourceForLog.Token );
    }

    private void MainWindow_OnClosing( object? sender, CancelEventArgs e )
    {
        if ( Application.Current is App ) return;
        e.Cancel = true;
        Hide();
    }

    public static void TryClose()
    {
        _instance.Close();
    }

    public static void TryQuit()
    {
        _instance.Quit();
    }

    public static void RestartApp( bool runas = false )
    {
        var psi = new ProcessStartInfo
        {
            FileName = Environment.ProcessPath,
            UseShellExecute = true
        };

        if ( runas ) psi.Verb = "runas";

        try
        {
            Process.Start( psi );
            Application.Current.Shutdown();
        }
        catch
        {
            //
        }
    }

    private void NavBar_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        var tab = (TabControl)sender;
        if ( tab.SelectedIndex == -1 ) return;
        var selectedTab = (TabItem)tab.SelectedItem;
        ContentFrame.Content = PageFactory.GetPage( NavBarItem.NavBarItemsInfo[selectedTab.Tag.ToString()] );
    }

    private void SettingsButton_OnClick( object sender, RoutedEventArgs e )
    {
        NavBar.SelectedIndex = -1;
        ContentFrame.Content = Settings.Create();
    }
}