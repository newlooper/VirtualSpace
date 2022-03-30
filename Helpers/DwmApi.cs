using System;
using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class DwmApi
    {
        [Flags]
        public enum DwmWindowAttribute : uint
        {
            /// <summary>
            ///     Determines whether non-client rendering is enabled. Use this value only with DwmGetWindowAttribute. The retrieved value is of type BOOL. TRUE if non-client
            ///     rendering is enabled; otherwise, FALSE.
            /// </summary>
            DWMWA_NCRENDERING_ENABLED = 1,

            /// <summary>
            ///     The non-client rendering policy. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value from the DWMNCRENDERINGPOLICY
            ///     enumeration.
            /// </summary>
            DWMWA_NCRENDERING_POLICY,

            /// <summary>
            ///     Enable or forcibly disable DWM transitions. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value of TRUE to disable
            ///     transitions or FALSE to enable transitions.
            /// </summary>
            DWMWA_TRANSITIONS_FORCEDISABLED,

            /// <summary>
            ///     Allow content rendered in the non-client area to be visible on the frame drawn by DWM. Use this value only with DwmSetWindowAttribute, with its pvAttribute
            ///     pointing to a value of TRUE to allow content rendered in the non-client area to be visible on the frame; otherwise, FALSE.
            /// </summary>
            DWMWA_ALLOW_NCPAINT,

            /// <summary>
            ///     Retrieves the bounds of the caption button area in the window-relative space. Use this value only with DwmGetWindowAttribute. The retrieved value is of
            ///     type RECT.
            /// </summary>
            DWMWA_CAPTION_BUTTON_BOUNDS,

            /// <summary>
            ///     Specifies whether non-client content is right-to-left (RTL) mirrored. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a
            ///     value of TRUE if the non-client content is right-to-left (RTL) mirrored; otherwise, FALSE.
            /// </summary>
            DWMWA_NONCLIENT_RTL_LAYOUT,

            /// <summary>
            ///     Force the window to display an iconic thumbnail or peek representation (a static bitmap), even if a live or snapshot representation of the window is
            ///     available. This value normally is set during a window's creation and not changed throughout the window's lifetime. Some scenarios, however, might require
            ///     the value to change over time. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value of TRUE to require a iconic
            ///     thumbnail or peek representation; otherwise, FALSE.
            /// </summary>
            DWMWA_FORCE_ICONIC_REPRESENTATION,

            /// <summary>
            ///     Sets how Flip3D treats the window. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value from the DWMFLIP3DWINDOWPOLICY
            ///     enumeration.
            /// </summary>
            DWMWA_FLIP3D_POLICY,

            /// <summary>
            ///     Retrieves the extended frame bounds rectangle in screen space. Use this value only with DwmGetWindowAttribute. The retrieved value is of type RECT.
            /// </summary>
            DWMWA_EXTENDED_FRAME_BOUNDS,

            /// <summary>
            ///     The window can provide a bitmap for use by DWM as an iconic thumbnail or peek representation (a static bitmap) for the window. This value can be specified
            ///     with DWMWA_FORCE_ICONIC_REPRESENTATION. This value normally is set during a window's creation and not changed throughout the window's lifetime. Some
            ///     scenarios, however, might require the value to change over time. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value
            ///     of TRUE to inform DWM that the window will provide an iconic thumbnail or peek representation; otherwise, FALSE.
            /// </summary>
            DWMWA_HAS_ICONIC_BITMAP,

            /// <summary>
            ///     Do not show peek preview for the window. The peek view shows a full-sized preview of the window when the mouse hovers over the window's thumbnail in the
            ///     taskbar. If this attribute is set, hovering the mouse pointer over the window's thumbnail dismisses peek (in case another window in the group has a peek
            ///     preview showing). Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a value of TRUE to prevent peek functionality; FALSE to
            ///     allow it.
            /// </summary>
            DWMWA_DISALLOW_PEEK,

            /// <summary>
            ///     Prevents a window from fading to a glass sheet when peek is invoked. Use this value only with DwmSetWindowAttribute, with its pvAttribute pointing to a
            ///     value of TRUE to prevent the window from fading during another window's peek; FALSE for normal behavior.
            /// </summary>
            DWMWA_EXCLUDED_FROM_PEEK,

            /// <summary>
            ///     Do not use.
            /// </summary>
            DWMWA_CLOAK,

            /// <summary>
            ///     Use with DwmGetWindowAttribute.
            /// </summary>
            DWMWA_CLOAKED,

            /// <summary>
            ///     Use with DwmSetWindowAttribute. Freeze the window's thumbnail image with its current visuals. Do no further live updates on the thumbnail image to match
            ///     the window's contents.
            /// </summary>
            DWMWA_FREEZE_REPRESENTATION,

            /// <summary>
            ///     The maximum recognized DWMWA value, used for validation purposes.
            /// </summary>
            DWMWA_LAST
        }

        public static readonly int DWM_TNP_VISIBLE         = 0x8;
        public static readonly int DWM_TNP_OPACITY         = 0x4;
        public static readonly int DWM_TNP_RECTDESTINATION = 0x1;

        [DllImport( "dwmapi.dll" )]
        public static extern int DwmRegisterThumbnail( IntPtr dest, IntPtr src, out IntPtr thumb );

        [DllImport( "dwmapi.dll" )]
        public static extern int DwmUnregisterThumbnail( IntPtr thumb );

        [DllImport( "dwmapi.dll" )]
        public static extern int DwmQueryThumbnailSourceSize( IntPtr thumb, out SIZE size );

        [DllImport( "dwmapi.dll" )]
        public static extern int DwmUpdateThumbnailProperties( IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props );

        [DllImport( "dwmapi.dll" )]
        public static extern int DwmGetWindowAttribute( IntPtr hWnd, uint dwAttribute, out int pvAttribute, int cbAttribute );
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public int  dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        public bool fVisible;
        public bool fSourceClientAreaOnly;
    }
}