//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

// based on https://github.com/microsoft/Windows.UI.Composition-Win32-Samples/blob/master/dotnet/WPF/ScreenCapture/Composition.WindowsRuntimeHelpers/CaptureHelper.cs

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Capture;
#if NET5_0_OR_GREATER
using WinRT;
#endif

namespace ScreenCapture
{
    public static class CaptureHelper
    {
        private static readonly Guid GraphicsCaptureItemGuid = new( "79C3F95B-31F7-4EC2-A464-632EF5D30760" );

        public static void SetWindow( this GraphicsCapturePicker picker, IntPtr hwnd )
        {
            var interop = (IInitializeWithWindow)(object)picker;
            interop.Initialize( hwnd );
        }

        public static GraphicsCaptureItem CreateItemForWindow( IntPtr hwnd )
        {
#if NET5_0_OR_GREATER
            var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
#else
            var factory = WindowsRuntimeMarshal.GetActivationFactory( typeof( GraphicsCaptureItem ) );
            var interop = (IGraphicsCaptureItemInterop)factory;
#endif
            var temp        = typeof( GraphicsCaptureItem );
            var itemPointer = interop.CreateForWindow( hwnd, GraphicsCaptureItemGuid );
#if NET5_0_OR_GREATER
            var item = MarshalInterface<GraphicsCaptureItem>.FromAbi( itemPointer );
#else
            var item = Marshal.GetObjectForIUnknown( itemPointer ) as GraphicsCaptureItem;
#endif
            Marshal.Release( itemPointer );

            return item;
        }

        public static GraphicsCaptureItem CreateItemForMonitor( IntPtr hmon )
        {
#if NET5_0_OR_GREATER
            var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
#else
            var factory = WindowsRuntimeMarshal.GetActivationFactory( typeof( GraphicsCaptureItem ) );
            var interop = (IGraphicsCaptureItemInterop)factory;
#endif
            var temp        = typeof( GraphicsCaptureItem );
            var itemPointer = interop.CreateForMonitor( hmon, GraphicsCaptureItemGuid );

#if NET5_0_OR_GREATER
            var item = MarshalInterface<GraphicsCaptureItem>.FromAbi( itemPointer );
#else
            var item = Marshal.GetObjectForIUnknown( itemPointer ) as GraphicsCaptureItem;
#endif
            Marshal.Release( itemPointer );

            return item;
        }

        [ComImport]
        [Guid( "3E68D4BD-7135-4D10-8018-9FB6D9F33FA1" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        [ComVisible( true )]
        private interface IInitializeWithWindow
        {
            void Initialize(
                IntPtr hwnd );
        }

        [ComImport]
        [Guid( "3628E81B-3CAC-4C60-B7F4-23CE0E0C3356" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        [ComVisible( true )]
        private interface IGraphicsCaptureItemInterop
        {
            IntPtr CreateForWindow(
                [In]     IntPtr window,
                [In] ref Guid   iid );

            IntPtr CreateForMonitor(
                [In]     IntPtr monitor,
                [In] ref Guid   iid );
        }
    }
}