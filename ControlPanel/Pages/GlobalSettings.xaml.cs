/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Controls;
using WPFLocalizeExtension.Engine;

namespace ControlPanel.Pages
{
    /// <summary>
    ///     GlobalSettings.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalSettings
    {
        private readonly List<string> _locales = new()
        {
            "en-US",
            "zh-CN"
        };

        public GlobalSettings()
        {
            InitializeComponent();
            foreach ( var locale in _locales )
            {
                var item = new ComboBoxItem {Content = locale};
                LanguageChooser.Items.Add( item );
                AutomationProperties.SetAutomationId( item, locale );
            }
        }

        private void LanguageChooser_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = new CultureInfo( (string)( (ComboBoxItem)LanguageChooser.SelectedItem ).Content );
        }
    }
}