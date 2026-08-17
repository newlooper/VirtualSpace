/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cube3D.Config;
using VirtualSpace.Plugin;
using VirtualSpace.PluginContracts;

[assembly: PluginMetadata(
    "Cube3D",
    "Cube3D",
    "2.1",
    "3D animation for virtual desktop switching",
    "Dylan Cheng",
    "newlooper@hotmail.com",
    Type = PluginType.VD_SWITCH_OBSERVER,
    DefaultAutoStart = true,
    DefaultAutoStartTiming = AutoStartTiming.MainWindowLoaded,
    MinWinMajor = 10,
    MinWinBuild = 19041,
    MinHostVersion = "0.1.454" )]

namespace Cube3D
{
    public sealed class Cube3DPlugin : IPlugin
    {
        private IHostContext       _host;
        private MainWindow         _mainWindow;
        private ResourceDictionary _resources;
        private Action<object>     _onSwitch;

        public string Name        => "Cube3D";
        public string Display     => "Cube3D";
        public string Version     => "2.1";
        public string Description => "3D animation for virtual desktop switching";
        public string Author      => "Dylan Cheng";
        public string Email       => "newlooper@hotmail.com";

        public PluginType Type => PluginType.VD_SWITCH_OBSERVER;

        public Requirements Requirements { get; } = new()
        {
            WinVer      = new WinVer { Min = new Ver { Major = 10, Build = 19041 } },
            HostVersion = new Version( 0, 1, 454 )
        };

        public IReadOnlyList<string> SubscribedEvents { get; } = new[] { PluginEvents.VirtualDesktopSwitch };

        public void Initialize( IHostContext hostContext )
        {
            _host = hostContext;
            SettingsManager.Initialize( hostContext.GetPluginDataPath( Name ) );
            MergeResources();
            D3DImages.D3DImages.Initialize( _resources );
            MainWindow.RestartRequested = RestartUi;
            _onSwitch = payload =>
            {
                if ( payload is VirtualDesktopSwitchInfo info )
                    _mainWindow?.OnVirtualDesktopSwitch( info );
            };
            hostContext.Subscribe( PluginEvents.VirtualDesktopSwitch, _onSwitch );
            StartUi();
        }

        public void Shutdown()
        {
            MainWindow.RestartRequested = null;
            if ( _host != null && _onSwitch != null )
                _host.Unsubscribe( PluginEvents.VirtualDesktopSwitch, _onSwitch );

            _mainWindow?.CloseAll();
            _mainWindow = null;
            D3DImages.D3DImages.Reset();
            PluginUi.Resources = null;
            _resources         = null;
            _onSwitch          = null;
            _host              = null;
        }

        public void ShowSettings()
        {
            _mainWindow?.OpenSettings();
        }

        private void RestartUi()
        {
            _mainWindow?.CloseAll();
            StartUi();
        }

        private void StartUi()
        {
            if ( _host == null ) return;
            _mainWindow = new MainWindow();
            _mainWindow.AttachHost( _host );
            _mainWindow.Show();
        }

        private void MergeResources()
        {
            _resources = new ResourceDictionary
            {
                [Const.Front]              = new D3DImage(),
                [Const.Others]             = new D3DImage(),
                [PluginUi.BackgroundLgbKey]   = CreateBackgroundBrush(),
                [PluginUi.BackgroundTransKey] = Brushes.Transparent
            };
            PluginUi.Resources = _resources;
        }

        private static LinearGradientBrush CreateBackgroundBrush()
        {
            return new LinearGradientBrush
            {
                StartPoint = new Point( 0.5, 0 ),
                EndPoint   = new Point( 0.5, 1 ),
                GradientStops =
                {
                    new GradientStop( Colors.Black, 0 ),
                    new GradientStop( Color.FromRgb( 0x32, 0x33, 0x34 ), 1 )
                }
            };
        }
    }
}
