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
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static class VirtualDesktopManager
    {
        private static readonly List<VisibleWindow>         VisibleWindows       = new();
        private static readonly User32.EnumChildWindowsProc EnumChildWindowsProc = ChildWindowFilter;
        private static readonly User32.EnumWindowsProc      EnumWindowsProc      = WindowFilter;
        private static readonly StringBuilder               SbWinTitle           = new( Const.WindowTitleMaxLength );
        private static readonly StringBuilder               SbClsName            = new( Const.WindowClassMaxLength );
        private static          List<VirtualDesktopWindow>  _virtualDesktops     = new();
        private static          IntPtr                      _coreWindowHandle;
        private static          Color                       _defaultBackColor;
        public static           bool                        NeedRepaintThumbs;
        public static           UserInterface               Ui            => ConfigManager.CurrentProfile.UI;
        public static           bool                        IsBatchCreate { get; set; }

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
            var vdwWidth   = ( size.Width - 2 * Ui.VDWBorderSize ) * dpi[0] + 1;
            var vdwHeight  = ( size.Height - 2 * Ui.VDWBorderSize ) * dpi[1] + 1;
            var commonSize = new Size( (int)vdwWidth, (int)vdwHeight );

            var currentImage = new List<VirtualDesktopWindow>();
            var cachePath    = ConfigManager.GetCachePath();
            for ( var index = 0; index < vdCount; index++ ) // build new list for current desktops
            {
                var guid = DesktopManagerWrapper.GetIdByIndex( index );
                if ( guid == default ) continue;
                var survival = _virtualDesktops.Find( v => v.VdId == guid );
                if ( survival == null )
                {
                    survival = VirtualDesktopWindow.Create( index, guid, _defaultBackColor, commonSize, Ui.VDWPadding );
                    Task.Run( () =>
                    {
                        survival.SetBackground( WinRegistry.GetWallpaperByDesktopGuid(
                            guid,
                            survival.Width,
                            survival.Height,
                            cachePath ) );
                    } );
                }
                else
                {
                    survival.VdIndex = index;
                    survival.Size = commonSize;
                }

                currentImage.Add( survival );
            }

            var guids = currentImage.Select( v => v.VdId ).ToList();
            foreach ( var old in _virtualDesktops.Where( old => !guids.Contains( old.VdId ) ) )
            {
                old.RealClose();
            }

            _virtualDesktops = currentImage;
        }

        public static void FixLayout()
        {
            MainWindow.ResetMainGrid();
            SyncVirtualDesktops();
            ReOrder();
        }

        public static async void InitLayout()
        {
            BootStrap();

            MainWindow.ResetMainGrid();

            var vdCount    = DesktopWrapper.Count;
            var dpi        = SysInfo.Dpi;
            var size       = MainWindow.MainGridCellSize;
            var vdwWidth   = ( size.Width - 2 * Ui.VDWBorderSize ) * dpi[0] + 1;
            var vdwHeight  = ( size.Height - 2 * Ui.VDWBorderSize ) * dpi[1] + 1;
            var commonSize = new Size( (int)vdwWidth, (int)vdwHeight );

            var tasks = new List<Task>();
            for ( var i = 0; i < vdCount; i++ )
            {
                var index = i;
                tasks.Add( Task.Run( () =>
                {
                    var guid = DesktopManagerWrapper.GetIdByIndex( index );
                    var vdw  = VirtualDesktopWindow.Create( index, guid, _defaultBackColor, commonSize, Ui.VDWPadding );

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

            ShowAllVirtualDesktops();
            ShowVisibleWindowsForDesktops();
        }

        private static void ReOrder( bool needSort = false )
        {
            var profile = ConfigManager.CurrentProfile;
            if ( needSort )
                _virtualDesktops.Sort( ( x, y ) => x.VdIndex.CompareTo( y.VdIndex ) );

            var guids = _virtualDesktops.Select( vdw => vdw.VdId ).ToList();

            if ( profile.DesktopOrder == null || profile.DesktopOrder.Count == 0 ) // no custom order, using system's
            {
                SaveOrder( guids );
            }
            else
            {
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
        }

        public static void ResetLayout()
        {
            FixLayout();
            ShowAllVirtualDesktops();
            if ( NeedRepaintThumbs )
            {
                ShowVisibleWindowsForDesktops();
                NeedRepaintThumbs = false;
            }
        }

        private static bool WindowFilter( IntPtr hWnd, int lParam )
        {
            User32.GetWindowText( hWnd, SbWinTitle, SbWinTitle.Capacity );
            var title = SbWinTitle.ToString();

            User32.GetClassName( hWnd, SbClsName, SbClsName.Capacity );
            var classname = SbClsName.ToString();

            if ( User32.IsWindowVisible( hWnd ) &&
                 !string.IsNullOrEmpty( title ) &&
                 !Filters.WndClsIgnoreList.Contains( classname ) &&
                 !Filters.WndTitleIgnoreList.Contains( title ) &&
                 !Filters.WndHandleIgnoreList.Contains( hWnd ) &&
                 !Filters.WndHandleManualIgnoreList.Contains( hWnd ) &&
                 !Filters.IsCloaked( hWnd )
               )
            {
                if ( classname == Const.ApplicationFrameWindow )
                {
                    User32.EnumChildWindows( hWnd, EnumChildWindowsProc, 0 );
                    if ( _coreWindowHandle != default )
                    {
                        VisibleWindows.Add( new VisibleWindow( title, classname, hWnd, _coreWindowHandle ) );
                        _coreWindowHandle = default;
                    }
                }
                else
                {
                    VisibleWindows.Add( new VisibleWindow( title, classname, hWnd ) );
                }
            }

            return true;
        }

        private static bool ChildWindowFilter( IntPtr hWnd, int lParam )
        {
            var sbCName = new StringBuilder( Const.WindowClassMaxLength );
            User32.GetClassName( hWnd, sbCName, sbCName.Capacity );
            var classname = sbCName.ToString();
            if ( classname == Const.WindowsUiCoreWindow && _coreWindowHandle == default )
                _coreWindowHandle = hWnd;
            return true;
        }

        private static List<VisibleWindow> GetVisibleWindows()
        {
            VisibleWindows.Clear();
            User32.EnumWindows( EnumWindowsProc, 0 );
            return VisibleWindows;
        }

        public static void ShowVisibleWindowsForDesktops( List<VirtualDesktopWindow>? vdws = null )
        {
            var count   = DesktopManagerWrapper.GetViewCount();
            var windows = GetVisibleWindows();
            Logger.Debug( $"VisibleWindows/ApplicationViews: {windows.Count}/{count}" );

            vdws ??= _virtualDesktops;

            var profile = ConfigManager.CurrentProfile;
            Parallel.ForEach( vdws, ( vdw, loopState ) => { vdw.ClearWindows(); } );
            foreach ( var win in windows )
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

                    var vdIndex = profile.DesktopOrder.IndexOf( DesktopWrapper.FromWindow( win.Handle ).Guid );
                    if ( vdws.Count == _virtualDesktops.Count )
                    {
                        vdws[vdIndex].AddWindow( win );
                        Logger.Debug( $"Desktop[{vdIndex}]({DesktopWrapper.DesktopNameFromIndex( vdIndex )}) CONTAINS {win.Title}" );
                    }
                    else
                    {
                        foreach ( var vdw in vdws )
                        {
                            if ( vdw.VdIndex == vdIndex )
                            {
                                vdw.AddWindow( win );
                                Logger.Debug( $"Desktop[{vdIndex}]({DesktopWrapper.DesktopNameFromIndex( vdIndex )}) CONTAINS {win.Title}" );
                            }
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Logger.Warning( $"{ex.Message} ∵ {win.Title}({win.Handle.ToString( "X2" )})" );
                    Filters.WndHandleIgnoreList.Add( win.Handle );
                }
            }

            foreach ( var vdw in vdws )
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
        }

        private static void BootStrap()
        {
            _defaultBackColor = Color.FromArgb( Ui.VDWDefaultBackColor.R, Ui.VDWDefaultBackColor.G, Ui.VDWDefaultBackColor.B );
            foreach ( var vdw in _virtualDesktops ) vdw.RealClose();
            _virtualDesktops.Clear();
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