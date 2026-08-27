// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using VirtualSpace.Config;

namespace VirtualSpace.Tools
{
    public static class WindowTool
    {
        private static void MoveWindowToScreen( IntPtr hWnd, Screen destScreen )
        {
            var srcScreen = Screen.FromHandle( hWnd );
            if ( srcScreen.DeviceName == destScreen.DeviceName ) return;

            var wp = new WINDOWPLACEMENT();
            wp.Length = Marshal.SizeOf( wp );
            if ( !User32.GetWindowPlacement( hWnd, ref wp ) ) return;

            var rect         = wp.NormalPosition;
            var targetX      = destScreen.WorkingArea.X + rect.Left - srcScreen.WorkingArea.Left;
            var targetY      = destScreen.WorkingArea.Y + rect.Top - srcScreen.WorkingArea.Top;
            var targetWidth  = rect.Right - rect.Left;
            var targetHeight = rect.Bottom - rect.Top;

            switch ( wp.ShowCmd )
            {
                case ShowState.SW_SHOWMAXIMIZED:
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_RESTORE );
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_MAXIMIZE );
                    break;
                case ShowState.SW_MINIMIZE:
                case ShowState.SW_SHOWMINIMIZED:
                    _ = User32.ShowWindow( hWnd, (short)ShowState.SW_RESTORE );
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    // User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_SHOWMINIMIZED );
                    break;
                case ShowState.SW_NORMAL:
                    User32.SetWindowPos( hWnd, IntPtr.Zero,
                        targetX, targetY, targetWidth, targetHeight, 0 );
                    break;
            }
        }

        public static void MoveWindowToScreen( IntPtr hWnd, int index )
        {
            var allScreens = Screen.AllScreens;

            if ( index < 0 || index > allScreens.Length ) return;

            MoveWindowToScreen( hWnd, allScreens[index] );
        }

        public static void MoveWindowToScreen( IntPtr hWnd, string deviceName )
        {
            var allScreens = Screen.AllScreens;
            var index      = -1;

            for ( var i = 0; i < allScreens.Length; i++ )
                if ( deviceName == allScreens[i].DeviceName )
                {
                    index = i;
                    break;
                }

            if ( index < 0 ) return;

            MoveWindowToScreen( hWnd, allScreens[index] );
        }

        public static int GetZOrderByHandle( IntPtr hWnd )
        {
            var index = 0;
            _ = User32.EnumWindows( ( wnd, param ) =>
            {
                index++;
                return hWnd != wnd;
            }, 0 );

            return index;
        }

        public static void ActivateWindow( IntPtr hWnd, int desktopIndex )
        {
            if ( DesktopWrapper.CurrentIndex != desktopIndex )
            {
                Logger.Verbose( $"CHANGE CURRENT DESKTOP TO Desktop[{desktopIndex.ToString()}]" );
                DesktopWrapper.MakeVisibleByIndex( desktopIndex );
            }

            TryActivateWindow( hWnd );
        }

        public static void ActivateWindow( IntPtr hWnd, Guid guid )
        {
            if ( DesktopWrapper.CurrentGuid != guid )
            {
                var sysIndex = DesktopWrapper.IndexFromGuid( guid );
                Logger.Verbose( $"CHANGE CURRENT DESKTOP TO Desktop[{sysIndex.ToString()}]" );
                DesktopWrapper.MakeVisibleByGuid( guid, false );
            }

            Logger.Verbose( $"Try activate window {hWnd:X}" );
            TryActivateWindow( hWnd );
            Logger.Verbose( "Activate window success." );
        }

        private static void TryActivateWindow( IntPtr hWnd )
        {
            try
            {
                WindowActivateHelper.RestoreAndActivateWindow( hWnd );
            }
            catch ( Exception ex )
            {
                Logger.Warning( $"Activate window with error: {ex.Message}" );
            }
        }

        public static bool IsModalWindow( IntPtr hWnd )
        {
            // child windows cannot have owners
            var style = User32.GetWindowLong( hWnd, (int)GetWindowLongFields.GWL_STYLE );
            if ( ( style & (int)WindowStyles.WS_CHILD ) > 0 ) return false;

            var hWndOwner = User32.GetWindow( hWnd, GetWindowType.GW_OWNER );
            if ( hWndOwner == IntPtr.Zero ) return false; // not an owned window
            if ( User32.IsWindowEnabled( hWndOwner ) ) return false; // owner is enabled
            return true; // an owned window whose owner is disabled
        }

        public static bool IsPopupToolWindow( IntPtr hWnd )
        {
            var style = (uint)User32.GetWindowLong( hWnd, (int)GetWindowLongFields.GWL_STYLE );
            return style == 0x96000000; // WS_POPUP | WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS
        }
    }
    
    public static class WindowActivateHelper
    {
        public static void RestoreAndActivateWindow( IntPtr hWnd )
        {
            if ( hWnd == IntPtr.Zero || !User32.IsWindow( hWnd ) )
                return;

            // 1) 先恢复/显示 (跨线程场景 Async)
            if ( User32.IsIconic( hWnd ) )
                User32.ShowWindowAsync( hWnd, (short)ShowState.SW_RESTORE );
            else
                User32.ShowWindowAsync( hWnd, (short)ShowState.SW_SHOW );

            // 2) 给目标线程一点时间处理 WM_QUERYOPEN / WM_SIZE / WM_PAINT 链
            Thread.Sleep( Manager.Configs.Cluster.ActivateWindowTimeout );

            // 3) 尝试激活 (管理员路径和非管理员路径统一做，减少分叉差异)
            TryActivate( hWnd );

            // 4) 强制整棵子窗口树重绘 (关键)
            User32.RedrawWindow(
                hWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                RedrawWindowFlags.RDW_INVALIDATE |
                RedrawWindowFlags.RDW_ALLCHILDREN |
                RedrawWindowFlags.RDW_UPDATENOW |
                RedrawWindowFlags.RDW_FRAME );
            
            if ( !Manager.Configs.Cluster.TryHarderActivateMinimizedWindow ) // 4.5) 上述代码足以应对大多数情况，若有极端情况则设置 TryHarderActivateMinimizedWindow=true
                return;
            
            // 5) 兜底：再给一小段时间 + 再刷一次
            Thread.Sleep( Manager.Configs.Cluster.ActivateWindowTimeout );
            User32.RedrawWindow(
                hWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                RedrawWindowFlags.RDW_INVALIDATE |
                RedrawWindowFlags.RDW_ALLCHILDREN |
                RedrawWindowFlags.RDW_UPDATENOW );
        }

        private static void TryActivate( IntPtr hWnd )
        {
            // 老 API，保留但降级为“可选尝试”
            try
            {
                User32.SwitchToThisWindow( hWnd, true ); // 兼容旧行为
            }
            catch
            {
                // ignore
            }

            var foreground = User32.GetForegroundWindow();
            var foregroundThread = foreground != IntPtr.Zero
                ? User32.GetWindowThreadProcessId( foreground, out _ )
                : 0;
            var targetThread = User32.GetWindowThreadProcessId( hWnd, out _ );

            var attached = false;
            if ( foregroundThread != 0 && foregroundThread != targetThread )
                attached = User32.AttachThreadInput( foregroundThread, targetThread, true );

            try
            {
                User32.BringWindowToTop( hWnd );
                User32.SetForegroundWindow( hWnd );
            }
            finally
            {
                if ( attached )
                    User32.AttachThreadInput( foregroundThread, targetThread, false );
            }
        }
    }
}