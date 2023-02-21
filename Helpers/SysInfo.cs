// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Management;

namespace VirtualSpace.Helpers
{
    public static class SysInfo
    {
        private const int                          DefaultDpi = 96;
        public static (float ScaleX, float ScaleY) Dpi => GetDpi();

        public static bool IsAdministrator()
        {
            var current          = WindowsIdentity.GetCurrent();
            var windowsPrincipal = new WindowsPrincipal( current );
            return windowsPrincipal.IsInRole( WindowsBuiltInRole.Administrator );
        }

        private static (float ScaleX, float ScaleY) GetDpi()
        {
            using var g = Graphics.FromHwnd( IntPtr.Zero );
            return ( g.DpiX / DefaultDpi, g.DpiY / DefaultDpi );
        }

        public static Version OSVersion
        {
            get
            {
                var winVer = Environment.OSVersion.Version;
                if ( winVer.Revision == 0 )
                {
                    using var registryKey = Registry.LocalMachine.OpenSubKey( @"SOFTWARE\Microsoft\Windows NT\CurrentVersion" );
                    var       ubr         = registryKey?.GetValue( "UBR" );
                    if ( ubr != null )
                    {
                        var buildNumber = int.Parse( ubr.ToString() );
                        winVer = new Version( winVer.Major, winVer.Minor, winVer.Build, buildNumber );
                    }
                }

                return winVer;
            }
        }

        public static ValueTuple<int, int> GetAspectRadioOfScreen()
        {
            var nGCD = GetGreatestCommonDivisor( Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height );
            return new ValueTuple<int, int>( Screen.PrimaryScreen.Bounds.Width / nGCD, Screen.PrimaryScreen.Bounds.Height / nGCD );
        }

        private static int GetGreatestCommonDivisor( int a, int b )
        {
            while ( true )
            {
                if ( b == 0 ) return a;
                var a1 = a;
                a = b;
                b = a1 % b;
            }
        }
    }

    /// <summary>
    /// https://stackoverflow.com/questions/52875087/getting-device-friendly-name-incorrect-result
    /// </summary>
    public static class ScreenInterrogatory
    {
        private const int ERROR_SUCCESS = 0;

        private static string MonitorFriendlyName( LUID adapterId, uint targetId )
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header =
                {
                    size = (uint)Marshal.SizeOf( typeof( DISPLAYCONFIG_TARGET_DEVICE_NAME ) ),
                    adapterId = adapterId,
                    id = targetId,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };
            var error = DisplayConfigGetDeviceInfo( ref deviceName );
            if ( error != ERROR_SUCCESS )
                throw new Win32Exception( error );
            return deviceName.monitorFriendlyDeviceName;
        }

        private static IEnumerable<string> GetAllMonitorsFriendlyNames()
        {
            var error = GetDisplayConfigBufferSizes(
                QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                out var pathCount,
                out var modeCount );

            if ( error != ERROR_SUCCESS )
                throw new Win32Exception( error );

            var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            error = QueryDisplayConfig(
                QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
                ref pathCount,
                displayPaths,
                ref modeCount,
                displayModes,
                IntPtr.Zero );

            if ( error != ERROR_SUCCESS )
                throw new Win32Exception( error );

            for ( var i = 0; i < modeCount; i++ )
                if ( displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET )
                    yield return MonitorFriendlyName( displayModes[i].adapterId, displayModes[i].id );
        }

        public static string DeviceFriendlyName( this Screen screen )
        {
            var allFriendlyNames = GetAllMonitorsFriendlyNames();
            for ( var index = 0; index < Screen.AllScreens.Length; index++ )
                if ( Equals( screen, Screen.AllScreens[index] ) )
                    return allFriendlyNames.ToArray()[index];
            return null;
        }

        #region enums

        public enum QUERY_DEVICE_CONFIG_FLAGS : uint
        {
            QDC_ALL_PATHS         = 0x00000001,
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            QDC_DATABASE_CURRENT  = 0x00000004
        }

        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER                = 0xFFFFFFFF,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15                 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO               = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO      = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO      = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI                  = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI                 = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS                 = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN                = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI                  = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL         = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED         = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE           = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST             = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL             = 0x80000000,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32         = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED                = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE                = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED                 = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32               = 0xFFFFFFFF
        }

        private enum DISPLAYCONFIG_ROTATION : uint
        {
            DISPLAYCONFIG_ROTATION_IDENTITY     = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90     = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180    = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270    = 4,
            DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
        }

        private enum DISPLAYCONFIG_SCALING : uint
        {
            DISPLAYCONFIG_SCALING_IDENTITY               = 1,
            DISPLAYCONFIG_SCALING_CENTERED               = 2,
            DISPLAYCONFIG_SCALING_STRETCHED              = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM                 = 5,
            DISPLAYCONFIG_SCALING_PREFERRED              = 128,
            DISPLAYCONFIG_SCALING_FORCE_UINT32           = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_PIXELFORMAT : uint
        {
            DISPLAYCONFIG_PIXELFORMAT_8BPP         = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP        = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP        = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP        = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI       = 5,
            DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
        }

        public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE       = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET       = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
        }

        public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME           = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME           = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME          = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE    = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE      = 6,
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32              = 0xFFFFFFFF
        }

        #endregion

        #region structs

        [StructLayout( LayoutKind.Sequential )]
        public struct LUID
        {
            public uint LowPart;
            public int  HighPart;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public uint statusFlags;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public           LUID                                  adapterId;
            public           uint                                  id;
            public           uint                                  modeInfoIdx;
            private readonly DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
            private readonly DISPLAYCONFIG_ROTATION                rotation;
            private readonly DISPLAYCONFIG_SCALING                 scaling;
            private readonly DISPLAYCONFIG_RATIONAL                refreshRate;
            private readonly DISPLAYCONFIG_SCANLINE_ORDERING       scanLineOrdering;
            public           bool                                  targetAvailable;
            public           uint                                  statusFlags;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint                           flags;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong                           pixelRate;
            public DISPLAYCONFIG_RATIONAL          hSyncFreq;
            public DISPLAYCONFIG_RATIONAL          vSyncFreq;
            public DISPLAYCONFIG_2DREGION          activeSize;
            public DISPLAYCONFIG_2DREGION          totalSize;
            public uint                            videoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct POINTL
        {
            private readonly int x;
            private readonly int y;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint                      width;
            public uint                      height;
            public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
            public POINTL                    position;
        }

        [StructLayout( LayoutKind.Explicit )]
        public struct DISPLAYCONFIG_MODE_INFO_UNION
        {
            [FieldOffset( 0 )] public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset( 0 )] public DISPLAYCONFIG_SOURCE_MODE sourceMode;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_MODE_INFO
        {
            public DISPLAYCONFIG_MODE_INFO_TYPE  infoType;
            public uint                          id;
            public LUID                          adapterId;
            public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public uint                           size;
            public LUID                           adapterId;
            public uint                           id;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER       header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY  outputTechnology;
            public ushort                                 edidManufactureId;
            public ushort                                 edidProductCodeId;
            public uint                                   connectorInstance;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 64 )]
            public string monitorFriendlyDeviceName;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
            public string monitorDevicePath;
        }

        #endregion

        #region DLLImports

        [DllImport( "user32.dll" )]
        public static extern int GetDisplayConfigBufferSizes(
            QUERY_DEVICE_CONFIG_FLAGS flags, out uint numPathArrayElements, out uint numModeInfoArrayElements );

        [DllImport( "user32.dll" )]
        public static extern int QueryDisplayConfig(
            QUERY_DEVICE_CONFIG_FLAGS flags,
            ref uint                  numPathArrayElements,     [Out] DISPLAYCONFIG_PATH_INFO[] PathInfoArray,
            ref uint                  numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] ModeInfoArray,
            IntPtr                    currentTopologyId
        );

        [DllImport( "user32.dll" )]
        public static extern int DisplayConfigGetDeviceInfo( ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName );

        #endregion
    }

    public static class ProcessTools
    {
        public static string GetCommandLineArgs( this Process process )
        {
            if ( process is null ) throw new ArgumentNullException( nameof( process ) );

            try
            {
                return GetCommandLineArgsCore();
            }
            catch
            {
                return string.Empty;
            }

            string GetCommandLineArgsCore()
            {
                using var searcher = new ManagementObjectSearcher( $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id.ToString()}" );
                using var objects  = searcher.Get();
                var       obj      = objects.Cast<ManagementBaseObject>().SingleOrDefault();
                return obj?["CommandLine"]?.ToString() ?? "";
            }
        }
    }
}