// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Windows;
using System.Windows.Controls;
using ControlPanel.ViewModels;

namespace ControlPanel.Pages.Menus;

public partial class LogsMenu : UserControl
{
    public LogsMenu()
    {
        InitializeComponent();
        DataContext = new LogsViewModel();
    }

    private void clearAll_OnClick( object sender, RoutedEventArgs e )
    {
        Logs.ClearAll();
    }

    private void openLogsDir_OnClick( object sender, RoutedEventArgs e )
    {
        Logs.OpenLogsDir();
    }
}