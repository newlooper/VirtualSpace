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
    public class Slide : Effect
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
        // 十字排列的五个全等面
        private readonly Model3DGroup    _face           = new();
        private readonly GeometryModel3D _faceBottom     = new();
        private readonly MeshGeometry3D  _faceBottomMesh = new();
        private readonly GeometryModel3D _faceCenter     = new();
        private readonly MeshGeometry3D  _faceCenterMesh = new();
        private readonly GeometryModel3D _faceLeft       = new();
        private readonly MeshGeometry3D  _faceLeftMesh   = new();
        private readonly GeometryModel3D _faceRight      = new();
        private readonly MeshGeometry3D  _faceRightMesh  = new();
        private readonly GeometryModel3D _faceTop        = new();
        private readonly MeshGeometry3D  _faceTopMesh    = new();

        public Slide()
        {
            Animation = new DoubleAnimation
            {
                From = 0,
                FillBehavior = FillBehavior.Stop
            };
        }

        public override void Build( Model3DGroup model3DGroup )
        {
            ////////////////////////////////////////////////////////////////
            // center
            var centerPoints = new Point3D[]
            {
                new( 0, 0, 0 ),
                new( MeshWidth, 0, 0 ),
                new( MeshWidth, MeshHeight, 0 ),
                new( 0, MeshHeight, 0 )
            };
            _faceCenterMesh.Positions.Add( centerPoints[0] );
            _faceCenterMesh.Positions.Add( centerPoints[1] );
            _faceCenterMesh.Positions.Add( centerPoints[2] );
            _faceCenterMesh.Positions.Add( centerPoints[3] );
            AddTriangleIndices( _faceCenterMesh );
            AddTextureCoordinatesFront( _faceCenterMesh );

            ////////////////////////////////////////////////////////////////
            // left
            var left = new Vector3D( -MeshWidth, 0, 0 );
            _faceLeftMesh.Positions.Add( centerPoints[0] + left );
            _faceLeftMesh.Positions.Add( centerPoints[1] + left );
            _faceLeftMesh.Positions.Add( centerPoints[2] + left );
            _faceLeftMesh.Positions.Add( centerPoints[3] + left );
            AddTriangleIndices( _faceLeftMesh );
            AddTextureCoordinatesFront( _faceLeftMesh );

            ////////////////////////////////////////////////////////////////
            // right
            var right = new Vector3D( MeshWidth, 0, 0 );
            _faceRightMesh.Positions.Add( centerPoints[0] + right );
            _faceRightMesh.Positions.Add( centerPoints[1] + right );
            _faceRightMesh.Positions.Add( centerPoints[2] + right );
            _faceRightMesh.Positions.Add( centerPoints[3] + right );
            AddTriangleIndices( _faceRightMesh );
            AddTextureCoordinatesFront( _faceRightMesh );

            ////////////////////////////////////////////////////////////////
            // top
            var top = new Vector3D( 0, MeshHeight, 0 );
            _faceTopMesh.Positions.Add( centerPoints[0] + top );
            _faceTopMesh.Positions.Add( centerPoints[1] + top );
            _faceTopMesh.Positions.Add( centerPoints[2] + top );
            _faceTopMesh.Positions.Add( centerPoints[3] + top );
            AddTriangleIndices( _faceTopMesh );
            AddTextureCoordinatesFront( _faceTopMesh );

            ////////////////////////////////////////////////////////////////
            // bottom
            var bottom = new Vector3D( 0, -MeshHeight, 0 );
            _faceBottomMesh.Positions.Add( centerPoints[0] + bottom );
            _faceBottomMesh.Positions.Add( centerPoints[1] + bottom );
            _faceBottomMesh.Positions.Add( centerPoints[2] + bottom );
            _faceBottomMesh.Positions.Add( centerPoints[3] + bottom );
            AddTriangleIndices( _faceBottomMesh );
            AddTextureCoordinatesFront( _faceBottomMesh );

            ////////////////////////////////////////////////////////////////
            // Front 永远显示当前桌面；其在动画中定格，动画结束后归位并继续截屏
            var frontMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.D3DImages.FrontD3DImage ) );
            _faceCenter.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.D3DImages.OthersD3DImage ) );
            _faceLeft.Material = othersMaterial;
            _faceRight.Material = othersMaterial;
            _faceTop.Material = othersMaterial;
            _faceBottom.Material = othersMaterial;

            ////////////////////////////////////////////
            // set GeometryModel3D' mesh
            _faceCenter.Geometry = _faceCenterMesh;
            _faceLeft.Geometry = _faceLeftMesh;
            _faceRight.Geometry = _faceRightMesh;
            _faceTop.Geometry = _faceTopMesh;
            _faceBottom.Geometry = _faceBottomMesh;

            ////////////////////////////////////////////
            // Model3D/Model3DGroup
            _face.Children.Add( _faceCenter );
            _face.Children.Add( _faceLeft );
            _face.Children.Add( _faceRight );
            _face.Children.Add( _faceTop );
            _face.Children.Add( _faceBottom );

            model3DGroup.Children.Add( _face );
        }

        public override void AnimationInDirection( KeyCode dir, Model3DGroup model3DGroup )
        {
            var offsetProperty = TranslateTransform3D.OffsetXProperty;
            var animation      = (DoubleAnimation)Animation;
            animation.Duration = new Duration( TimeSpan.FromMilliseconds( SettingsManager.Settings.AnimationDuration ) );
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

            model3DGroup.Transform = TransGroup;

            // animation.EasingFunction = new CircleEase();
            var transform = (TranslateTransform3D)Transform3D;
            transform.BeginAnimation( offsetProperty, animation );
            Interlocked.Increment( ref MainWindow.RunningAnimationCount );
        }
    }
}