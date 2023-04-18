/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text.Json;
using VirtualSpace.Config.DataAnnotations;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config
{
    public class Profile
    {
        [PropertyProtector] public UserInterface UI                             { get; set; }
        public                     bool          DaemonAutoStart                { get; set; }
        public                     List<Guid>?   DesktopOrder                   { get; set; }
        [PropertyProtector] public Mouse         Mouse                          { get; set; }
        public                     bool          IgnoreWindowOnRuleCheckTimeout { get; set; } = true;

        public Navigation Navigation { get; set; } = new()
        {
            CirculationH = false,
            CirculationV = false,
            CirculationHType = 0
        };

        public Profile Clone()
        {
            var profile = JsonSerializer.Deserialize<Profile>( JsonSerializer.Serialize( this ) );
            return profile!;
        }
    }
}