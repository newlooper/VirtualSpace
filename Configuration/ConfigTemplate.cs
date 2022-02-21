/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config
{
    public class ConfigTemplate
    {
        public Dictionary<string, Profile> Profiles           { get; set; }
        public string                      CurrentProfileName { get; set; }
        public string                      Version            { get; set; }
        public LogConfig                   LogConfig          { get; set; }
    }
}