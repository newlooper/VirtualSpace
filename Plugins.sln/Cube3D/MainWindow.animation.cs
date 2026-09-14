/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Cube3D.Config;
using VirtualSpace.PluginContracts;

namespace Cube3D;

public partial class MainWindow
{
    private static long _mainWindowRunningAnimationCount;

    private readonly ThicknessAnimation _animationNotifyGrid = new()
    {
        FillBehavior = FillBehavior.Stop
    };

    private readonly DoubleAnimation _animationNotifyHeight = new()
    {
        FillBehavior = FillBehavior.Stop
    };

    private readonly DoubleAnimation _animationNotifyWidth = new()
    {
        FillBehavior = FillBehavior.Stop
    };

    private void AnimationCompleted( object? sender, EventArgs e )
    {
        if ( _monitorInfo.IsPrimary )
        {
            Interlocked.Decrement( ref _mainWindowRunningAnimationCount );
            if ( _mainWindowRunningAnimationCount == 0 )
            {
                FakeHide( true );
            }
        }
        else
        {
            FakeHide( true );
        }
    }

    private void NotificationGridAnimation( VirtualDesktopSwitchInfo info, IEasingFunction? ef = null )
    {
        var fromEl = FindNotifyCell( info.FromIndex );
        var toEl   = FindNotifyCell( info.TargetIndex ) ?? fromEl;
        if ( fromEl == null )
        {
            return;
        }

        var fromPos    = fromEl.TranslatePoint( new Point( 0, 0 ), NotifyContainer );
        var toPos      = toEl!.TranslatePoint( new Point( 0, 0 ), NotifyContainer );
        var fromMargin = new Thickness( fromPos.X, fromPos.Y, 0, 0 );
        var toMargin   = new Thickness( toPos.X, toPos.Y, 0, 0 );

        CurrentIndicator.BeginAnimation( MarginProperty, null );
        CurrentIndicator.BeginAnimation( WidthProperty, null );
        CurrentIndicator.BeginAnimation( HeightProperty, null );
        CurrentIndicator.Width  = fromEl.ActualWidth;
        CurrentIndicator.Height = fromEl.ActualHeight;
        CurrentIndicator.Margin = fromMargin;

        var duration = new Duration( TimeSpan.FromMilliseconds( SettingsManager.Settings.AnimationDuration ) );

        _animationNotifyGrid.From           = fromMargin;
        _animationNotifyGrid.To             = toMargin;
        _animationNotifyGrid.Duration       = duration;
        _animationNotifyGrid.EasingFunction = ef;
        CurrentIndicator.BeginAnimation( MarginProperty, _animationNotifyGrid );

        _animationNotifyWidth.From           = fromEl.ActualWidth;
        _animationNotifyWidth.To             = toEl.ActualWidth;
        _animationNotifyWidth.Duration       = duration;
        _animationNotifyWidth.EasingFunction = ef;
        CurrentIndicator.BeginAnimation( WidthProperty, _animationNotifyWidth );

        _animationNotifyHeight.From           = fromEl.ActualHeight;
        _animationNotifyHeight.To             = toEl.ActualHeight;
        _animationNotifyHeight.Duration       = duration;
        _animationNotifyHeight.EasingFunction = ef;
        CurrentIndicator.BeginAnimation( HeightProperty, _animationNotifyHeight );
    }

    private void CameraAnimation()
    {
        var animationCamera = new Point3DAnimation
        {
            From         = new Point3D( MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z + 0.1 ),
            To           = new Point3D( MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z + 0.5 ),
            Duration     = new Duration( TimeSpan.FromMilliseconds( SettingsManager.Settings.AnimationDuration / 2.0 ) ),
            FillBehavior = FillBehavior.Stop,
            AutoReverse  = true
        };

        MainCamera.BeginAnimation( ProjectionCamera.PositionProperty, animationCamera );
    }
}