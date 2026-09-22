/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace VirtualSpace.AppLogs;

internal enum LogFile
{
    Verbose,
    Debug,
    Event,
    Plugin,
    Info,
    Warning,
    Error,
    Fatal
}

public static class LogManager
{
    private static readonly Lock                         Sync      = new();
    private static readonly List<ILoggerFactory>         Factories = [];
    private static readonly Dictionary<LogFile, ILogger> Loggers   = [];
    private static          LogLevel                     _minLevel = LogLevel.Trace;
    private static          bool                         _open;

    public static string LogsPath { get; private set; } = "";

    public static void InitLogger( string folder )
    {
        LogsPath = folder;
        Directory.CreateDirectory( LogsPath );
        ArchiveAndReset();
    }

    public static void ArchiveAndReset()
    {
        CloseAndFlush();
        LogArchiver.RotateAndArchiveInBackground( LogsPath );

        lock ( Sync )
        {
            _open = true;
        }
    }

    public static void CloseAndFlush()
    {
        lock ( Sync )
        {
            _open = false;

            foreach ( var factory in Factories )
            {
                factory.Dispose();
            }

            Factories.Clear();
            Loggers.Clear();
        }
    }

    public static void SetLogLevel( string level )
    {
        _minLevel = level switch
        {
            "VERBOSE" => LogLevel.Trace,
            "DEBUG" or "EVENT" => LogLevel.Debug,
            "INFO" => LogLevel.Information,
            "WARNING" => LogLevel.Warning,
            "ERROR" => LogLevel.Error,
            "FATAL" => LogLevel.Critical,
            _ => LogLevel.Information
        };
    }

    internal static void Write( LogFile file, LogLevel level, string message )
    {
        if ( level < _minLevel )
        {
            return;
        }

        ILogger logger;
        lock ( Sync )
        {
            if ( !_open )
            {
                return;
            }

            logger = GetOrCreate( file );
        }

        try
        {
            if ( !logger.IsEnabled( level ) )
            {
                return;
            }

            logger.Log( level, "{Message}", message );
        }
        catch ( ObjectDisposedException )
        {
        }
    }

    private static ILogger GetOrCreate( LogFile file )
    {
        if ( Loggers.TryGetValue( file, out var logger ) )
        {
            return logger;
        }

        logger = file switch
        {
            LogFile.Verbose => CreateFileLogger( "verbose.txt" ),
            LogFile.Debug => CreateFileLogger( "debug.txt" ),
            LogFile.Event => CreateFileLogger( "event.txt", "EVT" ),
            LogFile.Plugin => CreateFileLogger( "plugin.txt" ),
            LogFile.Info => CreateFileLogger( "info.txt" ),
            LogFile.Warning => CreateFileLogger( "warning.txt" ),
            LogFile.Error => CreateFileLogger( "error.txt" ),
            LogFile.Fatal => CreateFileLogger( "fatal.txt" ),
            _ => throw new ArgumentOutOfRangeException( nameof( file ), file, null )
        };

        Loggers[file] = logger;
        return logger;
    }

    private static ILogger CreateFileLogger( string fileName, string? levelTag = null )
    {
        var factory = LoggerFactory.Create( builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel( LogLevel.Trace );
            builder.AddZLoggerFile( Path.Combine( LogsPath, fileName ), options =>
            {
                options.UsePlainTextFormatter( formatter =>
                {
                    if ( levelTag is null )
                    {
                        formatter.SetPrefixFormatter( $"{0:longdate} [{1:short}] ",
                            ( in template, in info ) =>
                                template.Format( info.Timestamp, info.LogLevel ) );
                    }
                    else
                    {
                        formatter.SetPrefixFormatter( $"{0:longdate} [{1}] ",
                            ( in template, in info ) =>
                                template.Format( info.Timestamp, levelTag ) );
                    }
                } );
            } );
        } );

        Factories.Add( factory );
        return factory.CreateLogger( "VirtualSpace" );
    }
}