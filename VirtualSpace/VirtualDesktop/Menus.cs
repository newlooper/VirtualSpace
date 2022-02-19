/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.UIA;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace.VirtualDesktop
{
    public static class Menus
    {
        private static ContextMenuStrip _ctm;

        public static void ThumbCtm( MenuInfo mi )
        {
            _ctm = new ContextMenuStrip();
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

            var closeWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.Close" )
            };

            async void OnCloseWindowClick( object? s, EventArgs evt )
            {
                if ( mi.Vw.Classname == Const.WindowsUiCoreWindow )
                {
                    Uia.CloseButtonInvokeByWindowHandle( mi.Vw.Handle );
                }
                else if ( mi.Vw.CoreUiWindowHandle != default )
                {
                    User32.ShowWindow( mi.Vw.Handle, 0 );
                    User32.ShowWindow( mi.Vw.CoreUiWindowHandle, 0 );
                    User32.PostMessage( mi.Vw.Handle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );
                    User32.PostMessage( mi.Vw.CoreUiWindowHandle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );
                }
                else
                {
                    User32.PostMessage( mi.Vw.Handle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );
                    // User32.PostMessage( mi.Vw.Handle, WinMsg.WM_CLOSE, 0, 0 );
                    // User32.PostMessage( mi.Vw.Handle, WinMsg.WM_QUIT, 0, 0 );
                    // User32.PostMessage( mi.Vw.Handle, WinMsg.WM_DESTROY, 0, 0 );
                }

                await Task.Delay( Const.WindowCloseDelay );
                VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {mi.Self} );
            }

            closeWindow.Click += OnCloseWindowClick;
            _ctm.Items.Add( closeWindow );
            _ctm.Items.Add( "-" );

            var newRuleFromWindow = new ToolStripMenuItem
            {
                Text = Agent.Langs.GetString( "VDW.CTM.Window.NewRule" )
            };

            void OnCreateRuleFromWindow( object? s, EventArgs evt )
            {
                var ruleForm = new RuleForm( -1 );
                ruleForm.Init();
                ruleForm.SetFormValuesFromWindow( mi.Vw.Handle );
                ruleForm.TopMost = true;
                ruleForm.ShowDialog();
            }

            newRuleFromWindow.Click += OnCreateRuleFromWindow;
            _ctm.Items.Add( newRuleFromWindow );
            _ctm.Show( mi.Sender as Control, mi.Location );
        }

        public static void VdCtm( MenuInfo mi )
        {
            _ctm = new ContextMenuStrip();
            var vdIndex     = DesktopWrapper.IndexFromGuid( mi.Self.VdId );
            var currentName = DesktopWrapper.DesktopNameFromIndex( vdIndex );
            var desktopName = new ToolStripTextBox {Text = currentName, AutoSize = false, Width = 200};
            desktopName.KeyPress += ( s, evt ) =>
            {
                if ( evt.KeyChar == 13 )
                {
                    if ( currentName != desktopName.Text )
                    {
                        DesktopWrapper.SetNameByIndex( vdIndex, desktopName.Text );
                        mi.Self.UpdateDesktopName( desktopName.Text );
                    }

                    evt.Handled = true;
                    _ctm.Close();
                }
            };
            _ctm.Items.Add( desktopName );
            _ctm.Items.Add( "-" );
            var delVirtualDesktop = new ToolStripMenuItem( Agent.Langs.GetString( "VDW.CTM.Desktop.Remove" ) );
            delVirtualDesktop.Click += ( s, evt ) =>
            {
                if ( DesktopWrapper.RemoveDesktopByIndex( vdIndex ) )
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