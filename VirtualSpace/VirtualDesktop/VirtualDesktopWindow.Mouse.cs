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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.Tools;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow
    {
        private static int            _hoverVdIndex;
        private static Point          _startPoint;
        private static int            _dragState;
        private static Rectangle      _dragBounds = Rectangle.Empty;
        private static VisibleWindow? _selectedWindow;
        private static DragWindow?    _dw;
        private        bool           _isTheOnlyOneInMainView;

        public void ResetOnlyOneStatus()
        {
            _isTheOnlyOneInMainView = false;
        }

        private void VirtualDesktopWindow_MouseDown( object sender, MouseEventArgs e )
        {
            _virtualDesktops = VirtualDesktopManager.GetAllVirtualDesktops();
            _startPoint = e.Location;
            var dragSize = SystemInformation.DragSize * ConfigManager.CurrentProfile.Mouse.DragSizeFactor;
            _dragBounds = new Rectangle(
                new Point( _startPoint.X - dragSize.Width / 2, _startPoint.Y - dragSize.Height / 2 ),
                dragSize );
            foreach ( var window in _visibleWindows.Where( window => window.Rect.Contains( e.Location ) ) )
            {
                Logger.Verbose( "SELECT.Win " + window.Title );
                _selectedWindow = window;
                break;
            }
        }

        private static bool IsOutBounds( Point location )
        {
            return _dragBounds != Rectangle.Empty && !_dragBounds.Contains( location );
        }

        private void VirtualDesktopWindow_MouseMove( object sender, MouseEventArgs e )
        {
            if ( _dragState == 0 && e.Button == MouseButtons.Left && IsOutBounds( e.Location ) )
            {
                _dragState = 1;
            }

            if ( _dragState == 0 ) return;

            HoverOnDesktop( sender, e );

            if ( _selectedWindow != null )
            {
                if ( _dw == null )
                {
                    _dw = DragWindow.CreateAndShow( _selectedWindow.Rect.Width, _selectedWindow.Rect.Height );

                    var i = DwmApi.DwmRegisterThumbnail( _dw.Handle, _selectedWindow.Handle, out var thumb );
                    if ( i == 0 )
                    {
                        var props = new DWM_THUMBNAIL_PROPERTIES
                        {
                            fVisible = true,
                            dwFlags = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_OPACITY,
                            opacity = 255,
                            rcDestination = new RECT( 0, 0, _dw.Width, _dw.Height )
                        };
                        _dw.Thumb = thumb;
                        UpdateThumbnail( _dw.Thumb, props );
                    }

                    var dtp = _selectedWindow.DTP;
                    dtp.opacity = VirtualDesktopManager.Ui.ThumbDragSourceOpacity;
                    DwmApi.DwmUpdateThumbnailProperties( _selectedWindow.Thumb, ref dtp );
                }

                _dw.Left = Cursor.Position.X - _dw.Width / 2;
                _dw.Top = Cursor.Position.Y - _dw.Height / 2;
            }
            else
            {
                var vdw = sender as Form;
                vdw.Left = e.X + vdw.Left - _startPoint.X;
                vdw.Top = e.Y + vdw.Top - _startPoint.Y;
            }
        }

        private void VirtualDesktopWindow_MouseUp( object sender, MouseEventArgs e )
        {
            if ( null == sender ) return;

            _hoverVdIndex = HoverOnDesktop( sender, e );
            if ( _hoverVdIndex < 0 ) return;

            if ( _dragState > 0 )
            {
                if ( _selectedWindow != null ) // if we drag a thumbnail in a virtual desktop
                {
                    while ( true )
                    {
                        _virtualDesktops[_hoverVdIndex].Opacity = 1; // reset hover virtual desktop opacity unconditionally

                        if ( _hoverVdIndex == VdIndex ||
                             DesktopWrapper.IsWindowPinned( _selectedWindow.Handle ) ||
                             DesktopWrapper.IsApplicationPinned( _selectedWindow.Handle ) )
                        {
                            //////////////////////////
                            // goes here means no need to move the dragged window
                            var dtp = _selectedWindow.DTP;
                            dtp.opacity = 255;
                            DwmApi.DwmUpdateThumbnailProperties( _selectedWindow.Thumb, ref dtp );
                            break;
                        }

                        if ( User32.IsWindow( _selectedWindow.Handle ) )
                        {
                            ///////////////////////////
                            // goes here means the thumbnail window we dragged is drop in another virtual desktop
                            // we need to move it.
                            Logger.Verbose( $"DROP.Win {_selectedWindow.Title}({_selectedWindow.Handle.ToString( "X2" )}) IN Desktop[{_hoverVdIndex.ToString()}]" );

                            var sysIndex = DesktopWrapper.IndexFromGuid( _virtualDesktops[_hoverVdIndex].VdId );
                            DesktopWrapper.MoveWindowToDesktop( _selectedWindow.Handle, sysIndex );

                            var relevantVirtualDesktops = new List<VirtualDesktopWindow>
                            {
                                _virtualDesktops[_hoverVdIndex],
                                this
                            };
                            VirtualDesktopManager.ShowVisibleWindowsForDesktops( relevantVirtualDesktops );
                        }

                        break;
                    }
                }
                else // if we drag a virtual desktop
                {
                    if ( _hoverVdIndex == VdIndex )
                    {
                        Location = _fixedPosition;
                    }
                    else
                    {
                        Swap( ConfigManager.CurrentProfile.DesktopOrder, VdIndex, _hoverVdIndex );

                        VirtualDesktopManager.SaveOrder();

                        Logger.Verbose( $"SWAP.Desktop Desktop[{VdIndex.ToString()}] WITH Desktop[{_hoverVdIndex.ToString()}]" );
                        VirtualDesktopManager.FixLayout();
                        VirtualDesktopManager.ShowAllVirtualDesktops();
                        User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
                        User32.PostMessage( _virtualDesktops[_hoverVdIndex].Handle, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
                    }
                }
            }
            else
            {
                //////////////////////////////////
                // goes here means a Click
                if ( _selectedWindow != null && User32.IsWindow( _selectedWindow.Handle ) ) // click on a thumbnail
                {
                    void ActiveWindow()
                    {
                        Logger.Verbose( $"ACTIVE.Win {_selectedWindow.Title}({_selectedWindow.Handle.ToString( "X2" )})" );
                        WindowTool.ActiveWindow( _selectedWindow.Handle, ConfigManager.CurrentProfile.DesktopOrder[_hoverVdIndex] );
                    }

                    var action = Manager.Configs.GetMouseActionById( Const.MouseAction.GetActionId( e.Button, ModifierKeys, Const.MouseAction.MOUSE_NODE_WINDOW_PREFIX ) );
                    switch ( action )
                    {
                        case Const.MouseAction.Action.WindowActiveDesktopVisibleAndCloseView:
                            ActiveWindow();
                            MainWindow.HideAll();
                            break;
                        case Const.MouseAction.Action.WindowActiveDesktopVisibleOnly:
                            ActiveWindow();
                            break;
                        case Const.MouseAction.Action.WindowClose:
                            CloseSelectedWindow( _selectedWindow );
                            break;
                        case Const.MouseAction.Action.ContextMenu:
                            Menus.ThumbCtm( new MenuInfo
                            {
                                Vw = _selectedWindow,
                                Sender = sender,
                                Location = e.Location,
                                Self = this
                            } );
                            break;
                        case Const.MouseAction.Action.WindowHideFromView:
                            Filters.WndHandleIgnoreListByManual.TryAdd( _selectedWindow.Handle, 0 );
                            VirtualDesktopManager.RefreshThumbs( _selectedWindow.Handle, this );

                            break;
                        case Const.MouseAction.Action.WindowShowForSelectedProcessOnly:
                            try
                            {
                                _ = User32.GetWindowThreadProcessId( _selectedWindow.Handle, out var pId );
                                VirtualDesktopManager.ShowVisibleWindowsForDesktops( null, pId );
                            }
                            catch ( Exception ex )
                            {
                                Logger.Warning( "show windows from selected process: " + ex.Message );
                            }

                            break;
                        case Const.MouseAction.Action.WindowShowForSelectedProcessInSelectedDesktop:
                            try
                            {
                                _ = User32.GetWindowThreadProcessId( _selectedWindow.Handle, out var pId );
                                MakeTheOnlyOne( pId );
                            }
                            catch ( Exception ex )
                            {
                                Logger.Warning( "show windows from selected process: " + ex.Message );
                            }

                            break;
                        case Const.MouseAction.Action.DoNothing:
                            break;
                        default:
                            ActiveWindow();
                            MainWindow.HideAll();
                            break;
                    }
                }
                else // click on a virtual desktop
                {
                    var action = Manager.Configs.GetMouseActionById( Const.MouseAction.GetActionId( e.Button, ModifierKeys, Const.MouseAction.MOUSE_NODE_DESKTOP_PREFIX ) );
                    switch ( action )
                    {
                        case Const.MouseAction.Action.DesktopVisibleAndCloseView:
                            MakeVisible();
                            MainWindow.HideAll();
                            break;
                        case Const.MouseAction.Action.DesktopVisibleOnly:
                            MakeVisible();
                            break;
                        case Const.MouseAction.Action.ContextMenu:
                            Menus.VdCtm( new MenuInfo
                                {
                                    Sender = sender,
                                    Location = e.Location,
                                    Self = this,
                                    Vdws = _virtualDesktops
                                }
                            );
                            break;
                        case Const.MouseAction.Action.DesktopShowForSelectedDesktop:
                            MakeTheOnlyOne();

                            break;
                        case Const.MouseAction.Action.DoNothing:
                            break;
                        default:
                            MakeVisible();
                            MainWindow.HideAll();
                            break;
                    }
                }
            }

            VirtualDesktopManager.UpdateVdwBackground();

            if ( _dw != null )
            {
                DwmApi.DwmUnregisterThumbnail( _dw.Thumb );
                _dw.Close();
                _dw = null;
            }

            _dragState = 0;
            _selectedWindow = null;
            _dragBounds = Rectangle.Empty;
        }

        private int HoverOnDesktop( object sender, MouseEventArgs e )
        {
            if ( _virtualDesktops is null ) return -1;
            _hoverVdIndex = VdIndex;

            var cellIndex = MainWindow.InCell( new System.Windows.Point( Cursor.Position.X, Cursor.Position.Y ) );
            MainWindow.UpdateHoverBorder( cellIndex );
            foreach ( var vdw in _virtualDesktops )
            {
                var controlRectangle = vdw.RectangleToScreen( vdw.ClientRectangle );
                if ( controlRectangle.Contains( Cursor.Position ) )
                {
                    if ( vdw.VdIndex != VdIndex )
                    {
                        _hoverVdIndex = vdw.VdIndex;
                        if ( _selectedWindow != null )
                        {
                            if ( _dragState == 1 )
                            {
                                Logger.Verbose( $"DRAGGING.Win {_selectedWindow.Title} IN Desktop[{vdw.VdIndex.ToString()}]" );
                                _dragState++;
                            }

                            vdw.Opacity = VirtualDesktopManager.Ui.VDWDragTargetOpacity;
                        }
                        else
                        {
                            if ( _dragState == 1 )
                            {
                                Logger.Verbose( $"DRAGGING.Desk Desktop[{VdIndex.ToString()}]) ON Desktop[{vdw.VdIndex.ToString()}])" );
                                _dragState++;
                            }
                        }
                    }
                }
                else
                {
                    vdw.Opacity = 1;
                }
            }

            return _hoverVdIndex;
        }

        private void MakeVisible()
        {
            Logger.Verbose( $"SWITCH TO DESKTOP Desktop[{_hoverVdIndex.ToString()}]" );
            DesktopWrapper.MakeVisibleByGuid( VdId );
        }

        private static void Swap<T>( IList<T> list, int indexA, int indexB )
        {
            ( list[indexA], list[indexB] ) = ( list[indexB], list[indexA] );
        }

        public async void CloseSelectedWindow( VisibleWindow vw )
        {
            var isWindowPinned = DesktopWrapper.IsWindowPinned( vw.Handle ) || DesktopWrapper.IsApplicationPinned( vw.Handle );

            void RefreshVDs( bool isPinned )
            {
                if ( isPinned )
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops();
                }
                else
                {
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {this} );
                }
            }

            // _ = User32.ShowWindow( vw.Handle, 0 );

            User32.PostMessage( vw.Handle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );

            await Task.Run( () =>
            {
                var sw = Stopwatch.StartNew();
                while ( sw.ElapsedMilliseconds < Const.WindowCloseTimeout )
                {
                    Thread.Sleep( 100 );
                    // if ( User32.IsWindow( vw.Handle ) ) continue; // 严格的判断
                    if ( User32.IsWindowVisible( vw.Handle ) ) continue; // 宽松的判断

                    RefreshVDs( isWindowPinned );
                    return;
                }

                RefreshVDs( isWindowPinned );
            } ).ConfigureAwait( false );
        }

        public void MakeTheOnlyOne( int pId = 0 )
        {
            if ( _isTheOnlyOneInMainView )
            {
                MainWindow.ResetMainGrid();
                VirtualDesktopManager.HideAllVirtualDesktops();
                VirtualDesktopManager.ShowAllVirtualDesktops();
                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            }
            else
            {
                MainWindow.ResetMainGridForSingleDesktop( VdIndex );
                VirtualDesktopManager.HideAllVirtualDesktops();
                _isTheOnlyOneInMainView = true;
                VirtualDesktopManager.ShowAllVirtualDesktops();
                VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {this}, pId );
            }
        }
    }
}