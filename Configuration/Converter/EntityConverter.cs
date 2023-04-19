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
using System.Linq;

namespace VirtualSpace.Config.Converter
{
    public static class EntityConverter
    {
        public static void ConvertMouseAction( Dictionary<string, MouseAction.Action> oldFormat, Dictionary<string, MouseAction.Action> newFormat )
        {
            var prefix      = string.Empty;
            var combined    = string.Empty;
            var modifier    = string.Empty;
            var mouseButton = string.Empty;

            foreach ( var (maId, ma) in oldFormat )
            {
                if ( maId.StartsWith( MouseAction.MOUSE_NODE_DESKTOP_PREFIX ) )
                {
                    prefix = MouseAction.MOUSE_NODE_DESKTOP_PREFIX;
                    combined = maId[MouseAction.MOUSE_NODE_DESKTOP_PREFIX.Length..];
                }
                else if ( maId.StartsWith( MouseAction.MOUSE_NODE_WINDOW_PREFIX ) )
                {
                    prefix = MouseAction.MOUSE_NODE_WINDOW_PREFIX;
                    combined = maId[MouseAction.MOUSE_NODE_WINDOW_PREFIX.Length..];
                }

                if ( combined.Contains( MouseAction.KEY_SPLITTER ) )
                {
                    var arrMK = combined.Split( MouseAction.KEY_SPLITTER );
                    var key   = MouseAction.KeysName.Single( x => x.Value == arrMK[0] ).Key;
                    modifier = ( (int)key ).ToString( "X2" );
                    mouseButton = arrMK[1];
                }
                else
                {
                    modifier = MouseAction.NoneKeyCode;
                    mouseButton = combined;
                }

                newFormat.Add( prefix + modifier + MouseAction.KEY_SPLITTER + mouseButton, ma );
            }
        }
    }
}