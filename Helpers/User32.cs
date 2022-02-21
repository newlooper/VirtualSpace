using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VirtualSpace.Helpers
{
    public static class User32
    {
        public delegate bool EnumChildWindowsProc( IntPtr hWnd, int lParam );

        public delegate bool EnumWindowsProc( IntPtr hWnd, int lParam );

        public delegate IntPtr LowLevelKeyboardProc( int nCode, IntPtr wParam, IntPtr lParam );

        public const int  WS_EX_TOPMOST     = 0x8;
        public const int  WS_EX_TOOLWINDOW  = 0x80;
        public const int  WS_EX_LAYERED     = 0x80000;
        public const int  WS_EX_TRANSPARENT = 0x20;
        public const int  WS_EX_NOACTIVATE  = 0x08000000;
        public const int  SW_SHOWNOACTIVATE = 4;
        public const int  SW_HIDE           = 0;
        public const uint WS_POPUP          = 0x80000000;
        public const uint AW_HOR_POSITIVE   = 0x1;
        public const uint AW_HOR_NEGATIVE   = 0x2;
        public const uint AW_VER_POSITIVE   = 0x4;
        public const uint AW_VER_NEGATIVE   = 0x8;
        public const uint AW_CENTER         = 0x10;
        public const uint AW_HIDE           = 0x10000;
        public const uint AW_ACTIVATE       = 0x20000;
        public const uint AW_SLIDE          = 0x40000;
        public const uint AW_BLEND          = 0x80000;

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern int GetWindowLong( IntPtr hWnd, int nIndex );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool PostMessage( IntPtr hWnd, int Msg, uint wParam, uint lParam );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool SendMessage( IntPtr hWnd, int Msg, uint wParam, uint lParam );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern int SetWindowLong( IntPtr hWnd, int nIndex, int newLong );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern int ShowWindow( IntPtr hWnd, short cmdShow );

        [DllImport( "user32.dll" )]
        public static extern int GetWindowText( IntPtr hWnd, StringBuilder buf, int nMaxCount );

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

        [DllImport( "user32.dll" )]
        public static extern bool IsWindowVisible( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool IsWindow( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        public static extern int EnumWindows( EnumWindowsProc func, int lParam );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool EnumChildWindows( IntPtr hWndParent, EnumChildWindowsProc lpEnumFunc, int lParam );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern void SwitchToThisWindow( IntPtr hWnd, bool turnOn );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

        [DllImport( "user32.dll" )]
        public static extern int GetWindowThreadProcessId( IntPtr hWnd, out int processId );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern IntPtr SetWindowsHookEx( int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool UnhookWindowsHookEx( IntPtr hhk );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern IntPtr CallNextHookEx( IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam );

        [DllImport( "user32.dll" )]
        public static extern short GetAsyncKeyState( int vKey );

        [DllImport( "user32.dll" )]
        public static extern short GetKeyState( int vKey );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern uint SendInput( uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure );

        [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
        public static extern uint RegisterWindowMessage( string lpProcName );
    }
}