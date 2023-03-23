// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ControlPanel.ViewModels;

public sealed class FullObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
{
    private FullObservableCollection()
    {
        CollectionChanged += CollectionChangedHandler;
    }

    public FullObservableCollection( IEnumerable<T> normalList ) : this()
    {
        foreach ( var item in normalList )
        {
            Add( item );
        }
    }

    private void CollectionChangedHandler( object? sender, NotifyCollectionChangedEventArgs e )
    {
        switch ( e.Action )
        {
            case NotifyCollectionChangedAction.Remove:
            {
                foreach ( T item in e.OldItems )
                {
                    item.PropertyChanged -= OnNotifyPropertyChanged;
                }

                return;
            }
            case NotifyCollectionChangedAction.Add:
            {
                foreach ( T item in e.NewItems )
                {
                    item.PropertyChanged += OnNotifyPropertyChanged;
                }

                break;
            }
        }
    }

    private void OnNotifyPropertyChanged( object? sender, PropertyChangedEventArgs e )
    {
        var args = new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset );
        OnCollectionChanged( args );
    }
}