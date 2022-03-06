// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
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
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Cube3D.Effects
{
    public class InsideCube : Effect
    {
        private static readonly Dictionary<Keys, RotateTransform3D> TransformDirections = new()
        {
            {
                Keys.Left,
                new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, -1, 0 ), 0 ) )
                {
                    CenterX = MeshWidth / 2,
                    CenterZ = MeshWidth / 2
                }
            },
            {
                Keys.Right,
                new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 0 ) )
                {
                    CenterX = MeshWidth / 2,
                    CenterZ = MeshWidth / 2
                }
            },
            {
                Keys.Up,
                new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( -1, 0, 0 ), 0 ) )
                {
                    CenterY = MeshHeight / 2,
                    CenterZ = MeshHeight / 2
                }
            },
            {
                Keys.Down,
                new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 1, 0, 0 ), 0 ) )
                {
                    CenterY = MeshHeight / 2,
                    CenterZ = MeshHeight / 2
                }
            }
        };

        /////////////////////////////
        // 立方体
        // 前/后表面 —— 水平/垂直旋转
        // 左/右表面 —— 水平旋转
        // 上/下表面 —— 垂直旋转
        private readonly Model3DGroup    _cube           = new();
        private readonly GeometryModel3D _cubeBack       = new();
        private readonly MeshGeometry3D  _cubeBackMesh   = new();
        private readonly GeometryModel3D _cubeBottom     = new();
        private readonly MeshGeometry3D  _cubeBottomMesh = new();
        private readonly GeometryModel3D _cubeFront      = new();
        private readonly MeshGeometry3D  _cubeFrontMesh  = new();
        private readonly GeometryModel3D _cubeLeft       = new();
        private readonly MeshGeometry3D  _cubeLeftMesh   = new();
        private readonly GeometryModel3D _cubeRight      = new();
        private readonly MeshGeometry3D  _cubeRightMesh  = new();
        private readonly GeometryModel3D _cubeTop        = new();
        private readonly MeshGeometry3D  _cubeTopMesh    = new();

        public InsideCube()
        {
            Animation = new DoubleAnimation
            {
                From = 0,
                To = 90,
                Duration = new Duration( TimeSpan.FromMilliseconds( Config.Settings.AnimationDuration ) ),
                FillBehavior = FillBehavior.Stop
            };
        }

        public override void Build( Model3DGroup model3DGroup )
        {
            ////////////////////////////////////////////////////////////////
            // front
            _cubeFrontMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            AddTriangleIndices( _cubeFrontMesh );
            AddTextureCoordinatesFront( _cubeFrontMesh );

            ////////////////////////////////////////////////////////////////
            // back
            // _cubeBackMesh.Positions.Add( new Point3D( MeshWidth, 0, MeshWidth ) );
            // _cubeBackMesh.Positions.Add( new Point3D( 0, 0, MeshWidth ) );
            // _cubeBackMesh.Positions.Add( new Point3D( 0, MeshHeight, MeshWidth ) );
            // _cubeBackMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, MeshWidth ) );
            // AddTriangleIndices( _cubeBackMesh );
            // AddTextureCoordinatesFront( _cubeBackMesh );

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CubeH

            ////////////////////////////////////////////////////////////////
            // left
            _cubeLeftMesh.Positions.Add( new Point3D( 0, 0, MeshWidth ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, MeshHeight, MeshWidth ) );
            AddTriangleIndices( _cubeLeftMesh );
            AddTextureCoordinatesFront( _cubeLeftMesh );

            ////////////////////////////////////////////////////////////////
            // right
            _cubeRightMesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _cubeRightMesh.Positions.Add( new Point3D( MeshWidth, 0, MeshWidth ) );
            _cubeRightMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, MeshWidth ) );
            _cubeRightMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            AddTriangleIndices( _cubeRightMesh );
            AddTextureCoordinatesFront( _cubeRightMesh );

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CubeV

            ////////////////////////////////////////////////////////////////
            // top
            _cubeTopMesh.Positions.Add( new Point3D( 0, MeshHeight, 0 ) );
            _cubeTopMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, 0 ) );
            _cubeTopMesh.Positions.Add( new Point3D( MeshWidth, MeshHeight, MeshHeight ) );
            _cubeTopMesh.Positions.Add( new Point3D( 0, MeshHeight, MeshHeight ) );
            AddTriangleIndices( _cubeTopMesh );
            AddTextureCoordinatesFront( _cubeTopMesh );

            ////////////////////////////////////////////////////////////////
            // bottom
            _cubeBottomMesh.Positions.Add( new Point3D( 0, 0, MeshHeight ) );
            _cubeBottomMesh.Positions.Add( new Point3D( MeshWidth, 0, MeshHeight ) );
            _cubeBottomMesh.Positions.Add( new Point3D( MeshWidth, 0, 0 ) );
            _cubeBottomMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            AddTriangleIndices( _cubeBottomMesh );
            AddTextureCoordinatesFront( _cubeBottomMesh );

            ////////////////////////////////////////////////////////////////
            // Front 永远显示当前桌面；其在动画中定格，动画结束后归位并继续截屏
            var frontMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.FrontD3DImage ) );
            _cubeFront.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.OthersD3DImage ) );

            // _cubeBack.Material = othersMaterial;
            _cubeLeft.Material = othersMaterial;
            _cubeRight.Material = othersMaterial;
            _cubeTop.Material = othersMaterial;
            _cubeBottom.Material = othersMaterial;

            ////////////////////////////////////////////
            // set GeometryModel3D' mesh
            _cubeFront.Geometry = _cubeFrontMesh;
            // _cubeBack.Geometry = _cubeBackMesh;
            _cubeLeft.Geometry = _cubeLeftMesh;
            _cubeRight.Geometry = _cubeRightMesh;
            _cubeTop.Geometry = _cubeTopMesh;
            _cubeBottom.Geometry = _cubeBottomMesh;

            ////////////////////////////////////////////
            // Model3D/Model3DGroup
            _cube.Children.Add( _cubeFront );
            // _cube.Children.Add( _cubeBack );
            _cube.Children.Add( _cubeLeft );
            _cube.Children.Add( _cubeRight );
            _cube.Children.Add( _cubeTop );
            _cube.Children.Add( _cubeBottom );

            model3DGroup.Children.Add( _cube );
        }

        public override void AnimationInDirection( Keys dir, Model3DGroup model3DGroup )
        {
            switch ( dir )
            {
                case Keys.Left:
                    Transform3D = TransformDirections[Keys.Left];
                    break;
                case Keys.Right:
                    Transform3D = TransformDirections[Keys.Right];
                    break;
                case Keys.Up:
                    Transform3D = TransformDirections[Keys.Up];
                    break;
                case Keys.Down:
                    Transform3D = TransformDirections[Keys.Down];
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
            // animation.EasingFunction = new CircleEase();
            var transform = (RotateTransform3D)Transform3D;
            transform.Rotation.BeginAnimation( AxisAngleRotation3D.AngleProperty, animation );
            Interlocked.Increment( ref MainWindow.RunningAnimationCount );
        }
    }
}