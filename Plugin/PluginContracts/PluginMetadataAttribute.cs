// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of VirtualSpace.
//
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using VirtualSpace.Plugin;

namespace VirtualSpace.PluginContracts
{
    [AttributeUsage( AttributeTargets.Assembly )]
    public sealed class PluginMetadataAttribute : Attribute
    {
        public PluginMetadataAttribute( string name, string display, string version, string description, string author, string email )
        {
            Name        = name;
            Display     = display;
            Version     = version;
            Description = description;
            Author      = author;
            Email       = email;
        }

        public string Name        { get; }
        public string Display     { get; }
        public string Version     { get; }
        public string Description { get; }
        public string Author      { get; }
        public string Email       { get; }

        public PluginType      Type                     { get; set; }
        public bool            DefaultAutoStart         { get; set; }
        public AutoStartTiming DefaultAutoStartTiming   { get; set; } = AutoStartTiming.MainWindowLoaded;
        public int             MinWinMajor              { get; set; } = 10;
        public int             MinWinBuild              { get; set; } = 19041;
        public string?         MinHostVersion           { get; set; }
    }
}
