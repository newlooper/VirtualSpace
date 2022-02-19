/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Media;
using Notification.Wpf;
using Notification.Wpf.Constants;
using VirtualSpace.Commons;
using VirtualSpace.Log;

namespace VirtualSpace.AppLogs
{
    public static class Logger
    {
        private static readonly Channel<LogMessage> LogChannel = Channels.LogChannel;

        public static void Debug( string str )
        {
            LogManager.RootLogger.Verbose( str );
        }

        public static void Event( string str )
        {
            LogManager.RootLogger.Debug( str );
        }

        public static void Info( string str )
        {
            LogManager.RootLogger.Information( str );
        }

        public static void Warning( string str )
        {
            LogManager.RootLogger.Warning( str );
        }

        public static void Error( string str, NotifyObject? notify = null )
        {
            LogManager.RootLogger.Error( str );
            if ( notify != null )
            {
                notify.Background = new SolidColorBrush( Colors.DarkRed );
                notify.Foreground = new SolidColorBrush( Colors.White );
                notify.Type = NotificationType.Error;
                Notify( notify );
            }
        }

        private static async void Log( string type, string str )
        {
            var logMessage = LogMessage.CreateMessage(
                type,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{type}] {str} {{ThreadId:{Thread.CurrentThread.ManagedThreadId}}}\r\n" );
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
                RowsCount = 5, //Will show 5 rows and trim after
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
            notificationManager.Show( content, "", TimeSpan.FromSeconds( 10 ) );
            NotificationConstants.MaxWidth = 350;
        }
    }

    public class NotifyObject
    {
        public string           Title      { get; set; } = "";
        public string           Message    { get; set; } = "";
        public NotificationType Type       { get; set; }
        public SolidColorBrush? Background { get; set; }
        public SolidColorBrush? Foreground { get; set; }
    }
}