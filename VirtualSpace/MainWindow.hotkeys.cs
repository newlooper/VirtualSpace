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
using VirtualSpace.Commons;
using VirtualSpace.Helpers;
using GHK = VirtualSpace.Helpers.GlobalHotKey;
using LLGHK = VirtualSpace.Helpers.LowLevelGlobalHotKey;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static          bool   _inRising;
        private static readonly IntPtr Handled = (IntPtr)1;

        private void RegisterHotKey( IntPtr hWnd )
        {
            IpcPipeServer.MainWindowHandle = hWnd;
            GHK.RegHotKey( hWnd, UserMessage.RiseView,
                GHK.KeyModifiers.Ctrl | GHK.KeyModifiers.Shift,
                KeyInterop.VirtualKeyFromKey( Key.Tab ) );
            Logger.Info( "Register Global HotKey For Rise MainView - [CTRL+SHIFT+Tab]" );

            GHK.RegHotKey( hWnd, UserMessage.ShowAppController,
                GHK.KeyModifiers.Ctrl | GHK.KeyModifiers.Alt,
                KeyInterop.VirtualKeyFromKey( Key.F12 ) );
            Logger.Info( "Register Global HotKey For Open AppControl Panel - [CTRL+ALT+F12]" );

            // GHK.RegHotKey( hWnd, UserMessage.CloseView,
            //     GlobalHotKey.KeyModifiers.None,
            //     KeyInterop.VirtualKeyFromKey( Key.Escape ) );
            // Logger.Info( "Register Global HotKey For Close MainView - [Escape]" );

            var hookProc = new User32.LowLevelKeyboardProc( KeyboardHookCallback );
            Logger.Info( "Set Windows LowLevelKeyboardProc Hook" );
            LLGHK.SetHook( hookProc );
        }

        private IntPtr KeyboardHookCallback( int nCode, IntPtr wParam, IntPtr lParam )
        {
            var info    = (LowLevelGlobalHotKey.KBDLLHOOKSTRUCT)Marshal.PtrToStructure( lParam, typeof( LowLevelGlobalHotKey.KBDLLHOOKSTRUCT ) );
            var keyType = (int)wParam;
            if ( nCode >= 0 )
            {
                if ( keyType == LLGHK.WM_KEYDOWN
                     && info.vkCode == (int)Keys.Tab
                     && User32.GetAsyncKeyState( (int)Keys.LWin ) < 0 ) // hook LWin+Tab to replace TaskView
                {
                    LLGHK.MultipleKeyPress( new List<int> {LLGHK.DUMMY_KEY} );
                    LLGHK.MultipleKeyUp( new List<int> {(int)Keys.LWin} );
                    User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.RiseView, 0 );
                    _inRising = true;
                    return Handled;
                }

                if ( keyType == LLGHK.WM_KEYDOWN
                     && User32.GetAsyncKeyState( (int)Keys.LWin ) < 0
                     && User32.GetAsyncKeyState( (int)Keys.LControlKey ) < 0 ) // hook LWin+LCtrl+<DirKey> for switch virtual desktop
                {
                    var key = (Keys)info.vkCode;
                    switch ( key )
                    {
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Up:
                        case Keys.Down:
                            User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)info.vkCode );
                            return Handled;
                    }
                }

                if ( keyType == LLGHK.WM_KEYDOWN && info.vkCode == (int)Keys.Escape ) // hook Esc to hide MainView
                {
                    HideAll();
                }
            }

            if ( keyType == LLGHK.WM_KEYUP
                 && info.vkCode == (int)Keys.LWin
                 && _inRising ) // when MainView rising, send dummy_key to avoid other apps which listen LWin key trigger their actions
            {
                LLGHK.MultipleKeyPress( new List<int> {LLGHK.DUMMY_KEY} );
                _inRising = false;
            }

            return User32.CallNextHookEx( LLGHK.HookId, nCode, wParam, lParam );
        }

        private void Window_Closing( object sender, CancelEventArgs e )
        {
            Logger.Info( "Unset Windows LowLevelKeyboardProc Hook" );
            LowLevelGlobalHotKey.UnHook();
            Logger.Info( "Unregister Global HotKeys" );
            GlobalHotKey.UnRegHotKey();
        }
    }
}