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
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using VirtualSpace;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace AppController.WinTaskScheduler
{
    internal class TaskSchedulerHelper
    {
        public static bool CreateTask()
        {
            if ( !SysInfo.IsAdministrator )
            {
                MessageBox.Show( Agent.Langs.GetString( "General.RunOnStartup.Error.Permission" ) );
                return false;
            }

            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "autorun " + Const.AppName + " at system startup.";
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Settings.ExecutionTimeLimit = TimeSpan.FromSeconds( 0 );

            var lt = new LogonTrigger();
            lt.Delay = TimeSpan.FromSeconds( 5 );
            td.Triggers.Add( lt );

            var ea = new ExecAction( Manager.AppPath, "" );
            td.Actions.Add( ea );

            TaskService.Instance.RootFolder.RegisterTaskDefinition( Const.AppName, td );

            return true;
        }

        public static bool DeleteTaskByName( string taskName )
        {
            if ( !SysInfo.IsAdministrator )
            {
                MessageBox.Show( Agent.Langs.GetString( "General.RunOnStartup.Error.Permission" ) );
                return false;
            }

            using var ts = new TaskService();
            ts.RootFolder.DeleteTask( taskName );

            return true;
        }

        public static bool IsTaskExistsByName( string taskName )
        {
            using var ts = new TaskService();
            var       t  = ts.GetTask( taskName );
            return t != null;
        }
    }
}