/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config;

/// <summary>
/// Closed-form primary/secondary board math. Slot 0 is the k×k primary; smalls follow scan order.
/// Corner: K = 2N (N ≥ 3). Center: K = 4N − 3 (N ≥ 4).
/// </summary>
public static class PsLayout
{
    public static int Capacity( PsKind kind, int n ) => kind switch
    {
        PsKind.Corner => 2 * n,
        PsKind.Center => 4 * n - 3,
        _             => n * n
    };

    public static int NFromCapacity( PsKind kind, int k ) => kind switch
    {
        PsKind.Corner => k / 2,
        PsKind.Center => ( k + 3 ) / 4,
        _             => (int)Math.Ceiling( Math.Sqrt( k ) )
    };

    public static int NextCornerK( int kPrime )
    {
        if ( kPrime <= 6 )
        {
            return 6;
        }

        return kPrime + ( kPrime & 1 );
    }

    public static int NextCenterK( int kPrime )
    {
        if ( kPrime <= 13 )
        {
            return 13;
        }

        return 13 + 4 * (int)Math.Ceiling( ( kPrime - 13 ) / 4.0 );
    }

    public static int NextK( int kPrime ) => Math.Min( NextCornerK( kPrime ), NextCenterK( kPrime ) );

    public static (PsKind Kind, int N, int K) Fit( int needed )
    {
        var k = NextK( needed < 1 ? 1 : needed );
        if ( !TryKindFromK( k, out var kind ) )
        {
            return ( PsKind.Corner, 3, 6 );
        }

        return ( kind, NFromCapacity( kind, k ), k );
    }

    public static int NextSameTypeK( PsKind kind, int currentK ) => kind switch
    {
        PsKind.Corner => currentK + 2,
        PsKind.Center => currentK + 4,
        _             => currentK
    };

    public static bool TryKindFromK( int k, out PsKind kind )
    {
        if ( k >= 6 && ( k & 1 ) == 0 )
        {
            kind = PsKind.Corner;
            return true;
        }

        if ( k >= 13 && ( k - 13 ) % 4 == 0 )
        {
            kind = PsKind.Center;
            return true;
        }

        kind = PsKind.None;
        return false;
    }

    public static bool IsValid( PrimarySecondaryState ps )
    {
        return ps.Kind switch
        {
            PsKind.Corner => ps.N >= 3,
            PsKind.Center => ps.N >= 4,
            _             => false
        };
    }

    public static (int Row, int Col, int Span) PrimaryRect( PsKind kind, int n, PsCorner corner )
    {
        var span = kind == PsKind.Center ? n - 2 : n - 1;
        if ( kind == PsKind.Center )
        {
            return ( 1, 1, span );
        }

        return corner switch
        {
            PsCorner.NorthEast => ( 0, n - span, span ),
            PsCorner.SouthWest => ( n - span, 0, span ),
            PsCorner.SouthEast => ( n - span, n - span, span ),
            _                  => ( 0, 0, span )
        };
    }

    public static List<(int R, int C)> SmallCells( PsKind kind, int n, PsCorner corner )
    {
        var cells = new List<(int R, int C)>( Capacity( kind, n ) - 1 );
        if ( kind == PsKind.Center )
        {
            for ( var c = 0; c < n; c++ )
            {
                cells.Add( ( 0, c ) );
            }

            for ( var r = 1; r < n; r++ )
            {
                cells.Add( ( r, n - 1 ) );
            }

            for ( var c = n - 2; c >= 0; c-- )
            {
                cells.Add( ( n - 1, c ) );
            }

            for ( var r = n - 2; r >= 1; r-- )
            {
                cells.Add( ( r, 0 ) );
            }

            return cells;
        }

        var freeRow = corner is PsCorner.NorthWest or PsCorner.NorthEast ? n - 1 : 0;
        var freeCol = corner is PsCorner.NorthWest or PsCorner.SouthWest ? n - 1 : 0;
        for ( var c = 0; c < n; c++ )
        {
            cells.Add( ( freeRow, c ) );
        }

        for ( var r = 0; r < n; r++ )
        {
            if ( r == freeRow )
            {
                continue;
            }

            cells.Add( ( r, freeCol ) );
        }

        return cells;
    }

    public static int[,] BuildCellToSlot( PsKind kind, int n, PsCorner corner )
    {
        var map = new int[n, n];
        var (pr, pc, span) = PrimaryRect( kind, n, corner );
        for ( var r = pr; r < pr + span; r++ )
        for ( var c = pc; c < pc + span; c++ )
        {
            map[r, c] = 0;
        }

        var slot = 1;
        foreach ( var (r, c) in SmallCells( kind, n, corner ) )
        {
            map[r, c] = slot++;
        }

        return map;
    }

    public static (int R, int C) SlotOrigin( PsKind kind, int n, PsCorner corner, int slot )
    {
        if ( slot <= 0 )
        {
            var (r, c, _) = PrimaryRect( kind, n, corner );
            return ( r, c );
        }

        var smalls = SmallCells( kind, n, corner );
        var i      = slot - 1;
        return i >= 0 && i < smalls.Count ? smalls[i] : ( 0, 0 );
    }

    public static void EnsureSlotCount( PrimarySecondaryState ps )
    {
        var cap = Capacity( ps.Kind, ps.N );
        ps.Slots ??= [];
        while ( ps.Slots.Count < cap )
        {
            ps.Slots.Add( null );
        }

        if ( ps.Slots.Count > cap )
        {
            ps.Slots.RemoveRange( cap, ps.Slots.Count - cap );
        }
    }

    public static int SlotOf( PrimarySecondaryState ps, Guid id )
    {
        var slots = ps.Slots;
        if ( slots is null )
        {
            return -1;
        }

        for ( var i = 0; i < slots.Count; i++ )
        {
            if ( slots[i] == id )
            {
                return i;
            }
        }

        return -1;
    }

    public static Guid? GuidAt( PrimarySecondaryState ps, int slot )
    {
        if ( ps.Slots is null || (uint)slot >= (uint)ps.Slots.Count )
        {
            return null;
        }

        var g = ps.Slots[slot];
        return g is { } id && id != Guid.Empty ? id : null;
    }

    public static string LayoutKey( PrimarySecondaryState? ps, int vdCount, bool isPsLayout )
    {
        if ( !isPsLayout || ps is null || !IsValid( ps ) )
        {
            return $"eq-{vdCount}";
        }

        return $"ps-{(int)ps.Kind}-{ps.N}-{(int)ps.Corner}";
    }
}
