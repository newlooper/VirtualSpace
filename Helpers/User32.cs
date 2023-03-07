using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VirtualSpace.Helpers
{
    public static class User32
    {
        public delegate bool EnumChildWindowsProc( IntPtr hWnd, int lParam );

        public delegate bool EnumWindowsProc( IntPtr hWnd, int lParam );

        public delegate IntPtr HookProc( int nCode, IntPtr wParam, IntPtr lParam );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern IntPtr FindWindow( string lpClassName, string lpWindowName );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern int GetWindowLong( IntPtr hWnd, int nIndex );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern IntPtr GetWindow( IntPtr hWnd, GetWindowType uCmd );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool IsWindowEnabled( IntPtr hWnd );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool PostMessage( IntPtr hWnd, int msg, ulong wParam, ulong lParam );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        public static extern bool SendMessage( IntPtr hWnd, int msg, ulong wParam, ulong lParam );

        public static IntPtr SetWindowLongPtr( HandleRef hWnd, int nIndex, int dwNewLong )
        {
            if ( IntPtr.Size == 8 )
                return SetWindowLongPtr64( hWnd, nIndex, (IntPtr)dwNewLong );
            else
                return new IntPtr( SetWindowLong32( hWnd, nIndex, dwNewLong ) );
        }

        [DllImport( "user32.dll", EntryPoint = "SetWindowLong" )]
        private static extern int SetWindowLong32( HandleRef hWnd, int nIndex, int dwNewLong );

        [DllImport( "user32.dll", EntryPoint = "SetWindowLongPtr" )]
        private static extern IntPtr SetWindowLongPtr64( HandleRef hWnd, int nIndex, IntPtr dwNewLong );

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
        public static extern bool IsIconic( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        public static extern int EnumWindows( EnumWindowsProc func, int lParam );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool EnumChildWindows( IntPtr hWndParent, EnumChildWindowsProc lpEnumFunc, int lParam );

        [DllImport( "user32.dll" )]
        public static extern IntPtr GetForegroundWindow();

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool SetForegroundWindow( IntPtr hWnd );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern bool BringWindowToTop( IntPtr hWnd );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

        [DllImport( "user32.dll" )]
        public static extern int GetWindowThreadProcessId( IntPtr hWnd, out int processId );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern IntPtr SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId );

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

        [DllImport( "user32.dll" )]
        public static extern bool GetWindowRect( IntPtr hWnd, ref RECT rectangle );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags );

        [DllImport( "user32.dll", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern bool GetWindowPlacement( IntPtr hWnd, ref WINDOWPLACEMENT lpWndPl );

        [DllImport( "Shell32.dll", SetLastError = false )]
        public static extern int SHGetStockIconInfo( SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern void SwitchToThisWindow( IntPtr hWnd, bool fAltTab );
    }
}