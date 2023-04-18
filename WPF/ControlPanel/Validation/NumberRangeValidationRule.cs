// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Globalization;
using System.Windows.Controls;

namespace ControlPanel.Validation;

public class NumberRangeValidationRule : ValidationRule
{
    public override ValidationResult Validate( object? value, CultureInfo cultureInfo )
    {
        try
        {
            var v = int.Parse( value.ToString() );
            if ( v < Min || v > Max )
                return new ValidationResult( false, $"{Min} - {Max}" );
        }
        catch
        {
            return new ValidationResult( false, $"{Min} - {Max}" );
        }

        return ValidationResult.ValidResult;
    }

    public int Min { get; set; }
    public int Max { get; set; }
}