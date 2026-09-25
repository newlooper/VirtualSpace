/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Immutable;
using VirtualSpace.Helpers;

namespace VirtualSpace.VirtualDesktop;

public static class Filters
{
    public static readonly string[] WndClsIgnoreList =
    [
        "Progman",
        "RainmeterMeterWindow",
        "SysDragImage",
        "DuiMenuWnd",
        "PerryShadowWnd",
        "SysShadow",
        "Xaml_WindowedPopupClass",
        "TopLevelWindowForOverflowXamlIsland"
    ];

    public static readonly string[] WndTitleIgnoreList =
    [
        "WinFormsDesigner"
    ];

    /// <summary>
    ///     键为 HWND，值为忽略时的进程号。过滤时须与窗口当前进程号一致才视为同一窗口。
    ///     别的进程复用该句柄时当场放行。同一进程内句柄被复用仍会跳过。
    /// </summary>
    public static ImmutableDictionary<IntPtr, int> WndHandleIgnoreListByError = ImmutableDictionary<IntPtr, int>.Empty;

    public static ImmutableList<IntPtr> WndHandleIgnoreListByManual = ImmutableList<IntPtr>.Empty;

    public static bool IsIgnoredWindow( ImmutableDictionary<IntPtr, int> ignored, IntPtr hWnd )
    {
        if ( !ignored.TryGetValue( hWnd, out var storedPid ) )
        {
            return false;
        }

        _ = User32.GetWindowThreadProcessId( hWnd, out var pid );
        return pid != 0 && pid == storedPid;
    }

    /// <summary>
    ///     扫忽略表本身，去掉已销毁的句柄，以及句柄还在但进程号已经换过的项。
    /// </summary>
    public static ImmutableDictionary<IntPtr, int> DropStaleIgnoredWindows( ImmutableDictionary<IntPtr, int> ignored )
    {
        if ( ignored.IsEmpty )
        {
            return ignored;
        }

        var builder = ignored.ToBuilder();
        var removed = false;
        foreach ( var (hWnd, storedPid) in ignored )
        {
            if ( User32.IsWindow( hWnd ) )
            {
                _ = User32.GetWindowThreadProcessId( hWnd, out var pid );
                if ( pid == storedPid )
                {
                    continue;
                }
            }

            builder.Remove( hWnd );
            removed = true;
        }

        return removed ? builder.ToImmutable() : ignored;
    }

    public static bool IsCloaked( IntPtr handle )
    {
        _ = DwmApi.DwmGetWindowAttribute( handle,
            (uint)DwmApi.DwmWindowAttribute.DWMWA_CLOAKED,
            out var cloaked,
            sizeof( uint ) );

        return cloaked > 2;
    }
}