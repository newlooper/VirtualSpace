/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using ControlPanel.Pages;
using Microsoft.Extensions.Logging;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

internal static partial class VirtualDesktopManager
{
    public static void ApplyPrimarySecondaryLayout( bool createExtras, string from = "apply" )
    {
        var profile = ConfigManager.CurrentProfile;
        if ( !profile.IsPsLayout )
        {
            return;
        }

        var ps = profile.PrimarySecondary ??= new PrimarySecondaryState();
        TryFit( ps, DesktopWrapper.Count );
        if ( !PsLayout.IsValid( ps ) )
        {
            profile.Layout = LayoutKind.Auto;
            ConfigManager.Save( reason: "invalid primary/secondary layout", reasonName: "Layout", logLevel: LogLevel.Trace );
            MainWindow.ResetMainGrid();
            if ( MainWindow.IsShowing() )
            {
                ShowAllVirtualDesktops();
            }

            return;
        }

        var cap  = PsLayout.Capacity( ps.Kind, ps.N );
        var need = cap - DesktopWrapper.Count;
        if ( createExtras && need > 0 )
        {
            IsBatchCreate = true;
            try
            {
                for ( var i = 0; i < need; i++ )
                {
                    DesktopWrapper.Create();
                }
            }
            finally
            {
                IsBatchCreate = false;
            }
        }

        SyncVirtualDesktops();
        TryFit( ps, DesktopWrapper.Count );
        SeedFromCurrent( ps );
        ConfigManager.Save( reason: $"{from} primary/secondary layout", reasonName: "Layout", logLevel: LogLevel.Information );

        MainWindow.ResetMainGrid();
        if ( MainWindow.IsShowing() )
        {
            ShowAllVirtualDesktops();
        }
    }

    public static void ToggleLayout()
    {
        var profile = ConfigManager.CurrentProfile;
        if ( profile.IsPsLayout )
        {
            profile.Layout = LayoutKind.Auto;
            ConfigManager.Save( reason: "toggle automatic layout", reasonName: "Layout", logLevel: LogLevel.Information );
            MainWindow.ResetMainGrid();
            if ( MainWindow.IsShowing() )
            {
                ShowAllVirtualDesktops();
            }
        }
        else
        {
            profile.Layout = LayoutKind.PrimarySecondary;
            ApplyPrimarySecondaryLayout( false, "toggle" );
        }

        UI.SyncLayoutSelection();
    }

    private static bool TryFit( PrimarySecondaryState ps, int needed )
    {
        var (kind, n, _) = PsLayout.Fit( needed );
        if ( ps.Kind == kind && ps.N == n )
        {
            return false;
        }

        ps.Kind = kind;
        ps.N    = n;
        return true;
    }

    private static void ReconcilePrimarySecondary()
    {
        var profile = ConfigManager.CurrentProfile;
        var ps      = profile.PrimarySecondary;
        if ( !profile.IsPsLayout || ps is null )
        {
            return;
        }

        var order = AliveOrder( profile );
        if ( TryFit( ps, order.Count ) )
        {
            SeedFromCurrent( ps );
            ConfigManager.Save( reason: "fit primary/secondary layout", reasonName: "Layout", logLevel: LogLevel.Trace );
            return;
        }

        PsLayout.EnsureSlotCount( ps );
        var alive = order.ToHashSet();
        var dirty = false;

        for ( var i = 0; i < ps.Slots.Count; i++ )
        {
            if ( ps.Slots[i] is not { } id || alive.Contains( id ) )
            {
                continue;
            }

            ps.Slots[i] = null;
            dirty       = true;
        }

        foreach ( var guid in order )
        {
            if ( PsLayout.SlotOf( ps, guid ) >= 0 )
            {
                continue;
            }

            var empty = FirstEmptySlot( ps );
            if ( empty < 0 )
            {
                TryFit( ps, order.Count );
                SeedFromCurrent( ps );
                ConfigManager.Save( reason: "fit primary/secondary layout", reasonName: "Layout", logLevel: LogLevel.Trace );
                return;
            }

            ps.Slots[empty] = guid;
            dirty           = true;
        }

        if ( dirty )
        {
            ConfigManager.Save( reason: "reconcile primary/secondary occupancy", reasonName: "Layout", logLevel: LogLevel.Trace );
        }
    }

    public static bool IsPsPrimaryDesktop( Guid vdId )
    {
        var profile = ConfigManager.CurrentProfile;
        var ps      = profile.PrimarySecondary;
        return profile.IsPsLayout && ps is not null && PsLayout.SlotOf( ps, vdId ) == 0;
    }

    public static bool RelocateDesktop( Guid vdId, int targetSlot )
    {
        var ps = ConfigManager.CurrentProfile.PrimarySecondary;

        PsLayout.EnsureSlotCount( ps );
        var from = PsLayout.SlotOf( ps, vdId );
        if ( from < 0 || targetSlot < 0 || targetSlot >= ps.Slots.Count || from == targetSlot )
        {
            return false;
        }

        ( ps.Slots[from], ps.Slots[targetSlot] ) = ( ps.Slots[targetSlot], ps.Slots[from] );
        ConfigManager.Save( reason: "relocate primary/secondary slot", reasonName: "Layout", logLevel: LogLevel.Trace );
        if ( MainWindow.IsShowing() )
        {
            ShowAllVirtualDesktops();
        }

        return true;
    }

    public static bool TryGetGuidByMatrixIndex( int matrixIndex, out Guid guid )
    {
        guid = Guid.Empty;
        var profile = ConfigManager.CurrentProfile;
        if ( profile.IsPsLayout )
        {
            var id = PsLayout.GuidAt( profile.PrimarySecondary, matrixIndex );
            if ( id is null )
            {
                return false;
            }

            guid = id.Value;
            return true;
        }

        var vdIndex = GetVdIndexByMatrixIndex( matrixIndex );
        if ( vdIndex < 0 || profile.DesktopOrder is null || vdIndex >= profile.DesktopOrder.Count )
        {
            return false;
        }

        guid = profile.DesktopOrder[vdIndex];
        return guid != Guid.Empty;
    }

    private static void SeedFromCurrent( PrimarySecondaryState ps )
    {
        var cap   = PsLayout.Capacity( ps.Kind, ps.N );
        var order = AliveOrder( ConfigManager.CurrentProfile );
        var slots = new List<Guid?>( cap );
        for ( var i = 0; i < cap; i++ )
        {
            slots.Add( null );
        }

        var current = DesktopWrapper.CurrentGuid;
        if ( current != Guid.Empty )
        {
            slots[0] = current;
        }

        var slot = 1;
        foreach ( var guid in order )
        {
            if ( guid == current || slot >= cap )
            {
                continue;
            }

            slots[slot++] = guid;
        }

        if ( slots[0] is null && order.Count > 0 && slot > 1 )
        {
            slots[0] = slots[1];
            slots[1] = null;
        }

        ps.Slots = slots;
    }

    private static int FirstEmptySlot( PrimarySecondaryState ps )
    {
        for ( var i = 0; i < ps.Slots.Count; i++ )
        {
            if ( ps.Slots[i] is not { } id || id == Guid.Empty )
            {
                return i;
            }
        }

        return -1;
    }

    private static List<Guid> AliveOrder( Profile profile )
    {
        if ( profile.DesktopOrder is { Count: > 0 } )
        {
            return [.. profile.DesktopOrder];
        }

        var list = new List<Guid>( DesktopWrapper.Count );
        for ( var i = 0; i < DesktopWrapper.Count; i++ )
        {
            var id = DesktopManagerWrapper.GetIdByIndex( i );
            if ( id != Guid.Empty )
            {
                list.Add( id );
            }
        }

        return list;
    }
}