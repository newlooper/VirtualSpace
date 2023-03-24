// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Data;
using Cube3D.Config;

namespace VirtualSpace.Helpers
{
    public class TransitionTypeConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameters, System.Globalization.CultureInfo culture )
        {
            if ( value is null ) return null;

            var t = (TransitionType)value;

            return ( t & TransitionType.NotificationGridOnly ) > 0;
        }

        public object ConvertBack( object value, Type targetType, object parameters, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}