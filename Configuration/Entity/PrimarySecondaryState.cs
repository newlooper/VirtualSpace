/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace VirtualSpace.Config.Entity;

public enum PsKind : byte
{
    None   = 0,
    Corner = 1,
    Center = 2
}

public enum PsCorner : byte
{
    NorthWest = 0,
    NorthEast = 1,
    SouthWest = 2,
    SouthEast = 3
}

public class PrimarySecondaryState
{
    public PsKind       Kind   { get; set; }
    public PsCorner     Corner { get; set; }
    public int          N      { get; set; }
    public List<Guid?>  Slots  { get; set; } = [];

    public bool IsActive => Kind is not PsKind.None;
}
