/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        private static readonly List<VisibleWindow>        VisibleWindows   = new();
        private static readonly User32.EnumWindowsProc     EnumWindowsProc  = VisibleWindowFilter;
        private static readonly StringBuilder              SbWinInfo        = new( Const.WindowTitleMaxLength );
        private static          List<VirtualDesktopWindow> _virtualDesktops = new();
        public static           bool                       IsBatchCreate { get; set; }
        public static           Guid                       LastDesktopId = Guid.Empty;

        private static bool VisibleWindowFilter( IntPtr hWnd, int lParam )
        {
            if ( Filters.WndHandleIgnoreListByError.Contains( hWnd ) ||
                 Filters.WndHandleIgnoreListByManual.TryGetValue( hWnd, out _ ) ||
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

        public static void ShowVisibleWindowsForDesktops( List<VirtualDesktopWindow>? vdwList = null, int processId = 0 )
        {
            var visibleWindows = GetVisibleWindows();
            Logger.Debug( $"VisibleWindows/ApplicationViews: {visibleWindows.Count.ToString()}/{DesktopManagerWrapper.GetViewCount().ToString()}" );

            vdwList ??= _virtualDesktops;

            foreach ( var virtualDesktopWindow in vdwList )
            {
                virtualDesktopWindow.ClearVisibleWindows();
            }

            foreach ( var win in visibleWindows )
            {
                try
                {
                    if ( processId != 0 )
                    {
                        _ = User32.GetWindowThreadProcessId( win.Handle, out var pId );
                        if ( processId != pId ) continue;
                    }

                    if ( DesktopWrapper.IsWindowPinned( win.Handle ) ||
                         DesktopWrapper.IsApplicationPinned( win.Handle ) )
                    {
                        Logger.Debug( $"{win.Title} IS PINNED" );
                        foreach ( var vdw in _virtualDesktops )
                            vdw.AddWindow( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
                        continue;
                    }

                    var ownerId = DesktopWrapper.GuidFromWindow( win.Handle );
                    if ( vdwList.Count == _virtualDesktops.Count ) // show for all VDs
                    {
                        var owner = vdwList.Find( v => v.VdId == ownerId );
                        if ( owner is null ) continue;
                        owner.AddWindow( win );
                        Logger.Debug( $"Desktop[{owner.VdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( owner.VdIndex )}) CONTAINS {win.Title}" );
                    }
                    else // show for specific VDs
                    {
                        foreach ( var vdw in vdwList.Where( vdw => vdw.VdId == ownerId ) )
                        {
                            vdw.AddWindow( win );
                            Logger.Debug( $"Desktop[{vdw.VdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( vdw.VdIndex )}) CONTAINS {win.Title}" );
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

        public static void RefreshThumbs( IntPtr h, params VirtualDesktopWindow[] vdwList )
        {
            if ( DesktopWrapper.IsWindowPinned( h ) ||
                 DesktopWrapper.IsApplicationPinned( h ) )
            {
                ShowVisibleWindowsForDesktops();
            }
            else
            {
                ShowVisibleWindowsForDesktops( vdwList.ToList() );
            }
        }

        public static void ShowAllVirtualDesktops()
        {
            UpdateVdwBackground();
            foreach ( var vdw in _virtualDesktops )
            {
                User32.SendMessage( vdw.Handle, WinMsg.WM_HOTKEY, UserMessage.ShowVdw, 0 );
            }
        }

        public static void HideAllVirtualDesktops()
        {
            Menus.CloseContextMenu();
            foreach ( var vdw in _virtualDesktops )
            {
                vdw.ResetOnlyOneStatus();
                vdw.Hide();
                vdw.ClearVisibleWindows();
            }
        }

        public static List<VirtualDesktopWindow> GetAllVirtualDesktops()
        {
            return _virtualDesktops;
        }

        public static VirtualDesktopWindow GetCurrentVdw()
        {
            return _virtualDesktops.Single( v => v.VdId == DesktopWrapper.CurrentGuid );
        }
    }
}