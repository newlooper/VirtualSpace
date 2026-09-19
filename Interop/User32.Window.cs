using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers;

public static partial class User32
{
    public static readonly IntPtr HwndMessage = new( -3 );

    public const int ERROR_CLASS_ALREADY_EXISTS = 1410;

    public delegate IntPtr WindowProc( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

    [LibraryImport( "user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true )]
    public static partial ushort RegisterClassEx( in WNDCLASSEX lpwcx );

    [LibraryImport( "user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static partial bool UnregisterClass( string lpClassName, IntPtr hInstance );

    [LibraryImport( "user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr CreateWindowEx(
        uint    dwExStyle,
        string  lpClassName,
        string? lpWindowName,
        uint    dwStyle,
        int     x,
        int     y,
        int     nWidth,
        int     nHeight,
        IntPtr  hWndParent,
        IntPtr  hMenu,
        IntPtr  hInstance,
        IntPtr  lpParam );

    [LibraryImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static partial bool DestroyWindow( IntPtr hWnd );

    [LibraryImport( "user32.dll", EntryPoint = "DefWindowProcW" )]
    public static partial IntPtr DefWindowProc( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

    [LibraryImport( "user32.dll", EntryPoint = "FindWindowExW", StringMarshalling = StringMarshalling.Utf16 )]
    public static partial IntPtr FindWindowEx( IntPtr hWndParent, IntPtr hWndChildAfter, string? lpszClass, string? lpszWindow );
}
