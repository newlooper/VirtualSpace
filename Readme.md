# VirtualSpace

> A Virtual Desktop Enhancement GUI Program For Win10 & Win11

## Intro

### Design memo

https://newlooper.com/post/original/cs/os/windows/virtualdesktop/

### Main Projects

> Suggested Target Platform `x64`

#### main program

- build and run VirtualSpace —— for  Windows 11

- build and run VirtualSpace10 —— for window 10 19041+

> Note
> 
> WPF\ControlPanel is currently not used by VirtualSpace(10), you may unload it in your IDE

#### plugins

- build Cube3D —— plugin for virtual desktop switch effects (put `all generated files` into main program's plugins Folder eg: `plugins\Cube3D`, then Run Cube3D.exe after VirtualSpace(10) started)

### HowTo

#### Default Hotkey

- LWin+Tab  ——  rise main view
- Ctrl+Alt+F12  ——  config panel
- LWin+LCtrl+<↑ ↓ ← →>  ——  switch virtual desktop

## Q&A

Q1: Why hotkey not working sometimes.

A1: This is usually due to Windows UIPI (UAC).

S1: Run VirtualSpace as Administrator to fix this.

## Demos

### Move Window, Swap Virtual Desktops

![01](https://github.com/newlooper/images/blob/main/VirtualSpace/01_MoveWindow_SwapVirtualDesktops.gif?raw=true '01')

### Add/Remove Virtual Desktop

![02](https://github.com/newlooper/images/blob/main/VirtualSpace/02_AddRemoveVirtualDesktop.gif?raw=true '02')

### Hide/Pin Window, Pin Application

![03](https://github.com/newlooper/images/blob/main/VirtualSpace/03_HidePinWindow_PinApp.gif?raw=true '03')

### Rules For Window And/Or Application

![04](https://github.com/newlooper/images/blob/main/VirtualSpace/04_RulesForWindowOrApp.gif?raw=true '04')

### Switch Virtual Desktop In Directions

![05](https://github.com/newlooper/images/blob/main/VirtualSpace/05-SwitchVirtualDesktopInDirecti.gif?raw=true '05')
