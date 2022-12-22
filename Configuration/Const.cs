/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using VirtualSpace.Helpers;

namespace VirtualSpace.Config
{
    public static class Const
    {
        public const string RuleFileExt            = ".rules";
        public const string ProfilesFolder         = "Profiles";
        public const string CacheFolder            = "Cache";
        public const string PluginsFolder          = "Plugins";
        public const string SettingsFile           = "settings.json";
        public const string DefaultVersion         = "1.0";
        public const string DefaultLogLevel        = "EVENT";
        public const int    WindowTitleMaxLength   = 2048;
        public const int    WindowClassMaxLength   = 512;
        public const int    WindowCheckTimeout     = 10 * 1000;
        public const int    WindowCheckInterval    = 200;
        public const int    WindowCloseTimeout     = 60 * 1000;
        public const int    RiseViewInterval       = 500;
        public const int    SwitchDesktopInterval  = 100;
        public const int    DefaultDpi             = 96;
        public const int    FakeHideX              = -10000;
        public const int    FakeHideY              = -10000;
        public const string ApplicationFrameWindow = "ApplicationFrameWindow";
        public const string WindowsUiCoreWindow    = "Windows.UI.Core.CoreWindow";
        public const string TaskbarCreated         = "TaskbarCreated";
        public const int    NavHTypeNextRow        = 0;
        public const int    NavHTypeSameRow        = 1;
        public const string WindowsCRLF            = "\r\n";
        public const string AppName                = "VirtualSpace";
        public const string HideWindowSplitter     = "🔙🔜";

        public static class Args
        {
            public const string HIDE_ON_START = "--HideOnStart";
        }

        public static class Hotkey
        {
            public const string SPLITTER = "+";

            public const string NONE  = "_";
            public const string WIN   = "Win";
            public const string CTRL  = "Ctrl";
            public const string ALT   = "Alt";
            public const string SHIFT = "Shift";

            ///////////////////////////////////////////////////
            // 值与控件名称一一对应，若控件名被修改，则此处也须对应改变
            public const string RISE_VIEW           = "hk_node_rise_mainview";
            public const string SHOW_APP_CONTROLLER = "hk_node_open_app_controller";
            public const string SVD1                = "hk_node_svd_1";
            public const string SVD2                = "hk_node_svd_2";
            public const string SVD3                = "hk_node_svd_3";
            public const string SVD4                = "hk_node_svd_4";
            public const string SVD5                = "hk_node_svd_5";
            public const string SVD6                = "hk_node_svd_6";
            public const string SVD7                = "hk_node_svd_7";
            public const string SVD8                = "hk_node_svd_8";
            public const string SVD9                = "hk_node_svd_9";
            public const string NAV_LEFT            = "hk_node_nav_left";
            public const string NAV_RIGHT           = "hk_node_nav_right";
            public const string NAV_UP              = "hk_node_nav_up";
            public const string NAV_DOWN            = "hk_node_nav_down";

            ////////////////////////////////////////////////////////////////
            // 可由热键调用的程序功能表
            // tuple.Item1 => friendly name
            // tuple.Item2 => UserMessageId
            // tuple.Item3 => alternate hotkey, 由程序保留，只能在源码中修改
            public static Dictionary<string, Tuple<string, int, string>> Info = new()
            {
                {RISE_VIEW, new Tuple<string, int, string>( "Rise MainView", UserMessage.RiseView, "LWin+Tab" )},
                {SHOW_APP_CONTROLLER, new Tuple<string, int, string>( "Open AppController", UserMessage.ShowAppController, "" )},
                {SVD1, new Tuple<string, int, string>( "Switch To Desktop 1", UserMessage.SVD1, "" )},
                {SVD2, new Tuple<string, int, string>( "Switch To Desktop 2", UserMessage.SVD2, "" )},
                {SVD3, new Tuple<string, int, string>( "Switch To Desktop 3", UserMessage.SVD3, "" )},
                {SVD4, new Tuple<string, int, string>( "Switch To Desktop 4", UserMessage.SVD4, "" )},
                {SVD5, new Tuple<string, int, string>( "Switch To Desktop 5", UserMessage.SVD5, "" )},
                {SVD6, new Tuple<string, int, string>( "Switch To Desktop 6", UserMessage.SVD6, "" )},
                {SVD7, new Tuple<string, int, string>( "Switch To Desktop 7", UserMessage.SVD7, "" )},
                {SVD8, new Tuple<string, int, string>( "Switch To Desktop 8", UserMessage.SVD8, "" )},
                {SVD9, new Tuple<string, int, string>( "Switch To Desktop 9", UserMessage.SVD9, "" )},
                {NAV_LEFT, new Tuple<string, int, string>( "Left", UserMessage.NavLeft, "LWin+Ctrl+Left" )},
                {NAV_RIGHT, new Tuple<string, int, string>( "Right", UserMessage.NavRight, "LWin+Ctrl+Right" )},
                {NAV_UP, new Tuple<string, int, string>( "Up", UserMessage.NavUp, "LWin+Ctrl+Up" )},
                {NAV_DOWN, new Tuple<string, int, string>( "Down", UserMessage.NavDown, "LWin+Ctrl+Down" )}
            };
        }

        public static class MouseAction
        {
            ///////////////////////////////////////////////////
            // 值与控件名称一一对应，若控件名被修改，则此处也须对应改变
            public const string DESKTOP_LEFT_CLICK   = "mouse_node_d_l";
            public const string DESKTOP_MIDDLE_CLICK = "mouse_node_d_m";
            public const string DESKTOP_RIGHT_CLICK  = "mouse_node_d_r";
            public const string WINDOW_LEFT_CLICK    = "mouse_node_w_l";
            public const string WINDOW_MIDDLE_CLICK  = "mouse_node_w_m";
            public const string WINDOW_RIGHT_CLICK   = "mouse_node_w_r";

            ////////////////////////////////////////////////////////////////
            // 鼠标动作表，信息包含友好名称和默认行为
            public static Dictionary<string, Tuple<string, Action>> Info = new()
            {
                {DESKTOP_LEFT_CLICK, new Tuple<string, Action>( "Mouse LeftClick on VirtualDesktop", Action.DesktopVisibleAndCloseView )},
                {DESKTOP_MIDDLE_CLICK, new Tuple<string, Action>( "Mouse MiddleClick on VirtualDesktop", Action.DesktopVisibleOnly )},
                {DESKTOP_RIGHT_CLICK, new Tuple<string, Action>( "Mouse RightClick on VirtualDesktop", Action.ContextMenu )},
                {WINDOW_LEFT_CLICK, new Tuple<string, Action>( "Mouse LeftClick on Window Thumbnail", Action.WindowActiveDesktopVisibleAndCloseView )},
                {WINDOW_MIDDLE_CLICK, new Tuple<string, Action>( "Mouse MiddleClick on Window Thumbnail", Action.WindowActiveDesktopVisibleOnly )},
                {WINDOW_RIGHT_CLICK, new Tuple<string, Action>( "Mouse RightClick on Window Thumbnail", Action.ContextMenu )},
            };

            public enum Action
            {
                DoNothing,
                ContextMenu,
                DesktopVisibleAndCloseView,
                DesktopVisibleOnly,
                WindowActiveDesktopVisibleAndCloseView,
                WindowActiveDesktopVisibleOnly,
                WindowClose
            }
        }
    }
}