/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using GHK = VirtualSpace.Helpers.GlobalHotKey;
using LLKH = VirtualSpace.Helpers.LowLevelKeyboardHook;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static readonly Stopwatch RiseTaskViewTimer = Stopwatch.StartNew();

        private void RegisterHotKey( IntPtr hWnd )
        {
            foreach ( var (k, kbInProfile) in Manager.Configs.KeyBindings )
            {
                var ghkCode = kbInProfile.GhkCode;
                if ( ghkCode == "" ) continue;

                var func      = Const.Hotkey.GetFuncDesc( k );
                var messageId = Const.Hotkey.GetKeyBinding( k ).MessageId;
                var hotkeyStr = ghkCode.Replace( Const.Hotkey.NONE + Const.Hotkey.SPLITTER, "" );

                var arr = ghkCode.Split( Const.Hotkey.SPLITTER );
                if ( arr.Length == 5 )
                {
                    var km = arr[0] == Const.Hotkey.NONE ? GHK.KeyModifiers.None : GHK.KeyModifiers.WindowsKey;
                    km |= arr[1] == Const.Hotkey.NONE ? GHK.KeyModifiers.None : GHK.KeyModifiers.Ctrl;
                    km |= arr[2] == Const.Hotkey.NONE ? GHK.KeyModifiers.None : GHK.KeyModifiers.Alt;
                    km |= arr[3] == Const.Hotkey.NONE ? GHK.KeyModifiers.None : GHK.KeyModifiers.Shift;

                    try
                    {
                        var key = Enum.Parse<Key>( arr[4] );
                        Logger.Info( string.Format( "Register Global HotKey [{0}] For \"{1}\", {2}",
                            hotkeyStr,
                            func,
                            GHK.RegHotKey( hWnd, messageId, km, KeyInterop.VirtualKeyFromKey( key ) )
                                ? "Success"
                                : "Fail" ) );
                    }
                    catch ( Exception ex )
                    {
                        Logger.Error( string.Format( "Register Global HotKey [{0}] For \"{1}\" Error: {2}",
                            hotkeyStr,
                            func,
                            ex.Message ) );
                    }
                }
            }

            var keyboardHookProc = new User32.HookProc( KeyboardHookCallback );
            Logger.Info( "Set Windows LowLevelKeyboardProc Hook" );
            LLKH.SetHook( keyboardHookProc );

            if ( Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar )
            {
                EnableMouseHook();
            }
        }

        private void EnableMouseHook()
        {
            Logger.Info( "Set Windows LowLevelMouseProc Hook" );
            LowLevelMouseHook.SetHook( MouseHookCallback );
        }

        private static void DisableMouseHook()
        {
            Logger.Info( "Unset Windows LowLevelMouseProc Hook" );
            LowLevelMouseHook.UnHook();
        }

        private IntPtr KeyboardHookCallback( int nCode, IntPtr wParam, IntPtr lParam )
        {
            if ( nCode >= 0 )
            {
                var info = (LLKH.KBDLLHOOKSTRUCT)Marshal.PtrToStructure( lParam, typeof( LLKH.KBDLLHOOKSTRUCT ) );
                if ( (int)wParam != LLKH.WM_KEYDOWN ) goto NEXT;

                /////////////////////////////////////////////////////////////////////////////////
                // hook [LWin+Tab] to replace TaskView 
                if ( info.vkCode == (int)Keys.Tab
                     && LLKH.IsKeyHold( Keys.LWin )
                     && !( LLKH.IsKeyHold( Keys.ControlKey ) || LLKH.IsKeyHold( Keys.ShiftKey ) ) )
                {
                    if ( ( info.flags & LLKH.KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED ) == 0 ) // not come from fake input
                    {
                        LLKH.MultipleKeyPress( new List<Keys> {(Keys)LLKH.DUMMY_KEY} );
                        User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.RiseView, 0 );
                        return LowLevelHooks.Handled;
                    }

                    goto NEXT;
                }

                /////////////////////////////////////////////////////////////////////////////////
                // since we hook default [LWin+Tab],
                // we should use a alternative way to rise the TaskView in case user want it.
                // here choose [Ctrl+LWin+Shift+Tab] to try to avoid conflicts
                if ( info.vkCode == (int)Keys.Tab
                     && LLKH.IsKeyHold( Keys.LWin )
                     && LLKH.IsKeyHold( Keys.ControlKey )
                     && LLKH.IsKeyHold( Keys.ShiftKey ) )
                {
                    if ( ( info.flags & LLKH.KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED ) == 0 // not come from fake input
                         && RiseTaskViewTimer.ElapsedMilliseconds > Const.RiseViewInterval )
                    {
                        LLKH.MultipleKeyPress( new List<Keys> {(Keys)LLKH.DUMMY_KEY} );
                        LLKH.MultipleKeyUp( new List<Keys> {Keys.ControlKey, Keys.LWin, Keys.ShiftKey, Keys.Tab} );
                        LLKH.MultipleKeyPress( new List<Keys> {Keys.LWin, Keys.Tab} );
                        LLKH.MultipleKeyDown( new List<Keys> {Keys.ControlKey, Keys.LWin, Keys.ShiftKey} );
                        RiseTaskViewTimer.Restart();
                    }

                    return LowLevelHooks.Handled;
                }

                /////////////////////////////////////////////////////////////////////////////////
                // hook [LWin+LCtrl+<DirKey>] for switch virtual desktop
                if ( LLKH.IsKeyHold( Keys.LWin )
                     && LLKH.IsKeyHold( Keys.LControlKey )
                     && !( LLKH.IsKeyHold( Keys.ShiftKey ) || LLKH.IsKeyHold( Keys.Menu ) ) )
                {
                    var key = (Keys)info.vkCode;
                    switch ( key )
                    {
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Up:
                        case Keys.Down:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)info.vkCode );
                            return LowLevelHooks.Handled;
                    }

                    goto NEXT;
                }

                /////////////////////////////////////////////////////////////////////////////////
                // hook [Esc] to hide MainView
                if ( info.vkCode == (int)Keys.Escape && IsShowing() )
                {
                    HideAll();
                    return LowLevelHooks.Handled;
                }
            }

            NEXT:
            return User32.CallNextHookEx( LLKH.HookId, nCode, wParam, lParam );
        }

        private IntPtr MouseHookCallback( int nCode, IntPtr wParam, IntPtr lParam )
        {
            if ( nCode < 0 ) goto NEXT;

            var hTaskbar = User32.FindWindow( Const.TaskbarWndClass, "" );
            if ( hTaskbar == IntPtr.Zero ) goto NEXT;

            var info = (LowLevelMouseHook.MSLLHOOKSTRUCT)Marshal.PtrToStructure( lParam, typeof( LowLevelMouseHook.MSLLHOOKSTRUCT ) );
            var msg  = (int)wParam;
            if ( msg == LowLevelMouseHook.WM_MOUSEWHEEL )
            {
                var zOrder = WindowTool.GetZOrderByHandle( hTaskbar );
                if ( zOrder > Manager.CurrentProfile.Mouse.TaskbarVisibilityThreshold ) goto NEXT;

                var rect = new RECT();
                _ = User32.GetWindowRect( hTaskbar, ref rect );
                if ( info.pt.X >= rect.Left && info.pt.Y > rect.Top )
                {
                    uint dir;
                    if ( LLKH.IsKeyHold( Keys.ShiftKey ) )
                    {
                        dir = (uint)( info.mouseData >> 16 > 0 ? Keys.Up : Keys.Down );
                    }
                    else
                    {
                        dir = (uint)( info.mouseData >> 16 > 0 ? Keys.Left : Keys.Right );
                    }

                    User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, dir );
                    return LowLevelHooks.Handled;
                }
            }

            NEXT:
            return User32.CallNextHookEx( LowLevelMouseHook.HookId, nCode, wParam, lParam );
        }

        private void Window_Closing( object sender, CancelEventArgs e )
        {
            Logger.Info( "Unset Windows LowLevelKeyboardProc Hook" );
            LLKH.UnHook();

            DisableMouseHook();

            Logger.Info( "Unregister Global HotKeys" );
            GHK.UnRegAllHotKey();
        }
    }
}