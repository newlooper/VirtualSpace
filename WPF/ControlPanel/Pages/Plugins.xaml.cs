/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Plugin;

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
        PluginsList.DataContext = new PluginsViewModel();
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;
    }

    public static Plugins Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Plugins( headerKey, iconKind );
    }

    private void ToggleButton_OnChecked( object sender, RoutedEventArgs e )
    {
        var chb        = sender as CheckBox;
        var name       = chb.CommandParameter as string;
        var pvm        = PluginsList.DataContext as PluginsViewModel;
        var pluginInfo = pvm.Plugins.First( p => p.Name == name );

        pluginInfo.AutoStart = true;
        PluginHost.StartPlugin( pluginInfo );
        PluginManager.SavePluginInfo( pluginInfo );
    }

    private void ToggleButton_OnUnchecked( object sender, RoutedEventArgs e )
    {
        var chb        = sender as CheckBox;
        var name       = chb.CommandParameter as string;
        var pvm        = PluginsList.DataContext as PluginsViewModel;
        var pluginInfo = pvm.Plugins.First( p => p.Name == name );

        pluginInfo.AutoStart = false;
        PluginHost.ClosePlugin( pluginInfo );
        PluginManager.SavePluginInfo( pluginInfo );
    }

    private void Selector_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        var lb = sender as ListBox;
        if ( lb.SelectedIndex == -1 ) return;
        var lbi        = lb.SelectedItem as ListBoxItem;
        var name       = lbi.Tag as string;
        var pvm        = PluginsList.DataContext as PluginsViewModel;
        var pluginInfo = pvm.Plugins.First( p => p.Name == name );

        switch ( lb.SelectedIndex )
        {
            case 0:
                PluginHost.PluginSettings( pluginInfo );
                break;
            case 1:
                PluginHost.RestartPlugin( pluginInfo );
                break;
            case 2:
                PluginHost.ClosePlugin( pluginInfo );
                break;
        }

        lb.SelectedIndex = -1;
    }
}