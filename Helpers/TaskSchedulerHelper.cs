// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;

namespace VirtualSpace.Helpers
{
    public static class TaskSchedulerHelper
    {
        public static void CreateAutoRunTask( string taskName, string fullAppPath, string taskFolder = "" )
        {
            if ( !SysInfo.IsAdministrator )
            {
                throw new Exception( "General.RunOnStartup.Error.Permission" );
            }

            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "autorun " + taskName + " at system startup.";
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds( 0 );

            var lt = new LogonTrigger();
            lt.Delay = TimeSpan.FromSeconds( 5 );
            td.Triggers.Add( lt );

            var ea = new ExecAction( fullAppPath, "" );
            td.Actions.Add( ea );

            TaskService.Instance.RootFolder.RegisterTaskDefinition( GetTaskPath( taskName, taskFolder ), td );
        }

        public static void DeleteTaskByName( string taskName, string taskFolder = "" )
        {
            if ( !SysInfo.IsAdministrator )
            {
                throw new Exception( "General.RunOnStartup.Error.Permission" );
            }

            using var ts = new TaskService();
            ts.RootFolder.DeleteTask( GetTaskPath( taskName, taskFolder ) );
        }

        public static bool IsTaskExistsByName( string taskName, string taskFolder = "" )
        {
            using var ts       = new TaskService();
            var       t        = ts.GetTask( GetTaskPath( taskName, taskFolder ) );
            return t != null;
        }

        private static string GetTaskPath( string taskName, string taskFolder )
        {
            return string.IsNullOrEmpty( taskFolder ) ? taskName : taskFolder + @"\" + taskName;
        }

        public static void OpenWinTaskScheduler()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "taskschd.msc",
                UseShellExecute = true
            };
            Process.Start( psi );
        }
    }
}