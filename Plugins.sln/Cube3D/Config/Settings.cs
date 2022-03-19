// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using Cube3D.Effects;

namespace Cube3D
{
    public class Settings
    {
        private int _animationDuration;

        private int _checkAliveInterval;

        public int AnimationDuration
        {
            get => _animationDuration;
            set
            {
                if ( value < 500 || value > 1000 )
                {
                    _animationDuration = 500;
                }
                else
                {
                    _animationDuration = value;
                }
            }
        }

        public int CheckAliveInterval
        {
            get => _checkAliveInterval;
            set
            {
                if ( value < 1 || value > 60 )
                {
                    _checkAliveInterval = 10;
                }
                else
                {
                    _checkAliveInterval = value;
                }
            }
        }

        public EffectType SelectedEffect { get; set; }
    }
}