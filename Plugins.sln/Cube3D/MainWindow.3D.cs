/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of Cube3D.

Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Cube3D.Config;
using Cube3D.Effects;

namespace Cube3D
{
    public partial class MainWindow
    {
        private static double _workAreaHeight = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
        private static double _workAreaWidth  = 1.0;
        private static Effect _effect;

        private void CameraPosition()
        {
            var ratio = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
            _workAreaWidth = 1.0;
            _workAreaHeight = _workAreaWidth * ratio;
            var radianFov = MainCamera.FieldOfView * ( Math.PI / 180 );
            var cameraX   = _workAreaWidth / 2;
            var cameraY   = _workAreaHeight / 2;
            var cameraZ   = _workAreaWidth / 2 / Math.Tan( radianFov / 2 );
            MainCamera = new PerspectiveCamera
            {
                LookDirection = new Vector3D( 0, 0, -1 ),
                Position = new Point3D( cameraX, cameraY, cameraZ )
            };
            Vp3D.Camera = MainCamera;
        }

        private void Build3D()
        {
            var settings = SettingsManager.Settings;
            switch ( settings.SelectedEffect )
            {
                case EffectType.Cube:
                    _effect = new Cube();
                    break;
                case EffectType.Flip:
                    _effect = new Flip();
                    break;
                case EffectType.Slide:
                    _effect = new Slide();
                    break;
                case EffectType.Reveal:
                    _effect = new Reveal();
                    break;
                case EffectType.Fade:
                    _effect = new Fade();
                    break;
                case EffectType.InsideCube:
                    _effect = new InsideCube();
                    break;
                default:
                    _effect = new Cube();
                    break;
            }

            _effect.Build( MainModel3DGroup );
            _effect.AddAnimationCompletedListener( AnimationCompleted );
        }
    }
}