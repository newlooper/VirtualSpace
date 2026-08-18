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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Cube3D.Effects
{
    public abstract class Effect
    {
        protected static readonly double           MeshHeight  = SystemParameters.PrimaryScreenHeight / SystemParameters.PrimaryScreenWidth;
        protected static readonly double           MeshWidth   = 1.0;
        protected static readonly AmbientLight     CommonLight = new() {Color = Colors.White};
        protected readonly        Transform3DGroup TransGroup  = new();
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

        public abstract void AnimationInDirection( KeyCode dir, Model3DGroup model3DGroup, IEasingFunction ef = null );

        public void AddAnimationCompletedListener( EventHandler handler )
        {
            Animation.Completed += handler;
        }
    }

    public enum KeyCode
    {
        Left  = 0x25,
        Up    = 0x26,
        Right = 0x27,
        Down  = 0x28
    }

    public static class EffectFactory
    {
        public static readonly IReadOnlyList<Type> Types = new[]
        {
            typeof( Cube ),
            typeof( InsideCube ),
            typeof( Slide ),
            typeof( Reveal ),
            typeof( Fade ),
            typeof( Flip )
        };

        public static string Default { get; } = Types[0].Name;

        public static IReadOnlyList<string> Names { get; } =
            Types.Select( t => t.Name ).ToArray();

        public static Effect Create( string name )
        {
            foreach ( var type in Types )
            {
                if ( type.Name != name ) continue;
                return (Effect)Activator.CreateInstance( type );
            }

            return (Effect)Activator.CreateInstance( Types[0] );
        }

        internal static string NameFromLegacyIndex( int index )
        {
            if ( index < 0 || index >= Types.Count ) return Default;
            return Types[index].Name;
        }
    }

    public static class EaseFactory
    {
        public const string None = nameof( None );

        public static readonly IReadOnlyList<Type> Types = new[]
        {
            typeof( BackEase ),
            typeof( BounceEase ),
            typeof( CircleEase ),
            typeof( CubicEase ),
            typeof( ElasticEase ),
            typeof( ExponentialEase ),
            typeof( PowerEase ),
            typeof( QuadraticEase ),
            typeof( QuarticEase ),
            typeof( QuinticEase ),
            typeof( SineEase )
        };

        public static IReadOnlyList<string> Names { get; } =
            new[] { None }.Concat( Types.Select( t => t.Name ) ).ToArray();

        public static EasingFunctionBase GetEaseByName( string name, EasingMode mode )
        {
            if ( string.IsNullOrEmpty( name ) || name == None ) return null;

            foreach ( var type in Types )
            {
                if ( type.Name != name ) continue;
                var ef = (EasingFunctionBase)Activator.CreateInstance( type );
                ef.EasingMode = mode;
                return ef;
            }

            return null;
        }

        internal static string NameFromLegacyIndex( int index )
        {
            if ( index <= 0 || index > Types.Count ) return None;
            return Types[index - 1].Name;
        }
    }
}
