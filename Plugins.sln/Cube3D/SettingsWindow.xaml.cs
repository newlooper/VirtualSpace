﻿// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Cube3D.
// 
// Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows;
using System.Windows.Controls;
using Cube3D.Config;
using Cube3D.Effects;

namespace Cube3D
{
    public partial class SettingsWindow : Window
    {
        private MainWindow _mainWindow;
        private bool       _isLoaded = false;

        public SettingsWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        public int AnimationDuration
        {
            get => SettingsManager.Settings.AnimationDuration;
            set => SettingsManager.Settings.AnimationDuration = value;
        }

        public int CheckAliveInterval
        {
            get => SettingsManager.Settings.CheckAliveInterval;
            set => SettingsManager.Settings.CheckAliveInterval = value;
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            foreach ( EffectType effect in Enum.GetValues( typeof( EffectType ) ) )
            {
                ComboBoxEffects.Items.Add( effect );
            }

            ComboBoxEffects.SelectedItem = SettingsManager.Settings.SelectedEffect;

            foreach ( EaseType ease in Enum.GetValues( typeof( EaseType ) ) )
            {
                ComboBoxEase.Items.Add( ease );
            }

            ComboBoxEase.SelectedItem = SettingsManager.Settings.EaseType;

            foreach ( EaseMode easeMode in Enum.GetValues( typeof( EaseMode ) ) )
            {
                ComboBoxEaseMode.Items.Add( easeMode );
            }

            ComboBoxEaseMode.SelectedItem = SettingsManager.Settings.EaseMode;

            foreach ( TransitionType tt in Enum.GetValues( typeof( TransitionType ) ) )
            {
                ComboBoxTransitionType.Items.Add( tt );
            }

            ComboBoxTransitionType.SelectedItem = SettingsManager.Settings.TransitionType;

            _isLoaded = true;
        }

        private void ComboBoxEffects_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( !_isLoaded ) return;
            SettingsManager.Settings.SelectedEffect = (EffectType)ComboBoxEffects.SelectedItem;
            SettingsManager.SaveJson();
        }

        private void ComboBoxEase_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( !_isLoaded ) return;
            SettingsManager.Settings.EaseType = (EaseType)ComboBoxEase.SelectedItem;
            SettingsManager.SaveJson();
        }

        private void ComboBoxEaseMode_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( !_isLoaded ) return;
            SettingsManager.Settings.EaseMode = (EaseMode)ComboBoxEaseMode.SelectedItem;
            SettingsManager.SaveJson();
        }

        private void ComboBoxTransitionType_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( !_isLoaded ) return;
            SettingsManager.Settings.TransitionType = (TransitionType)ComboBoxTransitionType.SelectedItem;
            _mainWindow.SetTransitionType();
            SettingsManager.SaveJson();
        }

        public void SetMainWindow( MainWindow mw )
        {
            _mainWindow = mw;
        }

        private void ApplyEffect_OnClick( object sender, RoutedEventArgs e )
        {
            _mainWindow.Build3D();
        }

        private void Close_OnClick( object sender, RoutedEventArgs e )
        {
            SettingsManager.SaveJson();
            Close();
        }
    }
}