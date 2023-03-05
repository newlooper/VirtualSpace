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
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Cube3D.Config;

namespace Cube3D.Effects
{
    public class Fade : Effect
    {
        private readonly DoubleAnimation _animationOfFace2 = new()
        {
            From = 0,
            To = 1,
            FillBehavior = FillBehavior.Stop
        };

        /////////////////////////////
        // 重叠的两个面
        private readonly Model3DGroup    _face      = new();
        private readonly GeometryModel3D _face1     = new();
        private readonly MeshGeometry3D  _face1Mesh = new();
        private readonly GeometryModel3D _face2     = new();
        private readonly MeshGeometry3D  _face2Mesh = new();

        private readonly ImageBrush _frontD3DImage  = new( D3DImages.D3DImages.FrontD3DImage );
        private readonly ImageBrush _othersD3DImage = new( D3DImages.D3DImages.OthersD3DImage );

        public Fade()
        {
            Animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
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
            var frontMaterial = new DiffuseMaterial( _frontD3DImage );
            _face1.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( _othersD3DImage );
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
            var animationOfFace1 = (DoubleAnimation)Animation;
            animationOfFace1.Duration = new Duration( TimeSpan.FromMilliseconds( SettingsManager.Settings.AnimationDuration ) );
            animationOfFace1.EasingFunction = ef;
            _animationOfFace2.Duration = animationOfFace1.Duration;
            _animationOfFace2.EasingFunction = ef;

            _frontD3DImage.BeginAnimation( Brush.OpacityProperty, animationOfFace1 );
            _othersD3DImage.BeginAnimation( Brush.OpacityProperty, _animationOfFace2 );
            Interlocked.Increment( ref MainWindow.RunningAnimationCount );
        }
    }
}