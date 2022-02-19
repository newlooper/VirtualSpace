/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Globalization;
using System.Windows;
using ControlPanel.Pages;
using ModernWpf;
using ModernWpf.Controls;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace ControlPanel
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = new CultureInfo( "zh-Hans" );
        }

        private void ThemeButton_OnClick( object sender, RoutedEventArgs e )
        {
            DispatcherHelper.RunOnMainThread( () =>
            {
                ThemeManager.Current.ApplicationTheme = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark
                    ? ApplicationTheme.Light
                    : ApplicationTheme.Dark;
            } );
        }

        private void NavigationView_SelectionChanged( NavigationView sender, NavigationViewSelectionChangedEventArgs args )
        {
            if ( args.IsSettingsSelected )
            {
                ContentFrame.Navigate( typeof( GlobalSettings ) );
            }
            else
            {
                var selectedItem    = (NavigationViewItem)args.SelectedItem;
                var selectedItemTag = (string)selectedItem.Tag;
                sender.Header = "> " + LocExtension.GetLocalizedValue<string>( "TopBar." + selectedItemTag );
                var pageName = "ControlPanel.Pages." + selectedItemTag;
                var pageType = typeof( Logs ).Assembly.GetType( pageName );
                ContentFrame.Navigate( pageType, null, args.RecommendedNavigationTransitionInfo );
            }
        }

        private void SettingsButton_OnClick( object sender, RoutedEventArgs e )
        {
            TopNav.Header = "> " + LocExtension.GetLocalizedValue<string>( "TopBar.Setting" );
            ContentFrame.Navigate( typeof( GlobalSettings ) );
        }
    }
}