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
using System.Text.RegularExpressions;

namespace VirtualSpace.Helpers
{
    public static class StringHelper
    {
        public static bool IsValidRegex( string pattern )
        {
            if ( string.IsNullOrWhiteSpace( pattern ) ) return false;

            try
            {
                _ = Regex.Match( "", pattern );
            }
            catch ( ArgumentException )
            {
                return false;
            }

            return true;
        }
    }
}