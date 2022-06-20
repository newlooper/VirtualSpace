/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Drawing;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config.Profiles
{
    public class Default : Profile
    {
        public Default()
        {
            UI = new UserInterface
            {
                CanvasOpacity = 100,
                CanvasBackColor = new Colour {R = 55, G = 55, B = 55},
                VDWMargin = 8,
                VDWBorderSize = 1,
                VDWPadding = 0,
                VDWDefaultBackColor = new Colour {R = 55, G = 55, B = 55},
                VDWCurrentBackColor = new Colour {R = Color.Beige.R, G = Color.Beige.G, B = Color.Beige.B},
                VDWHighlightBackColor = new Colour {R = Color.Tomato.R, G = Color.Tomato.G, B = Color.Tomato.B},
                VDWDragTargetOpacity = 0.8f,
                ThumbMargin = new Margin {Top = 20, Left = 10},
                ThumbDragSourceOpacity = 150,
                Language = "en"
            };
            DaemonAutoStart = true;
            Mouse = new Mouse
            {
                DragSizeFactor = 10,
                LeftClickOnCanvas = 1,
                RightClickOnCanvas = 0,
                MiddleClickOnCanvas = 0
            };
            HideOnStartup = false;
            Navigation = new Navigation
            {
                CirculationH = false,
                CirculationV = false,
                CirculationHType = 0
            };
        }
    }
}