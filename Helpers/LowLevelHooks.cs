/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualSpace.Helpers
{
    public static class LowLevelHooks
    {
        public static readonly IntPtr Handled = (IntPtr)1;
    }

    public static class LowLevelKeyboardHook
    {
        public const   int             WM_KEYDOWN     = 0x0100;
        public const   int             WM_KEYUP       = 0x0101;
        public const   int             DUMMY_KEY      = 0xFF;
        private const  int             WH_KEYBOARD_LL = 13;
        private static User32.HookProc _hookProc;

        public static IntPtr HookId { get; private set; } = IntPtr.Zero;

        public static void SetHook( User32.HookProc proc )
        {
            _hookProc = proc;
            HookId = User32.SetWindowsHookEx( WH_KEYBOARD_LL, _hookProc, Kernel32.GetModuleHandle( null ), 0 );
        }

        public static void MultipleKeyDown( List<Keys> keys )
        {
            SendKeys( keys, 0 );
        }

        public static void MultipleKeyUp( List<Keys> keys )
        {
            SendKeys( keys, 2 );
        }

        public static void MultipleKeyPress( List<Keys> keys )
        {
            SendKeysCombine( keys, 2 );
        }

        private static void SendKeys( List<Keys> keys, int flags )
        {
            var inputs = new INPUT[keys.Count];
            for ( var pos = 0; pos < keys.Count; pos++ )
            {
                inputs[pos].Type = 1;
                inputs[pos].Data.Keyboard = new KEYBDINPUT
                {
                    Vk = (ushort)keys[pos],
                    Scan = 0,
                    Flags = Convert.ToUInt32( flags ),
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
            }

            var result = User32.SendInput( Convert.ToUInt32( inputs.Length ), inputs, Marshal.SizeOf( typeof( INPUT ) ) );
            if ( result == 0 )
                throw new Exception();
        }

        private static void SendKeysCombine( List<Keys> keys, int flags )
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
                    Flags = Convert.ToUInt32( flags ),
                    Time = 0,
                    ExtraInfo = IntPtr.Zero
                };
            }

            var result = User32.SendInput( Convert.ToUInt32( inputs.Length ), inputs, Marshal.SizeOf( typeof( INPUT ) ) );
            if ( result == 0 )
                throw new Exception();
        }

        public static bool IsKeyHold( Keys key )
        {
            return User32.GetAsyncKeyState( (int)key ) < 0;
        }

        public static void UnHook()
        {
            User32.UnhookWindowsHookEx( HookId );
        }

        public struct KBDLLHOOKSTRUCT
        {
            public  int                  vkCode;
            private int                  scanCode;
            public  KBDLLHOOKSTRUCTFlags flags;
            private int                  time;
            private int                  dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN  = 0x20,
            LLKHF_UP       = 0x80,
        }
    }

    public static class LowLevelMouseHook
    {
        private const  int             WH_MOUSE_LL   = 14;
        public const   int             WM_MOUSEWHEEL = 0x020A;
        private static User32.HookProc _hookProc;

        public static IntPtr HookId { get; private set; } = IntPtr.Zero;

        public static void SetHook( User32.HookProc proc )
        {
            _hookProc = proc;
            HookId = User32.SetWindowsHookEx( WH_MOUSE_LL, _hookProc, Kernel32.GetModuleHandle( null ), 0 );
        }

        public static void UnHook()
        {
            User32.UnhookWindowsHookEx( HookId );
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct MSLLHOOKSTRUCT
        {
            public POINT   pt;
            public int     mouseData; // be careful, this must be ints, not uints.
            public int     flags;
            public int     time;
            public UIntPtr dwExtraInfo;
        }
    }
}