/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Media.Media3D;
using Cube3D.Config;
using Cube3D.Effects;
using ScreenCapture;

namespace Cube3D
{
    public partial class MainWindow
    {
        private static Effect _effect;

        private void CameraPosition( MonitorInfo mi )
        {
            var ratio          = mi.ScreenSize.Y / mi.ScreenSize.X;
            var workAreaWidth  = 1.0;
            var workAreaHeight = workAreaWidth * ratio;
            var radianFov      = MainCamera.FieldOfView * ( Math.PI / 180 );
            var cameraX        = workAreaWidth / 2;
            var cameraY        = workAreaHeight / 2;
            var cameraZ        = workAreaWidth / 2 / Math.Tan( radianFov / 2 );
            MainCamera = new PerspectiveCamera
            {
                LookDirection = new Vector3D( 0, 0, -1 ),
                Position = new Point3D( cameraX, cameraY, cameraZ )
            };
            Vp3D.Camera = MainCamera;
        }

        public void Build3D()
        {
            var settings = SettingsManager.Settings;
            _effect = settings.SelectedEffect switch
            {
                EffectType.Cube => new Cube(),
                EffectType.Flip => new Flip(),
                EffectType.Slide => new Slide(),
                EffectType.Reveal => new Reveal(),
                EffectType.Fade => new Fade(),
                EffectType.InsideCube => new InsideCube(),
                _ => new Cube()
            };

            _effect.Build( MainModel3DGroup );
            _effect.AddAnimationCompletedListener( AnimationCompleted );
        }
    }
}