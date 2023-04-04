/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Windows.Forms;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;

namespace VirtualSpace.Config
{
    public static class Const
    {
        public const int    WindowTitleMaxLength   = 2048;
        public const int    WindowClassMaxLength   = 512;
        public const int    WindowCheckTimesLimit  = 10;
        public const int    OneSecond              = 1000;
        public const int    OneMinute              = 60 * OneSecond;
        public const int    WindowCloseTimeout     = 60 * OneSecond;
        public const int    RiseViewInterval       = 500;
        public const int    SwitchDesktopInterval  = 100;
        public const int    FakeHideX              = -10000;
        public const int    FakeHideY              = -10000;
        public const string ApplicationFrameWindow = "ApplicationFrameWindow";
        public const string WindowsUiCoreWindow    = "Windows.UI.Core.CoreWindow";
        public const string TaskbarCreated         = "TaskbarCreated";
        public const string TaskbarWndClass        = "Shell_TrayWnd";
        public const string WindowsCRLF            = "\r\n";
        public const string AppName                = "VirtualSpace";
        public const string HideWindowSplitter     = "🔙🔜";

        public static class Window
        {
            public const string VD_FRAME_TITLE      = "VirtualSpace__VirtualDesktopFrame!";
            public const string VD_CONTAINER_TITLE  = "VirtualSpace__VirtualDesktopWindow!";
            public const string VD_DRAG_TITLE       = "VirtualSpace__VirtualDesktopDragWindow!";
            public const string VS_CONTROLLER_TITLE = "[Virtual Space Controller]";
        }

        public static class VirtualDesktop
        {
            public const int NavHTypeNextRow = 0;
            public const int NavHTypeSameRow = 1;
            public const int VdwSizeFloor    = 100;
        }

        public static class Settings
        {
            public const string RuleFileExt     = ".rules";
            public const string ClusterFileExt  = ".cluster";
            public const string ProfilesFolder  = "Profiles";
            public const string CacheFolder     = "Cache";
            public const string PluginsFolder   = "Plugins";
            public const string SettingsFile    = "settings.json";
            public const string DefaultVersion  = "1.0";
            public const string DefaultLogLevel = "EVENT";
        }

        public static class Reg
        {
            public const string RegKeyApp        = @"Software\newlooper.com\VirtualSpace";
            public const string RegKeyConfigRoot = "ConfigRoot";
        }

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

            public const  string SVD_TREE_NODE_PREFIX = "hk_node_svd_";
            private const string SVD_FUNC_DESC_PREFIX = "Switch To Desktop ";
            public const  string MW_TREE_NODE_PREFIX  = "hk_node_mw_";
            private const string MW_FUNC_DESC_PREFIX  = "Move To Desktop ";
            public const  string MWF_TREE_NODE_PREFIX = "hk_node_mwf_";
            private const string MWF_FUNC_DESC_PREFIX = "Move and Follow To Desktop ";

            ///////////////////////////////////////////////////
            // 值与控件名称一一对应，若控件名被修改，则此处也须对应改变
            public const string RISE_VIEW                              = "hk_node_rise_mainview";
            public const string RISE_VIEW_FOR_ACTIVE_APP               = "hk_node_rise_mainview_for_active_app";
            public const string RISE_VIEW_FOR_CURRENT_VD               = "hk_node_rise_mainview_for_current_vd";
            public const string RISE_VIEW_FOR_ACTIVE_APP_IN_CURRENT_VD = "hk_node_rise_mainview_for_active_app_in_current_vd";
            public const string SHOW_APP_CONTROLLER                    = "hk_node_open_app_controller";
            public const string NAV_LEFT                               = "hk_node_nav_left";
            public const string NAV_RIGHT                              = "hk_node_nav_right";
            public const string NAV_UP                                 = "hk_node_nav_up";
            public const string NAV_DOWN                               = "hk_node_nav_down";
            public const string SWITCH_BACK_LAST                       = "hk_node_svd_back_last";

            ////////////////////////////////////////////////////////////////
            // 可由热键调用的程序功能表
            // tuple.Item1 => friendly name
            // tuple.Item2 => UserMessageId
            // tuple.Item3 => alternate hotkey, 由程序保留，只能在源码中修改
            public static readonly Dictionary<string, (string FuncDesc, int MessageId, string AltHotKey)> Info = new()
            {
                {RISE_VIEW, ( "Rise MainView", UserMessage.RiseView, "LWin+Tab" )},
                {RISE_VIEW_FOR_ACTIVE_APP, ( "Rise MainView For Active App", UserMessage.RiseViewForActiveApp, "" )},
                {RISE_VIEW_FOR_CURRENT_VD, ( "Rise MainView For Current Desktop", UserMessage.RiseViewForCurrentVD, "" )},
                {
                    RISE_VIEW_FOR_ACTIVE_APP_IN_CURRENT_VD,
                    ( "Rise MainView For Active App In Current Virtual Desktop", UserMessage.RiseViewForActiveAppInCurrentVD, "" )
                },
                {SHOW_APP_CONTROLLER, ( "Open AppController", UserMessage.ShowAppController, "" )},
                {NAV_LEFT, ( "Left", UserMessage.NavLeft, "LWin+Ctrl+Left" )},
                {NAV_RIGHT, ( "Right", UserMessage.NavRight, "LWin+Ctrl+Right" )},
                {NAV_UP, ( "Up", UserMessage.NavUp, "LWin+Ctrl+Up" )},
                {NAV_DOWN, ( "Down", UserMessage.NavDown, "LWin+Ctrl+Down" )},
                {SWITCH_BACK_LAST, ( "Switch Back To Last Desktop", UserMessage.SwitchBackToLastDesktop, "" )},
            };

            public static string GetFuncDesc( string key )
            {
                var func = "";
                if ( Info.ContainsKey( key ) )
                {
                    func = Info[key].FuncDesc;
                }
                else if ( key.StartsWith( SVD_TREE_NODE_PREFIX ) )
                {
                    func = SVD_FUNC_DESC_PREFIX + key.Replace( SVD_TREE_NODE_PREFIX, "" );
                }
                else if ( key.StartsWith( MW_TREE_NODE_PREFIX ) )
                {
                    func = MW_FUNC_DESC_PREFIX + key.Replace( MW_TREE_NODE_PREFIX, "" );
                }
                else if ( key.StartsWith( MWF_TREE_NODE_PREFIX ) )
                {
                    func = MWF_FUNC_DESC_PREFIX + key.Replace( MWF_TREE_NODE_PREFIX, "" );
                }

                return func;
            }

            public static KeyBinding GetKeyBinding( string key )
            {
                var kb = new KeyBinding();
                if ( Info.ContainsKey( key ) )
                {
                    kb.MessageId = Info[key].MessageId;
                }
                else if ( key.StartsWith( SVD_TREE_NODE_PREFIX ) )
                {
                    kb.MessageId = UserMessage.Meta.SVD_START + int.Parse( key.Replace( SVD_TREE_NODE_PREFIX, "" ) );
                }
                else if ( key.StartsWith( MW_TREE_NODE_PREFIX ) )
                {
                    kb.MessageId = UserMessage.Meta.MW_START + int.Parse( key.Replace( MW_TREE_NODE_PREFIX, "" ) );
                }
                else if ( key.StartsWith( MWF_TREE_NODE_PREFIX ) )
                {
                    kb.MessageId = UserMessage.Meta.MWF_START + int.Parse( key.Replace( MWF_TREE_NODE_PREFIX, "" ) );
                }

                return kb;
            }

            public static string GetHotkeyExtra( string key )
            {
                var extra = "";
                if ( Info.ContainsKey( key ) )
                {
                    extra = Info[key].AltHotKey;
                }

                return extra;
            }
        }

        public static class MouseAction
        {
            public enum Action
            {
                DoNothing,
                ContextMenu,
                DesktopVisibleAndCloseView,
                DesktopVisibleOnly,
                WindowActiveDesktopVisibleAndCloseView,
                WindowActiveDesktopVisibleOnly,
                WindowClose,
                WindowHideFromView,
                WindowShowForSelectedProcessOnly,
                WindowShowForSelectedProcessInSelectedDesktop,
                DesktopShowForSelectedDesktop
            }

            public const  string MOUSE_NODE_DESKTOP_PREFIX = "mouse_node_d_";
            public const  string MOUSE_NODE_WINDOW_PREFIX  = "mouse_node_w_";
            private const string KEY_SPLITTER              = "+";

            private static readonly Dictionary<MouseButtons, string> MouseButtonsName;
            private static readonly Dictionary<Keys, string>         KeysName;

            ////////////////////////////////////////////////////////////////
            // 鼠标动作表，信息包含默认行为
            public static readonly Dictionary<string, Action> Info;

            static MouseAction()
            {
                MouseButtonsName = new Dictionary<MouseButtons, string>
                {
                    {MouseButtons.Left, "Left"},
                    {MouseButtons.Middle, "Middle"},
                    {MouseButtons.Right, "Right"}
                };

                KeysName = new Dictionary<Keys, string>
                {
                    {Keys.Control, "Ctrl"},
                    {Keys.Alt, "Alt"},
                    {Keys.Shift, "Shift"}
                };

                Info = new Dictionary<string, Action>
                {
                    {MOUSE_NODE_DESKTOP_PREFIX + MouseButtonsName[MouseButtons.Left], Action.DesktopVisibleAndCloseView},
                    {MOUSE_NODE_DESKTOP_PREFIX + MouseButtonsName[MouseButtons.Middle], Action.DesktopVisibleOnly},
                    {MOUSE_NODE_DESKTOP_PREFIX + MouseButtonsName[MouseButtons.Right], Action.ContextMenu},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_DESKTOP_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing},

                    {MOUSE_NODE_WINDOW_PREFIX + MouseButtonsName[MouseButtons.Left], Action.WindowActiveDesktopVisibleAndCloseView},
                    {MOUSE_NODE_WINDOW_PREFIX + MouseButtonsName[MouseButtons.Middle], Action.WindowActiveDesktopVisibleOnly},
                    {MOUSE_NODE_WINDOW_PREFIX + MouseButtonsName[MouseButtons.Right], Action.ContextMenu},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Control] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Alt] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Left], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Middle], Action.DoNothing},
                    {MOUSE_NODE_WINDOW_PREFIX + KeysName[Keys.Shift] + KEY_SPLITTER + MouseButtonsName[MouseButtons.Right], Action.DoNothing}
                };
            }

            public static string GetActionId( MouseButtons mb, Keys key, string prefix )
            {
                var actionId = "";
                switch ( key )
                {
                    case Keys.None:
                        actionId = prefix + MouseButtonsName[mb];
                        break;
                    case Keys.Control:
                    case Keys.Alt:
                    case Keys.Shift:
                        actionId = prefix + KeysName[key] + KEY_SPLITTER + MouseButtonsName[mb];
                        break;
                }

                return actionId;
            }
        }
    }
}