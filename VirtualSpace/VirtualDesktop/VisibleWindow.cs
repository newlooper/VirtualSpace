/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop
{
    public class VisibleWindow
    {
        public VisibleWindow( string title, string classname, IntPtr handle )
        {
            Title = title;
            Classname = classname;
            Handle = handle;
        }

        public string Title { get; set; }

        public string Classname { get; set; }

        public IntPtr Handle { get; set; }

        public Rectangle Rect { get; set; }

        public IntPtr Thumb { get; set; }

        internal DWM_THUMBNAIL_PROPERTIES DTP { get; set; }

        internal void SetValidArea( DWM_THUMBNAIL_PROPERTIES props )
        {
            DTP = props;
            Rect = new Rectangle
            {
                X = props.rcDestination.Left,
                Y = props.rcDestination.Top,
                Width = props.rcDestination.Right - props.rcDestination.Left,
                Height = props.rcDestination.Bottom - props.rcDestination.Top
            };
        }
    }
}