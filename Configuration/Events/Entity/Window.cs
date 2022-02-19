/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;

namespace VirtualSpace.Config.Events.Entity
{
    public class Window
    {
        public IntPtr  Handle      { get; set; }
        public string  Title       { get; set; }
        public string  WndClass    { get; set; }
        public string  Screen      { get; set; }
        public int     VdIndex     { get; set; }
        public int?    ProcessId   { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessPath { get; set; }

        public static Window Create(
            IntPtr handle,
            string title,
            string wndClass,
            int    pId )
        {
            return new Window
            {
                Handle = handle,
                Title = title,
                WndClass = wndClass,
                ProcessId = pId
            };
        }
    }
}