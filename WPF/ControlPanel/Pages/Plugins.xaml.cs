/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;

namespace ControlPanel.Pages;

public partial class Plugins
{
    private static Plugins? _instance;

    private Plugins()
    {
        InitializeComponent();
    }

    private Plugins( string headerKey, PackIconKind iconKind ) : this()
    {
        var vm = new PluginsViewModel();
        PluginsContent.DataContext = vm;
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind  = iconKind;
    }

    public static Plugins Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Plugins( headerKey, iconKind );
    }
}
