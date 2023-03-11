/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow : Form
    {
        private static   List<VirtualDesktopWindow>? _virtualDesktops;
        private static   WindowInteropHelper         _windowInteropHelper;
        private readonly List<VisibleWindow>         _visibleWindows = new();
        private          string                      _desktopName;
        private          Point                       _fixedPosition;
        private          Size                        _initSize = Size.Empty;
        public           Guid                        VdId;
        public           int                         VdIndex;

        private VirtualDesktopWindow()
        {
            InitializeComponent();
            base.DoubleBuffered = ConfigManager.Configs.Cluster.EnableDoubleBufferedForVDW;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.Style = unchecked(cp.Style | (int)0x80000000); // WS_POPUP
                return cp;
            }
        }

        protected override void WndProc( ref Message m )
        {
            if ( m.Msg == WinMsg.WM_HOTKEY )
            {
                switch ( m.WParam.ToInt32() )
                {
                    case UserMessage.ShowVdw:
                        ShowByVdIndex();
                        return;
                    case UserMessage.RefreshVdw:
                        Refresh();
                        return;
                    case UserMessage.ShowThumbsOfVdw:
                        ShowThumbnails();
                        return;
                }
            }

            base.WndProc( ref m );
        }

        protected override bool ShowWithoutActivation => true;

        public static VirtualDesktopWindow Create( int index, Guid guid, Size initSize, Color defaultBackColor, int vdwPadding )
        {
            var vdw = new VirtualDesktopWindow
            {
                StartPosition = FormStartPosition.Manual,
                TabStop = false,
                TopLevel = true,
                TopMost = true,
                Name = "vdw_" + index,
                VdId = guid,
                VdIndex = index,
                Size = initSize,
                BackColor = defaultBackColor,
                Padding = new Padding( vdwPadding ),
                ResizeRedraw = true,
                Text = Const.Window.VD_CONTAINER_TITLE
            };
            vdw.SetOwner( MainWindow.GetMainWindow() );
            return vdw;
        }

        private void SetOwner( Window owner )
        {
            _windowInteropHelper ??= new WindowInteropHelper( owner );
            if ( owner.Dispatcher.CheckAccess() )
            {
                User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                    (int)GetWindowLongFields.GWL_HWNDPARENT,
                    _windowInteropHelper.Handle.ToInt32()
                );
            }
            else
            {
                owner.Dispatcher.Invoke( () =>
                    User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                        (int)GetWindowLongFields.GWL_HWNDPARENT,
                        _windowInteropHelper.Handle.ToInt32()
                    )
                );
            }
        }

        public void UpdateWallpaper()
        {
            if ( InvokeRequired )
            {
                Invoke( (MethodInvoker)Refresh );
            }
            else
            {
                Refresh();
            }
        }

        private void VirtualDesktopWindow_Closing( object? sender, FormClosingEventArgs e )
        {
            e.Cancel = true;
        }

        public void RealClose()
        {
            FormClosing -= VirtualDesktopWindow_Closing;
            ClearVisibleWindows();
            Close();
        }

        private void ShowByVdIndex()
        {
            var ui  = VirtualDesktopManager.Ui;
            var dpi = SysInfo.Dpi;

            var matrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex( VdIndex );
            var location    = MainWindow.GetCellLocationByMatrixIndex( matrixIndex );
            var point       = new Point( (int)( ( location.X + ui.VDWBorderSize ) * dpi.ScaleX ), (int)( ( location.Y + ui.VDWBorderSize ) * dpi.ScaleY ) );
            Location = point;
            _fixedPosition = point;

            var size      = MainWindow.GetCellSizeByMatrixIndex( matrixIndex );
            var vdwWidth  = ( size.Width - 2 * ui.VDWBorderSize ) * dpi.ScaleX + 1;
            var vdwHeight = ( size.Height - 2 * ui.VDWBorderSize ) * dpi.ScaleY + 1;

            ////////////////////////////////////////////////////////////////
            // 虚拟桌面容器的宽/高下限，宽/高任意一个低于此值，虚拟桌面尺寸强制归零
            if ( vdwWidth < Const.VirtualDesktop.VdwSizeFloor || vdwHeight < Const.VirtualDesktop.VdwSizeFloor )
            {
                Size = Size.Empty; // 强制归零，从而避免接收到鼠标事件
            }
            else
            {
                var vdName = DesktopWrapper.DesktopNameFromGuid( VdId );
                if ( vdName != _desktopName )
                {
                    UpdateDesktopName( vdName );
                }

                Size = new Size( (int)vdwWidth, (int)vdwHeight );

                if ( !Visible )
                    Show();
            }
        }

        private (bool isCached, string path, Color? color) CachedWallpaperInfo()
        {
            var wpPath = WinRegistry.GetWallPaperPathByGuid( VdId );
            if ( wpPath is null )
            {
                return new ValueTuple<bool, string, Color>( false, "", WinRegistry.GetBackColor() );
            }

            var wpInfo = Wallpaper.CachedWallPaperInfo( wpPath, ConfigManager.GetCachePath(), Width, Height );
            return new ValueTuple<bool, string, Color?>( wpInfo.Exists, wpPath, null );
        }

        private static void DrawImage( PaintEventArgs e, Wallpaper wp, int width = 0, int height = 0 )
        {
            if ( width > 0 && height > 0 )
            {
                e.Graphics.DrawImage( wp.Image, 0, 0, width, height );
            }
            else
            {
                e.Graphics.DrawImage( wp.Image, 0, 0 );
            }

            wp.Release();
        }

        private void InitPaint( (bool isCached, string path, Color? color) wpInfo, PaintEventArgs e )
        {
            Logger.Event( $"Init Desktop[{VdIndex}] background." );

            _initSize.Width = Width;
            _initSize.Height = Height;

            if ( wpInfo.color != null )
            {
                BackColor = (Color)wpInfo.color;
                return;
            }

            if ( wpInfo.isCached )
            {
                DrawImage( e, WinRegistry.GetWallpaperByPath( wpInfo.path,
                    Width,
                    Height,
                    ConfigManager.GetCachePath(),
                    ConfigManager.Configs.Cluster.VdwWallpaperQuality ) );
            }
            else
            {
                if ( VirtualDesktopManager.IsBatchCreate )
                {
                    DrawImage( e, WinRegistry.GetWallpaperByPath( wpInfo.path,
                        Width,
                        Height,
                        ConfigManager.GetCachePath(),
                        ConfigManager.Configs.Cluster.VdwWallpaperQuality ) );
                }
                else
                {
                    var hWnd = Handle;
                    Task.Run( () =>
                    {
                        WinRegistry.GetWallpaperByPath( wpInfo.path,
                            Width,
                            Height,
                            ConfigManager.GetCachePath(),
                            ConfigManager.Configs.Cluster.VdwWallpaperQuality ).Release();
                        User32.PostMessage( hWnd, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
                    } );
                }
            }
        }

        private void NormalPaint( (bool isCached, string path, Color? color) wpInfo, PaintEventArgs e )
        {
            if ( wpInfo.color != null )
            {
                BackColor = (Color)wpInfo.color;
                return;
            }

            if ( wpInfo.isCached )
            {
                DrawImage( e, WinRegistry.GetWallpaperByPath( wpInfo.path,
                    Width,
                    Height,
                    ConfigManager.GetCachePath(),
                    ConfigManager.Configs.Cluster.VdwWallpaperQuality ) );
            }
            else
            {
                Logger.Event( $"Create cache image({Width}*{Height}) for Desktop[{VdIndex}]" );
                Task.Run( () =>
                {
                    // only once for path with current Width*Height
                    WinRegistry.GetWallpaperByPath( wpInfo.path,
                            Width,
                            Height,
                            ConfigManager.GetCachePath(),
                            ConfigManager.Configs.Cluster.VdwWallpaperQuality )
                        .Release();
                } );

                ////////////////////////////////////////////////////////////////////////////////////
                // use init size, so we can create cache image async
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor; // faster
                DrawImage( e, WinRegistry.GetWallpaperByPath( wpInfo.path,
                    _initSize.Width,
                    _initSize.Height,
                    ConfigManager.GetCachePath(),
                    ConfigManager.Configs.Cluster.VdwWallpaperQuality ), Width, Height );
            }
        }

        private void RefreshThumbs( object? o, EventArgs e )
        {
            Logger.Event( $"Repaint thumbs in Desktop[{VdIndex}] due to size changed." );
            ReleaseThumbnails();
            ShowThumbnails();
        }

        private void pbWallpaper_Paint( object sender, PaintEventArgs e )
        {
            var wpInfo = CachedWallpaperInfo();
            if ( _initSize == Size.Empty )
            {
                Resize += RefreshThumbs;
                InitPaint( wpInfo, e );
            }
            else
            {
                NormalPaint( wpInfo, e );
            }

            var ui  = ConfigManager.CurrentProfile.UI;
            var str = "";

            if ( ui.ShowVdName )
            {
                str += _desktopName;
            }

            if ( ui.ShowVdIndex )
            {
                str += ui.ShowVdIndexType == 0 ? $"[{VdIndex}]" : $"[{VdIndex + 1}]";
            }

            if ( str == "" ) return;

            using var font = new Font( "Segoe UI emoji", 10 );
            e.Graphics.DrawString(
                str,
                font,
                Brushes.Beige,
                new Point( 2, Height - 30 )
            );
        }

        public void UpdateDesktopName( string name )
        {
            _desktopName = name;
            Refresh();
        }
    }
}