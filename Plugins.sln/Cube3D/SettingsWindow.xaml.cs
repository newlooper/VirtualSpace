// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Cube3D.Config;
using Cube3D.Effects;

namespace Cube3D
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        public int AnimationDuration
        {
            get => ConfigManager.Settings.AnimationDuration;
            set => ConfigManager.Settings.AnimationDuration = value;
        }

        public int CheckAliveInterval
        {
            get => ConfigManager.Settings.CheckAliveInterval;
            set => ConfigManager.Settings.CheckAliveInterval = value;
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            foreach ( EffectType effect in Enum.GetValues( typeof( EffectType ) ) )
            {
                ComboBoxEffects.Items.Add( effect );
            }

            ComboBoxEffects.SelectedItem = ConfigManager.Settings.SelectedEffect;
        }

        private void ComboBoxEffects_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            ConfigManager.Settings.SelectedEffect = (EffectType)ComboBoxEffects.SelectedItem;
            ConfigManager.SaveJson();
        }

        private void ButtonBase_OnClick( object sender, RoutedEventArgs e )
        {
            ConfigManager.SaveJson();
            Restart();
        }

        public static void Restart()
        {
            Process.Start( ConfigManager.GetAppPath() );
            Application.Current.Shutdown();
        }
    }
}