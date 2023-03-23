/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;

namespace VirtualSpace.Config.Events.Entity
{
    public class Behavior
    {
        public IntPtr Handle;
        public string RuleName;
        public string WindowTitle;
        public int    MoveToDesktop { get; set; } = -1;
        public bool   FollowWindow  { get; set; } = true;
        public bool   PinWindow     { get; set; }
        public bool   PinApp        { get; set; }
        public int    MoveToScreen  { get; set; } = -1;
        public bool   HideFromView  { get; set; }
        
        public Behavior Clone()
        {
            return new Behavior
            {
                Handle = Handle,
                RuleName = RuleName,
                WindowTitle = WindowTitle,
                MoveToDesktop = MoveToDesktop,
                FollowWindow = FollowWindow,
                PinWindow = PinWindow,
                PinApp = PinApp,
                MoveToScreen = MoveToScreen,
                HideFromView = HideFromView
            };
        }
    }
}