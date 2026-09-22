/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace VirtualSpace;

internal static class InstanceNotifyWindow
{
    private static User32.WindowProc? _wndProc;
    private static IntPtr             _hwnd;
    private static IntPtr             _hInstance;

    public static void Create()
    {
        if ( _hwnd != IntPtr.Zero )
        {
            return;
        }

        _wndProc   = WndProc;
        _hInstance = Kernel32.GetModuleHandle( null );

        var className = Marshal.StringToHGlobalUni( Const.Window.INSTANCE_CLASS );
        try
        {
            var wc = new WNDCLASSEX
            {
                cbSize        = (uint)Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc   = Marshal.GetFunctionPointerForDelegate( _wndProc ),
                hInstance     = _hInstance,
                lpszClassName = className
            };

            if ( User32.RegisterClassEx( in wc ) == 0 )
            {
                var err = Marshal.GetLastWin32Error();
                if ( err != User32.ERROR_CLASS_ALREADY_EXISTS )
                {
                    Logger.Warning( $"Instance notify window class registration failed: {err}" );
                    return;
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal( className );
        }

        _hwnd = User32.CreateWindowEx(
            0,
            Const.Window.INSTANCE_CLASS,
            string.Empty,
            0,
            0,
            0,
            0,
            0,
            User32.HwndMessage,
            IntPtr.Zero,
            _hInstance,
            IntPtr.Zero );

        if ( _hwnd == IntPtr.Zero )
        {
            Logger.Warning( $"Instance notify window creation failed: {Marshal.GetLastWin32Error()}" );
            return;
        }

        User32.ChangeWindowMessageFilterEx( _hwnd, WinMsg.UM_CLIENTNOTIFY, WinMsg.MSGFLT_ALLOW, IntPtr.Zero );
    }

    public static void Destroy()
    {
        if ( _hwnd != IntPtr.Zero )
        {
            User32.DestroyWindow( _hwnd );
            _hwnd = IntPtr.Zero;
        }

        if ( _hInstance != IntPtr.Zero )
        {
            User32.UnregisterClass( Const.Window.INSTANCE_CLASS, _hInstance );
            _hInstance = IntPtr.Zero;
        }

        _wndProc = null;
    }

    public static IntPtr Find()
    {
        return User32.FindWindowEx( User32.HwndMessage, IntPtr.Zero, Const.Window.INSTANCE_CLASS, null );
    }

    private static IntPtr WndProc( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam )
    {
        if ( msg != WinMsg.UM_CLIENTNOTIFY )
        {
            return User32.DefWindowProc( hWnd, msg, wParam, lParam );
        }

        var main = MainWindow.GetMainWindow();
        if ( main is not { Handle: not 0 } )
        {
            return IntPtr.Zero;
        }

        var kind = wParam.ToInt32();
        if ( kind == WinMsg.UM_CLIENTNOTIFY_SECOND_INSTANCE )
        {
            Logger.Info( "Only single instance allowed, just bring to top." );
            User32.PostMessage( main.Handle, WinMsg.UM_CLIENTNOTIFY, WinMsg.UM_CLIENTNOTIFY_SECOND_INSTANCE, 0 );
        }
        else
        {
            User32.SendMessage( main.Handle, WinMsg.UM_CLIENTNOTIFY, (ulong)kind, (ulong)lParam.ToInt64() );
        }

        return IntPtr.Zero;

    }
}
