// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ControlPanel.Pages.Dialogs;
using MaterialDesignThemes.Wpf;
using PropertyChanged;
using VirtualSpace;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class GeneralViewModel : ViewModelBase
{
    public GeneralViewModel()
    {
        NavH = Manager.CurrentProfile.Navigation.CirculationH;
        NavV = Manager.CurrentProfile.Navigation.CirculationV;
        NavHType = Manager.CurrentProfile.Navigation.CirculationHType;
        RunOnStartup = TaskSchedulerHelper.IsTaskExistsByName( Const.AppName, Const.AppName );
        Cluster = new ClusterProxy( Manager.Configs.Cluster );
        Manager.ProfileChanged += UpdateClusterProxy;
        _isInitialized = true;
    }

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        var propertyChanged = PropertyChanged;
        if ( propertyChanged == null ) return;
        if ( _isInitialized )
        {
            switch ( propertyName )
            {
                case nameof( NavHType ):
                case nameof( NavH ):
                case nameof( NavV ):
                    Manager.CurrentProfile.Navigation.CirculationH = NavH;
                    Manager.CurrentProfile.Navigation.CirculationV = NavV;
                    Manager.CurrentProfile.Navigation.CirculationHType = NavHType;
                    Manager.Save( reason: Manager.CurrentProfile.Navigation );
                    break;
                case nameof( RunOnStartup ):
                    if ( RunOnStartup == TaskSchedulerHelper.IsTaskExistsByName( Const.AppName, Const.AppName ) ) break;
                    try
                    {
                        if ( RunOnStartup )
                        {
                            TaskSchedulerHelper.CreateAutoRunTask( Const.AppName, Manager.AppPath, Const.AppName );
                        }
                        else
                        {
                            TaskSchedulerHelper.DeleteTaskByName( Const.AppName, Const.AppName );
                        }
                    }
                    catch ( Exception e )
                    {
                        var view = new YesNoWithNote( Agent.Langs.GetString( e.Message ), PackIconKind.CloseOctagon );
                        DialogHost.Show( view, "GeneralDialog" );
                        RunOnStartup = !RunOnStartup;
                    }

                    break;
            }
        }

        propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public bool         NavH         { get; set; }
    public int          NavHType     { get; set; }
    public bool         NavV         { get; set; }
    public bool         RunOnStartup { get; set; }
    public ClusterProxy Cluster      { get; set; }

    public List<object> NavHTypeList { get; set; } = new()
    {
        new {Value = Const.VirtualDesktop.NavHTypeNextRow, Text = ""},
        new {Value = Const.VirtualDesktop.NavHTypeSameRow, Text = ""},
    };

    private void UpdateClusterProxy( object? sender, EventArgs eventArgs )
    {
        Cluster = new ClusterProxy( Manager.Configs.Cluster );
    }

    [AddINotifyPropertyChangedInterface]
    public partial class ClusterProxy : Cluster
    {
        private bool _isInitialized;

        public ClusterProxy( Cluster cluster )
        {
            _cluster = cluster;
            HideMainViewIfItsShown = cluster.HideMainViewIfItsShown;
            NotificationOnVdChanged = cluster.NotificationOnVdChanged;
            ShowVDIndexOnTrayIcon = cluster.ShowVDIndexOnTrayIcon;
            StyleOfVDIndexOnTrayIcon = cluster.StyleOfVDIndexOnTrayIcon;
            HideOnStart = cluster.HideOnStart;
            _isInitialized = true;
        }

        private readonly Cluster _cluster;
        public new       bool    HideMainViewIfItsShown   { get; set; }
        public new       bool    NotificationOnVdChanged  { get; set; }
        public new       bool    ShowVDIndexOnTrayIcon    { get; set; }
        public new       int     StyleOfVDIndexOnTrayIcon { get; set; }
        public new       bool    HideOnStart              { get; set; }

        public void OnPropertyChanged( string propertyName, object before, object after )
        {
            var propertyChanged = PropertyChanged;
            if ( propertyChanged == null ) return;

            if ( _isInitialized )
            {
                switch ( propertyName )
                {
                    case nameof( HideMainViewIfItsShown ):
                        _cluster.HideMainViewIfItsShown = HideMainViewIfItsShown;
                        break;
                    case nameof( NotificationOnVdChanged ):
                        _cluster.NotificationOnVdChanged = NotificationOnVdChanged;
                        break;
                    case nameof( ShowVDIndexOnTrayIcon ):
                    case nameof( StyleOfVDIndexOnTrayIcon ):
                        _cluster.ShowVDIndexOnTrayIcon = ShowVDIndexOnTrayIcon;
                        _cluster.StyleOfVDIndexOnTrayIcon = StyleOfVDIndexOnTrayIcon;

                        User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RefreshTrayIcon, 0 );
                        break;
                    case nameof( HideOnStart ):
                        _cluster.HideOnStart = HideOnStart;
                        break;
                }

                Manager.Save( reason: Manager.Configs.Cluster );
            }

            propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}