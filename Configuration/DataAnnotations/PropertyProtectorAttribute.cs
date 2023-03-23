// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;

namespace VirtualSpace.Config.DataAnnotations
{
    public class PropertyProtectorAttribute : Attribute
    {
        public object[] Values { get; private set; }

        public PropertyProtectorAttribute()
        {
        }

        public PropertyProtectorAttribute( byte defaultV, byte min )
        {
            Values = new object[] {defaultV, min};
        }

        public PropertyProtectorAttribute( int defaultV, int min, int max )
        {
            Values = new object[] {defaultV, min, max};
        }

        public PropertyProtectorAttribute( long defaultV, long min, long max )
        {
            Values = new object[] {defaultV, min, max};
        }
    }

    public abstract class PropertyProtector
    {
        public static void Walk( object obj )
        {
            var props = from prop in obj.GetType().GetProperties()
                let attrs = prop.GetCustomAttributes( typeof( PropertyProtectorAttribute ), false )
                where attrs.Any()
                select new {Obj = obj, Property = prop, Attr = (PropertyProtectorAttribute)attrs.First()};

            foreach ( var pair in props )
            {
                if ( pair.Attr.Values is null ) // an object that have some properties which modified by [PropertyProtector( xxx )]
                {
                    var type = pair.Property.PropertyType;
                    if ( type.FullName.StartsWith( "System.Collections.Generic.Dictionary" ) ) // for collection Type, only Dictionary supported for now
                    {
                        var dict   = pair.Property.GetValue( pair.Obj );
                        var values = ( (dynamic)dict ).Values;
                        foreach ( var v in values )
                        {
                            Walk( v );
                        }
                    }
                    else
                    {
                        Walk( pair.Property.GetValue( pair.Obj ) ); // a class instance
                    }
                }
                else
                {
                    //////////////////////////////////////////////
                    // if validation fail, reset to default value
                    // only support byte, int, long for now
                    switch ( pair.Attr.Values[0] )
                    {
                        case byte:
                        {
                            var current = (byte)pair.Property.GetValue( obj );
                            if ( current < (byte)pair.Attr.Values[1] || current > byte.MaxValue )
                            {
                                pair.Property.SetValue( obj, pair.Attr.Values[0], null );
                            }

                            break;
                        }

                        case int:
                        {
                            var current = (int)pair.Property.GetValue( obj );
                            if ( current < (int)pair.Attr.Values[1] || current > (int)pair.Attr.Values[2] )
                            {
                                pair.Property.SetValue( obj, pair.Attr.Values[0], null );
                            }

                            break;
                        }

                        case long:
                        {
                            var current = (long)pair.Property.GetValue( obj );
                            if ( current < (long)pair.Attr.Values[1] || current > (long)pair.Attr.Values[2] )
                            {
                                pair.Property.SetValue( obj, pair.Attr.Values[0], null );
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}