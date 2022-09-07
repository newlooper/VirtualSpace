/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using VirtualSpace.Config.Entity;

namespace VirtualSpace.Config
{
    public class ConfigTemplate
    {
        public Dictionary<string, Profile> Profiles           { get; set; }
        public string                      CurrentProfileName { get; set; }
        public string                      Version            { get; set; }
        public LogConfig                   LogConfig          { get; set; }

        public Dictionary<string, KeyBinding>? KeyBindings { get; set; } = new()
        {
            {Const.Hotkey.RISE_VIEW, new KeyBinding {GhkCode = "_+Ctrl+_+Shift+Tab", MessageId = Const.Hotkey.Info[Const.Hotkey.RISE_VIEW].Item2}},
            {
                Const.Hotkey.SHOW_APP_CONTROLLER,
                new KeyBinding {GhkCode = "_+Ctrl+Alt+_+F12", MessageId = Const.Hotkey.Info[Const.Hotkey.SHOW_APP_CONTROLLER].Item2}
            },
            {Const.Hotkey.SVD1, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad1", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD1].Item2}},
            {Const.Hotkey.SVD2, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad2", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD2].Item2}},
            {Const.Hotkey.SVD3, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad3", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD3].Item2}},
            {Const.Hotkey.SVD4, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad4", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD4].Item2}},
            {Const.Hotkey.SVD5, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad5", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD5].Item2}},
            {Const.Hotkey.SVD6, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad6", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD6].Item2}},
            {Const.Hotkey.SVD7, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad7", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD7].Item2}},
            {Const.Hotkey.SVD8, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad8", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD8].Item2}},
            {Const.Hotkey.SVD9, new KeyBinding {GhkCode = "_+Ctrl+Alt+_+NumPad9", MessageId = Const.Hotkey.Info[Const.Hotkey.SVD9].Item2}},
            {Const.Hotkey.NAV_LEFT, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_LEFT].Item2}},
            {Const.Hotkey.NAV_RIGHT, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_RIGHT].Item2}},
            {Const.Hotkey.NAV_UP, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_UP].Item2}},
            {Const.Hotkey.NAV_DOWN, new KeyBinding {GhkCode = "", MessageId = Const.Hotkey.Info[Const.Hotkey.NAV_DOWN].Item2}}
        };

        public Dictionary<string, Const.MouseAction.Action>? MouseActions { get; set; } = new()
        {
            {Const.MouseAction.DESKTOP_LEFT_CLICK, Const.MouseAction.Action.DesktopVisibleAndCloseView},
            {Const.MouseAction.DESKTOP_MIDDLE_CLICK, Const.MouseAction.Action.DesktopVisibleOnly},
            {Const.MouseAction.DESKTOP_RIGHT_CLICK, Const.MouseAction.Action.ContextMenu},
            {Const.MouseAction.WINDOW_LEFT_CLICK, Const.MouseAction.Action.WindowActiveDesktopVisibleAndCloseView},
            {Const.MouseAction.WINDOW_MIDDLE_CLICK, Const.MouseAction.Action.WindowActiveDesktopVisibleOnly},
            {Const.MouseAction.WINDOW_RIGHT_CLICK, Const.MouseAction.Action.ContextMenu},
        };

        public Cluster Cluster { get; set; } = new Cluster {HideMainViewIfItsShown = false};
    }
}