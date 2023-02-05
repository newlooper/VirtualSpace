/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Media;
using Notification.Wpf;
using Notification.Wpf.Constants;

namespace VirtualSpace.AppLogs
{
    public static class Logger
    {
        public static readonly Channel<LogMessage> LogChannel = Channel.CreateUnbounded<LogMessage>();
        public static          bool                ShowLogsInGui { get; set; } = false;

        public static void Debug( string str )
        {
            LogToGui( "DEBUG", str );
            LogManager.RootLogger.Verbose( str );
        }

        public static void Event( string str )
        {
            LogToGui( "EVENT", str );
            LogManager.RootLogger.Debug( str );
        }

        public static void Info( string str )
        {
            LogToGui( "INFO", str );
            LogManager.RootLogger.Information( str );
        }

        public static void Warning( string str )
        {
            LogToGui( "WARNING", str );
            LogManager.RootLogger.Warning( str );
        }

        public static void Error( string str, NotifyObject? notify = null )
        {
            LogToGui( "ERROR", str );
            LogManager.RootLogger.Error( str );
            if ( notify != null )
            {
                notify.Background = new SolidColorBrush( Colors.DarkRed );
                notify.Foreground = new SolidColorBrush( Colors.White );
                notify.Type = NotificationType.Error;
                Notify( notify );
            }
        }

        private static async void LogToGui( string type, string str )
        {
            if ( !ShowLogsInGui ) return;
            var logMessage = LogMessage.CreateMessage(
                type,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{type}] {str} {{ThreadId:{Thread.CurrentThread.ManagedThreadId.ToString()}}}\r\n" );
            await LogChannel.Writer.WriteAsync( logMessage );
        }

        public static void Notify( NotifyObject no )
        {
            var notificationManager = new NotificationManager();
            var content = new NotificationContent
            {
                Title = no.Title,
                Message = no.Message,
                Type = no.Type,
                TrimType = NotificationTextTrimType.NoTrim, // will show attach button on message
                RowsCount = 5, // Will show 5 rows and trim after
                LeftButtonContent = "", // Left button content (string or what u want
                RightButtonContent = "", // Right button content (string or what u want
                CloseOnClick = true // Set true if u want close message when left mouse button click on message (base = true)
            };

            if ( no.Background != null )
            {
                content.Background = no.Background;
            }

            if ( no.Foreground != null )
            {
                content.Foreground = no.Foreground;
            }

            NotificationConstants.MaxWidth = 1024;
            notificationManager.Show( content, "", no.ExpTime );
            NotificationConstants.MaxWidth = 350;

            _ = User32.EnumWindows( ToastWindowFilter, 0 );
        }

        private static bool ToastWindowFilter( IntPtr hWnd, int lParam )
        {
            var sbTitle = new StringBuilder( 128 );
            User32.GetWindowText( hWnd, sbTitle, sbTitle.Capacity );
            var title = sbTitle.ToString();

            var sbCName = new StringBuilder( 512 );
            _ = User32.GetClassName( hWnd, sbCName, sbCName.Capacity );
            var classname = sbCName.ToString();

            if ( title == "ToastWindow" && classname.StartsWith( "HwndWrapper[VirtualSpace" ) )
            {
                var exStyle = User32.GetWindowLong( hWnd, (int)GetWindowLongFields.GWL_EXSTYLE );
                exStyle |= User32.WS_EX_TOOLWINDOW;
                User32.SetWindowLongPtr( new HandleRef( null, hWnd ), (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
                return false;
            }

            return true;
        }

        private enum GetWindowLongFields
        {
            GWL_USERDATA   = -21, // 0xFFFFFFEB
            GWL_EXSTYLE    = -20, // 0xFFFFFFEC
            GWL_STYLE      = -16, // 0xFFFFFFF0
            GWL_ID         = -12, // 0xFFFFFFF4
            GWL_HWNDPARENT = -8, // 0xFFFFFFF8
            GWL_HINSTANCE  = -6, // 0xFFFFFFFA
            GWL_WNDPROC    = -4 // 0xFFFFFFFC
        }

        private static class User32
        {
            public delegate bool EnumWindowsProc( IntPtr hWnd, int lParam );

            public const int WS_EX_TOOLWINDOW = 0x80;

            [DllImport( "user32.dll", CharSet = CharSet.Auto )]
            public static extern int GetWindowLong( IntPtr hWnd, int nIndex );

            public static IntPtr SetWindowLongPtr( HandleRef hWnd, int nIndex, int dwNewLong )
            {
                if ( IntPtr.Size == 8 )
                    return SetWindowLongPtr64( hWnd, nIndex, (IntPtr)dwNewLong );
                else
                    return new IntPtr( SetWindowLong32( hWnd, nIndex, dwNewLong ) );
            }

            [DllImport( "user32.dll", EntryPoint = "SetWindowLong" )]
            private static extern int SetWindowLong32( HandleRef hWnd, int nIndex, int dwNewLong );

            [DllImport( "user32.dll", EntryPoint = "SetWindowLongPtr" )]
            private static extern IntPtr SetWindowLongPtr64( HandleRef hWnd, int nIndex, IntPtr dwNewLong );

            [DllImport( "user32.dll" )]
            public static extern int GetWindowText( IntPtr hWnd, StringBuilder buf, int nMaxCount );

            [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
            public static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

            [DllImport( "user32.dll" )]
            public static extern int EnumWindows( EnumWindowsProc func, int lParam );
        }
    }

    public class NotifyObject
    {
        public string           Title      { get; set; } = "";
        public string           Message    { get; set; } = "";
        public NotificationType Type       { get; set; }
        public SolidColorBrush? Background { get; set; }
        public SolidColorBrush? Foreground { get; set; }
        public TimeSpan         ExpTime    { get; set; } = TimeSpan.FromSeconds( 10 );
    }
}