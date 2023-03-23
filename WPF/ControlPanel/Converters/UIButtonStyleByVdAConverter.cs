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
using System.Windows.Media;

namespace ControlPanel.Converters;

public class UIButtonStyleByVdAConverter : IMultiValueConverter
{
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
        if ( parameter.ToString() == "B" )
        {
            var vda         = (int)values[0];
            var buttonIndex = int.Parse( values[1].ToString() );
            return vda == buttonIndex ? new SolidColorBrush( Colors.Pink ) : new SolidColorBrush( Colors.LightGray );
        }

        var count = (int)values[0];
        return GetFlag( count ) || values[1].ToString() == "0";
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
        throw new NotImplementedException();
    }

    private static bool GetFlag( int num )
    {
        var result = Math.Sqrt( num );
        return result % 1 == 0;
    }
}