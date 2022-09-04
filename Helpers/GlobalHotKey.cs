/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class GlobalHotKey
    {
        [Flags]
        public enum KeyModifiers
        {
            None       = 0,
            Alt        = 1,
            Ctrl       = 2,
            Shift      = 4,
            WindowsKey = 8
        }

        private static          IntPtr    _handle = IntPtr.Zero;
        private static readonly List<int> Ids     = new();

        public static bool RegHotKey( IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk )
        {
            _handle = hWnd;
            Ids.Add( id );
            return RegisterHotKey( hWnd, id, fsModifiers, vk );
        }

        [DllImport( "user32.dll", SetLastError = true )]
        private static extern bool RegisterHotKey( IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool UnregisterHotKey( IntPtr hWnd, int id );

        public static void UnRegAllHotKey()
        {
            foreach ( var id in Ids )
            {
                UnregisterHotKey( _handle, id );
            }
        }
    }

    public static class LowLevelGlobalHotKey
    {
        public const   int                         WM_KEYDOWN     = 0x0100;
        public const   int                         WM_KEYUP       = 0x0101;
        public const   int                         WM_SYSKEYDOWN  = 0x0104;
        public const   int                         WM_SYSKEYUP    = 0x0105;
        public const   int                         DUMMY_KEY      = 0xFF;
        private const  int                         WH_KEYBOARD_LL = 13;
        private static User32.LowLevelKeyboardProc _hookProc;

        public static IntPtr HookId { get; private set; } = IntPtr.Zero;

        public static void SetHook( User32.LowLevelKeyboardProc proc )
        {
            _hookProc = proc;
            HookId = User32.SetWindowsHookEx( WH_KEYBOARD_LL, _hookProc, Kernel32.GetModuleHandle( null ), 0 );
        }

        public static void MultipleKeyPress( List<int> keys )
        {
            var inputs = new INPUT[keys.Count * 2];
            for ( var i = 0; i < keys.Count; i++ )
            {
                inputs[i].Type = 1;
                inputs[i].Data.Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keys[i],
                    Scan = 0,
                    Flags = Convert.ToUInt32( 0 ),
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
                inputs[inputs.Length - i - 1].Type = 1;
                inputs[inputs.Length - i - 1].Data.Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keys[i],
                    Scan = 0,
                    Flags = Convert.ToUInt32( 1 ),
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
            }

            var result = User32.SendInput( Convert.ToUInt32( inputs.Length ), inputs, Marshal.SizeOf( typeof( INPUT ) ) );
            if ( result == 0 )
                throw new Exception();
        }

        public static void MultipleKeyUp( List<int> keys )
        {
            var inputs = new INPUT[keys.Count];
            for ( var positive = 0; positive < keys.Count; positive++ )
            {
                inputs[positive].Type = 1;
                inputs[positive].Data.Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keys[positive],
                    Scan = 0,
                    Flags = Convert.ToUInt32( 1 ),
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
            }

            var result = User32.SendInput( Convert.ToUInt32( inputs.Length ), inputs, Marshal.SizeOf( typeof( INPUT ) ) );
            if ( result == 0 )
                throw new Exception();
        }

        public static void UnHook()
        {
            User32.UnhookWindowsHookEx( HookId );
        }

        public struct KBDLLHOOKSTRUCT
        {
            public  int vkCode;
            private int scanCode;
            public  int flags;
            private int time;
            private int dwExtraInfo;
        }
    }
}