/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cube3D
{
    public partial class MainWindow
    {
        private void NotificationGridLayout( int vdCount )
        {
            ////////////////////////////////////////////////////////////////
            // position and size
            var screenW = SystemParameters.PrimaryScreenWidth;
            var screenH = SystemParameters.PrimaryScreenHeight;
            var centerX = screenW / 2;
            var centerY = screenH * 5 / 16;
            NotifyContainer.Width = screenW / 6;
            NotifyContainer.Height = NotifyContainer.Width * 3 / 4;
            NotifyContainer.Margin = new Thickness
            {
                Top = centerY + NotifyContainer.Height
            };

            ////////////////////////////////////////////////////////////////
            // contents
            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
            var maxCount = rowsCols * rowsCols;
            if ( NotifyGrid.Children.Count != maxCount )
            {
                NotifyGrid.Children.Clear();
                for ( var i = 0; i < maxCount; i++ )
                {
                    NotifyGrid.Children.Add( new Button
                    {
                        // Content = i.ToString(),
                        Template = Resources["NotifyButtonTemplate"] as ControlTemplate
                    } );
                }
            }

            UpdateLayout();
            var firstCell = (Button)NotifyGrid.Children[0]; // 因为至少有一个桌面，所以 0 索引子元素必然存在
            var buttonMargin = new Thickness
            {
                Left = firstCell.ActualWidth / 10.0,
                Top = firstCell.ActualHeight / 10.0
            };

            Resources["NotifyButtonMargin"] = buttonMargin;
            NotifyBorder.Padding = new Thickness {Right = buttonMargin.Left, Bottom = buttonMargin.Top};

            UpdateLayout();

            CurrentIndicator.Width = firstCell.ActualWidth;
            CurrentIndicator.Height = firstCell.ActualHeight;
        }
    }
}