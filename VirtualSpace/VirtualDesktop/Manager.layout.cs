/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config.Entity;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop;

internal static partial class VirtualDesktopManager
{
    private static Color         _vdwDefaultBackColor;
    private static List<Guid>    _lastDesktopOrder = [];
    public static  UserInterface Ui => ConfigManager.CurrentProfile.UI;

    private static void SyncVirtualDesktops()
    {
        var existing = _virtualDesktops.ToDictionary( v => v.VdId );
        var survival = new List<VirtualDesktopWindow>( DesktopWrapper.Count );

        for ( var index = 0; index < DesktopWrapper.Count; index++ ) // build new list according to current system vd list
        {
            var guid = DesktopManagerWrapper.GetIdByIndex( index );
            if ( guid == Guid.Empty )
            {
                continue;
            }

            if ( existing.Remove( guid, out var vdw ) )
            {
                vdw.VdIndex = index;
            }
            else
            {
                vdw = VirtualDesktopWindow.Create( index, guid, _vdwDefaultBackColor, Ui.VDWPadding );
            }

            survival.Add( vdw );
        }

        foreach ( var old in existing.Values )
        {
            if ( ReferenceEquals( old, _onlyOneInMainView ) )
            {
                _onlyOneInMainView = null;
            }

            old.RealClose();
        }

        _virtualDesktops = survival; // system vd list order at this moment
        ReOrder(); // reorder by profile
    }

    private static void ReOrder()
    {
        var profile = ConfigManager.CurrentProfile;
        var byGuid  = _virtualDesktops.ToDictionary( vdw => vdw.VdId );

        if ( profile.DesktopOrder == null || profile.DesktopOrder.Count == 0 ) // no custom order, using system's
        {
            SaveOrder( [.. _virtualDesktops.Select( vdw => vdw.VdId )] );
            return;
        }

        profile.DesktopOrder.RemoveAll( g => !byGuid.ContainsKey( g ) );

        var orderedByProfile = new List<VirtualDesktopWindow>( byGuid.Count );
        foreach ( var guid in profile.DesktopOrder )
        {
            if ( !byGuid.Remove( guid, out var vdw ) )
            {
                continue;
            }

            vdw.VdIndex = orderedByProfile.Count;
            orderedByProfile.Add( vdw );
        }

        // remaining entries keep system relative order (dictionary insertion order)
        foreach ( var restVdw in byGuid.Values )
        {
            restVdw.VdIndex = orderedByProfile.Count;
            orderedByProfile.Add( restVdw );
            profile.DesktopOrder.Add( restVdw.VdId );
        }

        _virtualDesktops = orderedByProfile;
        SaveOrder();
    }

    private static void VdCountChangedHandler( VirtualDesktopNotification? vdn = null )
    {
        var exitOnlyOne = TryExitOnlyOneInMainView();

        FixLayout();

        if ( !MainWindow.IsShowing() )
        {
            return;
        }

        ShowAllVirtualDesktops();

        if ( exitOnlyOne )
        {
            ShowVisibleWindowsForDesktops();
            return;
        }

        if ( vdn is null )
        {
            return;
        }

        try
        {
            var idx = GetVdIndexByGuid( vdn.NewId );
            if ( idx < 0 || idx >= _virtualDesktops.Count )
            {
                return;
            }

            ShowVisibleWindowsForDesktops( [_virtualDesktops[idx]] );
        }
        catch ( Exception e )
        {
            Logger.Warning( "Refresh Thumbs due to VdCountChange: " + e.StackTrace );
        }
    }

    public static void FixLayout()
    {
        try
        {
            SyncVirtualDesktops();
            ReconcilePrimarySecondary();
            MainWindow.ResetMainGrid();
        }
        catch
        {
            MainWindow.NotifyDesktopManagerReset();
        }
    }

    public static void InitLayout()
    {
        MainWindow.ResetMainGrid();

        try
        {
            for ( var i = 0; i < DesktopWrapper.Count; i++ )
            {
                var guid = DesktopManagerWrapper.GetIdByIndex( i );
                _virtualDesktops.Add( VirtualDesktopWindow.Create( i, guid, _vdwDefaultBackColor, Ui.VDWPadding ) );
            }
        }
        catch ( Exception ex )
        {
            Logger.Error( "Init Layout: " + ex.Message );
            return;
        }

        ReOrder();
        ReconcilePrimarySecondary();
        MainWindow.ResetMainGrid();
    }

    public static void UpdateVdwBorder()
    {
        MainWindow.RenderCellBorder();
    }

    public static void SaveOrder( List<Guid>? newOrder = null )
    {
        if ( newOrder != null )
        {
            ConfigManager.CurrentProfile.DesktopOrder = newOrder;
        }

        if ( IsSameGuidList( _lastDesktopOrder, ConfigManager.CurrentProfile.DesktopOrder! ) )
        {
            return;
        }

        _lastDesktopOrder = [.. ConfigManager.CurrentProfile.DesktopOrder!];
        ConfigManager.Save( reason: "sync&save", reasonName: "ConfigManager.CurrentProfile.DesktopOrder" );

        return;

        static bool IsSameGuidList( List<Guid> a, List<Guid> b )
        {
            if ( a.Count != b.Count )
            {
                return false;
            }

            return !a.Where( ( t, i ) => t != b[i] ).Any();
        }
    }

    public static int GetVdIndexByGuid( Guid guid )
    {
        foreach ( var vdw in _virtualDesktops )
        {
            if ( vdw.VdId == guid )
            {
                return vdw.VdIndex;
            }
        }

        return -1;
    }

    public static void Bootstrap()
    {
        _vdwDefaultBackColor = Color.FromArgb( Ui.VDWDefaultBackColor!.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B );
    }
}