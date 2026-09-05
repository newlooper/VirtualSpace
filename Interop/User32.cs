#if !DEBUG
using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static partial class User32
    {
        public delegate bool EnumChildWindowsProc( IntPtr hWnd, int lParam );

        public delegate bool EnumWindowsProc( IntPtr hWnd, int lParam );

        public delegate IntPtr HookProc( int nCode, IntPtr wParam, IntPtr lParam );

        // LibraryImport is ExactSpelling: use *W entry points (DllImport CharSet.Auto used to append W).
        [LibraryImport( "user32.dll", EntryPoint = "FindWindowW", StringMarshalling = StringMarshalling.Utf16 )]
        public static partial IntPtr FindWindow( string lpClassName, string lpWindowName );

        [LibraryImport( "user32.dll", EntryPoint = "GetWindowLongW" )]
        public static partial int GetWindowLong( IntPtr hWnd, int nIndex );

        [LibraryImport( "user32.dll" )]
        public static partial IntPtr GetWindow( IntPtr hWnd, GetWindowType uCmd );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool IsWindowEnabled( IntPtr hWnd );

        [LibraryImport( "user32.dll", EntryPoint = "PostMessageW" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool PostMessage( IntPtr hWnd, int msg, ulong wParam, ulong lParam );

        [LibraryImport( "user32.dll", EntryPoint = "SendMessageW" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool SendMessage( IntPtr hWnd, int msg, ulong wParam, ulong lParam );

        public static IntPtr SetWindowLongPtr( HandleRef hWnd, int nIndex, int dwNewLong )
        {
            if ( IntPtr.Size == 8 )
                return SetWindowLongPtr64( hWnd.Handle, nIndex, (IntPtr)dwNewLong );
            return new IntPtr( SetWindowLong32( hWnd.Handle, nIndex, dwNewLong ) );
        }

        [LibraryImport( "user32.dll", EntryPoint = "SetWindowLongW" )]
        private static partial int SetWindowLong32( IntPtr hWnd, int nIndex, int dwNewLong );

        [LibraryImport( "user32.dll", EntryPoint = "SetWindowLongPtrW" )]
        private static partial IntPtr SetWindowLongPtr64( IntPtr hWnd, int nIndex, IntPtr dwNewLong );

        [LibraryImport( "user32.dll" )]
        public static partial int ShowWindow( IntPtr hWnd, short cmdShow );

        // LibraryImport cannot marshal char buffers unless runtime marshalling is disabled (SYSLIB1051).
        [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
        public static extern int GetWindowText( IntPtr hWnd, [Out] char[] lpString, int nMaxCount );

        [DllImport( "user32.dll", CharSet = CharSet.Unicode )]
        public static extern int GetClassName( IntPtr hWnd, [Out] char[] lpClassName, int nMaxCount );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool IsWindowVisible( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool IsWindow( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool IsIconic( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        public static partial int EnumWindows( EnumWindowsProc func, int lParam );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool EnumChildWindows( IntPtr hWndParent, EnumChildWindowsProc lpEnumFunc, int lParam );

        [LibraryImport( "user32.dll" )]
        public static partial IntPtr GetForegroundWindow();

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool SetForegroundWindow( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool BringWindowToTop( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        public static partial IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

        [LibraryImport( "user32.dll" )]
        public static partial int GetWindowThreadProcessId( IntPtr hWnd, out int processId );

        [LibraryImport( "user32.dll", EntryPoint = "SetWindowsHookExW" )]
        public static partial IntPtr SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool UnhookWindowsHookEx( IntPtr hhk );

        [LibraryImport( "user32.dll" )]
        public static partial IntPtr CallNextHookEx( IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam );

        [LibraryImport( "user32.dll" )]
        public static partial short GetAsyncKeyState( int vKey );

        [LibraryImport( "user32.dll" )]
        public static partial short GetKeyState( int vKey );

        [LibraryImport( "user32.dll" )]
        public static partial uint SendInput( uint numberOfInputs, [In] INPUT[] inputs, int sizeOfInputStructure );

        [LibraryImport( "user32.dll", EntryPoint = "RegisterWindowMessageW", StringMarshalling = StringMarshalling.Utf16 )]
        public static partial uint RegisterWindowMessage( string lpProcName );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool GetWindowRect( IntPtr hWnd, ref RECT rectangle );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool GetWindowPlacement( IntPtr hWnd, ref WINDOWPLACEMENT lpWndPl );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool AttachThreadInput( int idAttach, int idAttachTo, [MarshalAs( UnmanagedType.Bool )] bool fAttach );

        [LibraryImport( "user32.dll" )]
        public static partial IntPtr SetFocus( IntPtr hWnd );

        [LibraryImport( "user32.dll" )]
        public static partial void SwitchToThisWindow( IntPtr hWnd, [MarshalAs( UnmanagedType.Bool )] bool fAltTab );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool ShowWindowAsync( IntPtr hWnd, int nCmdShow );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool RedrawWindow(
            IntPtr            hWnd,
            IntPtr            lprcUpdate,
            IntPtr            hrgnUpdate,
            RedrawWindowFlags flags );

        [LibraryImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static partial bool ClipCursor( IntPtr lpRect );
    }
}
#endif
