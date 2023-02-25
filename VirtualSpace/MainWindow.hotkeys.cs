/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using GHK = VirtualSpace.Helpers.GlobalHotKey;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private void RegisterHotKey( IntPtr hWnd )
        {
            foreach ( var kv in Manager.Configs.KeyBindings )
            {
                var func = Const.Hotkey.GetFuncDesc( kv.Key );
                var kb   = kv.Value;

                var ghkCode = kb.GhkCode;
                if ( ghkCode == "" ) continue;

                var hotkey = ghkCode.Replace( Const.Hotkey.NONE + Const.Hotkey.SPLITTER, "" );

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
                            hotkey,
                            func,
                            GHK.RegHotKey( hWnd, kb.MessageId, km, KeyInterop.VirtualKeyFromKey( key ) )
                                ? "Success"
                                : "Fail" ) );
                    }
                    catch ( Exception ex )
                    {
                        Logger.Error( string.Format( "Register Global HotKey [{0}] For \"{1}\" Error: {2}",
                            hotkey,
                            func,
                            ex.Message ) );
                    }
                }
            }

            var keyboardHookProc = new User32.HookProc( KeyboardHookCallback );
            Logger.Info( "Set Windows LowLevelKeyboardProc Hook" );
            LowLevelKeyboardHook.SetHook( keyboardHookProc );

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
            var info = (LowLevelKeyboardHook.KBDLLHOOKSTRUCT)Marshal.PtrToStructure( lParam, typeof( LowLevelKeyboardHook.KBDLLHOOKSTRUCT ) );
            var msg  = (int)wParam;
            if ( nCode >= 0 )
            {
                /////////////////////////////////////////////////////////////////////////////////
                // hook LWin+Tab to replace TaskView 
                if ( msg == LowLevelKeyboardHook.WM_KEYDOWN
                     && info.vkCode == (int)Keys.Tab
                     && User32.GetAsyncKeyState( (int)Keys.LWin ) < 0
                     && User32.GetAsyncKeyState( (int)Keys.ControlKey ) >= 0
                     && User32.GetAsyncKeyState( (int)Keys.ShiftKey ) >= 0
                   )
                {
                    LowLevelKeyboardHook.MultipleKeyPress( new List<int> {LowLevelKeyboardHook.DUMMY_KEY} );
                    User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.RiseView, 0 );
                    return LowLevelHooks.Handled;
                }

                /////////////////////////////////////////////////////////////////////////////////
                // hook LWin+LCtrl+<DirKey> for switch virtual desktop
                if ( msg == LowLevelKeyboardHook.WM_KEYDOWN
                     && User32.GetAsyncKeyState( (int)Keys.LWin ) < 0
                     && User32.GetAsyncKeyState( (int)Keys.LControlKey ) < 0 )
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
                }

                /////////////////////////////////////////////////////////////////////////////////
                // hook Esc to hide MainView
                if ( msg == LowLevelKeyboardHook.WM_KEYDOWN && info.vkCode == (int)Keys.Escape )
                {
                    if ( IsShowing() ) HideAll();
                }
            }

            return User32.CallNextHookEx( LowLevelKeyboardHook.HookId, nCode, wParam, lParam );
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
                    if ( User32.GetAsyncKeyState( (int)Keys.ShiftKey ) < 0 )
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
            LowLevelKeyboardHook.UnHook();

            DisableMouseHook();

            Logger.Info( "Unregister Global HotKeys" );
            GlobalHotKey.UnRegAllHotKey();
        }
    }
}