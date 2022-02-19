using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class VisualEffects
    {
        public enum AccentState
        {
            ACCENT_DISABLED                   = 0,
            ACCENT_ENABLE_GRADIENT            = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND          = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND   = 4,
            ACCENT_INVALID_STATE              = 5
        }

        public enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        [DllImport( "user32.dll" )]
        public static extern int SetWindowCompositionAttribute( IntPtr hWnd, ref WindowCompositionAttributeData data );

        [StructLayout( LayoutKind.Sequential )]
        public struct AccentPolicy
        {
            public AccentState AccentState;
            public uint        AccentFlags;
            public uint        GradientColor;
            public uint        AnimationId;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr                     Data;
            public int                        SizeOfData;
        }
    }
}