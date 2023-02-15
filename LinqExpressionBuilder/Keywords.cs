/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of LinqExpressionBuilder.

LinqExpressionBuilder is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

LinqExpressionBuilder is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with LinqExpressionBuilder. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;

namespace LinqExpressionBuilder
{
    public static class Keywords
    {
        public const           string   String       = "string";
        public const           string   V            = nameof( V );
        public const           string   L            = nameof( L );
        public static readonly string   Operator     = nameof( Operator ).ToLower();
        public static readonly string   Type         = nameof( Type ).ToLower();
        public static readonly string   Field        = nameof( Field ).ToLower();
        public static readonly string   Value        = nameof( Value ).ToLower();
        public static readonly string[] Eq           = {"=", "is", "==", "eq", "equal", "equals"};
        public static readonly string[] In           = {"in", "∈"};
        public static readonly string[] StartsWith   = {"ssw", "starts with", "|-"};
        public static readonly string[] EndsWith     = {"esw", "ends with", "-|"};
        public static readonly string[] Contains     = {"sc", "contains", "∋"};
        public static readonly string[] RegexIsMatch = {"rim", "regex", "/r/"};
        public static readonly string   Condition    = nameof( Condition ).ToLower();
        public static readonly string   And          = nameof( And ).ToLower();
        public static readonly string   Or           = nameof( Or ).ToLower();
        public static readonly string   Rules        = nameof( Rules ).ToLower();
        public static readonly string   Boolean      = nameof( Boolean ).ToLower();
        public static readonly string   Number       = nameof( Number ).ToLower();
        public static readonly string   Id           = nameof( Id ).ToLower();
    }
}