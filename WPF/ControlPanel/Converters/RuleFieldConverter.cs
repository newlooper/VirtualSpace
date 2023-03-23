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
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlPanel.ViewModels;
using VirtualSpace.Config.Events.Expression;

namespace ControlPanel.Converters;

public class RuleFieldConverter : IMultiValueConverter
{
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
        var type = parameter.ToString();

        if ( type == "V" )
        {
            return ForValue( values );
        }

        if ( type == typeof( ComboBox ).FullName )
        {
            return ForCombobox( values );
        }

        if ( type == typeof( CheckBox ).FullName )
        {
            return ForCheckBox( values );
        }

        if ( type == typeof( TextBox ).FullName )
        {
            return ForTextBox( values );
        }

        return null;
    }

    private static int ForValue( object[] values )
    {
        if ( values is null || values[0] is null ||
             values[0] == DependencyProperty.UnsetValue ||
             values[1] == DependencyProperty.UnsetValue ) return 0;

        try
        {
            var jsonDocument       = (JsonDocument)values[0];
            var expressionTemplate = Conditions.ParseExpressionTemplate( jsonDocument );
            foreach ( var r in expressionTemplate.rules )
            {
                if ( r.field == values[1].ToString() )
                {
                    var index = RulesViewModel.Screens.Select( ( vv, index ) => new {nv = vv, index} )
                        .Where( pair => ( (dynamic)pair.nv ).Value.ToString() == r.value.V )
                        .Select( pair => pair.index + 1 )
                        .FirstOrDefault() - 1;

                    return index;
                }
            }
        }
        catch
        {
            return 0;
        }

        return 0;
    }

    private static int ForCombobox( object[] values )
    {
        if ( values is null || values[0] is null ||
             values[0] == DependencyProperty.UnsetValue ||
             values[1] == DependencyProperty.UnsetValue ) return 0;

        try
        {
            var jsonDocument       = (JsonDocument)values[0];
            var expressionTemplate = Conditions.ParseExpressionTemplate( jsonDocument );
            foreach ( var r in expressionTemplate.rules )
            {
                if ( r.field == values[1].ToString() )
                {
                    var index = RulesViewModel.Operators.Select( ( v, index ) => new {value = v, index} )
                        .Where( pair => ( (dynamic)pair.value ).Value.ToString() == r.@operator )
                        .Select( pair => pair.index + 1 )
                        .FirstOrDefault() - 1;

                    return index;
                }
            }
        }
        catch
        {
            return 0;
        }

        return 0;
    }

    private static bool ForCheckBox( object[] values )
    {
        if ( values is null || values[0] is null ||
             values[0] == DependencyProperty.UnsetValue ||
             values[1] == DependencyProperty.UnsetValue ) return false;

        try
        {
            var jsonDocument       = (JsonDocument)values[0];
            var expressionTemplate = Conditions.ParseExpressionTemplate( jsonDocument );
            foreach ( var r in expressionTemplate.rules )
            {
                if ( r.field == values[1].ToString() )
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static string ForTextBox( object[] values )
    {
        if ( values is null || values[0] is null ||
             values[0] == DependencyProperty.UnsetValue ||
             values[1] == DependencyProperty.UnsetValue ) return "";

        try
        {
            var jsonDocument       = (JsonDocument)values[0];
            var expressionTemplate = Conditions.ParseExpressionTemplate( jsonDocument );
            foreach ( var r in expressionTemplate.rules )
            {
                if ( r.field == values[1].ToString() )
                {
                    return r.value.V;
                }
            }
        }
        catch
        {
            return "";
        }

        return "";
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
        throw new NotImplementedException();
    }
}