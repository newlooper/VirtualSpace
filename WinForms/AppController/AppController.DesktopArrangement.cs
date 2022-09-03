/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Forms;
using VirtualSpace.Commons;
using VirtualSpace.Helpers;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void tlp_DesktopArrangement_SubControlClicked( object sender, EventArgs e )
        {
            var btn        = sender as Button;
            var selectedDa = btn.Name[^1..];

            Config.Manager.CurrentProfile.UI.DesktopArrangement = Int32.Parse( selectedDa );
            Config.Manager.Save();

            User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.DesktopArrangement, 0 );
        }

        public static void CheckDesktopArrangement( string selectedDa )
        {
            foreach ( var c in _instance.tlp_DesktopArrangement.Controls )
            {
                var i = c as Button;
                i.BackColor = i.Name.EndsWith( selectedDa ) ? System.Drawing.Color.MistyRose : System.Drawing.Color.Gainsboro;
            }
        }
    }
}