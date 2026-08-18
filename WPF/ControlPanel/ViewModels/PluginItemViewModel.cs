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
using System.Windows.Input;
using PropertyChanged;
using VirtualSpace.Plugin;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PluginItemViewModel : ViewModelBase
{
    private readonly PluginInfo _info;

    public PluginItemViewModel( PluginInfo info )
    {
        _info = info;
        SyncFrom( info );

        SettingsCommand = new RelayCommand( () => PluginHost.PluginSettings( _info ) );
        RestartCommand = new RelayCommand( () =>
        {
            _info.AutoStart = true;
            PluginHost.RestartPlugin( _info );
            PluginManager.SavePluginInfo( _info );
            RefreshLoadState();
        } );
        CloseCommand = new RelayCommand( () =>
        {
            _info.AutoStart = false;
            PluginHost.ClosePlugin( _info );
            PluginManager.SavePluginInfo( _info );
            RefreshLoadState();
        } );
        _isInitialized = true;
    }

    public string Name        { get; set; } = string.Empty;
    public string Version     { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author      { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string LoadStatus      { get; set; } = string.Empty;
    public bool   IsLoaded        { get; set; }
    public bool   ShowCloseButton { get; set; }

    public ICommand SettingsCommand { get; }
    public ICommand RestartCommand  { get; }
    public ICommand CloseCommand    { get; }

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        if ( _isInitialized && propertyName == nameof( IsLoaded ) && (bool)after != (bool)before )
            ApplyLoadState( (bool)after );

        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public void SyncFrom( PluginInfo info )
    {
        Name        = info.Display;
        Version     = info.Version;
        Description = info.Description;
        Author      = info.Author;
        Email       = info.Email;
        LoadStatus      = info.LoadStatus.ToString();
        IsLoaded        = info.IsLoaded;
        ShowCloseButton = info.Kind == PluginKind.ExternalProcess;
    }

    public void RefreshLoadState()
    {
        var wasInit = _isInitialized;
        _isInitialized = false;
        SyncFrom( _info );
        _isInitialized = wasInit;
    }

    private void ApplyLoadState( bool load )
    {
        _info.AutoStart = load;
        if ( load )
            PluginHost.StartPlugin( _info );
        else
            PluginHost.ClosePlugin( _info );

        PluginManager.SavePluginInfo( _info );
        RefreshLoadState();
    }
}
