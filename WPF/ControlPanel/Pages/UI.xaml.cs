/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ControlPanel.Resources;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace ControlPanel.Pages;

public partial class UI
{
    private static UI?         _instance;
    private static UIViewModel _vm = null!;

    private PsKind   _psKind   = PsKind.Corner;
    private PsCorner _psCorner = PsCorner.NorthWest;
    private int      _psN      = 3;
    private int      _psExtra;
    private Button?  _psSelected;

    public UI()
    {
        InitializeComponent();
    }

    private UI( string headerKey, PackIconKind iconKind ) : this()
    {
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind  = iconKind;
        _vm           = new UIViewModel();
        DataContext   = _vm;
        ButtonsContainer.AddHandler( ButtonBase.ClickEvent, new RoutedEventHandler( OnVdArrangementButtonClicked ) );
        _vm.PropertyChanged += OnViewModelPropertyChanged;
        IsVisibleChanged += ( _, e ) =>
        {
            if ( e.NewValue is true )
            {
                RefreshLayoutUi();
            }
        };
        RefreshLayoutUi();
        DesktopManagerWrapper.DesktopCreatedEvent -= OnVdCountChanged;
        DesktopManagerWrapper.DesktopCreatedEvent += OnVdCountChanged;
        DesktopManagerWrapper.DesktopDeletedEvent -= OnVdDeleted;
        DesktopManagerWrapper.DesktopDeletedEvent += OnVdDeleted;
    }

    private static void OnVdArrangementButtonClicked( object sender, RoutedEventArgs e )
    {
        if ( e.OriginalSource is not Button btn || btn.Tag is null )
        {
            return;
        }

        var newVdA = int.Parse( btn.Tag.ToString()! );

        if ( Manager.CurrentProfile.IsPsLayout )
        {
            Manager.CurrentProfile.Layout = LayoutKind.Auto;
            if ( _vm.VdArrangement == newVdA )
            {
                Manager.Save( reason: "apply automatic layout", reasonName: "Layout", logLevel: LogLevel.Trace );
            }
            else
            {
                Logger.Verbose( "apply automatic layout." );
            }
        }

        _vm.VdArrangement = newVdA;
        User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.DesktopArrangement, 0 );
        e.Handled = true;
    }

    public static UI Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new UI( headerKey, iconKind );
    }

    public static void SyncLayoutSelection()
    {
        if ( _instance is null )
        {
            return;
        }

        if ( _instance.Dispatcher.CheckAccess() )
        {
            _instance.SyncActiveTab();
        }
        else
        {
            _instance.Dispatcher.BeginInvoke( static () => _instance!.SyncActiveTab() );
        }
    }

    private void ApplyPsLayout()
    {
        var ps = Manager.CurrentProfile.PrimarySecondary ??= new PrimarySecondaryState();
        ps.Kind                       = _psKind;
        ps.Corner                     = _psCorner;
        ps.N                          = _psN;
        ps.Slots                      = [];
        Manager.CurrentProfile.Layout = LayoutKind.PrimarySecondary;
        var extras = PsCreateExtrasCheck.IsChecked == true && _psExtra > 0;
        User32.PostMessage( MainWindow.MainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.ApplyPsLayout, (ulong)( extras ? 1 : 0 ) );
    }

    private void OnViewModelPropertyChanged( object? sender, System.ComponentModel.PropertyChangedEventArgs e )
    {
        if ( e.PropertyName == nameof( UIViewModel.VdCount ) )
        {
            RebuildPsPreviews();
        }
    }

    private void RefreshLayoutUi()
    {
        SyncActiveTab();
        RebuildPsPreviews();
    }

    private void SyncActiveTab()
    {
        var ps = Manager.CurrentProfile.IsPsLayout;
        _vm.IsPsLayout           = ps;
        _vm.IsEqualLayout        = !ps;
        LayoutTabs.SelectedIndex = ps ? 1 : 0;
    }

    private void OnVdCountChanged()
    {
        if ( Dispatcher.CheckAccess() )
        {
            RefreshLayoutUi();
        }
        else
        {
            Dispatcher.BeginInvoke( RefreshLayoutUi );
        }
    }

    private void OnVdDeleted( VirtualSpace.Commons.VirtualDesktopNotification _ )
    {
        OnVdCountChanged();
    }

    private void RebuildPsPreviews()
    {
        var count = DesktopWrapper.Count;
        var (kind, n, k) = PsLayout.Fit( count );
        _psKind          = kind;
        _psN             = n;
        _psExtra         = k - count;
        if ( _psExtra < 0 )
        {
            _psExtra = 0;
        }

        PsHintText.Text = string.Format( Langs.Page_UI_PsLayout_Hint, k, count );
        if ( _psExtra > 0 )
        {
            PsCreateExtrasCheck.Visibility = Visibility.Visible;
            PsCreateExtrasCheck.Content    = string.Format( Langs.Page_UI_PsLayout_CreateExtras, _psExtra );
        }
        else
        {
            PsCreateExtrasCheck.Visibility = Visibility.Collapsed;
        }

        PsCreateExtrasCheck.IsChecked = false;

        PsPreviewHost.Children.Clear();
        _psSelected = null;
        if ( _psKind == PsKind.Center )
        {
            AddPsPreview( PsCorner.NorthWest, Langs.Page_UI_PsLayout_Center );
        }
        else
        {
            AddPsPreview( PsCorner.NorthWest, Langs.Page_UI_PsLayout_CornerNW );
            AddPsPreview( PsCorner.NorthEast, Langs.Page_UI_PsLayout_CornerNE );
            AddPsPreview( PsCorner.SouthWest, Langs.Page_UI_PsLayout_CornerSW );
            AddPsPreview( PsCorner.SouthEast, Langs.Page_UI_PsLayout_CornerSE );
        }

        var current = Manager.CurrentProfile.PrimarySecondary;
        if ( current is { Kind: not PsKind.None } && current.Kind == _psKind )
        {
            foreach ( var btn in PsPreviewButtons() )
            {
                if ( btn.Tag is PsPreviewItem item && item.Corner == current.Corner )
                {
                    SelectPsPreview( btn, item.Corner );
                    break;
                }
            }
        }
    }

    private IEnumerable<Button> PsPreviewButtons()
    {
        foreach ( var child in PsPreviewHost.Children )
        {
            if ( child is Panel panel )
            {
                foreach ( var inner in panel.Children )
                {
                    if ( inner is Button btn )
                    {
                        yield return btn;
                    }
                }
            }
            else if ( child is Button btn )
            {
                yield return btn;
            }
        }
    }

    private void AddPsPreview( PsCorner corner, string caption )
    {
        var (preview, cells) = BuildPsPreview( _psKind, _psN, corner );
        var btn = new Button
        {
            Width   = 148,
            Height  = 148,
            Padding = new Thickness( 8 ),
            Tag     = new PsPreviewItem { Corner = corner, Cells = cells },
            Cursor  = Cursors.Hand,
            Content = preview
        };
        if ( TryFindResource( "MaterialDesignOutlinedButton" ) is Style outlined )
        {
            btn.Style = outlined;
        }

        var label = new TextBlock
        {
            Text                = caption,
            TextAlignment       = TextAlignment.Center,
            TextWrapping        = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin              = new Thickness( 4, 8, 4, 0 ),
            MaxWidth            = 148,
            FontSize            = 12
        };
        label.SetResourceReference( TextBlock.ForegroundProperty, "MaterialDesignBody" );

        btn.Click += ( _, _ ) =>
        {
            SelectPsPreview( btn, corner );
            ApplyPsLayout();
        };

        var card = new StackPanel
        {
            Width               = 164,
            Margin              = new Thickness( 8 ),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        card.Children.Add( btn );
        card.Children.Add( label );
        PsPreviewHost.Children.Add( card );

        if ( _psSelected is null )
        {
            SelectPsPreview( btn, corner );
        }
    }

    private void SelectPsPreview( Button btn, PsCorner corner )
    {
        if ( _psSelected is not null )
        {
            ApplyPreviewSelection( _psSelected, false );
        }

        _psSelected = btn;
        _psCorner   = corner;
        ApplyPreviewSelection( btn, true );
    }

    private static void ApplyPreviewSelection( Button btn, bool selected )
    {
        if ( btn.Tag is not PsPreviewItem item )
        {
            return;
        }

        foreach ( var cell in item.Cells )
        {
            if ( selected )
            {
                cell.Background = new SolidColorBrush( Colors.Pink );
            }
            else
            {
                cell.SetResourceReference( Border.BackgroundProperty, "MaterialDesignBody" );
            }
        }
    }

    private static (UIElement Preview, List<Border> Cells) BuildPsPreview( PsKind kind, int n, PsCorner corner )
    {
        var grid  = new Grid { Width = 140, Height = 140 };
        var cells = new List<Border>();
        for ( var i = 0; i < n; i++ )
        {
            grid.RowDefinitions.Add( new RowDefinition() );
            grid.ColumnDefinitions.Add( new ColumnDefinition() );
        }

        var (pr, pc, span) = PsLayout.PrimaryRect( kind, n, corner );
        var primary = PsCell();
        Grid.SetRow( primary, pr );
        Grid.SetColumn( primary, pc );
        Grid.SetRowSpan( primary, span );
        Grid.SetColumnSpan( primary, span );
        grid.Children.Add( primary );
        cells.Add( primary );

        foreach ( var (r, c) in PsLayout.SmallCells( kind, n, corner ) )
        {
            var cell = PsCell();
            Grid.SetRow( cell, r );
            Grid.SetColumn( cell, c );
            grid.Children.Add( cell );
            cells.Add( cell );
        }

        var board = new Border
        {
            CornerRadius = new CornerRadius( 8 ),
            Padding      = new Thickness( 8 ),
            Child        = grid
        };
        board.SetResourceReference( Border.BackgroundProperty, "MaterialDesignPaper" );

        var preview = new Viewbox
        {
            Child   = board,
            Stretch = Stretch.Uniform,
            Width   = 128,
            Height  = 128
        };
        return ( preview, cells );
    }

    private static Border PsCell()
    {
        var cell = new Border
        {
            Margin       = new Thickness( 3 ),
            CornerRadius = new CornerRadius( 3 )
        };
        cell.SetResourceReference( Border.BackgroundProperty, "MaterialDesignBody" );
        return cell;
    }

    private sealed class PsPreviewItem
    {
        public required PsCorner     Corner { get; init; }
        public required List<Border> Cells  { get; init; }
    }
}