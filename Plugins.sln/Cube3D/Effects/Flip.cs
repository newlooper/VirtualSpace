// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of Plugins.
// 
// Plugins is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Plugins is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with Plugins. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Cube3D.Effects
{
    public class Flip : Effect
    {
        private static readonly Dictionary<KeyCode, RotateTransform3D> TransformDirections = new()
        {
            {
                KeyCode.Left, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 0 ) )
                {
                    CenterX = MeshWidth / 2
                }
            },
            {
                KeyCode.Right, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, -1, 0 ), 0 ) )
                {
                    CenterX = MeshWidth / 2
                }
            },
            {
                KeyCode.Up, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 1, 0, 0 ), 0 ) )
                {
                    CenterY = MeshHeight / 2
                }
            },
            {
                KeyCode.Down, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( -1, 0, 0 ), 0 ) )
                {
                    CenterY = MeshHeight / 2
                }
            }
        };

        /////////////////////////////
        // 单面翻转
        private readonly Model3DGroup    _face          = new();
        private readonly GeometryModel3D _faceFront     = new();
        private readonly MeshGeometry3D  _faceFrontMesh = new();

        private DiffuseMaterial _backH;
        private DiffuseMaterial _backV;

        public Flip()
        {
            Animation = new DoubleAnimation
            {
                From = 0,
                To = 180,
                FillBehavior = FillBehavior.Stop
            };
        }

        public override void Build( Model3DGroup model3DGroup )
        {
            ////////////////////////////////////////////////////////////////
            // front
            _faceFrontMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _faceFrontMesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _faceFrontMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            _faceFrontMesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            AddTriangleIndices( _faceFrontMesh );
            AddTextureCoordinatesFront( _faceFrontMesh );

            ////////////////////////////////////////////////////////////////
            // Front 永远显示当前桌面；其在动画中定格，动画结束后归位并继续截屏
            var frontMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.FrontD3DImage ) );
            _faceFront.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // Flip 特效利用了 BackMaterial, 水平/垂直翻转的贴图映射需要差异化
            _backH = new DiffuseMaterial( new ImageBrush( D3DImages.OthersD3DImage )
            {
                RelativeTransform = new ScaleTransform
                {
                    ScaleX = -1, CenterX = 0.5
                }
            } );

            _backV = new DiffuseMaterial( new ImageBrush( D3DImages.OthersD3DImage )
            {
                RelativeTransform = new ScaleTransform
                {
                    ScaleY = -1, CenterY = 0.5
                }
            } );

            ////////////////////////////////////////////
            // set GeometryModel3D' mesh
            _faceFront.Geometry = _faceFrontMesh;

            ////////////////////////////////////////////
            // Model3D/Model3DGroup
            _face.Children.Add( _faceFront );

            model3DGroup.Children.Add( _face );
        }

        public override void AnimationInDirection( KeyCode dir, Model3DGroup model3DGroup )
        {
            switch ( dir )
            {
                case KeyCode.Left:
                    Transform3D = TransformDirections[KeyCode.Left];
                    FlipInDirection( "H" );
                    break;
                case KeyCode.Right:
                    Transform3D = TransformDirections[KeyCode.Right];
                    FlipInDirection( "H" );
                    break;
                case KeyCode.Up:
                    Transform3D = TransformDirections[KeyCode.Up];
                    FlipInDirection( "V" );
                    break;
                case KeyCode.Down:
                    Transform3D = TransformDirections[KeyCode.Down];
                    FlipInDirection( "V" );
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

            model3DGroup.Transform = TransGroup;

            var animation = (DoubleAnimation)Animation;
            animation.Duration = new Duration( TimeSpan.FromMilliseconds( ConfigManager.Settings.AnimationDuration ) );
            // animation.EasingFunction = new CircleEase();
            var transform = (RotateTransform3D)Transform3D;
            transform.Rotation.BeginAnimation( AxisAngleRotation3D.AngleProperty, animation );
            Interlocked.Increment( ref MainWindow.RunningAnimationCount );
        }

        private void FlipInDirection( string dir )
        {
            _faceFront.BackMaterial = dir == "H" ? _backH : _backV;
        }
    }
}