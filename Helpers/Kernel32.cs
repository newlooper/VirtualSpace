using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static partial class Kernel32
    {
        [LibraryImport( "kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16 )]
        public static partial IntPtr GetModuleHandle( string lpModuleName );
    }
}
