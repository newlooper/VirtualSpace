/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace VirtualSpace.AppLogs;

public static class LogManager
{
    private static readonly List<ILoggerFactory> Factories = [];
    private static          LogLevel             _minLevel = LogLevel.Trace;

    public static   string  LogsPath { get; private set; } = "";
    internal static ILogger Verbose  { get; private set; } = null!;
    internal static ILogger Debug    { get; private set; } = null!;
    internal static ILogger Event    { get; private set; } = null!;
    internal static ILogger Plugin   { get; private set; } = null!;
    internal static ILogger Info     { get; private set; } = null!;
    internal static ILogger Warning  { get; private set; } = null!;
    internal static ILogger Error    { get; private set; } = null!;
    internal static ILogger Fatal    { get; private set; } = null!;

    public static void InitLogger( string folder )
    {
        LogsPath = folder;
        Directory.CreateDirectory( LogsPath );

        Verbose = CreateFileLogger( "verbose.txt" );
        Debug   = CreateFileLogger( "debug.txt" );
        Event   = CreateFileLogger( "event.txt", "EVT" );
        Plugin  = CreateFileLogger( "plugin.txt" );
        Info    = CreateFileLogger( "info.txt" );
        Warning = CreateFileLogger( "warning.txt" );
        Error   = CreateFileLogger( "error.txt" );
        Fatal   = CreateFileLogger( "fatal.txt" );
    }

    public static void CloseAndFlush()
    {
        foreach ( var factory in Factories )
        {
            factory.Dispose();
        }

        Factories.Clear();
    }

    public static void GorgeousDividingLine()
    {
        var line = new string( '=', 50 );
        Write( Verbose, LogLevel.Trace, line );
        Write( Debug, LogLevel.Debug, line );
        Write( Event, LogLevel.Debug, line );
        Write( Plugin, LogLevel.Debug, line );
        Write( Info, LogLevel.Information, line );
        Write( Warning, LogLevel.Warning, line );
        Write( Error, LogLevel.Error, line );
        Write( Fatal, LogLevel.Critical, line );
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

    internal static void Write( ILogger logger, LogLevel level, string message )
    {
        if ( level < _minLevel )
        {
            return;
        }

        logger.Log( level, "{Message}", message );
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