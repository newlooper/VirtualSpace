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

namespace ControlPanel.Validation;

public abstract class Helper
{
    public static bool HasError( DependencyProperty dp, params FrameworkElement[] controls )
    {
        foreach ( var control in controls )
        {
            var bd = control.GetBindingExpression( dp );
            bd?.UpdateSource();
            if ( bd?.ValidationError != null ) return true;
        }

        return false;
    }
}