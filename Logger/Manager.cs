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
        private static readonly LoggingLevelSwitch  LevelSwitch   = new( LogEventLevel.Verbose );
        public const            string              PROP_IS_EVENT = "IsEvent";
        public static           Serilog.Core.Logger RootLogger    = null!;
        public static           string              LogsPath { get; private set; } = "";

        public static void InitLogger( string folder )
        {
            LogsPath = folder;

            RootLogger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy( LevelSwitch )
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Verbose )
                    .WriteTo.File( $"{LogsPath}/verbose.txt", LogEventLevel.Verbose ) )
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Debug )
                    .WriteTo.File( $"{LogsPath}/debug.txt", LogEventLevel.Debug ) )

                // 普通 Information：排除包含 PROP_IS_EVENT 属性的日志
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt =>
                        evt.Level == LogEventLevel.Information &&
                        !( evt.Properties.TryGetValue( PROP_IS_EVENT, out var v ) && v is ScalarValue { Value: true } ) )
                    .WriteTo.File( $"{LogsPath}/info.txt", LogEventLevel.Information ) )

                // Event 日志：借用 Information，并用 PROP_IS_EVENT 属性筛选
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt =>
                        evt.Properties.TryGetValue( PROP_IS_EVENT, out var v ) && v is ScalarValue { Value: true } )
                    .WriteTo.File(
                        $"{LogsPath}/event.txt", LogEventLevel.Information,
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [EVT] {Message:lj}{NewLine}{Exception}" ) )
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Warning )
                    .WriteTo.File( $"{LogsPath}/warning.txt", LogEventLevel.Warning ) )
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Error )
                    .WriteTo.File( $"{LogsPath}/error.txt", LogEventLevel.Error ) )
                .WriteTo.Logger( c => c.Filter.ByIncludingOnly( evt => evt.Level == LogEventLevel.Fatal )
                    .WriteTo.File( $"{LogsPath}/fatal.txt", LogEventLevel.Fatal ) )
                .CreateLogger();
        }

        public static void GorgeousDividingLine()
        {
            string line = new( '=', 50 );
            RootLogger.Verbose( line );
            RootLogger.Debug( line );
            RootLogger.Information( line );
            RootLogger.ForContext( PROP_IS_EVENT, true ).Information( "{Message}", line ); // same Level as Information
            RootLogger.Warning( line );
            RootLogger.Error( line );
            RootLogger.Fatal( line );
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