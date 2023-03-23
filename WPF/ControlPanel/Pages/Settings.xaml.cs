/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using ControlPanel.Pages.Dialogs;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace;
using VirtualSpace.Config;
using VirtualSpace.Config.Events.Expression;

namespace ControlPanel.Pages;

public partial class Settings
{
    private static Settings? _instance;

    private Settings()
    {
        InitializeComponent();

        DataContext = SettingsViewModel.GetInstance();
    }

    public static Settings Create()
    {
        return _instance ??= new Settings();
    }

    private void ChangeConfigPath_OnClick( object sender, RoutedEventArgs e )
    {
        using var fbd = new FolderBrowserDialog();

        if ( fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
        {
            var vm = DataContext as SettingsViewModel;
            vm.ConfigRootPath = fbd.SelectedPath;
            Manager.SetConfigRoot( fbd.SelectedPath );
        }
    }

    private async void ProfileClone_OnClick( object sender, RoutedEventArgs e )
    {
        var newProfileName = cbbProfiles.Text + " (copy)";

        var isValid = !string.IsNullOrEmpty( newProfileName ) &&
                      newProfileName.IndexOfAny( Path.GetInvalidFileNameChars() ) < 0 &&
                      !File.Exists( Path.Combine( Manager.ProfileFolder, newProfileName ) );

        if ( Manager.Configs.Profiles.ContainsKey( newProfileName ) || !isValid )
        {
            var view = new YesNoWithNote( Agent.Langs.GetString( "Profile.Warning.InvalidProfileName" ), PackIconKind.CloseOctagon );
            await DialogHost.Show( view, "ProfileDialog" );
            return;
        }

        CreateProfile( newProfileName );
    }

    private void CreateProfile( string profileName )
    {
        var newProfile   = Manager.CurrentProfile.Clone();
        var currentRules = Conditions.FetchRules();

        Manager.Configs.Profiles.Add( profileName, newProfile );
        Manager.Configs.CurrentProfileName = profileName;
        Manager.Save( reason: "Create", reasonName: "Profile" );

        Manager.SaveCluster( Manager.Configs.Cluster ); // save cluster for new profile
        Conditions.SaveRules( currentRules ); // save rules for new profile

        RulesViewModel.ReloadRules();

        var vm = DataContext as SettingsViewModel;
        vm.ProfileList.Add( new {Value = profileName} );
        vm.CurrentProfile = profileName;
    }

    private async void ProfileRename_OnClick( object sender, RoutedEventArgs e )
    {
        if ( Manager.Configs.Profiles.Count == 1 )
        {
            var view = new YesNoWithNote( Agent.Langs.GetString( "Profile.Warning.LastProfileProtect" ), PackIconKind.CloseOctagon );
            await DialogHost.Show( view, "ProfileDialog" );
            return;
        }

        var pndView = new ProfileNameDialog( cbbProfiles.Text );
        await DialogHost.Show( pndView, "ProfileDialog", null, ClosingEventHandler, null );
    }

    private async void ProfileRemove_OnClick( object sender, RoutedEventArgs e )
    {
        YesNoWithNote view;
        if ( Manager.Configs.Profiles.Count == 1 )
        {
            view = new YesNoWithNote( Agent.Langs.GetString( "Profile.Warning.LastProfileProtect" ), PackIconKind.CloseOctagon );
            await DialogHost.Show( view, "ProfileDialog" );
            return;
        }

        view = new YesNoWithNote( Agent.Langs.GetString( "Profile.Confirm.Delete" ), PackIconKind.Warning );
        var result = await DialogHost.Show( view, "ProfileDialog" );
        if ( result is false ) return;

        var delProfile = cbbProfiles.Text;

        Manager.Configs.Profiles.Remove( delProfile );
        Manager.Configs.CurrentProfileName = Manager.Configs.Profiles.Keys.Last();
        Manager.Save( reason: "Delete", reasonName: "Profile" );
        Manager.DeleteFilesOfProfile( delProfile );

        var vm = DataContext as SettingsViewModel;
        vm.CurrentProfile = Manager.Configs.CurrentProfileName;
        vm.ProfileList.Remove( vm.ProfileList.First( x => ( (dynamic)x ).Value == delProfile ) );
    }

    private void ClosingEventHandler( object sender, DialogClosingEventArgs eventArgs )
    {
        eventArgs.Handled = true;
        if ( eventArgs.Parameter is false ) return;

        var pndView = eventArgs.Session.Content as ProfileNameDialog;
        if ( pndView == null ) return;

        var oldName = cbbProfiles.SelectedValue.ToString();

        var newName = pndView.EditProfileName;
        if ( newName == oldName ) return;

        var isValid = !string.IsNullOrEmpty( newName ) &&
                      newName.IndexOfAny( Path.GetInvalidFileNameChars() ) < 0 &&
                      !File.Exists( Path.Combine( Manager.ProfileFolder, newName ) );

        if ( Manager.Configs.Profiles.ContainsKey( newName ) || !isValid )
        {
            pndView.SetErrors( Agent.Langs.GetString( "Profile.Warning.InvalidProfileName" ) );
            eventArgs.Cancel();
            return;
        }

        CreateProfile( newName );

        Manager.Configs.Profiles.Remove( oldName );
        Manager.Save( reason: "Delete", reasonName: "Profile" );
        Manager.DeleteFilesOfProfile( oldName );

        var vm = DataContext as SettingsViewModel;
        vm.ProfileList.Remove( vm.ProfileList.First( x => ( (dynamic)x ).Value == oldName ) );
    }
}