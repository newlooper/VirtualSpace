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
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Cube3D.Effects
{
    public class Slide : Effect
    {
        private static readonly Dictionary<Keys, TranslateTransform3D> TransformDirections = new()
        {
            {
                Keys.Left, new TranslateTransform3D()
            },
            {
                Keys.Right, new TranslateTransform3D()
            },
            {
                Keys.Up, new TranslateTransform3D()
            },
            {
                Keys.Down, new TranslateTransform3D()
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
                Duration = new Duration( TimeSpan.FromMilliseconds( Config.Settings.AnimationDuration ) ),
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
            var frontMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.FrontD3DImage ) );
            _faceCenter.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( new ImageBrush( D3DImages.OthersD3DImage ) );
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

        public override void AnimationInDirection( Keys dir, Model3DGroup model3DGroup )
        {
            var offsetProperty = TranslateTransform3D.OffsetXProperty;
            var animation      = (DoubleAnimation)Animation;
            switch ( dir )
            {
                case Keys.Left:
                    Transform3D = TransformDirections[Keys.Left];
                    animation.To = MeshWidth;
                    break;
                case Keys.Right:
                    Transform3D = TransformDirections[Keys.Right];
                    animation.To = -MeshWidth;
                    break;
                case Keys.Up:
                    Transform3D = TransformDirections[Keys.Up];
                    animation.To = -MeshHeight;
                    offsetProperty = TranslateTransform3D.OffsetYProperty;
                    break;
                case Keys.Down:
                    Transform3D = TransformDirections[Keys.Down];
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