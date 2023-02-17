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

            ////////////////////////////////////////////////////////////////
            // pin window 
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

            ////////////////////////////////////////////////////////////////
            // pin app
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

            ////////////////////////////////////////////////////////////////
            // hide from view
            var hideWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.HideFromView" )
            };

            void OnIgnoreWindowClick( object? s, EventArgs evt )
            {
                Filters.WndHandleIgnoreListByManual.Add( mi.Vw.Handle );
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
            _ctm.Items.Add( hideWindow );

            _ctm.Items.Add( "-" );

            ////////////////////////////////////////////////////////////////
            // move to screen
            var itemScreen = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Window.Screen" ) );

            void MoveToScreen( object? s, EventArgs evt )
            {
                var selectedScreen = s as ToolStripMenuItem;
                WindowTool.MoveWindowToScreen( mi.Vw.Handle, itemScreen.DropDownItems.IndexOf( selectedScreen ) );
            }

            foreach ( var s in Screen.AllScreens )
            {
                var screen = Screen.FromHandle( mi.Vw.Handle );
                var item   = new ToolStripMenuItem( $"{s.DeviceName}  ({s.DeviceFriendlyName()})" );
                item.Checked = screen.DeviceName == s.DeviceName;
                item.Click += MoveToScreen;
                itemScreen.DropDownItems.Add( item );
            }

            _ctm.Items.Add( itemScreen );

            _ctm.Items.Add( "-" );

            ////////////////////////////////////////////////////////////////
            // rule for window
            var newRuleFromWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.NewRule" )
            };

            void OnCreateRuleFromWindow( object? s, EventArgs evt )
            {
                var ruleForm = new RuleForm( -1 );
                ruleForm.Init( new VirtualDesktopInfo() );
                ruleForm.SetFormValuesByWindow( mi.Vw.Handle );
                ruleForm.TopMost = true;
                ruleForm.ShowDialog();
            }

            newRuleFromWindow.Click += OnCreateRuleFromWindow;
            _ctm.Items.Add( newRuleFromWindow );

            _ctm.Items.Add( "-" );

            ////////////////////////////////////////////////////////////////
            // close window 
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

            ////////////////////////////////////////////////////////////////
            // Show Window ContextMenu 
            _ctm.Show( mi.Sender as Control, mi.Location );
        }

        public static void VdCtm( MenuInfo mi )
        {
            _ctm ??= new ContextMenuStrip();
            _ctm.Items.Clear();

            //////////////////////////////////////////////////////////////
            // show & edit desktop's name
            var sysIndex    = DesktopWrapper.IndexFromGuid( mi.Self.VdId );
            var currentName = DesktopWrapper.DesktopNameFromIndex( sysIndex );
            var desktopName = new ToolStripTextBox {Text = currentName, AutoSize = false, Width = 200};
            desktopName.KeyPress += ( s, evt ) =>
            {
                if ( evt.KeyChar != (char)Keys.Enter ) return;
                if ( currentName != desktopName.Text )
                {
                    DesktopWrapper.SetNameByIndex( sysIndex, desktopName.Text );
                    mi.Self.UpdateDesktopName( desktopName.Text );
                }

                evt.Handled = true;
                _ctm.Close();
            };
            _ctm.Items.Add( desktopName );

            _ctm.Items.Add( "-" );

            //////////////////////////////////////////////////////////////
            // UnHideWindow
            var unHideWindow = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.UnHideWindow" ) );

            void OnUnHideWindow( object? s, EventArgs evt )
            {
                var item = s as ToolStripMenuItem;
                var m    = Regex.Match( item.Text, $@".*{Const.HideWindowSplitter}(.*)" );

                var h = (IntPtr)int.Parse( m.Groups[1].Value );

                Filters.WndHandleIgnoreListByManual.Remove( h );
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

            var sb = new StringBuilder( Const.WindowTitleMaxLength );
            foreach ( var handle in Filters.WndHandleIgnoreListByManual )
            {
                if ( !User32.IsWindow( handle ) ) continue;
                if ( !DesktopWrapper.IsWindowPinned( handle ) &&
                     !DesktopWrapper.IsApplicationPinned( handle ) &&
                     DesktopWrapper.FromWindow( handle ).Guid != mi.Self.VdId ) continue;

                _ = User32.GetWindowThreadProcessId( handle, out var pId );
                var process = Process.GetProcessById( pId );

                _ = User32.GetWindowText( handle, sb, sb.Capacity );
                var title = sb.ToString();

                var item = new ToolStripMenuItem( $"[{title}] of {process.ProcessName}(.exe){Const.HideWindowSplitter}{handle}" );
                item.Click += OnUnHideWindow;
                unHideWindow.DropDownItems.Add( item );
            }

            unHideWindow.Enabled = unHideWindow.DropDownItems.Count > 0;
            _ctm.Items.Add( unHideWindow );

            _ctm.Items.Add( "-" );

            //////////////////////////////////////////////////////////////
            // delete virtual desktop
            var delVirtualDesktop = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.Remove" ) );
            delVirtualDesktop.Click += ( s, evt ) =>
            {
                if ( DesktopWrapper.RemoveDesktopByGuid( mi.Self.VdId ) )
                {
                    var vdw = mi.Self;
                    vdw.RealClose();
                    mi.Vdws.RemoveAt( mi.Self.VdIndex );
                }
            };
            _ctm.Items.Add( delVirtualDesktop );

            //////////////////////////////////////////////////////////////
            // create virtual desktop
            var createVirtualDesktop = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.Create" ) );

            void BatchCreate( object? s, EventArgs evt )
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
                count.Click += BatchCreate;
                createVirtualDesktop.DropDownItems.Add( count );
            }

            _ctm.Items.Add( createVirtualDesktop );

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