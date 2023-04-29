/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/


using System.Windows;
using VirtualSpace.Config;
using VirtualSpace.VirtualDesktop;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private void ToggleWindowFilter()
        {
            if ( !IsShowing() ) return;

            var filterRow = Canvas.RowDefinitions[1];
            if ( filterRow.Height.Value == 0 )
            {
                filterRow.Height = new GridLength( Const.Window.WINDOW_FILTER_BAR_HEIGHT, GridUnitType.Pixel );
                ShowFilterWindow();
            }
            else
            {
                filterRow.Height = new GridLength( 0 );
                HideFilterWindow( clearKeyword: false );
            }

            UpdateLayout();
            VirtualDesktopManager.ShowAllVirtualDesktops();
        }

        private void ShowFilterWindow()
        {
            var wf = WindowFilter.GetInstance( _instance.Handle );
            wf.Width = Width;
            wf.Left = Left;
            wf.Top = Height - Const.Window.WINDOW_FILTER_BAR_HEIGHT;
            wf.Show();
            wf.SetFocus();
        }

        private static void HideFilterWindow( bool clearKeyword = true )
        {
            var filterRow = _instance.Canvas.RowDefinitions[1];
            filterRow.Height = new GridLength( 0 );

            var wf = WindowFilter.GetInstance( _instance.Handle );
            wf.ClearAndHide( clearKeyword );
        }
    }
}