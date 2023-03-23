/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace;

namespace ControlPanel.Pages;

public partial class Control
{
    private static Control? _instance;

    public Control()
    {
        InitializeComponent();
    }

    private Control( string headerKey, PackIconKind iconKind ) : this()
    {
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;

        KeyBindingBox.DataContext = new KeyBindingModel();
        DesktopMouseActionBox.DataContext = new MouseActionModel();
        WindowMouseActionBox.DataContext = new MouseActionModel();
        UseWheelSwitchDesktopWhenOnTaskbar.DataContext = WindowMouseActionBox.DataContext;

        LoadKeyboardTreeView();

        SettingsViewModel.LanguageChanged -= SettingsOnLanguageChanged;
        SettingsViewModel.LanguageChanged += SettingsOnLanguageChanged;
    }

    private void SettingsOnLanguageChanged( object? sender, EventArgs e )
    {
        foreach ( var item in KeyboardTreeView.Items )
        {
            var node = item as TreeViewItem;
            if ( node is null ) continue;

            var tag = node.Tag;
            node.Header = Agent.Langs.GetString( tag is null ? node.Name : tag.ToString() );

            VisitTreeViewItem( node );
        }
    }

    private static void VisitTreeViewItem( TreeViewItem item )
    {
        foreach ( var child in item.Items )
        {
            if ( child is not TreeViewItem childItem ) continue;

            var tag = childItem.Tag;
            if ( tag is null )
            {
                childItem.Header = Agent.Langs.GetString( childItem.Name );
            }
            else
            {
                var currentHeader = childItem.Header.ToString();
                var m             = Regex.Match( currentHeader, @"[^\d]+(\d+)$" );
                if ( m.Success )
                {
                    var index = m.Groups[1].Value;
                    childItem.Header = Agent.Langs.GetString( tag.ToString() ) + index;
                }
                else
                {
                    childItem.Header = Agent.Langs.GetString( tag.ToString() );
                }
            }

            if ( childItem.Items.Count > 0 )
                VisitTreeViewItem( childItem );
        }
    }

    public static Control Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Control( headerKey, iconKind );
    }
}