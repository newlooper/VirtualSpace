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
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
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
        var commonSize = GetCommonVdwSize();
        var existing   = _virtualDesktops.ToDictionary( v => v.VdId );
        var survival   = new List<VirtualDesktopWindow>( DesktopWrapper.Count );

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
                vdw = VirtualDesktopWindow.Create( index, guid, commonSize, _vdwDefaultBackColor, Ui.VDWPadding );
            }

            survival.Add( vdw );
        }

        foreach ( var old in existing.Values )
        {
            old.RealClose();
        }

        _virtualDesktops = survival; // system vd list order at this moment
        ReOrder(); // reorder by profile
    }

    private static Size GetCommonVdwSize()
    {
        var dpi       = SysInfo.Dpi;
        var size      = MainWindow.GetCellSizeByMatrixIndex( 0 );
        var vdwWidth  = ( size.Width - 2 * Ui.VDWBorderSize ) * dpi.ScaleX + 1;
        var vdwHeight = ( size.Height - 2 * Ui.VDWBorderSize ) * dpi.ScaleY + 1;
        return new Size( (int)vdwWidth, (int)vdwHeight );
    }

    private static void ReOrder( bool needSort = false )
    {
        if ( needSort )
        {
            _virtualDesktops.Sort( ( x, y ) => x.VdIndex.CompareTo( y.VdIndex ) );
        }

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

    private static void UpdateMainView( VirtualDesktopNotification? vdn = null )
    {
        if ( !MainWindow.IsShowing() )
        {
            return;
        }

        FixLayout();
        ShowAllVirtualDesktops();

        if ( vdn is null )
        {
            return;
        }

        try
        {
            var fallback = _virtualDesktops[GetVdIndexByGuid( vdn.NewId )];
            ShowVisibleWindowsForDesktops( [fallback] );
        }
        catch ( Exception e )
        {
            Logger.Warning( "Update MainView: " + e.StackTrace );
        }
    }

    public static void FixLayout()
    {
        try
        {
            MainWindow.ResetMainGrid();
        }
        catch
        {
            MainWindow.NotifyDesktopManagerReset();
            return;
        }

        SyncVirtualDesktops();
    }

    public static async Task InitLayout()
    {
        MainWindow.ResetMainGrid();

        var commonSize = GetCommonVdwSize();

        var tasks = new List<Task>();
        for ( var i = 0; i < DesktopWrapper.Count; i++ )
        {
            var index = i;
            tasks.Add( Task.Run( () =>
            {
                var guid = DesktopManagerWrapper.GetIdByIndex( index );
                var vdw  = VirtualDesktopWindow.Create( index, guid, commonSize, _vdwDefaultBackColor, Ui.VDWPadding );

                lock ( _virtualDesktops ) // thread safe
                {
                    _virtualDesktops.Add( vdw ); // added in random order, need call "ReOrder( true )" afterwards
                }
            } ) );
        }

        try
        {
            await Task.WhenAll( tasks.ToArray() );
        }
        catch ( Exception ex )
        {
            Logger.Error( "Init Layout: " + ex.Message );
            return;
        }

        ReOrder( true );
    }

    public static void UpdateVdwBackground()
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
        return ( from vdw in _virtualDesktops where vdw.VdId == guid select vdw.VdIndex ).FirstOrDefault();
    }

    public static void Bootstrap()
    {
        _vdwDefaultBackColor = Color.FromArgb( Ui.VDWDefaultBackColor!.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B );
    }
}