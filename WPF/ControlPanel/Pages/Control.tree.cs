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
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VirtualSpace;

namespace ControlPanel.Pages;

public partial class Control
{
    private static Stack<TreeViewItem> GetNodePath( UIElement element, bool includeSelf = true )
    {
        var path = new Stack<TreeViewItem>();
        var tvi  = element as TreeViewItem;

        if ( includeSelf )
            path.Push( tvi );

        while ( element != null )
        {
            element = VisualTreeHelper.GetParent( element ) as UIElement;
            tvi = element as TreeViewItem;
            if ( tvi != null )
                path.Push( tvi );
        }

        return path;
    }

    private static void BuildTreeView( object node, object jsonDoc, (string Name, string Header, string Tag, string Nodes) keys )
    {
        switch ( node )
        {
            case TreeView treeView:
                var nodes = ( (JsonDocument)jsonDoc ).RootElement.GetProperty( keys.Nodes );
                foreach ( var child in nodes.EnumerateArray() )
                {
                    var topLevelNode = new TreeViewItem
                    {
                        Header = Agent.Langs.GetString( child.GetProperty( keys.Header ).GetString() ),
                        IsExpanded = true
                    };

                    if ( child.TryGetProperty( keys.Name, out var name ) )
                        topLevelNode.Name = name.GetString();

                    if ( child.TryGetProperty( keys.Nodes, out var subNodes ) )
                    {
                        BuildTreeView( topLevelNode, subNodes, keys );
                    }

                    treeView.Items.Add( topLevelNode );
                }

                break;
            case TreeViewItem treeViewItem:
                foreach ( var child in ( (JsonElement)jsonDoc ).EnumerateArray() )
                {
                    var subNode = new TreeViewItem
                    {
                        Header = Agent.Langs.GetString( child.GetProperty( keys.Header ).GetString() ),
                        IsExpanded = true
                    };

                    if ( child.TryGetProperty( keys.Name, out var name ) )
                        subNode.Name = name.GetString();

                    if ( child.TryGetProperty( keys.Nodes, out var subNodes ) )
                    {
                        BuildTreeView( subNode, subNodes, keys );
                    }

                    treeViewItem.Items.Add( subNode );
                }

                break;
        }
    }
}