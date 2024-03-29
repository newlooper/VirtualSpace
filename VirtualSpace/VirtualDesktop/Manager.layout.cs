﻿/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

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

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        private static Color         _vdwDefaultBackColor;
        public static  UserInterface Ui => ConfigManager.CurrentProfile.UI;

        private static void SyncVirtualDesktops()
        {
            var commonSize = GetCommonVdwSize();

            var survivalDesktops = new List<VirtualDesktopWindow>();
            for ( var index = 0; index < DesktopWrapper.Count; index++ ) // build new list according to current system vd list
            {
                var guid = DesktopManagerWrapper.GetIdByIndex( index );
                if ( guid == default ) continue;
                var survival = _virtualDesktops.Find( v => v.VdId == guid );
                if ( survival == null )
                {
                    survival = VirtualDesktopWindow.Create( index, guid, commonSize, _vdwDefaultBackColor, Ui.VDWPadding );
                }
                else
                {
                    survival.VdIndex = index;
                }

                survivalDesktops.Add( survival );
            }

            var sysGuids = survivalDesktops.Select( v => v.VdId ).ToList();
            foreach ( var old in _virtualDesktops.Where( old => !sysGuids.Contains( old.VdId ) ) )
            {
                old.RealClose();
            }

            _virtualDesktops = survivalDesktops; // system vd list order at this moment

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
                _virtualDesktops.Sort( ( x, y ) => x.VdIndex.CompareTo( y.VdIndex ) );

            var profile  = ConfigManager.CurrentProfile;
            var sysGuids = _virtualDesktops.Select( vdw => vdw.VdId ).ToList();

            if ( profile.DesktopOrder == null || profile.DesktopOrder.Count == 0 ) // no custom order, using system's
            {
                SaveOrder( sysGuids );
                return;
            }

            profile.DesktopOrder.RemoveAll( g => !sysGuids.Contains( g ) );

            var orderedByProfile = new List<VirtualDesktopWindow>();
            for ( var idx = 0; idx < profile.DesktopOrder.Count; idx++ )
            {
                var vdw = _virtualDesktops.Find( vdw => vdw.VdId == profile.DesktopOrder[idx] );
                if ( vdw is null ) continue;
                vdw.VdIndex = idx; // reposition
                orderedByProfile.Add( vdw );
                _virtualDesktops.Remove( vdw );
            }

            foreach ( var restVdw in _virtualDesktops )
            {
                restVdw.VdIndex = orderedByProfile.Count;
                orderedByProfile.Add( restVdw ); // increase orderedByProfile.Count every turn, so that vdw.VdIndex can be set properly.
                profile.DesktopOrder.Add( restVdw.VdId ); // append to tail
            }

            _virtualDesktops = orderedByProfile;
            SaveOrder();
        }

        private static void UpdateMainView( VirtualDesktopNotification? vdn = null )
        {
            if ( !MainWindow.IsShowing() ) return;

            FixLayout();
            ShowAllVirtualDesktops();

            if ( vdn is null ) return;

            try
            {
                var fallback = _virtualDesktops[GetVdIndexByGuid( vdn.NewId )];
                ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {fallback} );
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

        private static List<Guid> _lastDesktopOrder = new();

        public static void SaveOrder( List<Guid>? newOrder = null )
        {
            static bool IsSameGuidList( List<Guid> a, List<Guid> b )
            {
                if ( a.Count != b.Count ) return false;
                return !a.Where( ( t, i ) => t != b[i] ).Any();
            }

            if ( newOrder != null )
            {
                ConfigManager.CurrentProfile.DesktopOrder = newOrder;
            }

            if ( IsSameGuidList( _lastDesktopOrder, ConfigManager.CurrentProfile.DesktopOrder! ) ) return;

            ConfigManager.Save( reason: "sync&save", reasonName: "ConfigManager.CurrentProfile.DesktopOrder" );
            _lastDesktopOrder = new List<Guid>( ConfigManager.CurrentProfile.DesktopOrder! );
        }

        public static int GetVdIndexByGuid( Guid guid )
        {
            return ( from vdw in _virtualDesktops where vdw.VdId == guid select vdw.VdIndex ).FirstOrDefault();
        }

        public static void Bootstrap()
        {
            _vdwDefaultBackColor = Color.FromArgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B );
        }
    }
}