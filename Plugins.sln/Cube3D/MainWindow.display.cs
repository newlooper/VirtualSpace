/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Cube3D.Config;
using ScreenCapture;

#pragma warning disable CA1416

namespace Cube3D
{
    public partial class MainWindow
    {
        private static int _loadGeneration;

        private int               _windowLoadGeneration;
        private DispatcherTimer?  _displayChangeDebounceTimer;

        private static bool IsLoadCurrent( int generation ) =>
            generation == Volatile.Read( ref _loadGeneration );

        private static void InvalidateInFlightLoads() =>
            Interlocked.Increment( ref _loadGeneration );

        internal void InvalidateLoadsOnClose() =>
            InvalidateInFlightLoads();

        private static MonitorInfo? TryGetPrimaryMonitor()
        {
            var monitor = MonitorEnumerationHelper.GetMonitors().FirstOrDefault( m => m.IsPrimary );
            if ( monitor == null ) return null;
            if ( monitor.ScreenSize.X <= 0 || monitor.ScreenSize.Y <= 0 ) return null;
            return monitor;
        }

        private void ApplyMonitorLayout( MonitorInfo mi )
        {
            var dpi = GetDpiForMonitor( mi.Hmon );
            Left   = Const.FakeHideX;
            Top    = Const.FakeHideY;
            Width  = mi.ScreenSize.X / dpi.ScaleX;
            Height = mi.ScreenSize.Y / dpi.ScaleY;
        }

        private void ScheduleDisplayChangeRecovery()
        {
            if ( !_monitorInfo.IsPrimary ) return;

            _displayChangeDebounceTimer ??= new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds( 400 )
            };

            _displayChangeDebounceTimer.Stop();
            _displayChangeDebounceTimer.Tick -= OnDisplayChangeDebounced;
            _displayChangeDebounceTimer.Tick += OnDisplayChangeDebounced;
            _displayChangeDebounceTimer.Start();
        }

        private async void OnDisplayChangeDebounced( object? sender, EventArgs e )
        {
            _displayChangeDebounceTimer?.Stop();
            if ( !await TryRecoverDisplayLayoutAsync().ConfigureAwait( true ) )
            {
                Trace.WriteLine( "[Cube3D.Error] display recovery failed, restarting UI." );
                RestartRequested?.Invoke();
            }
        }

        private async Task<bool> TryRecoverDisplayLayoutAsync()
        {
            if ( !_monitorInfo.IsPrimary ) return true;

            var generation = Volatile.Read( ref _loadGeneration );
            StopCapture();
            D3D9ShareCapture.ReleaseSharedDevices();

            var monitor = TryGetPrimaryMonitor();
            if ( monitor == null ) return false;

            _monitorInfo = monitor;
            ApplyMonitorLayout( monitor );
            CameraPosition( monitor );
            CreateOtherScreens();

            await Dispatcher.Yield( DispatcherPriority.Background );
            if ( !IsLoadCurrent( generation ) || !IsLoaded ) return true;

            await D3D9ShareCapture.PreloadD3D11Async().ConfigureAwait( true );
            if ( !IsLoadCurrent( generation ) || !IsLoaded ) return true;

            return await WarmupMonitorCaptureAsync( generation ).ConfigureAwait( true );
        }

        private void ScheduleCaptureRetry()
        {
            _ = Dispatcher.BeginInvoke( async () =>
            {
                await Task.Delay( 1000 ).ConfigureAwait( true );
                if ( !IsLoaded ) return;
                if ( await WarmupMonitorCaptureAsync( _windowLoadGeneration ).ConfigureAwait( true ) ) return;
                RestartRequested?.Invoke();
            }, DispatcherPriority.Background );
        }
    }
}

#pragma warning restore CA1416
