/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows.Automation;
using VirtualSpace.AppLogs;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using Process = System.Diagnostics.Process;

namespace VirtualSpace.Config.Events
{
    public class WindowWatcher : IDisposable
    {
        private static readonly int SelfPid = Process.GetCurrentProcess().Id;

        public void Dispose()
        {
            Automation.RemoveAllEventHandlers();
        }

        public static void Start()
        {
            Logger.Info( "Begin Window Watch." );
            Automation.AddAutomationEventHandler(
                WindowPattern.WindowOpenedEvent,
                AutomationElement.RootElement,
                TreeScope.Children,
                OnWindowOpened );
        }

        private static void OnWindowOpened( object sender, AutomationEventArgs automationEventArgs )
        {
            var element = sender as AutomationElement;
            if ( element != null )
            {
                var handle   = new IntPtr( element.Current.NativeWindowHandle );
                var title    = element.Current.Name;
                var wndClass = element.Current.ClassName;
                try
                {
                    var pId = element.Current.ProcessId;
                    if ( pId != SelfPid )
                    {
                        var we = Window.Create(
                            handle,
                            title,
                            wndClass,
                            pId );
                        Conditions.CheckWindow( we );
                    }
                }
                catch ( Exception ex )
                {
                    Logger.Warning( ex.Message + $" ∵ {title}({handle})" );
                }
            }
        }
    }
}