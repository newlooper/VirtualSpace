/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text.Json;

namespace VirtualSpace.Config.Events.Entity
{
    public class RuleTemplate
    {
        public Func<Window, bool>? Exp;
        public Guid                Id         { get; set; } = Guid.NewGuid();
        public string?             Name       { get; set; }
        public JsonDocument?       Expression { get; set; }
        public Behavior?           Action     { get; set; }
        public bool                Enabled    { get; set; }
        public DateTime?           Created    { get; set; }
        public DateTime?           Updated    { get; set; }
    }

    public static class RuleFields
    {
        public const string Title       = nameof( Title );
        public const string ProcessName = nameof( ProcessName );
        public const string ProcessPath = nameof( ProcessPath );
        public const string WndClass    = nameof( WndClass );
        public const string WinInScreen = nameof( WinInScreen );
    }
}