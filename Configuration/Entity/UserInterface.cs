/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using VirtualSpace.Config.DataAnnotations;

namespace VirtualSpace.Config.Entity;

public class UserInterface
{
    [PropertyProtector] public            Colour?       CanvasBackColor       { get; set; }
    [PropertyProtector] public            Colour?       VDWDefaultBackColor   { get; set; }
    [PropertyProtector] public            Colour?       VDWCurrentBackColor   { get; set; }
    [PropertyProtector] public            Colour?       VDWHighlightBackColor { get; set; }
    [PropertyProtector] public            ShadowColour? VDWDefaultShadowColor { get; set; } = new() { R = 20, G  = 20, B  = 20 };
    [PropertyProtector] public            ShadowColour? VDWCurrentShadowColor { get; set; } = new() { R = 255, G = 255, B = 255 };
    public                                float         VDWDragTargetOpacity  { get; set; }
    public                                string        Language              { get; set; } = string.Empty;
    public                                bool          ShowVdName            { get; set; } = true;
    public                                bool          ShowVdIndex           { get; set; } = true;
    [PropertyProtector( 0, 0, 1 )] public int           ShowVdIndexType       { get; set; }

    [PropertyProtector<VdNamePosition>( VdNamePosition.BottomLeft )]
    public VdNamePosition ShowVdNamePosition { get; set; } = VdNamePosition.BottomLeft;

    public                                  bool    EnableVdNameColorEmoji  { get; set; }
    [PropertyProtector( 0, 0, 50 )]  public int     VDWPadding              { get; set; }
    [PropertyProtector( 5, 0, 50 )]  public int     VDWBorderSize           { get; set; }
    [PropertyProtector( 8, 8, 50 )]  public int     VDWMargin               { get; set; }
    [PropertyProtector( 6, 2, 16 )]  public int     VDWShadowSize           { get; set; }
    [PropertyProtector( 12, 8, 22 )] public int     VDWCurrentShadowSize    { get; set; }
    public                                  byte    VDWDefaultShadowOpacity { get; set; } = 100;
    public                                  byte    VDWCurrentShadowOpacity { get; set; } = 220;
    [PropertyProtector( 1, 1 )] public      byte    CanvasOpacity           { get; set; }
    [PropertyProtector]         public      Margin? ThumbMargin             { get; set; }
    public                                  byte    ThumbDragSourceOpacity  { get; set; }
    [PropertyProtector( 0, 0, 7 )] public   int?    DesktopArrangement      { get; set; }
    public                                  int     Theme                   { get; set; }
}