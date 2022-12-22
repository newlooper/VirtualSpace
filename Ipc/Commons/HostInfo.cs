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
using System.Diagnostics;
using System.Reflection;

namespace VirtualSpace.Commons
{
    public class HostInfo
    {
        public Version Version     { get; set; }
        public string  Product     { get; set; }
        public string  InfoVersion { get; set; }
        public string  AppPath     { get; set; }
    }

    public static class HostInfoHelper
    {
        public static HostInfo GetHostInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            var product = ( (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyProductAttribute ),
                false ) ).Product;

            var fileVersion = ( (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyFileVersionAttribute ),
                false ) ).Version;

            var infoVersion = ( (AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyInformationalVersionAttribute ),
                false ) ).InformationalVersion;

            return new HostInfo
            {
                Version = new Version( fileVersion ),
                Product = product,
                InfoVersion = infoVersion,
                AppPath = Process.GetCurrentProcess().MainModule.FileName
            };
        }
    }
}