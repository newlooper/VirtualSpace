/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VirtualSpace.Helpers;
using VirtualSpace.Config;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow : Form
    {
        private static   List<VirtualDesktopWindow>? _virtualDesktops;
        private readonly List<VisibleWindow>         _visibleWindows = new();
        public           Guid                        VdId;
        private          string                      _desktopName;
        private          Point                       _fixedPosition;

        private VirtualDesktopWindow()
        {
            InitializeComponent();
            base.DoubleBuffered = ConfigManager.Configs.Cluster.EnableDoubleBufferedForVDW;
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true );
            UpdateStyles();
        }

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public int VdIndex { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.Style   =  unchecked( cp.Style | (int)0x80000000 ); // WS_POPUP
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;

        protected override void WndProc( ref Message m )
        {
            switch ( m.Msg )
            {
                case WinMsg.WM_QUERYENDSESSION:
                    m.Result = new IntPtr( 1 );
                    return;
                case WinMsg.WM_HOTKEY:
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

                    break;
            }

            base.WndProc( ref m );
        }

        public static VirtualDesktopWindow Create( int index, Guid guid, Size initSize, Color defaultBackColor, int vdwPadding )
        {
            var vdw = new VirtualDesktopWindow
            {
                StartPosition = FormStartPosition.Manual,
                TabStop       = false,
                TopLevel      = true,
                TopMost       = true,
                Name          = "vdw_" + index,
                VdId          = guid,
                VdIndex       = index,
                Size          = initSize,
                BackColor     = defaultBackColor,
                Padding       = new Padding( vdwPadding ),
                ResizeRedraw  = true,
                Text          = Const.Window.VD_CONTAINER_TITLE
            };
            vdw.SetOwner( MainWindow.GetMainWindow() );
            return vdw;
        }

        private void SetOwner( MainWindow owner )
        {
            void DoSetOwner()
            {
                User32.SetWindowLongPtr( new HandleRef( this, Handle ),
                    (int)GetWindowLongFields.GWL_HWNDPARENT,
                    owner.Handle.ToInt32()
                );
            }

            if ( owner.Dispatcher.CheckAccess() )
                DoSetOwner();
            else
                owner.Dispatcher.Invoke( DoSetOwner );
        }

        private void VirtualDesktopWindow_Closing( object? sender, FormClosingEventArgs e )
        {
            e.Cancel = true;
        }

        public void RealClose()
        {
            FormClosing -= VirtualDesktopWindow_Closing;
            ClearVisibleWindows();
            ReleaseWallpaperResources();
            Close();
        }

        private void ShowByVdIndex()
        {
            var ui = VirtualDesktopManager.Ui;
            var (scaleX, scaleY) = SysInfo.Dpi;

            var matrixIndex = VirtualDesktopManager.GetMatrixIndexByVdIndex( VdIndex );
            var location    = MainWindow.GetCellLocationByMatrixIndex( matrixIndex );
            var point       = new Point( (int)( ( location.X + ui.VDWBorderSize ) * scaleX ), (int)( ( location.Y + ui.VDWBorderSize ) * scaleY ) );
            Location       = point;
            _fixedPosition = point;

            var size      = MainWindow.GetCellSizeByMatrixIndex( matrixIndex );
            var vdwWidth  = ( size.Width - 2 * ui.VDWBorderSize ) * scaleX + 1;
            var vdwHeight = ( size.Height - 2 * ui.VDWBorderSize ) * scaleY + 1;

            ////////////////////////////////////////////////////////////////
            // 虚拟桌面容器的宽/高下限，宽/高任意一个低于此值，虚拟桌面尺寸强制归零
            if ( vdwWidth < Const.VirtualDesktop.VdwSizeFloor || vdwHeight < Const.VirtualDesktop.VdwSizeFloor )
            {
                Size = Size.Empty; // 强制归零，从而避免接收到鼠标事件
            }
            else
            {
                var vdName = DesktopWrapper.DesktopNameFromGuid( VdId );
                if ( vdName != _desktopName ) UpdateDesktopName( vdName );

                Size = new Size( (int)vdwWidth, (int)vdwHeight );

                if ( !Visible )
                    Show();
            }
        }
    }
}