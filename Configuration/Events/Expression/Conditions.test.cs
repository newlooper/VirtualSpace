/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text.Json;
using LinqExpressionBuilder;
using VirtualSpace.Config.Events.Entity;

namespace VirtualSpace.Config.Events.Expression
{
    public static partial class Conditions
    {
        private static void TestData( string path )
        {
            var test1 = new ExpressionTemplate
            {
                field = RuleFields.Title,
                @operator = Keywords.EndsWith[0],
                type = Keywords.String,
                value = new Value {V = "Notepad3"}
            };
            var test2 = new ExpressionTemplate
            {
                condition = Keywords.Or,
                rules = new List<ExpressionTemplate>
                {
                    new()
                    {
                        field = RuleFields.Title,
                        @operator = Keywords.EndsWith[0],
                        type = Keywords.String,
                        value = new Value {V = "Notepad3"}
                    },
                    new()
                    {
                        field = RuleFields.Title,
                        @operator = Keywords.Contains[0],
                        type = Keywords.String,
                        value = new Value {V = "炉石传说"}
                    }
                }
            };

            var writeOptions = GetJsonSerializerOptions();

            var tempE1 = new RuleTemplate
            {
                Name = "test1",
                Expression = JsonDocument.Parse( JsonSerializer.Serialize( test1, writeOptions ) ),
                Action = new Behavior {MoveToDesktop = 1},
                Enabled = true,
                Created = new DateTime( 2021, 10, 11 )
            };
            var tempE2 = new RuleTemplate
            {
                Name = "test2",
                Expression = JsonDocument.Parse( JsonSerializer.Serialize( test2, writeOptions ) ),
                Action = new Behavior {MoveToDesktop = 2},
                Enabled = true,
                Created = DateTime.Now
            };

            SaveRules( path, new List<RuleTemplate> {tempE1, tempE2} );
        }
    }
}