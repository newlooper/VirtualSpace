// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VirtualSpace.Config.Events.Entity;

namespace ControlPanel.Pages;

public partial class Rules
{
    private ListSortDirection    _lastDirection = ListSortDirection.Ascending;
    private GridViewColumnHeader _lastHeaderClicked;
    private bool                 _needRefresh;

    private void RuleList_OnLoaded( object sender, RoutedEventArgs e )
    {
        if ( !_needRefresh ) return;
        SortSelectedColumn( DefaultSortColumnHeader, ListSortDirection.Descending, RuleList.ItemsSource );
        var view = (CollectionView)CollectionViewSource.GetDefaultView( RuleList.ItemsSource );
        view.Filter = NameFilter;

        view.GroupDescriptions.Clear();
        var groupDescription = new PropertyGroupDescription( "Tag" );
        view.GroupDescriptions.Add( groupDescription );
        _needRefresh = false;
    }

    private bool NameFilter( object item )
    {
        if ( string.IsNullOrEmpty( tbNameFilter.Text ) )
            return true;

        var keyword = tbNameFilter.Text.Trim();
        if ( keyword is @"\" or @"\\" or @"\\G" )
            return true;

        if ( keyword.StartsWith( @"\\G" ) && keyword.Length > 3 )
        {
            keyword = keyword[3..];
            return ( item as RuleTemplate ).Tag?.IndexOf( keyword, StringComparison.OrdinalIgnoreCase ) >= 0;
        }

        return ( item as RuleTemplate ).Name.IndexOf( tbNameFilter.Text, StringComparison.OrdinalIgnoreCase ) >= 0;
    }

    private void TbNameFilter_OnTextChanged( object sender, TextChangedEventArgs e )
    {
        CollectionViewSource.GetDefaultView( RuleList.ItemsSource ).Refresh();
    }

    private void RuleList_OnPreviewKeyDown( object sender, KeyEventArgs e )
    {
        switch ( e.Key )
        {
            case Key.Delete:
                BtnDeleteRule_OnClick( sender, e );
                break;
            case Key.E:
                BtnEditRule_OnClick( sender, e );
                break;
        }
    }

    private void SortSelectedColumn( GridViewColumnHeader targetHeader, ListSortDirection direction, IEnumerable itemsSource )
    {
        var columnBinding = targetHeader.Column.DisplayMemberBinding as Binding;
        var sortBy        = columnBinding?.Path.Path ?? targetHeader.Column.Header as string;

        void Sort( string sortBy, ListSortDirection direction )
        {
            var dataView = (CollectionView)CollectionViewSource.GetDefaultView( itemsSource );
            dataView.SortDescriptions.Clear();
            var sd = new SortDescription( sortBy, direction );
            dataView.SortDescriptions.Add( sd );
            dataView.Refresh();
        }

        Sort( sortBy, direction );

        if ( direction == ListSortDirection.Ascending )
        {
            targetHeader.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
        }
        else
        {
            targetHeader.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
        }

        if ( _lastHeaderClicked != null && _lastHeaderClicked != targetHeader )
        {
            _lastHeaderClicked.Column.HeaderTemplate = null;
        }

        _lastHeaderClicked = targetHeader;
        _lastDirection = direction;
    }

    private void RuleList_OnColumnHeaderClick( object sender, RoutedEventArgs e )
    {
        var headerClicked = e.OriginalSource as GridViewColumnHeader;
        if ( headerClicked == null ||
             headerClicked.Role == GridViewColumnHeaderRole.Padding ) return;

        ListSortDirection direction;
        if ( headerClicked != _lastHeaderClicked )
        {
            direction = ListSortDirection.Ascending;
        }
        else
        {
            direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        SortSelectedColumn( headerClicked, direction, RuleList.ItemsSource );
    }
}