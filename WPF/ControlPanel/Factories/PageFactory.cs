// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Windows.Controls;
using ControlPanel.Pages;
using MaterialDesignThemes.Wpf;
using Control = ControlPanel.Pages.Control;

namespace ControlPanel.Factories;

public static class PageFactory
{
    public static UserControl GetPage( (PackIconKind kind, string locKey) info )
    {
        return info.locKey switch
        {
            "NavBar.General" => General.Create( info.locKey, info.kind ),
            "NavBar.UI" => UI.Create( info.locKey, info.kind ),
            "NavBar.Rules" => Rules.Create( info.locKey, info.kind ),
            "NavBar.Control" => Control.Create( info.locKey, info.kind ),
            "NavBar.Plugins" => Plugins.Create( info.locKey, info.kind ),
            "NavBar.Logs" => Logs.Create( info.locKey, info.kind ),
            "NavBar.About" => Help.Instance,
            "NavBar.Settings" => Settings.Create(),
            _ => General.Create( info.locKey, info.kind )
        };
    }
}