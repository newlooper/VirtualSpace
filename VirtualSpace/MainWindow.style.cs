/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Runtime.InteropServices;
using VirtualSpace.Helpers;

namespace VirtualSpace
{
    public partial class MainWindow
    {
        private uint _blurOpacity;
        private uint BlurBackgroundColor { get; set; } = 0x555555;

        private uint BlurOpacity
        {
            get => _blurOpacity;
            set
            {
                _blurOpacity = value;
                EnableBlur();
            }
        }

        private void FixStyle()
        {
            var style = User32.GetWindowLong( Handle, (int)GetWindowLongFields.GWL_STYLE );
            style = unchecked(style | (int)0x80000000); // WS_POPUP
            User32.SetWindowLongPtr( new HandleRef( this, Handle ), (int)GetWindowLongFields.GWL_STYLE, style );

            var exStyle = User32.GetWindowLong( Handle, (int)GetWindowLongFields.GWL_EXSTYLE );
            exStyle |= 0x08000000; // WS_EX_NOACTIVATE
            exStyle &= ~0x00040000; // WS_EX_APPWINDOW
            User32.SetWindowLongPtr( new HandleRef( this, Handle ), (int)GetWindowLongFields.GWL_EXSTYLE, exStyle );
        }

        private void EnableBlur()
        {
            var accent = new VisualEffects.AccentPolicy
            {
                AccentState = VisualEffects.AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = ( BlurOpacity << 24 ) | ( BlurBackgroundColor & 0xFFFFFF )
            };

            var accentStructSize = Marshal.SizeOf( accent );
            var accentPtr        = Marshal.AllocHGlobal( accentStructSize );
            Marshal.StructureToPtr( accent, accentPtr, false );

            var data = new VisualEffects.WindowCompositionAttributeData
            {
                Attribute = VisualEffects.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            _ = VisualEffects.SetWindowCompositionAttribute( Handle, ref data );

            Marshal.FreeHGlobal( accentPtr );
        }
    }
}