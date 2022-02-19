/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Management;
using System.Threading.Tasks;
using VirtualSpace.AppLogs;

namespace VirtualSpace.Config.Events
{
    public class ProcessWatcher : IDisposable
    {
        private static readonly ManagementEventWatcher StartWatch = new(
            "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'" );

        private static readonly ManagementEventWatcher StopWatch = new(
            "SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'" );

        public void Dispose()
        {
            Stop();
        }

        public static void Start()
        {
            Logger.Info( "Begin Process Start/Stop Watch." );

            new Task( () =>
            {
                StartWatch.EventArrived += StartWatch_EventArrived;
                StartWatch.Start();

                StopWatch.EventArrived += StopWatch_EventArrived;
                StopWatch.Start();
            } ).Start();
        }

        private static void StopWatch_EventArrived( object sender, EventArrivedEventArgs e )
        {
            // e.NewEvent now have only 3 properties, we should focus on TargetInstance property
            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            // TargetInstance has more than 40 properties, some properties can be null
            var name = targetInstance["Name"]?.ToString();
            var pId  = targetInstance["ProcessId"]?.ToString();
            Logger.Debug( $"Process stopped: {name}({pId})" );
        }

        private static void StartWatch_EventArrived( object sender, EventArrivedEventArgs e )
        {
            // e.NewEvent now have only 3 properties, we should focus on TargetInstance property
            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            // TargetInstance has more than 40 properties, some properties can be null
            var name = targetInstance["Name"]?.ToString();
            var pId  = targetInstance["ProcessId"]?.ToString();
            Logger.Debug( $"Process started: {name}({pId})" );
        }

        private static void Stop()
        {
            StartWatch.Stop();
            StopWatch.Stop();
        }
    }
}