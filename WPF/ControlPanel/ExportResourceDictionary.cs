// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of ControlPanel.
// 
// ControlPanel is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// ControlPanel is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with ControlPanel. If not, see <https://www.gnu.org/licenses/>.

using System.Windows.Markup;

[assembly: XmlnsDefinition( "http://newlooper.com/virtualspace/share", "ControlPanel" )]

namespace ControlPanel;

public partial class ExportResourceDictionary
{
    public static ExportResourceDictionary Instance { get; } = new ExportResourceDictionary();

    public ExportResourceDictionary()
    {
        InitializeComponent();
    }
}