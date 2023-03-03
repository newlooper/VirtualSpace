/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Threading.Channels;
using VirtualSpace.Config.Events.Entity;

namespace VirtualSpace.Commons
{
    public static class Channels
    {
        public static readonly Channel<Behavior>                   ActionChannel               = Channel.CreateUnbounded<Behavior>();
        public static readonly Channel<Window>                     VisibleWindowsChannel       = Channel.CreateUnbounded<Window>();
        public static readonly Channel<VirtualDesktopNotification> VirtualDesktopNotifications = Channel.CreateUnbounded<VirtualDesktopNotification>();
    }
}