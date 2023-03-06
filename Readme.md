# VirtualSpace

> A Virtual Desktop Enhancement GUI Program For Win10 & Win11

[![License](https://img.shields.io/github/license/newlooper/VirtualSpace "License")](https://github.com/newlooper/VirtualSpace/blob/main/COPYING)
[![Download Lastest](https://img.shields.io/github/v/release/newlooper/VirtualSpace "Download Lastest")](https://github.com/newlooper/VirtualSpace/releases)
[![Total Downloads](https://img.shields.io/github/downloads/newlooper/VirtualSpace/total "Total Downloads")](https://github.com/newlooper/VirtualSpace/releases)

## 1. Intro

### 1.1 Design memo

cn: https://newlooper.com/post/original/cs/os/windows/virtualdesktop/

## 2. Download & Installation

Download from [Releases](https://github.com/newlooper/VirtualSpace/releases) page.

VirtualSpace is green software, All used files are in its own directory, just unzip to a local dir (eg. `your desktop`\VirtualSpace) and run.

or build the program yourself (see below).

## 3. Build

> Suggested Target Platform `x64`

### 3.1 main program

- build VirtualSpace

> Note
> 
> WPF\ControlPanel is currently not used by VirtualSpace, you may unload it in your IDE

### 3.2 plugins

- Cube3D —— plugin for virtual desktop switch effects
  - build Cube3D project
  - put `all generated files` into main program's plugins Folder eg: `plugins\Cube3D`
  - Run Cube3D.exe after VirtualSpace started

## 4. HowTo

### 4.1 Default Hotkey

- LWin+Tab  ——  rise main view
- Ctrl+Alt+F12  ——  config panel
- LWin+LCtrl+<↑ ↓ ← →>  ——  switch virtual desktop

## 5. Q&A

- Q1: Why hotkey not working sometimes.
- A1: This is usually due to Windows UIPI (UAC).
- S1: Run VirtualSpace as Administrator to fix this.

[Others](https://github.com/newlooper/VirtualSpace/issues?q=is%3Aissue)

## 6. Demos

### Video

[![](https://res.cloudinary.com/marcomontalbano/image/upload/v1662744032/video_to_markdown/images/youtube--aFUo2kLYUy0-c05b58ac6eb4c4700831b2b3070cd403.jpg)](https://www.youtube.com/watch?v=aFUo2kLYUy0 "")

### Image

![05](https://github.com/newlooper/images/blob/main/VirtualSpace/05-SwitchVirtualDesktopInDirecti.gif?raw=true '05')