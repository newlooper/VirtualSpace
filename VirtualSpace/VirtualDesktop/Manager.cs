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
using System.Text;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;
using VirtualSpace.Commons;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        private static readonly List<VisibleWindow>    VisibleWindows  = new();
        private static readonly User32.EnumWindowsProc EnumWindowsProc = VisibleWindowFilter;
        private static readonly StringBuilder          SbWinInfo       = new( Const.WindowTitleMaxLength );

        private static List<VirtualDesktopWindow> _virtualDesktops = new();

        private static Color         _vdwDefaultBackColor;
        public static  bool          NeedRepaintThumbs;
        public static  UserInterface Ui            => ConfigManager.CurrentProfile.UI;
        public static  bool          IsBatchCreate { get; set; }

        public static int CurrentDesktopIndex()
        {
            var guid = DesktopWrapper.CurrentGuid;

            return ( from vdw in _virtualDesktops where vdw.VdId == guid select vdw.VdIndex ).FirstOrDefault();
        }

        private static void SyncVirtualDesktops()
        {
            var vdCount    = DesktopWrapper.Count;
            var dpi        = SysInfo.Dpi;
            var size       = MainWindow.MainGridCellSize;
            var vdwWidth   = ( size.Width - 2 * Ui.VDWBorderSize ) * dpi.ScaleX + 1;
            var vdwHeight  = ( size.Height - 2 * Ui.VDWBorderSize ) * dpi.ScaleY + 1;
            var commonSize = new Size( (int)vdwWidth, (int)vdwHeight );

            var survivalDesktops = new List<VirtualDesktopWindow>();
            for ( var index = 0; index < vdCount; index++ ) // build new list for current desktops
            {
                var guid = DesktopManagerWrapper.GetIdByIndex( index );
                if ( guid == default ) continue;
                var survival = _virtualDesktops.Find( v => v.VdId == guid );
                if ( survival == null )
                {
                    survival = VirtualDesktopWindow.Create( index, guid, _vdwDefaultBackColor, commonSize, Ui.VDWPadding );
                    Task.Run( () =>
                    {
                        survival.SetBackground( WinRegistry.GetWallpaperByDesktopGuid(
                            guid,
                            survival.Width,
                            survival.Height,
                            ConfigManager.GetCachePath() ) );
                    } );
                }
                else
                {
                    survival.VdIndex = index;
                    survival.Size = commonSize;
                }

                survivalDesktops.Add( survival );
            }

            var guids = survivalDesktops.Select( v => v.VdId ).ToList();
            foreach ( var old in _virtualDesktops.Where( old => !guids.Contains( old.VdId ) ) )
            {
                old.RealClose();
            }

            _virtualDesktops = survivalDesktops;
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
            ReOrder();
        }

        public static async Task InitLayout()
        {
            MainWindow.ResetMainGrid();

            var vdCount    = DesktopWrapper.Count;
            var dpi        = SysInfo.Dpi;
            var size       = MainWindow.MainGridCellSize;
            var vdwWidth   = ( size.Width - 2 * Ui.VDWBorderSize ) * dpi.ScaleX + 1;
            var vdwHeight  = ( size.Height - 2 * Ui.VDWBorderSize ) * dpi.ScaleY + 1;
            var commonSize = new Size( (int)vdwWidth, (int)vdwHeight );

            var tasks = new List<Task>();
            for ( var i = 0; i < vdCount; i++ )
            {
                var index = i;
                tasks.Add( Task.Run( () =>
                {
                    var guid = DesktopManagerWrapper.GetIdByIndex( index );
                    var vdw  = VirtualDesktopWindow.Create( index, guid, _vdwDefaultBackColor, commonSize, Ui.VDWPadding );

                    vdw.SetBackground( WinRegistry.GetWallpaperByDesktopGuid( guid, vdw.Width, vdw.Height, ConfigManager.GetCachePath() ) );
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
                Logger.Error( ex.Message );
                return;
            }

            ReOrder( true );
        }

        private static void ReOrder( bool needSort = false )
        {
            if ( needSort )
                _virtualDesktops.Sort( ( x, y ) => x.VdIndex.CompareTo( y.VdIndex ) );

            var profile = ConfigManager.CurrentProfile;
            var guids   = _virtualDesktops.Select( vdw => vdw.VdId ).ToList();

            if ( profile.DesktopOrder == null || profile.DesktopOrder.Count == 0 ) // no custom order, using system's
            {
                SaveOrder( guids );
                return;
            }

            var diff = profile.DesktopOrder.FindAll( o => !guids.Contains( o ) );
            foreach ( var d in diff )
            {
                profile.DesktopOrder.Remove( d );
            }

            var reOrdered = new List<VirtualDesktopWindow>();
            for ( var idx = 0; idx < profile.DesktopOrder.Count; idx++ )
            {
                var vdw = _virtualDesktops.Find( vdw => vdw.VdId == profile.DesktopOrder[idx] );
                vdw.VdIndex = idx; // reposition
                reOrdered.Add( vdw );
                _virtualDesktops.Remove( vdw );
            }

            foreach ( var unOrdered in _virtualDesktops )
            {
                unOrdered.VdIndex = reOrdered.Count;
                reOrdered.Add( unOrdered ); // change reOrdered.Count every turn
                profile.DesktopOrder.Add( unOrdered.VdId ); // append to tail
            }

            _virtualDesktops = reOrdered;
            SaveOrder();
        }

        public static void UpdateMainView( VirtualDesktopNotification? vdn = null )
        {
            FixLayout();
            ShowAllVirtualDesktops();
            if ( NeedRepaintThumbs )
            {
                ShowVisibleWindowsForDesktops();
                NeedRepaintThumbs = false;
            }
            else
            {
                if ( vdn is null ) return;

                var vdwList = GetAllVirtualDesktops();
                try
                {
                    var fallback = vdwList[DesktopWrapper.IndexFromGuid( vdn.NewId )];
                    ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {fallback} );
                }
                catch ( Exception e )
                {
                    Logger.Warning( e.StackTrace );
                }
            }
        }

        private static bool VisibleWindowFilter( IntPtr hWnd, int lParam )
        {
            if ( Filters.WndHandleIgnoreListByError.Contains( hWnd ) ||
                 Filters.WndHandleIgnoreListByManual.Contains( hWnd ) ||
                 !User32.IsWindowVisible( hWnd ) ||
                 Filters.IsCloaked( hWnd ) )
                return true;

            _ = User32.GetWindowText( hWnd, SbWinInfo, SbWinInfo.Capacity );
            var title = SbWinInfo.ToString();
            if ( string.IsNullOrEmpty( title ) ||
                 Filters.WndTitleIgnoreList.Contains( title ) )
                return true;

            _ = User32.GetClassName( hWnd, SbWinInfo, SbWinInfo.Capacity );
            var classname = SbWinInfo.ToString();
            if ( Filters.WndClsIgnoreList.Contains( classname ) )
                return true;

            if ( classname != Const.WindowsUiCoreWindow )
            {
                VisibleWindows.Add( new VisibleWindow( title, classname, hWnd ) );
            }

            return true;
        }

        private static List<VisibleWindow> GetVisibleWindows()
        {
            VisibleWindows.Clear();
            _ = User32.EnumWindows( EnumWindowsProc, 0 );
            return VisibleWindows;
        }

        public static void ShowVisibleWindowsForDesktops( List<VirtualDesktopWindow>? vdwList = null )
        {
            var visibleWindows = GetVisibleWindows();
            Logger.Debug( $"VisibleWindows/ApplicationViews: {visibleWindows.Count.ToString()}/{DesktopManagerWrapper.GetViewCount().ToString()}" );

            vdwList ??= _virtualDesktops;

            Parallel.ForEach( vdwList, ( vdw, loopState ) => { vdw.ClearWindows(); } );

            foreach ( var win in visibleWindows )
            {
                try
                {
                    if ( DesktopWrapper.IsWindowPinned( win.Handle ) ||
                         DesktopWrapper.IsApplicationPinned( win.Handle ) )
                    {
                        Logger.Debug( $"{win.Title} IS PIN TO ALL DESKTOPS" );
                        foreach ( var vdw in _virtualDesktops )
                            vdw.AddWindow( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
                        continue;
                    }

                    var vdIndex = ConfigManager.CurrentProfile.DesktopOrder.IndexOf( DesktopWrapper.FromWindow( win.Handle ).Guid );
                    if ( vdwList.Count == _virtualDesktops.Count ) // show for all VDs
                    {
                        vdwList[vdIndex].AddWindow( win );
                        Logger.Debug( $"Desktop[{vdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( vdIndex )}) CONTAINS {win.Title}" );
                    }
                    else // show for specific VDs
                    {
                        foreach ( var vdw in vdwList.Where( vdw => vdw.VdIndex == vdIndex ) )
                        {
                            vdw.AddWindow( win );
                            Logger.Debug( $"Desktop[{vdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( vdIndex )}) CONTAINS {win.Title}" );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    if ( win.Classname != Const.ApplicationFrameWindow )
                    {
                        Logger.Warning( $"{ex.Message} ∵ {win.Title}({win.Handle.ToString( "X2" )}), WndClass: {win.Classname}" );
                        Filters.WndHandleIgnoreListByError.Add( win.Handle );
                    }
                }
            }

            foreach ( var vdw in vdwList )
            {
                vdw.ShowThumbnails();
            }
        }

        public static void ShowAllVirtualDesktops()
        {
            foreach ( var vdw in _virtualDesktops )
            {
                vdw.ShowByVdIndex();
            }

            ResetAllBackground();
        }

        public static void ResetAllBackground()
        {
            MainWindow.ResetAllBorder();
        }

        public static void HideAllVirtualDesktops()
        {
            Menus.CloseContextMenu();
            foreach ( var vdw in _virtualDesktops ) vdw.Hide();

            Parallel.ForEach( _virtualDesktops, ( vdw, loopState ) => { vdw.ClearWindows(); } );
        }

        public static void Bootstrap()
        {
            _vdwDefaultBackColor = Color.FromArgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B );
        }

        public static List<VirtualDesktopWindow> GetAllVirtualDesktops()
        {
            return _virtualDesktops;
        }

        public static void SaveOrder( List<Guid>? newOrder = null )
        {
            if ( newOrder != null )
            {
                ConfigManager.CurrentProfile.DesktopOrder = newOrder;
            }

            ConfigManager.Save();
        }
    }
}