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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ControlPanel.ViewModels;
using VirtualSpace.Helpers;

namespace ControlPanel.Pages.Menus.Commons;

public partial class MenuContainer : UserControl
{
    public MenuContainer()
    {
        InitializeComponent();
        DataContext = new MenuContainerViewModel();
        ThemeSettings.DataContext = SettingsViewModel.GetInstance();
        CheckAdmin();
    }

    private void CloseWindow_OnClick( object sender, RoutedEventArgs e )
    {
        MainWindow.TryClose();
    }

    private void RestartApp_OnClick( object sender, RoutedEventArgs e )
    {
        if ( Application.Current is App )
        {
            MainWindow.RestartApp();
        }
        else
        {
            User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RestartApp, 0 );
        }
    }

    private void Shutdown_OnClick( object sender, RoutedEventArgs e )
    {
        if ( Application.Current is App )
        {
            MainWindow.TryQuit();
        }
        else
        {
            User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_CLOSE, 0, 0 );
        }
    }

    private void CheckAdmin()
    {
        if ( SysInfo.IsAdministrator )
        {
            SIID_SHIELD.Visibility = Visibility.Collapsed;
            menuItemRunAsAdmin.Visibility = Visibility.Collapsed;
            return;
        }

        var iconResult = new SHSTOCKICONINFO();
        iconResult.cbSize = (uint)Marshal.SizeOf( iconResult );

        _ = User32.SHGetStockIconInfo( SHSTOCKICONID.SIID_SHIELD, SHGSI.SHGSI_ICON | SHGSI.SHGSI_SMALLICON, ref iconResult );
        using var icon = Bitmap.FromHicon( iconResult.hIcon );
        icon.MakeTransparent();

        var iconSource      = Imaging.CreateBitmapSourceFromHBitmap( icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
        var writeableBitmap = new WriteableBitmap( iconSource );
        SIID_SHIELD.Source = writeableBitmap;

        SIID_SHIELD.Visibility = Visibility.Visible;
        menuItemRunAsAdmin.Visibility = Visibility.Visible;
    }

    private void MenuItemRunAsAdmin_OnClick( object sender, RoutedEventArgs e )
    {
        User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RunAsAdministrator, 0 );
    }
}