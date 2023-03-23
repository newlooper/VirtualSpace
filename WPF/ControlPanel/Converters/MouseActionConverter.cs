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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;
using VirtualSpace.Config;

namespace ControlPanel.Converters;

public class MouseActionConverter : IMultiValueConverter
{
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
        if ( values.Length == 0 ) return null;

        var prefix = values[0].ToString() == MouseAction.MOUSE_NODE_DESKTOP_PREFIX ? MouseAction.MOUSE_NODE_DESKTOP_PREFIX : MouseAction.MOUSE_NODE_WINDOW_PREFIX;
        var mks    = Keys.None;
        if ( (bool)values[1] ) mks |= Keys.LWin;
        if ( (bool)values[2] ) mks |= Keys.Control;
        if ( (bool)values[3] ) mks |= Keys.Alt;
        if ( (bool)values[4] ) mks |= Keys.Shift;

        var mb      = values[5];
        var keyCode = ( (int)mks ).ToString( "X2" );

        var maId = prefix + keyCode + MouseAction.KEY_SPLITTER + mb;

        if ( Manager.Configs.MouseActions.ContainsKey( maId ) )
        {
            return Manager.Configs.MouseActions[maId].ToString();
        }

        return MouseAction.Action.DoNothing.ToString();
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
        throw new NotImplementedException();
    }
}