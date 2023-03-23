/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using VirtualSpace.Config.DataAnnotations;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config
{
    public class ConfigTemplate
    {
        [PropertyProtector] public Dictionary<string, Profile> Profiles           { get; set; }
        public                     string                      CurrentProfileName { get; set; }
        public                     string                      Version            { get; set; }
        public                     LogConfig                   LogConfig          { get; set; }

        public Dictionary<string, KeyBinding>? KeyBindings { get; set; } = new()
        {
            {Const.Hotkey.RISE_VIEW, new KeyBinding {GhkCode = "_+Ctrl+_+Shift+Tab", MessageId = Const.Hotkey.Info[Const.Hotkey.RISE_VIEW].MessageId}},
            {Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP].MessageId}},
            {Const.Hotkey.RISE_VIEW_FOR_CURRENT_VD, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.RISE_VIEW_FOR_CURRENT_VD].MessageId}},
            {
                Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP_IN_CURRENT_VD,
                new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.RISE_VIEW_FOR_ACTIVE_APP_IN_CURRENT_VD].MessageId}
            },
            {Const.Hotkey.SHOW_APP_CONTROLLER, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+F12", MessageId = Const.Hotkey.Info[Const.Hotkey.SHOW_APP_CONTROLLER].MessageId}},
            {Const.Hotkey.NAV_LEFT, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_LEFT].MessageId}},
            {Const.Hotkey.NAV_RIGHT, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_RIGHT].MessageId}},
            {Const.Hotkey.NAV_UP, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_UP].MessageId}},
            {Const.Hotkey.NAV_DOWN, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_DOWN].MessageId}},
            {Const.Hotkey.SWITCH_BACK_LAST, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.SWITCH_BACK_LAST].MessageId}}
        };

        public Dictionary<string, MouseAction.Action>? MouseAction  { get; set; } = null;
        public Dictionary<string, MouseAction.Action>? MouseActions { get; set; } = Config.MouseAction.Info;

        [PropertyProtector]
        public Cluster Cluster { get; set; } = new()
        {
            HideMainViewIfItsShown = false,
            NotificationOnVdChanged = false,
            ShowVDIndexOnTrayIcon = false,
            HideOnStart = false
        };

        public MouseAction.Action GetMouseActionById( string id )
        {
            if ( MouseActions == null || MouseActions.Count == 0 )
            {
                MouseActions = Config.MouseAction.Info;
            }

            return MouseActions.ContainsKey( id )
                ? MouseActions[id]
                : Config.MouseAction.Action.DoNothing;
        }
    }
}