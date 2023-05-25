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
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.VirtualDesktop.Api;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class UIViewModel : ViewModelBase
{
    public UIViewModel()
    {
        VdArrangement = (int)Manager.CurrentProfile.UI.DesktopArrangement!;
        VdCount = DesktopWrapper.Count;
        ShowVdName = Manager.CurrentProfile.UI.ShowVdName;
        ShowVdIndex = Manager.CurrentProfile.UI.ShowVdIndex;
        ShowVdIndexType = Manager.CurrentProfile.UI.ShowVdIndexType;
        _isInitialized = true;

        DesktopManagerWrapper.DesktopCreatedEvent -= OnDesktopCreatedEvent;
        DesktopManagerWrapper.DesktopCreatedEvent += OnDesktopCreatedEvent;
        DesktopManagerWrapper.DesktopDeletedEvent -= OnDesktopDeletedEvent;
        DesktopManagerWrapper.DesktopDeletedEvent += OnDesktopDeletedEvent;
    }

    private void OnDesktopCreatedEvent()
    {
        VdArrangement = (int)Manager.CurrentProfile.UI.DesktopArrangement!;
        VdCount = DesktopWrapper.Count;
    }

    private void OnDesktopDeletedEvent( VirtualDesktopNotification vdn )
    {
        VdArrangement = (int)Manager.CurrentProfile.UI.DesktopArrangement!;
        VdCount = DesktopWrapper.Count;
    }

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        var propertyChanged = PropertyChanged;
        if ( propertyChanged == null ) return;
        if ( _isInitialized )
        {
            switch ( propertyName )
            {
                case nameof( VdArrangement ):
                    Manager.CurrentProfile.UI.DesktopArrangement = (int)after;
                    Manager.Save( reason: Manager.CurrentProfile.UI.DesktopArrangement );
                    break;
                case nameof( ShowVdName ):
                    Manager.CurrentProfile.UI.ShowVdName = (bool)after;
                    Manager.Save( reason: Manager.CurrentProfile.UI.ShowVdName );
                    break;
                case nameof( ShowVdIndex ):
                    Manager.CurrentProfile.UI.ShowVdIndex = (bool)after;
                    Manager.Save( reason: Manager.CurrentProfile.UI.ShowVdIndex );
                    break;
                case nameof( ShowVdIndexType ):
                    Manager.CurrentProfile.UI.ShowVdIndexType = (int)after;
                    Manager.Save( reason: Manager.CurrentProfile.UI.ShowVdIndexType );
                    break;
            }
        }

        propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public bool ShowVdName      { get; set; }
    public bool ShowVdIndex     { get; set; }
    public int  ShowVdIndexType { get; set; }
    public int  VdArrangement   { get; set; }
    public int  VdCount         { get; set; }
}