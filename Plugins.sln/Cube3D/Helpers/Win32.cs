using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public enum GetWindowLongFields
    {
        GWL_USERDATA   = -21, // 0xFFFFFFEB
        GWL_EXSTYLE    = -20, // 0xFFFFFFEC
        GWL_STYLE      = -16, // 0xFFFFFFF0
        GWL_ID         = -12, // 0xFFFFFFF4
        GWL_HWNDPARENT = -8, // 0xFFFFFFF8
        GWL_HINSTANCE  = -6, // 0xFFFFFFFA
        GWL_WNDPROC    = -4 // 0xFFFFFFFC
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct INPUT
    {
        public uint                    Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    [StructLayout( LayoutKind.Explicit )]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset( 0 )] public HARDWAREINPUT Hardware;
        [FieldOffset( 0 )] public KEYBDINPUT    Keyboard;
        [FieldOffset( 0 )] public MOUSEINPUT    Mouse;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct HARDWAREINPUT
    {
        public uint   Msg;
        public ushort ParamL;
        public ushort ParamH;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint   Flags;
        public uint   Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct MOUSEINPUT
    {
        public int    X;
        public int    Y;
        public uint   MouseData;
        public uint   Flags;
        public uint   Time;
        public IntPtr ExtraInfo;
    }
}