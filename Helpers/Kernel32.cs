using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class Kernel32
    {
        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = false )]
        public static extern IntPtr GetModuleHandle( string lpModuleName );
    }
}