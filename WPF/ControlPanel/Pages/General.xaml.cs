/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Windows;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Helpers;

namespace ControlPanel.Pages;

public partial class General
{
    private static General?         _instance;
    private static GeneralViewModel _vm;

    public General()
    {
        InitializeComponent();
    }

    private General( string headerKey, PackIconKind iconKind ) : this()
    {
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;
        _vm = new GeneralViewModel();
        DataContext = _vm;
    }

    public static General Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new General( headerKey, iconKind );
    }

    private void OpenTaskScheduler_OnClick( object sender, RoutedEventArgs e )
    {
        TaskSchedulerHelper.OpenWinTaskScheduler();
    }
}