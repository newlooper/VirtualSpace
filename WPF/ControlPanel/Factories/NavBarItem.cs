// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using WPFLocalizeExtension.Extensions;

namespace ControlPanel.Factories;

public class NavBarItem : TabItem
{
    public static readonly Dictionary<string, (PackIconKind kind, string locKey)> NavBarItemsInfo = new()
    {
        {"General", ( PackIconKind.MonitorDashboard, "NavBar.General" )},
        {"UI", ( PackIconKind.TableAccount, "NavBar.UI" )},
        {"Control", ( PackIconKind.CursorPointer, "NavBar.Control" )},
        {"Rules", ( PackIconKind.BookOpenPageVariant, "NavBar.Rules" )},
        {"Plugins", ( PackIconKind.ToyBrickMarkerOutline, "NavBar.Plugins" )},
        {"Logs", ( PackIconKind.BookSearchOutline, "NavBar.Logs" )},
        {"About", ( PackIconKind.HelpBox, "NavBar.About" )},
    };

    private NavBarItem( string tag, PackIconKind kind, string locKey )
    {
        var stackPanel = new StackPanel
        {
            Width = double.NaN,
            Height = double.NaN
        };
        var packIcon = new PackIcon
        {
            Width = 24,
            Height = 24,
            HorizontalAlignment = HorizontalAlignment.Center,
            Kind = kind
        };
        var tb = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center
        };

        new LocExtension( locKey ).SetBinding( tb, TextBlock.TextProperty );

        stackPanel.Children.Add( packIcon );
        stackPanel.Children.Add( tb );
        Header = stackPanel;
        Tag = tag;
    }

    public static void InitNavBar( TabControl tc )
    {
        foreach ( var kv in NavBarItemsInfo )
            tc.Items.Add( new NavBarItem( kv.Key, kv.Value.kind, kv.Value.locKey ) );
    }
}