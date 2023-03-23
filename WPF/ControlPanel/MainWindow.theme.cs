// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace ControlPanel;

public partial class MainWindow
{
    private (Color pColor, Color sColor, IBaseTheme theme) GetThemeInfo()
    {
        var   theme = Theme.Light;
        Color pColor;
        Color sColor;

        var pColorDark = SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Blue];
        var sColorDark = SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.LightBlue];

        var pColorLight = SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Amber];
        var sColorLight = SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.Yellow];

        switch ( Manager.CurrentProfile.UI.Theme )
        {
            case 0:
                theme = SysInfo.GetAppsTheme() == SysInfo.WinAppsTheme.LIGHT ? Theme.Light : Theme.Dark;
                pColor = theme == Theme.Dark ? pColorDark : pColorLight;
                sColor = theme == Theme.Dark ? sColorDark : sColorLight;
                break;
            case 1:
                pColor = pColorLight;
                sColor = sColorLight;
                theme = Theme.Light;
                break;
            case 2:
                pColor = pColorDark;
                sColor = sColorDark;
                theme = Theme.Dark;
                break;
        }

        return ( pColor, sColor, theme );
    }

    private void InitTheme()
    {
        UpdateTheme();
        RegValueMonitor.OnRegValueChanged += ( o, args ) =>
        {
            if ( Manager.CurrentProfile.UI.Theme != 0 ) return;
            var theme = Resources.GetTheme();
            var (pColor, sColor, newTheme) = GetThemeInfo();
            theme.SetPrimaryColor( pColor );
            theme.SetSecondaryColor( sColor );
            theme.SetBaseTheme( newTheme );
            Dispatcher.Invoke( () => { Resources.SetTheme( theme ); } );
        };
    }

    public static void UpdateTheme()
    {
        var (primaryColor, secondaryColor, initTheme) = _instance.GetThemeInfo();
        _instance.Resources.SetTheme( Theme.Create( initTheme, primaryColor, secondaryColor ) );
    }
}