// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of VirtualSpace.
//
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using VirtualSpace.Plugin;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PluginsViewModel : ViewModelBase
{
    public PluginsViewModel()
    {
        Plugins        = new ObservableCollection<PluginItemViewModel>( PluginHost.Plugins.Select( p => new PluginItemViewModel( p ) ) );
        RefreshCommand = new RelayCommand( Refresh );
        _isInitialized = true;
    }

    public ObservableCollection<PluginItemViewModel> Plugins        { get; set; }
    public ICommand                                  RefreshCommand { get; }

    public void Refresh()
    {
        PluginHost.RefreshPlugins();
        Plugins.Clear();
        foreach ( var item in PluginHost.Plugins.Select( p => new PluginItemViewModel( p ) ) )
            Plugins.Add( item );
    }
}
