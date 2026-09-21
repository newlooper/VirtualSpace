/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.Tools;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;
using Cursor = System.Windows.Forms.Cursor;
using MouseAction = VirtualSpace.Config.MouseAction;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using WinFormsKeys = System.Windows.Forms.Keys;
using WinFormsMouseButtons = System.Windows.Forms.MouseButtons;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    private static int                  _hoverVdIndex;
    private static Point                _startPoint;
    private static System.Windows.Point _startDip;
    private static int                  _dragState;
    private static Rectangle            _dragBounds = Rectangle.Empty;
    private static VisibleWindow?       _selectedWindow;
    private static DragWindow?          _dw;

    private void VirtualDesktopWindow_MouseDown( object sender, MouseButtonEventArgs e )
    {
        _surface.CaptureMouse();
        _virtualDesktops = VirtualDesktopManager.GetAllVirtualDesktops();
        _startDip        = e.GetPosition( _surface );
        _startPoint      = ToPixel( _startDip );
        var dragSize = SystemInformation.DragSize * ConfigManager.CurrentProfile.Mouse.DragSizeFactor;
        _dragBounds = new Rectangle(
            new Point( _startPoint.X - dragSize.Width / 2, _startPoint.Y - dragSize.Height / 2 ),
            dragSize );

        _selectedWindow = _visibleWindows.FirstOrDefault( w => w.Rect.Contains( _startPoint ) );
        if ( _selectedWindow != null )
        {
            Logger.Verbose( $"SELECT.Win {_selectedWindow.Title}" );
        }
    }

    private static bool IsOutBounds( Point location )
    {
        return _dragBounds != Rectangle.Empty && !_dragBounds.Contains( location );
    }

    private void VirtualDesktopWindow_MouseMove( object sender, MouseEventArgs e )
    {
        var dip   = e.GetPosition( _surface );
        var pixel = ToPixel( dip );

        if ( _dragState == 0 && e.LeftButton == MouseButtonState.Pressed && IsOutBounds( pixel ) )
        {
            _dragState = 1;
            if ( _selectedWindow is null )
            {
                ShrinkPrimaryToSmallCellForDrag();
            }
        }

        if ( _dragState == 0 )
        {
            return;
        }

        HoverOnDesktop();

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
                        fVisible      = true,
                        dwFlags       = DwmApi.DWM_TNP_VISIBLE | DwmApi.DWM_TNP_RECTDESTINATION | DwmApi.DWM_TNP_OPACITY,
                        opacity       = byte.MaxValue,
                        rcDestination = new RECT( 0, 0, _dw.PixelWidth, _dw.PixelHeight )
                    };
                    _dw.Thumb = thumb;
                    UpdateThumbnail( _dw.Thumb, props );
                }

                var dtp = _selectedWindow.DTP;
                dtp.opacity = VirtualDesktopManager.Ui.ThumbDragSourceOpacity;
                _           = DwmApi.DwmUpdateThumbnailProperties( _selectedWindow.Thumb, ref dtp );
            }

            _dw.MoveCenterToScreenPixel( Cursor.Position.X, Cursor.Position.Y );
        }
        else
        {
            Left = dip.X + Left - _startDip.X;
            Top  = dip.Y + Top - _startDip.Y;
        }
    }

    private void VirtualDesktopWindow_MouseUp( object sender, MouseButtonEventArgs e )
    {
        try
        {
            HandleMouseUp( ToWinFormsButton( e.ChangedButton ), ToPixel( e.GetPosition( _surface ) ) );
        }
        catch ( Exception ex )
        {
            CultureInfo.CurrentUICulture = new CultureInfo( ConfigManager.CurrentProfile.UI.Language );
            Logger.Error(
                $"MouseUp: {ex.Message}",
                new NotifyObject
                {
                    Title   = Agent.Langs.GetString( "Error.Title" )!,
                    Message = ex.Message
                } );
        }
        finally
        {
            if ( _surface.IsMouseCaptured )
            {
                _surface.ReleaseMouseCapture();
            }

            VirtualDesktopManager.UpdateVdwBorder();

            if ( _dw != null )
            {
                _ = DwmApi.DwmUnregisterThumbnail( _dw.Thumb );
                _dw.Close();
                _dw = null;
            }

            _dragState      = 0;
            _selectedWindow = null;
            _dragBounds     = Rectangle.Empty;
        }
    }

    private void HandleMouseUp( WinFormsMouseButtons button, Point location )
    {
        _hoverVdIndex = HoverOnDesktop();
        if ( _hoverVdIndex < 0 )
        {
            return;
        }

        if ( _dragState > 0 )
        {
            if ( _selectedWindow != null ) // if we drag a thumbnail in a virtual desktop
            {
                while ( true )
                {
                    _virtualDesktops![_hoverVdIndex].Opacity = 1; // reset hover virtual desktop opacity unconditionally

                    if ( _hoverVdIndex == VdIndex ||
                         DesktopWrapper.IsWindowPinned( _selectedWindow.Handle ) ||
                         DesktopWrapper.IsApplicationPinned( _selectedWindow.Handle ) )
                    {
                        //////////////////////////
                        // goes here means no need to move the dragged window
                        var dtp = _selectedWindow.DTP;
                        dtp.opacity = byte.MaxValue;
                        _           = DwmApi.DwmUpdateThumbnailProperties( _selectedWindow.Thumb, ref dtp );
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
                var cursor = Cursor.Position;
                var slot   = MainWindow.InCell( new System.Windows.Point( cursor.X, cursor.Y ) );
                if ( ConfigManager.CurrentProfile.IsPsLayout )
                {
                    if ( slot < 0 && _hoverVdIndex != VdIndex && _virtualDesktops is not null )
                    {
                        slot = VirtualDesktopManager.GetMatrixIndexByVdIndex( _hoverVdIndex );
                    }

                    if ( VirtualDesktopManager.RelocateDesktop( VdId, slot ) )
                    {
                        Logger.Verbose( $"PLACE.Desktop Desktop[{VdIndex}] ON slot[{slot}]" );
                    }
                    else
                    {
                        PlaceInCell();
                    }
                }
                else if ( _hoverVdIndex == VdIndex )
                {
                    PlaceInCell();
                }
                else
                {
                    ConfigManager.CurrentProfile.DesktopOrder?.Swap( VdIndex, _hoverVdIndex );

                    VirtualDesktopManager.SaveOrder();

                    Logger.Verbose( $"SWAP.Desktop Desktop[{VdIndex}] WITH Desktop[{_hoverVdIndex}]" );
                    VirtualDesktopManager.FixLayout();
                    VirtualDesktopManager.ShowAllVirtualDesktops();
                    User32.PostMessage( Handle, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
                    User32.PostMessage( _virtualDesktops![_hoverVdIndex].Handle, WinMsg.WM_HOTKEY, UserMessage.RefreshVdw, 0 );
                }
            }
        }
        else
        {
            //////////////////////////////////
            // goes here means a Click
            if ( _selectedWindow != null && User32.IsWindow( _selectedWindow.Handle ) ) // click on a thumbnail
            {
                void ActivateWindow()
                {
                    Logger.Verbose( $"ACTIVATE.Win {_selectedWindow!.Title}({_selectedWindow.Handle:X2})" );
                    if ( User32.IsIconic( _selectedWindow.Handle ) )
                    {
                        _ = DwmApi.DwmUnregisterThumbnail( _selectedWindow.Thumb );
                    }

                    WindowTool.ActivateWindow( _selectedWindow.Handle, ConfigManager.CurrentProfile.DesktopOrder![_hoverVdIndex], "MouseClick.ActivateWindow" );
                }

                var action = ConfigManager.Configs.GetMouseActionById(
                    MouseAction.GetActionId( button, CurrentModifierKeys(), MouseAction.MOUSE_NODE_WINDOW_PREFIX ) );
                switch ( action )
                {
                    case MouseAction.Action.WindowActiveDesktopVisibleAndCloseView:
                        ActivateWindow();
                        MainWindow.HideAll();
                        break;
                    case MouseAction.Action.WindowActiveDesktopVisibleOnly:
                        ActivateWindow();
                        VirtualDesktopManager.RefreshThumbs( _selectedWindow.Handle, this );
                        break;
                    case MouseAction.Action.WindowClose:
                        CloseSelectedWindow( _selectedWindow );
                        break;
                    case MouseAction.Action.ContextMenu:
                        Menus.ThumbCtm( new MenuInfo
                        {
                            Vw       = _selectedWindow,
                            Sender   = this,
                            Location = location,
                            Self     = this
                        } );
                        break;
                    case MouseAction.Action.WindowHideFromView:
                        ImmutableInterlocked.Update( ref Filters.WndHandleIgnoreListByManual, list => list.Add( _selectedWindow.Handle ) );
                        VirtualDesktopManager.RefreshThumbs( _selectedWindow.Handle, this );

                        break;
                    case MouseAction.Action.WindowShowForSelectedProcessOnly:
                        try
                        {
                            _ = User32.GetWindowThreadProcessId( _selectedWindow.Handle, out var pId );
                            VirtualDesktopManager.ShowVisibleWindowsForDesktops( null, pId );
                        }
                        catch ( Exception ex )
                        {
                            Logger.Warning( $"show windows from selected process: {ex.Message}" );
                        }

                        break;
                    case MouseAction.Action.WindowShowForSelectedProcessInSelectedDesktop:
                        try
                        {
                            _ = User32.GetWindowThreadProcessId( _selectedWindow.Handle, out var pId );
                            MakeTheOnlyOne( pId );
                        }
                        catch ( Exception ex )
                        {
                            Logger.Warning( $"show windows from selected process: {ex.Message}" );
                        }

                        break;
                    case MouseAction.Action.DoNothing:
                        break;
                    default:
                        ActivateWindow();
                        MainWindow.HideAll();
                        break;
                }
            }
            else // click on a virtual desktop
            {
                var action = ConfigManager.Configs.GetMouseActionById(
                    MouseAction.GetActionId( button, CurrentModifierKeys(), MouseAction.MOUSE_NODE_DESKTOP_PREFIX ) );
                switch ( action )
                {
                    case MouseAction.Action.DesktopVisibleAndCloseView:
                        MakeVisible();
                        MainWindow.HideAll();
                        break;
                    case MouseAction.Action.DesktopVisibleOnly:
                        MakeVisible();
                        break;
                    case MouseAction.Action.ContextMenu:
                        Menus.VdCtm( new MenuInfo
                            {
                                Sender   = this,
                                Location = location,
                                Self     = this,
                                Vdws     = _virtualDesktops!
                            }
                        );
                        break;
                    case MouseAction.Action.DesktopShowForSelectedDesktop:
                        MakeTheOnlyOne();

                        break;
                    case MouseAction.Action.DoNothing:
                        break;
                    default:
                        MakeVisible();
                        MainWindow.HideAll();
                        break;
                }
            }
        }
    }

    private void ShrinkPrimaryToSmallCellForDrag()
    {
        if ( !VirtualDesktopManager.IsPsPrimaryDesktop( VdId ) )
        {
            return;
        }

        var ui   = VirtualDesktopManager.Ui;
        var size = MainWindow.GetCellSizeByMatrixIndex( 1 );
        var w    = size.Width - 2 * ui.VDWBorderSize;
        var h    = size.Height - 2 * ui.VDWBorderSize;
        if ( w <= 0 || h <= 0 )
        {
            return;
        }

        var maxS = VdwShadow.MaxDip;
        var winW = size.Width + 2 * maxS;
        var winH = size.Height + 2 * maxS;
        SetSurfaceSize( winW, winH );

        var (scaleX, scaleY) = SysInfo.Dpi;
        var cursor = Cursor.Position;
        Left      = cursor.X / scaleX - winW / 2;
        Top       = cursor.Y / scaleY - winH / 2;
        _startDip = new System.Windows.Point( winW / 2, winH / 2 );
    }

    private int HoverOnDesktop()
    {
        if ( _virtualDesktops is null )
        {
            return -1;
        }

        _hoverVdIndex = VdIndex;

        var cursor    = Cursor.Position;
        var cellIndex = MainWindow.InCell( new System.Windows.Point( cursor.X, cursor.Y ) );
        MainWindow.UpdateHoverBorder( cellIndex );
        foreach ( var vdw in _virtualDesktops )
        {
            var controlRectangle = vdw.PixelBoundsOnScreen;
            if ( controlRectangle.Contains( cursor ) )
            {
                if ( vdw.VdIndex == VdIndex )
                {
                    continue;
                }

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
                    if ( _dragState != 1 )
                    {
                        continue;
                    }

                    Logger.Verbose( $"DRAGGING.Desk Desktop[{VdIndex.ToString()}]) ON Desktop[{vdw.VdIndex.ToString()}])" );
                    _dragState++;
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
        Logger.Verbose( $"Try Switch To Desktop[{_hoverVdIndex.ToString()}]" );
        DesktopWrapper.MakeVisibleByGuid( VdId );
    }

    public async void CloseSelectedWindow( VisibleWindow vw )
    {
        var isWindowPinned = DesktopWrapper.IsWindowPinned( vw.Handle ) || DesktopWrapper.IsApplicationPinned( vw.Handle );

        User32.PostMessage( vw.Handle, WinMsg.WM_SYSCOMMAND, WinMsg.SC_CLOSE, 0 );

        await Task.Run( () =>
        {
            var sw = Stopwatch.StartNew();
            while ( sw.ElapsedMilliseconds < Const.WindowCloseTimeout )
            {
                Thread.Sleep( 100 );
                if ( !User32.IsWindowVisible( vw.Handle ) )
                {
                    break;
                }
            }
        } ).ConfigureAwait( false );

        if ( _surface.Dispatcher.CheckAccess() )
        {
            DoRefresh();
        }
        else
        {
            _surface.Dispatcher.Invoke( DoRefresh );
        }

        return;

        void RefreshVDs( bool isPinned )
        {
            if ( isPinned )
            {
                VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            }
            else
            {
                VirtualDesktopManager.ShowVisibleWindowsForDesktops( [this] );
            }
        }

        void DoRefresh()
        {
            RefreshVDs( isWindowPinned );
        }
    }

    public void MakeTheOnlyOne( int pId = 0 )
    {
        if ( VirtualDesktopManager.TryGetOnlyOneInMainView( out var current ) && current == this )
        {
            VirtualDesktopManager.ClearOnlyOneInMainView();
            MainWindow.ResetMainGrid();
            VirtualDesktopManager.HideAllVirtualDesktops();
            VirtualDesktopManager.ShowAllVirtualDesktops();
            VirtualDesktopManager.ShowVisibleWindowsForDesktops();
            return;
        }

        VirtualDesktopManager.HideAllVirtualDesktops();
        VirtualDesktopManager.SetOnlyOneInMainView( this );
        MainWindow.ResetMainGrid();
        VirtualDesktopManager.ShowAllVirtualDesktops();
        VirtualDesktopManager.ShowVisibleWindowsForDesktops( [this], pId );
    }

    private static Point ToPixel( System.Windows.Point dip )
    {
        var (scaleX, scaleY) = SysInfo.Dpi;
        return new Point( (int)Math.Round( dip.X * scaleX ), (int)Math.Round( dip.Y * scaleY ) );
    }

    private static WinFormsMouseButtons ToWinFormsButton( MouseButton button )
    {
        return button switch
        {
            MouseButton.Left => WinFormsMouseButtons.Left,
            MouseButton.Right => WinFormsMouseButtons.Right,
            MouseButton.Middle => WinFormsMouseButtons.Middle,
            MouseButton.XButton1 => WinFormsMouseButtons.XButton1,
            MouseButton.XButton2 => WinFormsMouseButtons.XButton2,
            _ => WinFormsMouseButtons.None
        };
    }

    private static WinFormsKeys CurrentModifierKeys()
    {
        var keys = WinFormsKeys.None;
        var mods = Keyboard.Modifiers;
        if ( ( mods & ModifierKeys.Control ) != 0 )
        {
            keys |= WinFormsKeys.Control;
        }

        if ( ( mods & ModifierKeys.Alt ) != 0 )
        {
            keys |= WinFormsKeys.Alt;
        }

        if ( ( mods & ModifierKeys.Shift ) != 0 )
        {
            keys |= WinFormsKeys.Shift;
        }

        return keys;
    }
}