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
using MaterialDesignThemes.Wpf;

namespace ControlPanel.ViewModels;

public class MenuContainerViewModel : ViewModelBase
{
    public MenuContainerViewModel()
    {
        _isInitialized = true;
    }

    public string       HeaderKey { get; set; } = "";
    public PackIconKind IconKind  { get; set; }

    public UserControl? InjectContent { get; set; }
}