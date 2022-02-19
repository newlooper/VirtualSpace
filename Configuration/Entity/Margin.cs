/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace.Config.Entity
{
    public class Margin
    {
        private const int MIN     = 0;
        private const int MAX     = 50;
        private const int DEFAULT = 10;

        private int _bottom;

        private int _left;

        private int _right;

        private int _top;

        public Margin()
        {
        }

        public Margin( int all )
        {
            Top = all;
            Right = all;
            Bottom = all;
            Left = all;
        }

        public int Top
        {
            get => _top;
            set
            {
                if ( value >= MIN && value <= MAX )
                    _top = value;
                else
                    _top = DEFAULT;
            }
        }

        public int Right
        {
            get => _right;
            set
            {
                if ( value >= MIN && value <= MAX )
                    _right = value;
                else
                    _right = DEFAULT;
            }
        }

        public int Bottom
        {
            get => _bottom;
            set
            {
                if ( value >= MIN && value <= MAX )
                    _bottom = value;
                else
                    _bottom = DEFAULT;
            }
        }

        public int Left
        {
            get => _left;
            set
            {
                if ( value >= MIN && value <= MAX )
                    _left = value;
                else
                    _left = DEFAULT;
            }
        }
    }
}