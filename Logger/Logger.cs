/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows.Media;
using Notification.Core;
using Notification.Wpf;
using Notification.Wpf.Constants;
using VirtualSpace.Helpers;

namespace VirtualSpace.AppLogs;

public static class Logger
{
    public static readonly Channel<LogMessage> LogChannel = Channel.CreateUnbounded<LogMessage>();
    public static          bool                ShowLogsInGui { get; set; }

    public static void Verbose( string str )
    {
        LogToGui( "VERBOSE", str );
        LogManager.RootLogger.Verbose( str );
    }

    public static void Debug( string str )
    {
        LogToGui( "DEBUG", str );
        LogManager.RootLogger.Debug( str );
    }

    public static void Event( string str )
    {
        LogToGui( "EVENT", str );
        LogManager.RootLogger
            .ForContext( LogManager.PROP_IS_EVENT, true )
            .Information( "{Message}", str );
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
            notify.Type       = NotificationType.Error;
            Notify( notify );
        }
    }

    private static async void LogToGui( string type, string str )
    {
        if ( !ShowLogsInGui )
        {
            return;
        }

        var logMessage = LogMessage.CreateMessage(
            type,
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{type}] {str} {{ThreadId:{Environment.CurrentManagedThreadId.ToString()}}}\r\n" );
        await LogChannel.Writer.WriteAsync( logMessage ).ConfigureAwait( false );
    }

    public static void Notify( NotifyObject no )
    {
        var notificationManager = new NotificationManager();
        var content = new NotificationContent
        {
            Title              = no.Title,
            Message            = no.Message,
            Type               = no.Type,
            TrimType           = NotificationTextTrimType.NoTrim, // will show attach button on message
            RowsCount          = 5, // Will show 5 rows and trim after
            LeftButtonContent  = "", // Left button content (string or what u want
            RightButtonContent = "", // Right button content (string or what u want
            CloseOnClick       = true // Set true if u want close message when left mouse button click on message (base = true)
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
        var titleBuf = new char[128];
        var titleLen = User32.GetWindowText( hWnd, titleBuf, titleBuf.Length );
        var title    = titleLen <= 0 ? string.Empty : new string( titleBuf, 0, titleLen );

        var classBuf  = new char[512];
        var classLen  = User32.GetClassName( hWnd, classBuf, classBuf.Length );
        var classname = classLen <= 0 ? string.Empty : new string( classBuf, 0, classLen );

        if ( title == "ToastWindow" && classname.StartsWith( "HwndWrapper[VirtualSpace" ) )
        {
            var exStyle = User32.GetWindowLong( hWnd, (int)GetWindowLongFields.GWL_EXSTYLE );
            exStyle |= 0x80; // WS_EX_TOOLWINDOW
            User32.SetWindowLongPtr( new HandleRef( null, hWnd ), (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
            return false;
        }

        return true;
    }
}

public class NotifyObject
{
    public string           Title      { get; init; } = string.Empty;
    public string           Message    { get; init; } = string.Empty;
    public NotificationType Type       { get; set; }
    public SolidColorBrush? Background { get; set; }
    public SolidColorBrush? Foreground { get; set; }
    public TimeSpan         ExpTime    { get; init; } = TimeSpan.FromSeconds( 10 );
}