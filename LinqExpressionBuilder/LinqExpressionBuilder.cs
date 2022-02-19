/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of LinqExpressionBuilder.

LinqExpressionBuilder is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

LinqExpressionBuilder is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LinqExpressionBuilder. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LinqExpressionBuilder
{
    public class JsonParser
    {
        private readonly MethodInfo _listContains = typeof( Enumerable )
            .GetMethods( BindingFlags.Static | BindingFlags.Public )
            .Single( m =>
                m.Name == nameof( Enumerable.Contains )
                && m.GetParameters().Length == 2 );

        private readonly MethodInfo? _regexIsMatch  = typeof( Regex ).GetMethod( nameof( Regex.IsMatch ), new Type[] {typeof( string ), typeof( string )} );
        private readonly MethodInfo? _strContains   = typeof( string ).GetMethod( nameof( string.Contains ), new Type[] {typeof( string )} );
        private readonly MethodInfo? _strEndsWith   = typeof( string ).GetMethod( nameof( string.EndsWith ), new Type[] {typeof( string )} );
        private readonly MethodInfo? _strStartsWith = typeof( string ).GetMethod( nameof( string.StartsWith ), new Type[] {typeof( string )} );

        private Expression? ParseTree( JsonElement condition, ParameterExpression param )
        {
            if ( condition.TryGetProperty( Keywords.Condition, out var combine ) )
            {
                var    gate = condition.GetProperty( Keywords.Condition ).GetString();
                Binder binder;

                if ( gate == Keywords.And )
                    binder = Expression.And;
                else
                    binder = Expression.Or;

                Expression? Bind( Expression? l, Expression? r )
                {
                    return l == null ? r : binder( l, r );
                }

                Expression? left  = null;
                var         rules = condition.GetProperty( Keywords.Rules );
                foreach ( var rule in rules.EnumerateArray() )
                {
                    if ( rule.TryGetProperty( Keywords.Condition, out var nested ) )
                    {
                        left = Bind( left, ParseTree( rule, param ) );
                    }
                    else
                    {
                        left = Bind( left, SimpleCondition( rule, param ) );
                    }
                }

                return left;
            }

            return SimpleCondition( condition, param );
        }

        private Expression? SimpleCondition( JsonElement rule, ParameterExpression param )
        {
            var @operator = rule.GetProperty( Keywords.Operator ).GetString().ToLower();
            var type      = rule.GetProperty( Keywords.Type ).GetString().ToLower();
            var field     = rule.GetProperty( Keywords.Field ).GetString();
            var value     = rule.GetProperty( Keywords.Value );

            var V = value.GetProperty( Keywords.In.Contains( @operator ) ? Keywords.L : Keywords.V );

            var    property = Expression.Property( param, field );
            object target;

            Expression? right = default;

            if ( Keywords.Eq.Contains( @operator ) )
            {
                if ( type == Keywords.String || type == Keywords.Boolean )
                    target = V.GetString();
                else
                    target = V.GetDecimal();
                var toCompare = Expression.Constant( target );
                right = Expression.Equal( property, toCompare );
            }
            else if ( Keywords.In.Contains( @operator ) )
            {
                var listContains = _listContains.MakeGenericMethod( typeof( string ) );
                target = V.EnumerateArray().Select( e => e.GetString() ).ToList();
                right = Expression.Call(
                    listContains,
                    Expression.Constant( target ),
                    property );
            }
            else if ( Keywords.StartsWith.Contains( @operator ) )
            {
                right = Expression.Call(
                    property,
                    _strStartsWith,
                    Expression.Constant( V.GetString() )
                );
            }
            else if ( Keywords.EndsWith.Contains( @operator ) )
            {
                right = Expression.Call(
                    property,
                    _strEndsWith,
                    Expression.Constant( V.GetString() )
                );
            }
            else if ( Keywords.Contains.Contains( @operator ) )
            {
                right = Expression.Call(
                    property,
                    _strContains,
                    Expression.Constant( V.GetString() )
                );
            }
            else if ( Keywords.RegexIsMatch.Contains( @operator ) )
            {
                right = Expression.Call(
                    _regexIsMatch,
                    property,
                    Expression.Constant( V.GetString() )
                );
            }

            return right;
        }

        private Expression<Func<T, bool>> BuildPredicate<T>( JsonDocument doc )
        {
            var itemTypeExpr = Expression.Parameter( typeof( T ) );
            var conditions   = ParseTree( doc.RootElement, itemTypeExpr );
            if ( conditions.CanReduce )
            {
                conditions = conditions.ReduceAndCheck();
            }

            return Expression.Lambda<Func<T, bool>>( conditions, itemTypeExpr );
        }

        public Func<T, bool> ExpressionFromJsonDoc<T>( JsonDocument doc )
        {
            return BuildPredicate<T>( doc ).Compile();
        }

        private delegate Expression? Binder( Expression left, Expression right );
    }
}