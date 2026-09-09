// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using PropertyChanged;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace ControlPanel.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class ControlViewModel : ViewModelBase
{
    public ControlViewModel()
    {
        _isInitialized = true;
    }
}

[AddINotifyPropertyChangedInterface]
public partial class MouseActionModel : ViewModelBase
{
    public MouseActionModel()
    {
        _isInitialized = true;
    }

    public static List<object> DesktopActions { get; } =
    [
        new { Value = nameof( MouseAction.Action.DoNothing ), Text                     = "" },
        new { Value = nameof( MouseAction.Action.ContextMenu ), Text                   = "" },
        new { Value = nameof( MouseAction.Action.DesktopVisibleAndCloseView ), Text    = "" },
        new { Value = nameof( MouseAction.Action.DesktopVisibleOnly ), Text            = "" },
        new { Value = nameof( MouseAction.Action.DesktopShowForSelectedDesktop ), Text = "" }
    ];

    public static List<object> WindowActions { get; } =
    [
        new { Value = nameof( MouseAction.Action.DoNothing ), Text                                     = "" },
        new { Value = nameof( MouseAction.Action.ContextMenu ), Text                                   = "" },
        new { Value = nameof( MouseAction.Action.WindowActiveDesktopVisibleAndCloseView ), Text        = "" },
        new { Value = nameof( MouseAction.Action.WindowActiveDesktopVisibleOnly ), Text                = "" },
        new { Value = nameof( MouseAction.Action.WindowClose ), Text                                   = "" },
        new { Value = nameof( MouseAction.Action.WindowHideFromView ), Text                            = "" },
        new { Value = nameof( MouseAction.Action.WindowShowForSelectedProcessOnly ), Text              = "" },
        new { Value = nameof( MouseAction.Action.WindowShowForSelectedProcessInSelectedDesktop ), Text = "" }
    ];

    public bool UseWheelSwitchDesktopWhenOnTaskbar { get; set; } = Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar;

    public bool LWin  { get; set; }
    public bool Ctrl  { get; set; }
    public bool Alt   { get; set; }
    public bool Shift { get; set; }

    public List<object> MouseButtons { get; set; } = GetMouseButtons();

    public string MouseButton { get; set; } = "Left";

    private static List<object> GetMouseButtons() =>
        [.. MouseAction.MouseButtonsName.Values.Select( name => (object)new { Value = name, Text = "" } )];

    public void OnPropertyChanged( string propertyName, object before, object after )
    {
        var propertyChanged = PropertyChanged;
        if ( propertyChanged == null )
        {
            return;
        }

        if ( _isInitialized && propertyName == nameof( UseWheelSwitchDesktopWhenOnTaskbar ) )
        {
            Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar = (bool)after;
            Manager.Save( reason: Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar );
            var msg = Manager.CurrentProfile.Mouse.UseWheelSwitchDesktopWhenOnTaskbar ? UserMessage.EnableMouseHook : UserMessage.DisableMouseHook;
            User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, (ulong)msg, 0 );
        }

        propertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public void Clear()
    {
        LWin        = Ctrl = Alt = Shift = false;
        MouseButton = "";
    }
}

[AddINotifyPropertyChangedInterface]
public partial class KeyBindingModel : ViewModelBase
{
    public KeyBindingModel()
    {
        BoxVisible     = Visibility.Hidden;
        _isInitialized = true;
    }

    public Visibility BoxVisible { get; set; }
    public string     Path       { get; set; }
    public string     Extra      { get; set; }
    public bool       LWin       { get; set; }
    public bool       Ctrl       { get; set; }
    public bool       Alt        { get; set; }
    public bool       Shift      { get; set; }
    public string     Key        { get; set; }

    public void Clear()
    {
        LWin = Ctrl = Alt = Shift = false;
        Key  = Const.Hotkey.NONE;
    }
}