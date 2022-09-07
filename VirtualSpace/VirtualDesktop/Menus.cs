/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VirtualSpace.Config;
using VirtualSpace.Factory;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace.VirtualDesktop
{
    public static class Menus
    {
        private static ContextMenuStrip _ctm;

        public static void ThumbCtm( MenuInfo mi )
        {
            _ctm ??= new ContextMenuStrip();
            _ctm.Items.Clear();
            var pinWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.PinWin" ),
                Checked = DesktopWrapper.IsWindowPinned( mi.Vw.Handle )
            };
            pinWindow.Click += ( s, evt ) =>
            {
                DesktopWrapper.PinWindow( mi.Vw.Handle, pinWindow.Checked );
                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            };
            _ctm.Items.Add( pinWindow );

            var pinApp = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.PinApp" ),
                Checked = DesktopWrapper.IsApplicationPinned( mi.Vw.Handle )
            };
            pinApp.Click += ( s, evt ) =>
            {
                DesktopWrapper.PinApp( mi.Vw.Handle, pinApp.Checked );
                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            };
            _ctm.Items.Add( pinApp );

            var hideWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.HideFromView" )
            };

            void OnIgnoreWindowClick( object? s, EventArgs evt )
            {
                Filters.WndHandleManualIgnoreList.Add( mi.Vw.Handle );
                if ( DesktopWrapper.IsWindowPinned( mi.Vw.Handle ) ||
                     DesktopWrapper.IsApplicationPinned( mi.Vw.Handle ) )
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                }
                else
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {mi.Self} );
                }
            }

            hideWindow.Click += OnIgnoreWindowClick;
            // hideWindow.Enabled = !DesktopWrapper.IsWindowPinned( mi.Vw.Handle );
            _ctm.Items.Add( hideWindow );

            var closeWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.Close" )
            };

            void OnCloseWindowClick( object? s, EventArgs evt )
            {
                mi.Self.CloseSelectedWindow( mi.Vw );
            }

            closeWindow.Click += OnCloseWindowClick;
            _ctm.Items.Add( closeWindow );
            _ctm.Items.Add( "-" );

            var itemScreen = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Window.Screen" ) );

            void MoveToScreen( object? s, EventArgs evt )
            {
                var selectedScreen = s as ToolStripMenuItem;
                var srcScreen      = Screen.FromHandle( mi.Vw.Handle );
                if ( srcScreen.DeviceName == selectedScreen?.Text ) return;

                var destScreen = Screen.AllScreens.FirstOrDefault( x => x.DeviceName == selectedScreen?.Text );
                if ( destScreen == null ) return;

                var wp = new WINDOWPLACEMENT();
                wp.Length = Marshal.SizeOf( wp );
                if ( !User32.GetWindowPlacement( mi.Vw.Handle, ref wp ) ) return;

                var rect         = wp.NormalPosition;
                var targetX      = destScreen.WorkingArea.X + rect.Left - srcScreen.WorkingArea.Left;
                var targetY      = destScreen.WorkingArea.Y + rect.Top - srcScreen.WorkingArea.Top;
                var targetWidth  = rect.Right - rect.Left;
                var targetHeight = rect.Bottom - rect.Top;

                switch ( wp.ShowCmd )
                {
                    case ShowState.SW_SHOWMAXIMIZED:
                        User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_RESTORE );
                        User32.SetWindowPos( mi.Vw.Handle, IntPtr.Zero,
                            targetX, targetY, targetWidth, targetHeight, 0 );
                        User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_MAXIMIZE );
                        break;
                    case ShowState.SW_MINIMIZE:
                    case ShowState.SW_SHOWMINIMIZED:
                        User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_RESTORE );
                        User32.SetWindowPos( mi.Vw.Handle, IntPtr.Zero,
                            targetX, targetY, targetWidth, targetHeight, 0 );
                        // User32.ShowWindow( mi.Vw.Handle, (short)ShowState.SW_SHOWMINIMIZED );
                        break;
                    case ShowState.SW_NORMAL:
                        User32.SetWindowPos( mi.Vw.Handle, IntPtr.Zero,
                            targetX, targetY, targetWidth, targetHeight, 0 );
                        break;
                }
            }

            foreach ( var s in Screen.AllScreens )
            {
                var screen = Screen.FromHandle( mi.Vw.Handle );
                var item   = new ToolStripMenuItem( s.DeviceName );
                item.Checked = screen.DeviceName == s.DeviceName;
                item.Click += MoveToScreen;
                itemScreen.DropDownItems.Add( item );
            }

            _ctm.Items.Add( itemScreen );
            _ctm.Items.Add( "-" );

            var newRuleFromWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.NewRule" )
            };

            void OnCreateRuleFromWindow( object? s, EventArgs evt )
            {
                var ruleForm = new RuleForm( -1 );
                ruleForm.Init( new VirtualDesktopInfo() );
                ruleForm.SetFormValuesFromWindow( mi.Vw.Handle );
                ruleForm.TopMost = true;
                ruleForm.ShowDialog();
            }

            newRuleFromWindow.Click += OnCreateRuleFromWindow;
            _ctm.Items.Add( newRuleFromWindow );

            //////////////////////////
            // Show Window ContextMenu 
            _ctm.Show( mi.Sender as Control, mi.Location );
        }

        public static void VdCtm( MenuInfo mi )
        {
            _ctm ??= new ContextMenuStrip();
            _ctm.Items.Clear();
            var sysIndex    = DesktopWrapper.IndexFromGuid( mi.Self.VdId );
            var currentName = DesktopWrapper.DesktopNameFromIndex( sysIndex );
            var desktopName = new ToolStripTextBox {Text = currentName, AutoSize = false, Width = 200};
            desktopName.KeyPress += ( s, evt ) =>
            {
                if ( evt.KeyChar == (char)Keys.Enter )
                {
                    if ( currentName != desktopName.Text )
                    {
                        DesktopWrapper.SetNameByIndex( sysIndex, desktopName.Text );
                        mi.Self.UpdateDesktopName( desktopName.Text );
                    }

                    evt.Handled = true;
                    _ctm.Close();
                }
            };
            _ctm.Items.Add( desktopName );
            _ctm.Items.Add( "-" );

            var unHideWindow = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.UnHideWindow" ) );

            void OnUnHideWindow( object? s, EventArgs evt )
            {
                var item = s as ToolStripMenuItem;
                var m    = Regex.Match( item.Text, $@".*\|\|\|(.*)" );

                var h = (IntPtr)int.Parse( m.Groups[1].Value );

                Filters.WndHandleManualIgnoreList.Remove( h );
                if ( DesktopWrapper.IsWindowPinned( h ) ||
                     DesktopWrapper.IsApplicationPinned( h ) )
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                }
                else
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {mi.Self} );
                }
            }

            foreach ( var handle in Filters.WndHandleManualIgnoreList )
            {
                if ( !User32.IsWindow( handle ) ) continue;
                if ( !DesktopWrapper.IsWindowPinned( handle ) &&
                     !DesktopWrapper.IsApplicationPinned( handle ) &&
                     DesktopWrapper.FromWindow( handle ).Guid != mi.Self.VdId ) continue;

                _ = User32.GetWindowThreadProcessId( handle, out var pId );
                var process = Process.GetProcessById( pId );

                var sb = new StringBuilder( Const.WindowTitleMaxLength );
                User32.GetWindowText( handle, sb, sb.Capacity );
                var title = sb.ToString();

                var item = new ToolStripMenuItem( $"[{title}] of {process.ProcessName}(.exe) |||{handle}" );
                item.Click += OnUnHideWindow;
                unHideWindow.DropDownItems.Add( item );
            }

            unHideWindow.Enabled = unHideWindow.DropDownItems.Count > 0;
            _ctm.Items.Add( unHideWindow );
            _ctm.Items.Add( "-" );

            var delVirtualDesktop = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.Remove" ) );
            delVirtualDesktop.Click += ( s, evt ) =>
            {
                if ( DesktopWrapper.RemoveDesktopByGuid( mi.Self.VdId ) )
                {
                    var vdw = mi.Self;
                    vdw.RealClose();
                    mi.Vdws.RemoveAt( mi.Self.VdIndex );

                    // VirtualDesktopManager.FixLayout();
                    // VirtualDesktopManager.ShowAllVirtualDesktops();
                    // if ( VirtualDesktopManager.NeedRepaintThumbs )
                    // {
                    //     VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                    //     VirtualDesktopManager.NeedRepaintThumbs = false;
                    // }
                }
            };
            _ctm.Items.Add( delVirtualDesktop );

            var addVirtualDesktop = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.Create" ) );

            void BatchAdd( object? s, EventArgs evt )
            {
                var item  = s as ToolStripMenuItem;
                var count = int.Parse( item.Text );
                if ( count > 1 )
                {
                    VirtualDesktopManager.IsBatchCreate = true;

                    for ( var i = 0; i < count; i++ )
                        _ = DesktopWrapper.Create();

                    VirtualDesktopManager.FixLayout();
                    VirtualDesktopManager.ShowAllVirtualDesktops();
                    if ( VirtualDesktopManager.NeedRepaintThumbs )
                    {
                        VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                        VirtualDesktopManager.NeedRepaintThumbs = false;
                    }

                    VirtualDesktopManager.IsBatchCreate = false;
                }
                else
                {
                    VirtualDesktopManager.IsBatchCreate = false;
                    _ = DesktopWrapper.Create();
                }
            }

            for ( var i = 1; i <= 10; i++ )
            {
                var count = new ToolStripMenuItem( i.ToString() );
                count.Click += BatchAdd;
                addVirtualDesktop.DropDownItems.Add( count );
            }

            _ctm.Items.Add( addVirtualDesktop );

            ///////////////////////////////////
            // Show Virtual Desktop ContextMenu 
            _ctm.Show( mi.Sender as Control, mi.Location );
        }

        public static void CloseContextMenu()
        {
            _ctm?.Close();
        }
    }

    public class MenuInfo
    {
        public VisibleWindow              Vw       { get; set; }
        public VirtualDesktopWindow       Self     { get; set; }
        public object                     Sender   { get; set; }
        public Point                      Location { get; set; }
        public List<VirtualDesktopWindow> Vdws     { get; set; }
    }
}