/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace.Config
{
    public static class Const
    {
        public const string RuleFileExt            = ".rules";
        public const string ProfilesFolder         = "Profiles";
        public const string CacheFolder            = "Cache";
        public const string SettingsFile           = "settings.json";
        public const string DefaultVersion         = "1.0";
        public const string DefaultLogLevel        = "EVENT";
        public const int    WindowTitleMaxLength   = 2048;
        public const int    WindowClassMaxLength   = 512;
        public const int    WindowCheckTimeout     = 10 * 1000;
        public const int    WindowCheckInterval    = 200;
        public const int    WindowCloseDelay       = 300;
        public const int    RiseViewInterval       = 500;
        public const int    DefaultDpi             = 96;
        public const string ApplicationFrameWindow = "ApplicationFrameWindow";
        public const string WindowsUiCoreWindow    = "Windows.UI.Core.CoreWindow";
    }
}