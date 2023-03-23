// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.ComponentModel;
using PropertyChanged;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class LogsViewModel : ViewModelBase
{
    public LogsViewModel()
    {
        IsPrintLogs = Manager.Configs.LogConfig.ShowLogsInGui;
        _isInitialized = true;
    }

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        var propertyChanged = PropertyChanged;
        if ( propertyChanged == null ) return;
        if ( _isInitialized )
        {
            Logger.ShowLogsInGui = (bool)after;
            Manager.Configs.LogConfig.ShowLogsInGui = Logger.ShowLogsInGui;
            Manager.Save( reason: Manager.Configs.LogConfig.ShowLogsInGui );
        }

        propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public bool IsPrintLogs { get; set; }
}