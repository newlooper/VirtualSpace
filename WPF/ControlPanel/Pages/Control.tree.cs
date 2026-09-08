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
using System.Windows.Media;

namespace ControlPanel.Pages;

public partial class Control
{
    private static Stack<TreeViewItem> GetNodePath( UIElement element, bool includeSelf = true )
    {
        var path = new Stack<TreeViewItem>();
        var tvi  = element as TreeViewItem;

        if ( includeSelf )
        {
            path.Push( tvi! );
        }

        while ( element != null )
        {
            element = (UIElement)VisualTreeHelper.GetParent( element );
            tvi     = element as TreeViewItem;
            if ( tvi != null )
            {
                path.Push( tvi );
            }
        }

        return path;
    }
}