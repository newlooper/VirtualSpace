// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;
using VirtualSpace.Config;

namespace VirtualSpace.VirtualDesktop;

public static class Navigation
{
    public static int CalculateTargetIndex( int vdCount, int fromIndex, Keys dir, Config.Entity.Navigation nav )
    {
        if ( Manager.CurrentProfile.IsPsLayout )
        {
            return CalculatePsTargetIndex( fromIndex, dir, nav );
        }

        var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
        var maxIndex = vdCount - 1;

        var da = Manager.CurrentProfile.UI.DesktopArrangement;

        var currentRowCol = RowColFromIndex( rowsCols, fromIndex, da );
        var targetRowCol  = TargetRowColByDirection( rowsCols, currentRowCol, dir, currentRowCol, nav );
        var targetIndex   = IndexFromRowCol( rowsCols, targetRowCol, da );

        while ( targetIndex > maxIndex ) // 暴力应对目标桌面不存在的情况
        {
            targetRowCol = TargetRowColByDirection( rowsCols, targetRowCol, dir, currentRowCol, nav );
            targetIndex  = IndexFromRowCol( rowsCols, targetRowCol, da );
        }

        return IndexFromRowCol( rowsCols, targetRowCol, 0 );
    }

    private static int CalculatePsTargetIndex( int fromIndex, Keys dir, Config.Entity.Navigation nav )
    {
        var profile = Manager.CurrentProfile;
        var ps      = profile.PrimarySecondary;
        var order   = profile.DesktopOrder;
        if ( !PsLayout.IsValid( ps ) || order is null || fromIndex < 0 || fromIndex >= order.Count )
        {
            return fromIndex;
        }

        var fromGuid = order[fromIndex];
        var fromSlot = PsLayout.SlotOf( ps, fromGuid );
        if ( fromSlot < 0 )
        {
            return fromIndex;
        }

        var n     = ps.N;
        var map   = PsLayout.BuildCellToSlot( ps.Kind, n, ps.Corner );
        var start = PsLayout.SlotOrigin( ps.Kind, n, ps.Corner, fromSlot );
        var rc    = start;
        var guard = n * n + 1;
        for ( var step = 0; step < guard; step++ )
        {
            var next = TargetRowColByDirection( n, rc, dir, start, nav );
            if ( next == start && rc == start )
            {
                return fromSlot;
            }

            rc = next;
            if ( rc == start )
            {
                return fromSlot;
            }

            var slot = map[rc.R, rc.C];
            var guid = PsLayout.GuidAt( ps, slot );
            if ( guid is { } id && id != fromGuid )
            {
                return slot;
            }
        }

        return fromSlot;
    }

    /// <summary>
    ///     导航不受 DesktopArrangement 影响：在单位格子上按方向走，环绕规则与均等模式相同。
    /// </summary>
    private static (int R, int C) TargetRowColByDirection(
        int n, (int R, int C) currentRC, Keys direction, (int R, int C) validRC, Config.Entity.Navigation nav )
    {
        var r         = currentRC.R;
        var c         = currentRC.C;
        var targetRow = currentRC.R;
        var targetCol = currentRC.C;

        switch ( direction )
        {
            case Keys.Left:
                if ( c == 0 )
                {
                    if ( nav.CirculationH )
                    {
                        if ( nav.CirculationHType == Const.VirtualDesktop.NavHTypeNextRow )
                        {
                            targetRow = r == 0 ? n - 1 : r - 1;
                        }

                        targetCol = n - 1;
                    }
                    else
                    {
                        return validRC;
                    }
                }
                else
                {
                    targetCol--;
                }

                break;
            case Keys.Right:
                if ( c == n - 1 )
                {
                    if ( nav.CirculationH )
                    {
                        if ( nav.CirculationHType == Const.VirtualDesktop.NavHTypeNextRow )
                        {
                            targetRow = r == n - 1 ? 0 : r + 1;
                        }

                        targetCol = 0;
                    }
                    else
                    {
                        return validRC;
                    }
                }
                else
                {
                    targetCol++;
                }

                break;
            case Keys.Up:
                if ( r == 0 )
                {
                    if ( nav.CirculationV )
                    {
                        targetRow = n - 1;
                    }
                    else
                    {
                        return validRC;
                    }
                }
                else
                {
                    targetRow--;
                }

                break;
            case Keys.Down:
                if ( r == n - 1 )
                {
                    if ( nav.CirculationV )
                    {
                        targetRow = 0;
                    }
                    else
                    {
                        return validRC;
                    }
                }
                else
                {
                    targetRow++;
                }

                break;
        }

        return ( targetRow, targetCol );
    }

    public static int IndexFromRowCol( int n, (int R, int C) currentRC, int? desktopArrangement )
    {
        var r = currentRC.R;
        var c = currentRC.C;
        switch ( desktopArrangement )
        {
            case 0:
                // TopLeft To BottomRight H
                return r * n + c;
            case 1:
                // TopRight To BottomLeft H
                return r * n + ( n - 1 - c );
            case 2:
                // BottomLeft To TopRight H
                return ( n - 1 - r ) * n + c;
            case 3:
                // BottomRight To TopLeft H
                return ( n - 1 - r ) * n + ( n - 1 - c );
            case 4:
                // TopLeft To BottomRight V
                return c * n + r;
            case 5:
                // TopRight To BottomLeft V
                return ( n - 1 - c ) * n + r;
            case 6:
                // BottomLeft To TopRight V
                return c * n + ( n - 1 - r );
            case 7:
                // BottomRight To TopLeft V
                return ( n - 1 - c ) * n + ( n - 1 - r );
            default:
                // TopLeft To BottomRight H
                return r * n + c;
        }
    }

    public static (int R, int C) RowColFromIndex( int n, int logicIndex, int? desktopArrangement )
    {
        int row, col;
        switch ( desktopArrangement )
        {
            case 0:
                // TopLeft To BottomRight H
                row = logicIndex / n;
                col = logicIndex % n;
                break;
            case 1:
                // TopRight To BottomLeft H
                row = logicIndex / n;
                col = n - 1 - logicIndex % n;
                break;
            case 2:
                // BottomLeft To TopRight H
                row = n - 1 - logicIndex / n;
                col = logicIndex % n;
                break;
            case 3:
                // BottomRight To TopLeft H
                row = n - 1 - logicIndex / n;
                col = n - 1 - logicIndex % n;
                break;
            case 4:
                // TopLeft To BottomRight V
                row = logicIndex % n;
                col = logicIndex / n;
                break;
            case 5:
                // TopRight To BottomLeft V
                row = logicIndex % n;
                col = n - 1 - logicIndex / n;
                break;
            case 6:
                // BottomLeft To TopRight V
                row = n - 1 - logicIndex % n;
                col = logicIndex / n;
                break;
            case 7:
                // BottomRight To TopLeft V
                row = n - 1 - logicIndex % n;
                col = n - 1 - logicIndex / n;
                break;
            default:
                // TopLeft To BottomRight H
                row = logicIndex / n;
                col = logicIndex % n;
                break;
        }

        return ( row, col );
    }
}