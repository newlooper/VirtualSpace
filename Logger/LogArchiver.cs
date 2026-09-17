/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace VirtualSpace.AppLogs;

internal static partial class LogArchiver
{
    private const string ZIP_FILE_NAME      = "logs.zip";
    private const string ZIP_TEMP_FILE_NAME = "logs.zip.tmp";
    private const string TIMESTAMP_FORMAT   = "yyyy-MM-dd_HH-mm-ss";

    private static readonly string[] LogFileNames =
    [
        "verbose.txt",
        "debug.txt",
        "event.txt",
        "plugin.txt",
        "info.txt",
        "warning.txt",
        "error.txt",
        "fatal.txt"
    ];

    private static readonly Regex ArchivedLogName = MyRegex();

    internal static void RotateAndArchiveInBackground( string logsPath )
    {
        RotateCurrentLogs( logsPath );

        var pending = FindPendingArchiveFiles( logsPath );
        if ( pending.Count == 0 )
        {
            return;
        }

        new Thread( () => CompressPendingFiles( logsPath, pending ) )
        {
            IsBackground = true,
            Name         = "LogArchiver"
        }.Start();
    }

    private static void RotateCurrentLogs( string logsPath )
    {
        foreach ( var fileName in LogFileNames )
        {
            var path = Path.Combine( logsPath, fileName );
            if ( !File.Exists( path ) )
            {
                continue;
            }

            try
            {
                var info = new FileInfo( path );
                if ( info.Length == 0 )
                {
                    info.Delete();
                    continue;
                }

                File.Move( path,
                    Path.Combine( logsPath, $"{Path.GetFileNameWithoutExtension( fileName )}_{info.LastWriteTime.ToString( TIMESTAMP_FORMAT )}.txt" ) );
            }
            catch
            {
                // Keep the original file so ZLogger can continue writing.
            }
        }
    }

    private static List<string> FindPendingArchiveFiles( string logsPath )
    {
        return [.. Directory.EnumerateFiles( logsPath, "*.txt" ).Where( path => ArchivedLogName.IsMatch( Path.GetFileName( path ) ) )];
    }

    private static void CompressPendingFiles( string logsPath, List<string> pending )
    {
        var zipPath  = Path.Combine( logsPath, ZIP_FILE_NAME );
        var tempPath = Path.Combine( logsPath, ZIP_TEMP_FILE_NAME );

        try
        {
            if ( File.Exists( tempPath ) )
            {
                File.Delete( tempPath );
            }

            if ( File.Exists( zipPath ) )
            {
                File.Copy( zipPath, tempPath );
            }

            using ( var stream = new FileStream( tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None ) )
            {
                var       mode = stream.Length == 0 ? ZipArchiveMode.Create : ZipArchiveMode.Update;
                using var zip  = new ZipArchive( stream, mode );
                foreach ( var path in pending )
                {
                    if ( !File.Exists( path ) )
                    {
                        continue;
                    }

                    var fileName = Path.GetFileName( path );
                    var match    = ArchivedLogName.Match( fileName );
                    if ( !match.Success )
                    {
                        continue;
                    }

                    var entryName = $"{match.Groups[2].Value}/{fileName}";
                    if ( mode == ZipArchiveMode.Update && zip.GetEntry( entryName ) != null )
                    {
                        continue;
                    }

                    zip.CreateEntryFromFile( path, entryName, CompressionLevel.Optimal );
                }
            }

            File.Move( tempPath, zipPath, overwrite: true );

            foreach ( var path in pending )
            {
                try
                {
                    File.Delete( path );
                }
                catch
                {
                    // Retry on next startup.
                }
            }
        }
        catch
        {
            try
            {
                if ( File.Exists( tempPath ) )
                {
                    File.Delete( tempPath );
                }
            }
            catch
            {
                // Ignore cleanup failures; pending txt files remain for the next start.
            }
        }
    }

    [GeneratedRegex( @"^(verbose|debug|event|plugin|info|warning|error|fatal)_(\d{4}-\d{2}-\d{2})_(\d{2}-\d{2}-\d{2})\.txt$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled, "zh-CN" )]
    private static partial Regex MyRegex();
}