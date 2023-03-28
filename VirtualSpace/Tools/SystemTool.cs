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
using System.Windows;

namespace VirtualSpace.Tools
{
    public static class SystemTool
    {
        public static bool VersionCheck()
        {
            var version = Environment.OSVersion.Version;
            if ( version is {Major: >= 10, Build: >= 17763 and < 25000} )
            {
                return true;
            }

            MessageBox.Show( Agent.Langs.GetString( "VersionCheckFail" ), @"Error" );
            return false;
        }
    }
}