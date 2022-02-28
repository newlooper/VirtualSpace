/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Cube3D
{
    public partial class MainWindow
    {
        private static readonly string Front = nameof( Front );

        private static readonly string Others = nameof( Others );

        private static double _meshHeight = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
        private static double _meshWidth  = 1.0;

        /////////////////////////////
        // 立方体
        // 前/后表面 —— 外立方/内立方、水平/垂直旋转
        // 左/右表面 —— 水平旋转
        // 上/下表面 —— 垂直旋转
        private readonly Model3DGroup    _cube           = new Model3DGroup();
        private readonly GeometryModel3D _cubeBack       = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeBackMesh   = new MeshGeometry3D();
        private readonly GeometryModel3D _cubeBottom     = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeBottomMesh = new MeshGeometry3D();
        private readonly GeometryModel3D _cubeFront      = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeFrontMesh  = new MeshGeometry3D();
        private readonly GeometryModel3D _cubeLeft       = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeLeftMesh   = new MeshGeometry3D();
        private readonly GeometryModel3D _cubeRight      = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeRightMesh  = new MeshGeometry3D();
        private readonly GeometryModel3D _cubeTop        = new GeometryModel3D();
        private readonly MeshGeometry3D  _cubeTopMesh    = new MeshGeometry3D();

        private static void AddTriangleIndices( MeshGeometry3D meshGeometry3D )
        {
            meshGeometry3D.TriangleIndices.Add( 0 );
            meshGeometry3D.TriangleIndices.Add( 1 );
            meshGeometry3D.TriangleIndices.Add( 2 );
            meshGeometry3D.TriangleIndices.Add( 2 );
            meshGeometry3D.TriangleIndices.Add( 3 );
            meshGeometry3D.TriangleIndices.Add( 0 );
        }

        private static void AddTextureCoordinatesFront( MeshGeometry3D meshGeometry3D )
        {
            meshGeometry3D.TextureCoordinates.Add( new Point( 0, 1 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 1, 1 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 1, 0 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 0, 0 ) );
        }

        private void CameraPosition( int area )
        {
            var ratio = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
            _meshWidth = 1.0;
            _meshHeight = _meshWidth * ratio;
            var radianFov = MainCamera.FieldOfView * ( Math.PI / 180 );
            var cameraX   = _meshWidth / 2 + area * _meshWidth;
            var cameraY   = _meshHeight / 2 + area * _meshHeight;
            var cameraZ   = _meshWidth / 2 / Math.Tan( radianFov / 2 );
            MainCamera = new PerspectiveCamera
            {
                LookDirection = new Vector3D( 0, 0, -1 ),
                Position = new Point3D( cameraX, cameraY, cameraZ )
            };
            Vp3D.Camera = MainCamera;
        }

        private void BuildCube()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CubeH

            ////////////////////////////////////////////////////////////////
            // front
            _cubeFrontMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( _meshWidth, 0, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( _meshWidth, _meshHeight, 0 ) );
            _cubeFrontMesh.Positions.Add( new Point3D( 0, _meshHeight, 0 ) );
            AddTriangleIndices( _cubeFrontMesh );
            AddTextureCoordinatesFront( _cubeFrontMesh );

            ////////////////////////////////////////////////////////////////
            // back
            _cubeBackMesh.Positions.Add( new Point3D( _meshWidth, 0, -_meshWidth ) );
            _cubeBackMesh.Positions.Add( new Point3D( 0, 0, -_meshWidth ) );
            _cubeBackMesh.Positions.Add( new Point3D( 0, _meshHeight, -_meshWidth ) );
            _cubeBackMesh.Positions.Add( new Point3D( _meshWidth, _meshHeight, -_meshWidth ) );
            AddTriangleIndices( _cubeBackMesh );
            AddTextureCoordinatesFront( _cubeBackMesh );

            ////////////////////////////////////////////////////////////////
            // left
            _cubeLeftMesh.Positions.Add( new Point3D( 0, 0, -_meshWidth ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, 0, 0 ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, _meshHeight, 0 ) );
            _cubeLeftMesh.Positions.Add( new Point3D( 0, _meshHeight, -_meshWidth ) );
            AddTriangleIndices( _cubeLeftMesh );
            AddTextureCoordinatesFront( _cubeLeftMesh );

            ////////////////////////////////////////////////////////////////
            // right
            _cubeRightMesh.Positions.Add( new Point3D( _meshWidth, 0, 0 ) );
            _cubeRightMesh.Positions.Add( new Point3D( _meshWidth, 0, -_meshWidth ) );
            _cubeRightMesh.Positions.Add( new Point3D( _meshWidth, _meshHeight, -_meshWidth ) );
            _cubeRightMesh.Positions.Add( new Point3D( _meshWidth, _meshHeight, 0 ) );
            AddTriangleIndices( _cubeRightMesh );
            AddTextureCoordinatesFront( _cubeRightMesh );

            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CubeV

            ////////////////////////////////////////////////////////////////
            // top
            _cubeTopMesh.Positions.Add( new Point3D( 0 * _meshWidth, 1 * _meshHeight, 0 ) );
            _cubeTopMesh.Positions.Add( new Point3D( 1 * _meshWidth, 1 * _meshHeight, 0 ) );
            _cubeTopMesh.Positions.Add( new Point3D( 1 * _meshWidth, 1 * _meshHeight, -_meshHeight ) );
            _cubeTopMesh.Positions.Add( new Point3D( 0 * _meshWidth, 1 * _meshHeight, -_meshHeight ) );
            AddTriangleIndices( _cubeTopMesh );
            AddTextureCoordinatesFront( _cubeTopMesh );

            ////////////////////////////////////////////////////////////////
            // bottom
            _cubeBottomMesh.Positions.Add( new Point3D( 0 * _meshWidth, 0 * _meshHeight, -_meshHeight ) );
            _cubeBottomMesh.Positions.Add( new Point3D( 1 * _meshWidth, 0 * _meshHeight, -_meshHeight ) );
            _cubeBottomMesh.Positions.Add( new Point3D( 1 * _meshWidth, 0 * _meshHeight, 0 ) );
            _cubeBottomMesh.Positions.Add( new Point3D( 0 * _meshWidth, 0 * _meshHeight, 0 ) );
            AddTriangleIndices( _cubeBottomMesh );
            AddTextureCoordinatesFront( _cubeBottomMesh );

            ////////////////////////////////////////////////////////////////
            // _cubeFront 永远显示当前桌面；其在动画中定格，动画结束后归位并继续截屏
            var frontMaterial = new DiffuseMaterial( new ImageBrush( (ImageSource)Resources[Front] ) );
            _cubeFront.Material = frontMaterial;

            ////////////////////////////////////////////////////////////////
            // 其他位面永远显示目标桌面，可以共享同一个材质；其在动画中持续截屏，动画结束后归位并停止截屏
            var othersMaterial = new DiffuseMaterial( new ImageBrush( (ImageSource)Resources[Others] ) );
            _cubeBack.Material = othersMaterial;
            _cubeLeft.Material = othersMaterial;
            _cubeRight.Material = othersMaterial;
            _cubeTop.Material = othersMaterial;
            _cubeBottom.Material = othersMaterial;

            // _cubeFront.BackMaterial = frontMaterial;
            // _cubeRight.BackMaterial = rightMaterial;
            // _cubeBack.BackMaterial = backMaterial;
            // _cubeLeft.BackMaterial = leftMaterial;
            // _cubeTop.BackMaterial = topMaterial;
            // _cubeBottom.BackMaterial = bottomMaterial;

            ////////////////////////////////////////////
            // set GeometryModel3D' mesh
            _cubeFront.Geometry = _cubeFrontMesh;
            _cubeBack.Geometry = _cubeBackMesh;
            _cubeLeft.Geometry = _cubeLeftMesh;
            _cubeRight.Geometry = _cubeRightMesh;
            _cubeTop.Geometry = _cubeTopMesh;
            _cubeBottom.Geometry = _cubeBottomMesh;

            ////////////////////////////////////////////
            // Model3D/Model3DGroup
            _cube.Children.Add( _cubeFront );
            _cube.Children.Add( _cubeBack );
            _cube.Children.Add( _cubeLeft );
            _cube.Children.Add( _cubeRight );
            _cube.Children.Add( _cubeTop );
            _cube.Children.Add( _cubeBottom );

            CubeModel3DGroup.Children.Add( _cube );
        }
    }
}