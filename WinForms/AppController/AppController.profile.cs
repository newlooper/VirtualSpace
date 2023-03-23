// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VirtualSpace.Config.Events.Expression;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private string _editProfileName = string.Empty;

        private void InitProfileList()
        {
            cbb_profiles.Items.Clear();
            AddToComboBox( ConfigManager.Configs.Profiles.Keys.ToArray(), ConfigManager.Configs.CurrentProfileName );
            cbb_profiles.KeyDown += cbb_profilesOnKeyDown;
            cbb_profiles.KeyUp += cbb_profilesOnKeyUp;
            cbb_profiles.LostFocus += cbb_profilesOnLostFocus;
        }

        private void AddToComboBox( string[] profileNames, string profileName )
        {
            cbb_profiles.SelectedIndexChanged -= cbb_profilesOnSelectedIndexChanged;
            cbb_profiles.Items.AddRange( profileNames );
            cbb_profiles.SelectedItem = profileName;
            cbb_profiles.SelectedIndexChanged += cbb_profilesOnSelectedIndexChanged;
        }

        private void cbb_profilesOnKeyDown( object? sender, KeyEventArgs e )
        {
            switch ( e.KeyCode )
            {
                case Keys.Down or Keys.Up or Keys.Enter or Keys.Return:
                    if ( cbb_profiles.DropDownStyle == ComboBoxStyle.Simple )
                    {
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void cbb_profilesOnKeyUp( object? sender, KeyEventArgs e )
        {
            if ( e.KeyCode is not (Keys.Enter or Keys.Return) ) return;
            NameProfile();
            e.Handled = true;
        }

        private void cbb_profilesOnLostFocus( object? sender, EventArgs e )
        {
            if ( cbb_profiles.DropDownStyle == ComboBoxStyle.Simple )
            {
                cbb_profiles.DropDownStyle = ComboBoxStyle.DropDownList;
                cbb_profiles.SelectedItem = _editProfileName;
            }
        }

        private void NameProfile()
        {
            if ( _editProfileName == string.Empty || _editProfileName == cbb_profiles.Text )
            {
                goto RESET;
            }

            var newProfileName = cbb_profiles.Text;
            var isValid = !string.IsNullOrEmpty( newProfileName ) &&
                          newProfileName.IndexOfAny( Path.GetInvalidFileNameChars() ) < 0 &&
                          !File.Exists( Path.Combine( ConfigManager.ProfileFolder, newProfileName ) );

            if ( ConfigManager.Configs.Profiles.ContainsKey( newProfileName ) || !isValid )
            {
                MessageBox.Show( Agent.Langs.GetString( "Profile.Warning.InvalidProfileName" ),
                    Agent.Langs.GetString( "MsgBox.Caption.Warning" ),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
                goto RESET;
            }

            var index = cbb_profiles.Items.IndexOf( _editProfileName );
            RemoveProfile( _editProfileName );
            CreateProfile( cbb_profiles.Text );
            cbb_profiles.Items[index] = cbb_profiles.Text;
            cbb_profiles.SelectedItem = newProfileName;

            RESET:

            cbb_profiles.DropDownStyle = ComboBoxStyle.DropDownList;
            _editProfileName = string.Empty;
        }

        private void cbb_profilesOnSelectedIndexChanged( object? sender, EventArgs e )
        {
            if ( cbb_profiles.DropDownStyle == ComboBoxStyle.Simple && cbb_profiles.Items.Contains( cbb_profiles.Text ) ) return;

            var selectedProfile = cbb_profiles.SelectedItem.ToString();
            if ( selectedProfile == ConfigManager.Configs.CurrentProfileName ) return;
            SwitchProfile( selectedProfile );
            // User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RestartAppController, 0 );
        }

        private void btn_profile_dup_Click( object sender, EventArgs e )
        {
            var newProfileName = cbb_profiles.Text + " (copy)";

            var isValid = !string.IsNullOrEmpty( newProfileName ) &&
                          newProfileName.IndexOfAny( Path.GetInvalidFileNameChars() ) < 0 &&
                          !File.Exists( Path.Combine( ConfigManager.ProfileFolder, newProfileName ) );

            if ( ConfigManager.Configs.Profiles.ContainsKey( newProfileName ) || !isValid )
            {
                MessageBox.Show( Agent.Langs.GetString( "Profile.Warning.InvalidProfileName" ),
                    Agent.Langs.GetString( "MsgBox.Caption.Warning" ),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
                return;
            }

            CreateProfile( newProfileName );
            AddToComboBox( new[] {newProfileName}, newProfileName );
        }

        private void btn_profile_del_Click( object sender, EventArgs e )
        {
            if ( ConfigManager.Configs.Profiles.Count == 1 )
            {
                MessageBox.Show( Agent.Langs.GetString( "Profile.Warning.LastProfileProtect" ),
                    Agent.Langs.GetString( "MsgBox.Caption.Warning" ),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
                return;
            }

            if ( MessageBox.Show( Agent.Langs.GetString( "Profile.Confirm.Delete" ),
                    Agent.Langs.GetString( "MsgBox.Caption.Confirm" ),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning ) != DialogResult.Yes ) return;

            var delProfile = cbb_profiles.Text;
            RemoveProfile( delProfile );

            cbb_profiles.Items.Remove( delProfile );
            cbb_profiles.SelectedItem = ConfigManager.Configs.CurrentProfileName;

            SwitchProfile( ConfigManager.Configs.CurrentProfileName );
        }

        private void btn_profile_rename_Click( object sender, EventArgs e )
        {
            if ( ConfigManager.Configs.Profiles.Count == 1 )
            {
                MessageBox.Show( Agent.Langs.GetString( "Profile.Warning.LastProfileProtect" ),
                    Agent.Langs.GetString( "MsgBox.Caption.Warning" ),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
                return;
            }

            cbb_profiles.DropDownStyle = ComboBoxStyle.Simple;
            cbb_profiles.Focus();
            _editProfileName = cbb_profiles.Text;
        }

        private void RemoveProfile( string delProfile )
        {
            ConfigManager.Configs.Profiles.Remove( delProfile );
            ConfigManager.Configs.CurrentProfileName = ConfigManager.Configs.Profiles.Keys.Last();
            ConfigManager.Save( reason: "Delete", reasonName: "Profile" );
            ConfigManager.DeleteFilesOfProfile( delProfile );
        }

        private void CreateProfile( string profileName )
        {
            var newProfile = ConfigManager.CurrentProfile.Clone();
            ConfigManager.Configs.Profiles.Add( profileName, newProfile );
            ConfigManager.Configs.CurrentProfileName = profileName;
            ConfigManager.Save( reason: "Create", reasonName: "Profile" );

            ConfigManager.SaveCluster( ConfigManager.Configs.Cluster ); // save cluster for new profile
            Conditions.SaveRules( Conditions.FetchRules() ); // save rules for new profile

            ReadRules();
        }

        private void SwitchProfile( string name )
        {
            ConfigManager.SwitchProfile( name );

            InitClusterConfig( false );
            InitUiConfig( false );
            ReadNavConfig( false );
            ReadRules();
        }
    }
}