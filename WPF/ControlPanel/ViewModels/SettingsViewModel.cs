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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using PropertyChanged;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using WPFLocalizeExtension.Engine;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class SettingsViewModel : ViewModelBase
{
    private static SettingsViewModel? _instance;

    private SettingsViewModel()
    {
        Theme = Manager.CurrentProfile.UI.Theme;
        Language = Manager.CurrentProfile.UI.Language;
        CurrentProfile = Manager.Configs.CurrentProfileName;
        ProfileList = new ObservableCollection<object>();
        foreach ( var profileName in Manager.Configs.Profiles.Keys )
        {
            ProfileList.Add( new {Value = profileName} );
        }

        _isInitialized = true;
    }

    public string ConfigRootPath { get; set; } = Manager.ConfigRootFolder;

    public ObservableCollection<object> ProfileList    { get; set; }
    public string                       CurrentProfile { get; set; }

    public int    Theme    { get; set; }
    public string Language { get; set; }

    public static SettingsViewModel GetInstance()
    {
        return _instance ??= new SettingsViewModel();
    }

    public static event EventHandler? LanguageChanged;

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        var propertyChanged = PropertyChanged;
        if ( propertyChanged == null ) return;

        if ( _isInitialized )
        {
            switch ( propertyName )
            {
                case nameof( Theme ):
                    Manager.CurrentProfile.UI.Theme = (int)after;
                    Manager.Save( reason: Manager.CurrentProfile.UI.Theme );
                    break;
                case nameof( CurrentProfile ):
                    if ( string.IsNullOrEmpty( after.ToString() ) || before == after ) break;

                    Manager.SwitchProfile( after.ToString() );
                    RulesViewModel.ReloadRules();
                    Theme = Manager.CurrentProfile.UI.Theme;
                    Language = Manager.CurrentProfile.UI.Language;

                    break;
                case nameof( Language ):
                    var lang = after.ToString();
                    LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                    LocalizeDictionary.Instance.Culture = new CultureInfo( lang );
                    Manager.CurrentProfile.UI.Language = lang;
                    Manager.Save( reason: Manager.CurrentProfile.UI.Language );
                    User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.UpdateTrayLang, 0 );
                    LanguageChanged?.Invoke( null, EventArgs.Empty );
                    break;
            }
        }

        propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );

        if ( _isInitialized )
        {
            switch ( propertyName )
            {
                case nameof( Theme ):
                    if ( before != after )
                        MainWindow.UpdateTheme();
                    break;
            }
        }
    }
}