/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace.Config.Entity
{
    public class UserInterface
    {
        private byte _canvasOpacity;
        private int  _desktopArrangement;
        private byte _showVdIndexType;
        private int  _vDwBorderSize;
        private int  _vDwMargin;
        private int  _vDwPadding;

        public Colour? CanvasBackColor       { get; set; }
        public Colour? VDWDefaultBackColor   { get; set; }
        public Colour? VDWCurrentBackColor   { get; set; }
        public Colour? VDWHighlightBackColor { get; set; }
        public float   VDWDragTargetOpacity  { get; set; }
        public string  Language              { get; set; }
        public bool    ShowVdName            { get; set; } = true;
        public bool    ShowVdIndex           { get; set; } = true;

        public byte ShowVdIndexType
        {
            get => _showVdIndexType;
            set
            {
                if ( value == 0 || value == 1 )
                    _showVdIndexType = value;
                else
                    _showVdIndexType = 0;
            }
        }

        public int VDWPadding
        {
            get => _vDwPadding;
            set
            {
                if ( value >= 0 && value <= 50 )
                    _vDwPadding = value;
                else
                    _vDwPadding = 0;
            }
        }

        public int VDWBorderSize
        {
            get => _vDwBorderSize;
            set
            {
                if ( value >= 0 && value <= 50 )
                    _vDwBorderSize = value;
                else
                    _vDwBorderSize = 5;
            }
        }

        public int VDWMargin
        {
            get => _vDwMargin;
            set
            {
                if ( value >= 8 && value <= 50 )
                    _vDwMargin = value;
                else
                    _vDwMargin = 8;
            }
        }

        public byte CanvasOpacity
        {
            get => _canvasOpacity;
            set => _canvasOpacity = value > 0 ? value : (byte)1;
        }

        public Margin? ThumbMargin { get; set; }

        public byte ThumbDragSourceOpacity { get; set; }

        public int? DesktopArrangement
        {
            get => _desktopArrangement;
            set
            {
                _desktopArrangement = value switch
                {
                    null => 0,
                    >= 0 and <= 7 => (int)value,
                    _ => 0
                };
            }
        }
    }
}