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
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VirtualSpace.Plugin
{
    public class PluginInfo
    {
        [JsonIgnore] public string          Folder       = string.Empty;
        [JsonIgnore] public IntPtr          Handle;
        [JsonIgnore] public int             ProcessId;
        [JsonIgnore] public string          AssemblyPath = string.Empty;
        [JsonIgnore] public string          FileHash     = string.Empty;
        [JsonIgnore] public PluginKind      Kind;
        [JsonIgnore] public PluginLoadStatus LoadStatus  = PluginLoadStatus.Available;
        [JsonIgnore] public bool            IsLoaded;

        public PluginType      Type            { get; set; }
        public string          Name            { get; set; } = string.Empty;
        public string          Display         { get; set; } = string.Empty;
        public string          Version         { get; set; } = string.Empty;
        public string          Description     { get; set; } = string.Empty;
        public string          Author          { get; set; } = string.Empty;
        public string          Email           { get; set; } = string.Empty;
        public string          Entry           { get; set; } = string.Empty;
        public bool            AutoStart       { get; set; }
        public AutoStartTiming AutoStartTiming { get; set; } = AutoStartTiming.MainWindowLoaded;
        public Policy?         RestartPolicy   { get; set; }
        public Policy?         ClosePolicy     { get; set; }
        public Requirements?   Requirements    { get; set; }
    }

    public class Policy
    {
        public PolicyTrigger Trigger { get; set; }
        public List<string>  Values  { get; set; } = new();
        public bool          Enabled { get; set; }
    }

    public enum PolicyTrigger
    {
        WINDOWS_MESSAGE
    }

    public enum PluginType
    {
        NONE,
        VD_SWITCH_OBSERVER
    }

    public enum PluginKind
    {
        ExternalProcess,
        InProcess
    }

    public enum PluginLoadStatus
    {
        Available,
        Loaded,
        Missing,
        Error
    }

    public class Requirements
    {
        public WinVer   WinVer      { get; set; } = new();
        public Version? HostVersion { get; set; }
    }

    public class WinVer
    {
        public Ver  Min { get; set; } = new();
        public Ver? Max { get; set; }
    }

    public class Ver
    {
        public int Major { get; set; }
        public int Build { get; set; }
    }

    public enum AutoStartTiming
    {
        AppStart,
        MainWindowLoaded
    }
}
