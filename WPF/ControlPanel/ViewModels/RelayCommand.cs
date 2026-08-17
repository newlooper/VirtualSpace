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
using System.Windows.Input;

namespace ControlPanel.ViewModels;

public sealed class RelayCommand : ICommand
{
    private readonly Predicate<object?>? _canExecute;
    private readonly Action<object?>     _execute;

    public RelayCommand( Action execute ) : this( _ => execute() )
    {
    }

    public RelayCommand( Action<object?> execute, Predicate<object?>? canExecute = null )
    {
        _execute    = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute( object? parameter )
    {
        return _canExecute?.Invoke( parameter ) ?? true;
    }

    public void Execute( object? parameter )
    {
        _execute( parameter );
    }
}
