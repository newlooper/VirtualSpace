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
using System.Windows.Forms;
using ControlPanel.ViewModels;
using VirtualSpace.Config;

namespace ControlPanel.Pages;

public partial class Control
{
    private void DesktopActionBind_OnClick( object sender, RoutedEventArgs e )
    {
        MouseActionBind( DesktopMouseActionBox.DataContext as MouseActionModel,
            MouseAction.MOUSE_NODE_DESKTOP_PREFIX,
            cbbDesktopActions.SelectedValue.ToString() );
    }

    private void WindowActionBind_OnClick( object sender, RoutedEventArgs e )
    {
        MouseActionBind( WindowMouseActionBox.DataContext as MouseActionModel,
            MouseAction.MOUSE_NODE_WINDOW_PREFIX,
            cbbWindowActions.SelectedValue.ToString() );
    }

    private void MouseActionBind( MouseActionModel vm, string prefix, string actionName )
    {
        var mks = Keys.None;
        if ( vm.LWin ) mks |= Keys.LWin;
        if ( vm.Ctrl ) mks |= Keys.Control;
        if ( vm.Alt ) mks |= Keys.Alt;
        if ( vm.Shift ) mks |= Keys.Shift;

        var mb      = vm.MouseButton;
        var keyCode = ( (int)mks ).ToString( "X2" );
        var maId    = prefix + keyCode + MouseAction.KEY_SPLITTER + mb;

        var action = (MouseAction.Action)Enum.Parse( typeof( MouseAction.Action ), actionName );

        Manager.Configs.MouseActions[maId] = action;
        Manager.Save( reason: action, reasonName: maId );
    }
}