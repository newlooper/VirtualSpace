﻿/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Threading;
using System.Threading.Channels;
using ControlPanel.Pages;
using VirtualSpace.AppLogs;

namespace ControlPanel;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly Channel<LogMessage>     LogChannel               = Logger.LogChannel;
    private readonly        CancellationTokenSource _cancelTokenSourceForLog = new();

    private async void PickLogAndWrite( CancellationToken stoppingToken )
    {
        try
        {
            while ( await LogChannel.Reader.WaitToReadAsync( stoppingToken ) )
            {
                if ( LogChannel.Reader.TryRead( out var message ) )
                {
                    Logs.Append( message.Message, message.Type );
                }
            }
        }
        catch
        {
            // ignored
        }
        finally
        {
            _cancelTokenSourceForLog.Dispose();
        }
    }
}