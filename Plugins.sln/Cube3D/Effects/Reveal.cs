// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Cube3D.
// 
// Cube3D is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Cube3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Cube3D. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Cube3D.Config;

namespace Cube3D.Effects
{
    public class Reveal : Effect
    {
        private static readonly Dictionary<KeyCode, TranslateTransform3D> TransformDirections = new()
        {
            {
                KeyCode.Left, new TranslateTransform3D()
            },
            {
                KeyCode.Right, new TranslateTransform3D()
            },
            {
                KeyCode.Up, new TranslateTransform3D()
            },
            {
                KeyCode.Down, new TranslateTransform3D()
            }
        };

        /////////////////////////////
        // 重叠的两个面
        private readonly Model3DGroup    _face      = new();
        private readonly GeometryModel3D _face1     = new();
        private readonly MeshGeometry3D  _face1Mesh = new();
        private readonly GeometryModel3D _face2     = new();
        private readonly MeshGeometry3D  _face2Mesh = new();

        public Reveal()
        {
            Animation = new DoubleAnimation
            {
                From = 0,
                FillBehavior = FillBehavior.Stop
            };
        }

        public override void Build( Model3DGroup model3DGroup )
        {
            _face1Mesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _face1Mesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _face1Mesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            _face1Mesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            AddTriangleIndices( _face1Mesh );
            AddTextureCoordinatesFront( _face1Mesh );

            _face2Mesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _face2Mesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _face2Mesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            _face2Mesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            AddTriangleIndices( _face2Mesh );
            AddTextureCoordinatesFront( _face2Mesh );

            ////////////////////////////////////////////////////////////////
            // Front 永远显示当前桌面；其在动画中定格，动画结束后归位并继续截屏
            var frontMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.D3DImages.FrontD3DImage ) );
            _face1.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.D3DImages.OthersD3DImage ) );
            _face2.Material = othersMaterial;

            ////////////////////////////////////////////
            // set GeometryModel3D' mesh
            _face1.Geometry = _face1Mesh;
            _face2.Geometry = _face2Mesh;

            ////////////////////////////////////////////
            // Model3D/Model3DGroup
            _face.Children.Add( _face2 );
            _face.Children.Add( _face1 ); // _face1 above _face2

            model3DGroup.Children.Clear();
            model3DGroup.Children.Add( _face );
            model3DGroup.Children.Add( CommonLight );
        }

        public override void AnimationInDirection( KeyCode dir, Model3DGroup model3DGroup, IEasingFunction ef = null )
        {
            var offsetProperty = TranslateTransform3D.OffsetXProperty;
            var animation      = (DoubleAnimation)Animation;
            animation.Duration = new Duration( TimeSpan.FromMilliseconds( SettingsManager.Settings.AnimationDuration ) );
            animation.EasingFunction = ef;

            switch ( dir )
            {
                case KeyCode.Left:
                    Transform3D = TransformDirections[KeyCode.Left];
                    animation.To = MeshWidth;
                    break;
                case KeyCode.Right:
                    Transform3D = TransformDirections[KeyCode.Right];
                    animation.To = -MeshWidth;
                    break;
                case KeyCode.Up:
                    Transform3D = TransformDirections[KeyCode.Up];
                    animation.To = -MeshHeight;
                    offsetProperty = TranslateTransform3D.OffsetYProperty;
                    break;
                case KeyCode.Down:
                    Transform3D = TransformDirections[KeyCode.Down];
                    animation.To = MeshHeight;
                    offsetProperty = TranslateTransform3D.OffsetYProperty;
                    break;
            }

            if ( TransGroup.Children.Count == 0 )
            {
                TransGroup.Children.Add( Transform3D );
            }
            else
            {
                TransGroup.Children[0] = Transform3D;
            }

            _face1.Transform = TransGroup;

            var transform = (TranslateTransform3D)Transform3D;
            transform.BeginAnimation( offsetProperty, animation );
        }
    }
}