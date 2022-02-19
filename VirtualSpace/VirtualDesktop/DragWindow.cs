/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;

namespace VirtualSpace.VirtualDesktop
{
    public partial class DragWindow : Form
    {
        private DragWindow()
        {
            InitializeComponent();
        }

        public IntPtr Thumb { get; set; }

        public static DragWindow CreateAndShow( int width, int height )
        {
            var dw = new DragWindow();
            dw.TopLevel = true;
            dw.TopMost = true;
            dw.ShowInTaskbar = false;
            dw.Width = width;
            dw.Height = height;
            dw.Show();
            return dw;
        }
    }
}