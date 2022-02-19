/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace.Config.Entity
{
    public class Colour
    {
        private const byte MIN     = 1;
        private const byte DEFAULT = 55;
        private       byte _b;
        private       byte _g;
        private       byte _r;

        public byte R
        {
            get => _r;
            set => _r = value >= MIN ? value : DEFAULT;
        }

        public byte G
        {
            get => _g;
            set => _g = value >= MIN ? value : DEFAULT;
        }

        public byte B
        {
            get => _b;
            set => _b = value >= MIN ? value : DEFAULT;
        }

        public uint GetLongOfColor()
        {
            return (uint)( R * 0x10000 + G * 0x100 + B );
        }
    }
}