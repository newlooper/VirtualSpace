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

// base on https://github.com/microsoft/Windows.UI.Composition-Win32-Samples/blob/master/dotnet/WPF/ScreenCapture/CaptureSampleCore/BasicCapture.cs 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using SharpDX.DXGI;
using D3D11 = SharpDX.Direct3D11;
using D3D9 = SharpDX.Direct3D9;

namespace ScreenCapture
{
    public class D3D9ShareCapture : IDisposable
    {
        private static D3D9.Direct3DEx _d3D9Context = new D3D9.Direct3DEx();

        private static D3D9.DeviceEx _d3D9Device = new D3D9.DeviceEx( _d3D9Context, 0, D3D9.DeviceType.Hardware,
            IntPtr.Zero, D3D9.CreateFlags.HardwareVertexProcessing | D3D9.CreateFlags.Multithreaded | D3D9.CreateFlags.FpuPreserve,
            GetPresentParameters() );

        private readonly FrameProcessor _fp;

        private readonly Dictionary<IntPtr, D3D11.Texture2D> _frameCopyPool = new Dictionary<IntPtr, D3D11.Texture2D>();
        private readonly Direct3D11CaptureFramePool          _framePool;
        private readonly IDirect3DDevice                     _rawD3DDevice;
        private readonly Dictionary<IntPtr, D3D9.Texture>    _renderTargets = new Dictionary<IntPtr, D3D9.Texture>();
        private readonly GraphicsCaptureSession              _session;
        private readonly D3D11.Device                        _sharpDxD3D11Device;
        private readonly Stopwatch                           _sw = Stopwatch.StartNew();
        private          GraphicsCaptureItem                 _item;
        private          SizeInt32                           _lastSize;
        private          D3D9.Surface                        _targetSurface;

        public D3D9ShareCapture( GraphicsCaptureItem i, FrameProcessor fp )
        {
            _rawD3DDevice = Direct3D11Helper.CreateDevice();
            _item = i;
            _fp = fp;

            _sharpDxD3D11Device = Direct3D11Helper.CreateSharpDXDevice( _rawD3DDevice );
            _framePool = Direct3D11CaptureFramePool.Create( _rawD3DDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized, 1, i.Size );
            _session = _framePool.CreateCaptureSession( i );
            _lastSize = i.Size;

            _framePool.FrameArrived += OnFrameArrived;
        }

        public void Dispose()
        {
            _session?.Dispose();
            _framePool?.Dispose();
            _sharpDxD3D11Device?.Dispose();

            foreach ( var renderTarget in _renderTargets )
                renderTarget.Value?.Dispose();

            foreach ( var copy in _frameCopyPool )
                copy.Value?.Dispose();

            _targetSurface?.Dispose();
            _d3D9Device?.Dispose();
            _d3D9Context?.Dispose();
        }

        public void StartCapture()
        {
            _session.IsBorderRequired = false;
            _session.IsCursorCaptureEnabled = false;
            _session.StartCapture();
        }

        private void OnFrameArrived( Direct3D11CaptureFramePool sender, object args )
        {
            var newSize = false;

            using ( var frame = sender.TryGetNextFrame() )
            {
                if ( frame.ContentSize.Width != _lastSize.Width ||
                     frame.ContentSize.Height != _lastSize.Height )
                {
                    // The thing we have been capturing has changed size.
                    // We need to resize the swap chain first, then blit the pixels.
                    // After we do that, retire the frame and then recreate the frame pool.
                    newSize = true;
                    _lastSize = frame.ContentSize;
                }

                using ( var bitmap = Direct3D11Helper.CreateSharpDXTexture2D( frame.Surface ) )
                {
                    if ( _sw.ElapsedMilliseconds > _fp.Interval )
                    {
                        if ( !_frameCopyPool.ContainsKey( bitmap.NativePointer ) || newSize )
                        {
                            var desc = new D3D11.Texture2DDescription
                            {
                                BindFlags = D3D11.BindFlags.RenderTarget | D3D11.BindFlags.ShaderResource,
                                Format = Format.B8G8R8A8_UNorm,
                                Width = bitmap.Description.Width,
                                Height = bitmap.Description.Height,
                                MipLevels = 1,
                                SampleDescription = new SampleDescription( 1, 0 ),
                                Usage = D3D11.ResourceUsage.Default,
                                OptionFlags = D3D11.ResourceOptionFlags.Shared,
                                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                                ArraySize = 1
                            };
                            _frameCopyPool[bitmap.NativePointer] = new D3D11.Texture2D( _sharpDxD3D11Device, desc );
                        }

                        var copy = _frameCopyPool[bitmap.NativePointer];
                        _sharpDxD3D11Device.ImmediateContext.CopyResource( bitmap, copy );
                        var sharedHandle = GetSharedHandle( copy );

                        if ( !_renderTargets.ContainsKey( sharedHandle ) )
                        {
                            try
                            {
                                _renderTargets[sharedHandle] = new D3D9.Texture( _d3D9Device,
                                    copy.Description.Width, copy.Description.Height,
                                    1, D3D9.Usage.RenderTarget,
                                    TranslateFormat( bitmap.Description.Format ),
                                    D3D9.Pool.Default,
                                    ref sharedHandle );
                            }
                            catch
                            {
                                _d3D9Context = new D3D9.Direct3DEx();
                                _d3D9Device = new D3D9.DeviceEx( _d3D9Context, 0, D3D9.DeviceType.Hardware,
                                    IntPtr.Zero, D3D9.CreateFlags.HardwareVertexProcessing | D3D9.CreateFlags.Multithreaded | D3D9.CreateFlags.FpuPreserve,
                                    GetPresentParameters() );

                                _renderTargets[sharedHandle] = new D3D9.Texture( _d3D9Device,
                                    copy.Description.Width, copy.Description.Height,
                                    1, D3D9.Usage.RenderTarget,
                                    TranslateFormat( bitmap.Description.Format ),
                                    D3D9.Pool.Default,
                                    ref sharedHandle );
                            }
                        }

                        _targetSurface = _renderTargets[sharedHandle].GetSurfaceLevel( 0 );

                        _fp?.Proceed( _targetSurface.NativePointer );

                        _sw.Restart();
                    }
                }
            } // Retire the frame.

            if ( newSize )
            {
                _framePool.Recreate(
                    _rawD3DDevice,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    1,
                    _lastSize );
            }
        }

        private static D3D9.PresentParameters GetPresentParameters()
        {
            var presentParams = new D3D9.PresentParameters
            {
                Windowed = true,
                SwapEffect = D3D9.SwapEffect.Discard,
                DeviceWindowHandle = NativeMethods.GetDesktopWindow(),
                PresentationInterval = D3D9.PresentInterval.Default
            };

            return presentParams;
        }

        private static IntPtr GetSharedHandle( D3D11.Texture2D texture )
        {
            using var resource = texture.QueryInterface<Resource>();
            return resource.SharedHandle;
        }

        private static D3D9.Format TranslateFormat( Format format )
        {
            return format switch
            {
                Format.R10G10B10A2_UNorm => D3D9.Format.A2B10G10R10,
                Format.R16G16B16A16_Float => D3D9.Format.A16B16G16R16F,
                Format.B8G8R8A8_UNorm => D3D9.Format.A8R8G8B8,
                _ => D3D9.Format.Unknown
            };
        }
    }

    public static class NativeMethods
    {
        [DllImport( "user32.dll", SetLastError = false )]
        public static extern IntPtr GetDesktopWindow();
    }
}