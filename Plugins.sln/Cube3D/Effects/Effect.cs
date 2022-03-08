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
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Cube3D.Effects
{
    public abstract class Effect
    {
        protected static readonly double           MeshHeight = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
        protected static readonly double           MeshWidth  = 1.0;
        protected readonly        Transform3DGroup TransGroup = new();
        protected                 Timeline         Animation;
        protected                 Transform3D      Transform3D;

        protected static void AddTriangleIndices( MeshGeometry3D meshGeometry3D )
        {
            meshGeometry3D.TriangleIndices.Add( 0 );
            meshGeometry3D.TriangleIndices.Add( 1 );
            meshGeometry3D.TriangleIndices.Add( 2 );
            meshGeometry3D.TriangleIndices.Add( 2 );
            meshGeometry3D.TriangleIndices.Add( 3 );
            meshGeometry3D.TriangleIndices.Add( 0 );
        }

        protected static void AddTextureCoordinatesFront( MeshGeometry3D meshGeometry3D )
        {
            meshGeometry3D.TextureCoordinates.Add( new Point( 0, 1 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 1, 1 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 1, 0 ) );
            meshGeometry3D.TextureCoordinates.Add( new Point( 0, 0 ) );
        }

        public abstract void Build( Model3DGroup model3DGroup );

        public abstract void AnimationInDirection( KeyCode dir, Model3DGroup model3DGroup );

        public void AddAnimationCompletedListener( EventHandler handler )
        {
            Animation.Completed += handler;
        }
    }

    public enum EffectType
    {
        Cube,
        InsideCube,
        Slide,
        Reveal,
        Fade,
        Flip
    }

    public enum KeyCode
    {
        Left  = 0x25,
        Up    = 0x26,
        Right = 0x27,
        Down  = 0x28
    }
}