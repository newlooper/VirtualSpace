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

[![](https://res.cloudinary.com/marcomontalbano/image/upload/v1662744032/video_to_markdown/images/youtube--aFUo2kLYUy0-c05b58ac6eb4c4700831b2b3070cd403.jpg)](https://www.youtube.com/watch?v=aFUo2kLYUy0 "")
