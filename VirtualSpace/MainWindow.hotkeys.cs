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
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using GHK = VirtualSpace.Helpers.GlobalHotKey;
using LLGHK = VirtualSpace.Helpers.LowLevelGlobalHotKey;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private static          bool   _inRising;
        private static          bool   _switching;
        private static readonly IntPtr Handled = (IntPtr)1;

        private void RegisterHotKey( IntPtr hWnd )
        {
            IpcPipeServer.MainWindowHandle = hWnd;

            foreach ( var kv in Manager.Configs.KeyBindings )
            {
                var func = Const.Hotkey.Info[kv.Key].Item1;
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
                     && User32.GetAsyncKeyState( (int)Keys.LWin ) < 0
                     && User32.GetAsyncKeyState( (int)Keys.ControlKey ) >= 0
                     && User32.GetAsyncKeyState( (int)Keys.ShiftKey ) >= 0
                   ) // hook LWin+Tab to replace TaskView
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
                            if (!_switching)
                            {
                                _switching = true;
                                User32.PostMessage(Handle, WinMsg.WM_HOTKEY, UserMessage.SwitchDesktop, (uint)info.vkCode);
                            }
                            return Handled;
                    }
                }

                if (keyType == LLGHK.WM_KEYUP
                     && User32.GetAsyncKeyState((int)Keys.LWin) < 0
                     && User32.GetAsyncKeyState((int)Keys.LControlKey) < 0) // hook LWin+LCtrl+<DirKey> for switch virtual desktop
                {
                    var key = (Keys)info.vkCode;
                    switch (key)
                    {
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Up:
                        case Keys.Down:
                            _switching = false;
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
            GlobalHotKey.UnRegAllHotKey();
        }
    }
}