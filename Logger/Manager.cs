/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace VirtualSpace.AppLogs
{
    public static class LogManager
    {
        private static readonly LoggingLevelSwitch LevelSwitch = new( LogEventLevel.Verbose );

        public const LogEventLevel LOG_LEVEL_EVENT = (LogEventLevel)0xFF;

        public static readonly Serilog.Core.Logger RootLogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy( LevelSwitch )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Verbose ).WriteTo.File( "Logs/verbose.txt", LogEventLevel.Verbose ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Debug ).WriteTo.File( "Logs/debug.txt", LogEventLevel.Debug ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Information ).WriteTo.File( "Logs/info.txt", LogEventLevel.Information ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Warning ).WriteTo.File( "Logs/warning.txt", LogEventLevel.Warning ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Error ).WriteTo.File( "Logs/error.txt", LogEventLevel.Error ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Fatal ).WriteTo.File( "Logs/fatal.txt", LogEventLevel.Fatal ) )
            .WriteTo.Logger( c =>
                c.Filter.ByIncludingOnly( evt => evt.Level == LOG_LEVEL_EVENT ).WriteTo.File( "Logs/event.txt", LOG_LEVEL_EVENT ) )
            .CreateLogger();

        public static void GorgeousDividingLine()
        {
            const string line = "==================================================";
            RootLogger.Verbose( line );
            RootLogger.Debug( line );
            RootLogger.Information( line );
            RootLogger.Warning( line );
            RootLogger.Error( line );
            RootLogger.Fatal( line );
            RootLogger.Write( LOG_LEVEL_EVENT, line );
        }

        public static void SetLogLevel( string level )
        {
            LevelSwitch.MinimumLevel = level switch
            {
                "DEBUG" => LogEventLevel.Verbose,
                "EVENT" => LogEventLevel.Debug,
                "INFO" => LogEventLevel.Information,
                "WARNING" => LogEventLevel.Warning,
                "ERROR" => LogEventLevel.Error,
                "FATAL" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }
    }
}