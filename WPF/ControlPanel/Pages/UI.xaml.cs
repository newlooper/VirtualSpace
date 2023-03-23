/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Windows;
using System.Windows.Controls;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Helpers;

namespace ControlPanel.Pages;

public partial class UI
{
    private static UI?         _instance;
    private static UIViewModel _vm;

    public UI()
    {
        InitializeComponent();
    }

    private UI( string headerKey, PackIconKind iconKind ) : this()
    {
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;
        _vm = new UIViewModel();
        DataContext = _vm;
        ButtonsContainer.AddHandler( Button.ClickEvent, new RoutedEventHandler( OnVdArrangementButtonClicked ) );
    }

    private static void OnVdArrangementButtonClicked( object sender, RoutedEventArgs e )
    {
        var btn = (Button)e.OriginalSource;

        _vm.VdArrangement = int.Parse( btn.Tag.ToString() );

        User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.DesktopArrangement, 0 );

        e.Handled = true;
    }

    public static UI Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new UI( headerKey, iconKind );
    }
}