// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

namespace VirtualSpace.Helpers
{
    public static class UserMessage
    {
        public const int RiseView                        = 1000;
        public const int ShowAppController               = 1001;
        public const int AppControllerClosed             = 1002;
        public const int SwitchDesktop                   = 1003;
        public const int DesktopArrangement              = 1004;
        public const int RunAsAdministrator              = 1005;
        public const int RestartApp                      = 1006;
        public const int EnableMouseHook                 = 1007;
        public const int DisableMouseHook                = 1008;
        public const int RiseViewForActiveApp            = 1009;
        public const int RestartAppController            = 1010;
        public const int RiseViewForCurrentVD            = 1011;
        public const int RiseViewForActiveAppInCurrentVD = 1012;
        public const int RefreshVdw                      = 1013;
        public const int SwitchBackToLastDesktop         = 1014;
        public const int ShowVdw                         = 1015;
        public const int ShowThumbsOfVdw                 = 1016;
        public const int RefreshTrayIcon                 = 1017;
        public const int UpdateTrayLang                  = 1018;
        public const int ToggleWindowFilter              = 1019;

        public const int NavLeft  = 1201;
        public const int NavRight = 1202;
        public const int NavUp    = 1203;
        public const int NavDown  = 1204;

        public static class Meta
        {
            public const int SVD_START = 1100;
            public const int SVD_END   = 1200;
            public const int MW_START  = 1300;
            public const int MW_END    = 1400;
            public const int MWF_START = 1500;
            public const int MWF_END   = 1600;
        }
    }
}