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
using ScreenCapture;
using VirtualSpace.Plugin;
using VirtualSpace.PluginContracts;

[assembly: PluginMetadata(
    "Cube3D",
    "Cube3D",
    "3.1",
    "3D animation for virtual desktop switching",
    "Dylan Cheng",
    "newlooper@hotmail.com",
    Type = PluginType.VD_SWITCH_OBSERVER,
    DefaultAutoStart = false,
    DefaultAutoStartTiming = AutoStartTiming.MainWindowLoaded,
    MinWinMajor = 10,
    MinWinBuild = 19041,
    MinHostVersion = "1.1.0" )]

namespace Cube3D
{
    public sealed class Cube3DPlugin : PluginBase
    {
        private IHostContext       _host;
        private MainWindow         _mainWindow;
        private ResourceDictionary _resources;
        private Action<object>     _onSwitch;

        public override IReadOnlyList<string> SubscribedEvents { get; } = new[] { PluginEvents.VirtualDesktopSwitch };

        public override void Initialize( IHostContext hostContext )
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

        public override void Shutdown()
        {
            MainWindow.RestartRequested = null;
            if ( _host != null && _onSwitch != null )
                _host.Unsubscribe( PluginEvents.VirtualDesktopSwitch, _onSwitch );

            void TearDown()
            {
                _mainWindow?.CloseAll();
                _mainWindow = null;
                D3DImages.D3DImages.Reset();
                D3D9ShareCapture.ReleaseSharedDevices();
                PluginUi.Resources = null;
                _resources         = null;
            }

            var dispatcher = _mainWindow?.Dispatcher ?? Application.Current?.Dispatcher;
            if ( dispatcher != null && !dispatcher.CheckAccess() )
                dispatcher.Invoke( TearDown );
            else
                TearDown();

            _onSwitch = null;
            _host     = null;
        }

        public override void ShowSettings()
        {
            _mainWindow?.OpenSettings();
        }

        private readonly object _restartLock = new();

        private void RestartUi()
        {
            lock ( _restartLock )
            {
                _mainWindow?.CloseAll();
                D3D9ShareCapture.ReleaseSharedDevices();
                StartUi();
            }
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
