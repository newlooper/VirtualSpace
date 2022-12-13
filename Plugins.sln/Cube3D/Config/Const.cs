// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Cube3D.
// 
// Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.

namespace Cube3D.Config
{
    public static class Const
    {
        public const double FakeHideX                 = -10000.0;
        public const double FakeHideY                 = -10000.0;
        public const int    CaptureInitTimer          = 50;
        public const int    AnimationDurationMin      = 100;
        public const int    AnimationDurationMax      = 1000;
        public const int    CheckAliveIntervalMin     = 1;
        public const int    CheckAliveIntervalMax     = 60;
        public const int    CheckAliveIntervalDefault = 10;
        public const string PluginInfoFile            = "plugin.json";
        public const string PluginSettingFile         = "settings.json";
        public const string Front                     = nameof( Front );
        public const string Others                    = nameof( Others );
    }
}