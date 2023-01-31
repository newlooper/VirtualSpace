/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    public partial class VirtualDesktopWindow
    {
        private void VirtualDesktopWindow_MouseDown( object sender, MouseEventArgs e )
        {
            _virtualDesktops = VirtualDesktopManager.GetAllVirtualDesktops();
            _startPoint = e.Location;
            var dragSize = SystemInformation.DragSize * ConfigManager.CurrentProfile.Mouse.DragSizeFactor;
            _dragBounds = new Rectangle(
                new Point( _startPoint.X - dragSize.Width / 2, _startPoint.Y - dragSize.Height / 2 ),
                dragSize );
            foreach ( var window in _visibleWindows )
            {
                if ( window.Rect.Contains( e.Location ) )
                {
                    Logger.Debug( "SELECT.Win " + window.Title );
                    _selectedWindow = window;
                    break;
                }
            }
        }

        private bool IsOutBounds( Point location )
        {
            return _dragBounds != Rectangle.Empty && !_dragBounds.Contains( location );
        }

        private void VirtualDesktopWindow_MouseMove( object sender, MouseEventArgs e )
        {
            if ( !_dragging && e.Button == MouseButtons.Left && IsOutBounds( e.Location ) )
            {
                _dragging = true;
            }

            if ( _dragging )
            {
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
        }

        private void VirtualDesktopWindow_MouseUp( object sender, MouseEventArgs e )
        {
            if ( null == sender ) return;

            _hoverVdIndex = HoverOnDesktop( sender, e );

            if ( _dragging )
            {
                if ( _selectedWindow != null ) // if we drag a thumbnail in a virtual desktop
                {
                    while ( true )
                    {
                        _virtualDesktops[_hoverVdIndex].Opacity = 1; // reset hover virtual desktop opacity unconditionally

                        if ( _hoverVdIndex == VdIndex ||
                             DesktopWrapper.IsWindowPinned( _selectedWindow.Handle ) ||
                             DesktopWrapper.IsApplicationPinned( _selectedWindow.Handle )
                           )
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
                            Logger.Debug( $"DROP.Win {_selectedWindow.Title}({_selectedWindow.Handle.ToString( "X2" )}) IN Desktop[{_hoverVdIndex}]" );

                            var sysIndex = DesktopWrapper.IndexFromGuid( _virtualDesktops[_hoverVdIndex].VdId );
                            DesktopWrapper.MoveWindowToDesktop( _selectedWindow.Handle, sysIndex );
                            // if ( _selectedWindow.CoreUiWindowHandle != default )
                            // {
                            //     DesktopWrapper.MoveWindowToDesktop( _selectedWindow.CoreUiWindowHandle, sysIndex );
                            //     User32.SetParent( _selectedWindow.CoreUiWindowHandle, _selectedWindow.Handle );
                            // }

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

                        Logger.Debug( $"SWAP.Desk Desktop[{VdIndex}] WITH Desktop[{_hoverVdIndex}]" );
                        VirtualDesktopManager.FixLayout();
                        VirtualDesktopManager.ShowAllVirtualDesktops();
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
                        Logger.Debug( $"ACTIVE.Win {_selectedWindow.Title}({_selectedWindow.Handle.ToString( "X2" )})" );
                        Logger.Debug( $"CHANGE CURRENT DESKTOP TO Desktop[{_hoverVdIndex}]" );
                        DesktopWrapper.MakeVisibleByGuid( ConfigManager.CurrentProfile.DesktopOrder[_hoverVdIndex] );
                        User32.SwitchToThisWindow( _selectedWindow.Handle, true );
                    }

                    void WindowAction( Const.MouseAction.Action action )
                    {
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
                            case Const.MouseAction.Action.DoNothing:
                                break;
                            default:
                                ActiveWindow();
                                MainWindow.HideAll();
                                break;
                        }
                    }

                    switch ( e.Button )
                    {
                        case MouseButtons.Left:
                            WindowAction( ConfigManager.Configs.MouseActions[Const.MouseAction.WINDOW_LEFT_CLICK] );
                            break;
                        case MouseButtons.Middle:
                            WindowAction( ConfigManager.Configs.MouseActions[Const.MouseAction.WINDOW_MIDDLE_CLICK] );
                            break;
                        case MouseButtons.Right:
                            WindowAction( ConfigManager.Configs.MouseActions[Const.MouseAction.WINDOW_RIGHT_CLICK] );
                            break;
                    }
                }
                else // click on a virtual desktop
                {
                    void DesktopAction( Const.MouseAction.Action action )
                    {
                        switch ( action )
                        {
                            case Const.MouseAction.Action.DesktopVisibleAndCloseView:
                                SwitchDesktop();
                                MainWindow.HideAll();
                                break;
                            case Const.MouseAction.Action.DesktopVisibleOnly:
                                SwitchDesktop();
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
                            case Const.MouseAction.Action.DoNothing:
                                break;
                            default:
                                SwitchDesktop();
                                MainWindow.HideAll();
                                break;
                        }
                    }

                    switch ( e.Button )
                    {
                        case MouseButtons.Left:
                            DesktopAction( ConfigManager.Configs.MouseActions[Const.MouseAction.DESKTOP_LEFT_CLICK] );
                            break;
                        case MouseButtons.Middle:
                            DesktopAction( ConfigManager.Configs.MouseActions[Const.MouseAction.DESKTOP_MIDDLE_CLICK] );
                            break;
                        case MouseButtons.Right:
                            DesktopAction( ConfigManager.Configs.MouseActions[Const.MouseAction.DESKTOP_RIGHT_CLICK] );
                            break;
                    }
                }
            }

            VirtualDesktopManager.ResetAllBackground();
            if ( _dw != null )
            {
                DwmApi.DwmUnregisterThumbnail( _dw.Thumb );
                _dw.Close();
                _dw = null;
            }

            _dragging = false;
            _selectedWindow = null;
            _dragBounds = Rectangle.Empty;
        }

        private int HoverOnDesktop( object sender, MouseEventArgs e )
        {
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
                            Logger.Debug( $"DRAGGING.Win {_selectedWindow.Title} IN Desktop[{vdw.VdIndex}]" );
                            vdw.Opacity = VirtualDesktopManager.Ui.VDWDragTargetOpacity;
                        }
                        else
                        {
                            Logger.Debug( $"DRAGGING.Desk Desktop[{VdIndex}]) ON Desktop[{vdw.VdIndex}])" );
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

        private void SwitchDesktop()
        {
            Logger.Debug( $"SWITCH TO DESKTOP Desktop[{_hoverVdIndex}]" );
            DesktopWrapper.MakeVisibleByGuid( VdId );
        }

        private static void Swap<T>( IList<T> list, int indexA, int indexB )
        {
            ( list[indexA], list[indexB] ) = ( list[indexB], list[indexA] );
        }

        public async void CloseSelectedWindow( VisibleWindow vw )
        {
            User32.ShowWindow( vw.Handle, 0 );

            User32.PostMessage( vw.Handle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );

            await Task.Run( () =>
            {
                var sw = Stopwatch.StartNew();
                while ( sw.ElapsedMilliseconds < Const.WindowCloseTimeout )
                {
                    Thread.Sleep( 100 );
                    // if ( User32.IsWindow( vw.Handle ) ) continue; // 严格的判断
                    if ( User32.IsWindowVisible( vw.Handle ) ) continue; // 宽松的判断
                    VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {this} );
                    return;
                }

                VirtualDesktopManager.ShowVisibleWindowsForDesktops( new List<VirtualDesktopWindow> {this} );
            } );
        }
    }
}