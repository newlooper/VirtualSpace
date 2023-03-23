/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using VirtualSpace.Config.DataAnnotations;

namespace VirtualSpace.Config.Entity
{
    public class Colour
    {
        [PropertyProtector( 55, 1 )] public byte R { get; set; }
        [PropertyProtector( 55, 1 )] public byte G { get; set; }
        [PropertyProtector( 55, 1 )] public byte B { get; set; }

        public uint GetLongOfColor()
        {
            return (uint)( R * 0x10000 + G * 0x100 + B );
        }
    }
}