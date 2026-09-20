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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using ControlPanel.ViewModels;
using VirtualSpace.Config;
using ComboBox = System.Windows.Controls.ComboBox;
using ListView = System.Windows.Controls.ListView;

namespace ControlPanel.Pages;

public partial class Control
{
    private void DesktopActionBind_OnClick( object sender, RoutedEventArgs e )
    {
        MouseActionBind( DesktopMouseActionBox.DataContext as MouseActionModel,
            MouseAction.MOUSE_NODE_DESKTOP_PREFIX,
            cbbDesktopActions.SelectedValue.ToString()!,
            DesktopBindingList,
            cbbDesktopActions );
    }

    private void WindowActionBind_OnClick( object sender, RoutedEventArgs e )
    {
        MouseActionBind( WindowMouseActionBox.DataContext as MouseActionModel,
            MouseAction.MOUSE_NODE_WINDOW_PREFIX,
            cbbWindowActions.SelectedValue.ToString()!,
            WindowBindingList,
            cbbWindowActions );
    }

    private static void MouseActionBind( MouseActionModel? vm, string prefix, string actionName, ListView list, ComboBox actionCombo )
    {
        if ( vm is null )
        {
            return;
        }

        var mks = Keys.None;
        if ( vm.LWin )
        {
            mks |= Keys.LWin;
        }

        if ( vm.Ctrl )
        {
            mks |= Keys.Control;
        }

        if ( vm.Alt )
        {
            mks |= Keys.Alt;
        }

        if ( vm.Shift )
        {
            mks |= Keys.Shift;
        }

        var mb      = vm.MouseButton;
        var keyCode = ( (int)mks ).ToString( "X2" );
        var maId    = prefix + keyCode + MouseAction.KEY_SPLITTER + mb;

        var action = (MouseAction.Action)Enum.Parse( typeof( MouseAction.Action ), actionName );

        Manager.Configs.MouseActions?[maId] = action;
        Manager.Save( reason: action, reasonName: maId );

        var existing = vm.Bindings.FirstOrDefault( b => b.Id == maId );
        if ( existing is null )
        {
            if ( !MouseBindingItem.TryCreate( maId, action, out var item ) )
            {
                return;
            }

            vm.Bindings.Add( item );
            list.SelectedItem = item;
            list.ScrollIntoView( item );
        }
        else
        {
            existing.ActionName = action.ToString();
            list.SelectedItem   = existing;
            list.ScrollIntoView( existing );
        }

        RefreshActionCombo( actionCombo );
    }

    private void MouseActionUnbind_OnClick( object sender, RoutedEventArgs e )
    {
        if ( sender is not FrameworkElement { DataContext: MouseBindingItem item } )
        {
            return;
        }

        if ( DesktopMouseActionBox.DataContext is MouseActionModel desktopVm &&
             desktopVm.Bindings.Contains( item ) )
        {
            RemoveBinding( desktopVm, item.Id, cbbDesktopActions );
            return;
        }

        if ( WindowMouseActionBox.DataContext is MouseActionModel windowVm &&
             windowVm.Bindings.Contains( item ) )
        {
            RemoveBinding( windowVm, item.Id, cbbWindowActions );
        }
    }

    private static void RemoveBinding( MouseActionModel vm, string maId, ComboBox actionCombo )
    {
        if ( !Manager.Configs.MouseActions!.Remove( maId ) &&
             vm.Bindings.All( b => b.Id != maId ) )
        {
            return;
        }

        var existing = vm.Bindings.FirstOrDefault( b => b.Id == maId );
        if ( existing is not null )
        {
            vm.Bindings.Remove( existing );
        }

        Manager.Save( reason: maId, reasonName: "MouseAction.Unbind" );
        RefreshActionCombo( actionCombo );
    }

    private void MouseBindingList_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        if ( sender is not ListView { SelectedItem: MouseBindingItem item, DataContext: MouseActionModel vm } )
        {
            return;
        }

        if ( !MouseAction.TryParseActionId( item.Id, out _, out var keys, out var mouseButton ) )
        {
            return;
        }

        vm.LWin        = ( keys & Keys.LWin ) == Keys.LWin;
        vm.Ctrl        = ( keys & Keys.Control ) == Keys.Control;
        vm.Alt         = ( keys & Keys.Alt ) == Keys.Alt;
        vm.Shift       = ( keys & Keys.Shift ) == Keys.Shift;
        vm.MouseButton = mouseButton;
    }

    private static void RefreshActionCombo( ComboBox actionCombo )
    {
        BindingOperations.GetMultiBindingExpression( actionCombo, Selector.SelectedValueProperty )?.UpdateTarget();
    }
}
