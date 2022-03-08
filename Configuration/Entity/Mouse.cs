// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

namespace VirtualSpace.Config.Entity
{
    public class Mouse
    {
        private int _dragSizeFactor;
        public  int LeftClickOnCanvas   { get; set; }
        public  int RightClickOnCanvas  { get; set; }
        public  int MiddleClickOnCanvas { get; set; }

        public int DragSizeFactor
        {
            get => _dragSizeFactor;
            set => _dragSizeFactor = value is < 1 or > 100 ? 10 : value;
        }
    }
}