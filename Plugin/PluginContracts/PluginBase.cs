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
using System.Reflection;
using VirtualSpace.Plugin;

namespace VirtualSpace.PluginContracts
{
    public abstract class PluginBase : IPlugin
    {
        protected PluginBase()
        {
            Metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>()
                       ?? throw new InvalidOperationException(
                           $"{GetType().Assembly.GetName().Name} is missing [assembly: {nameof( PluginMetadataAttribute )}]." );
            Requirements = Metadata.ToRequirements();
        }

        protected PluginMetadataAttribute Metadata { get; }

        public string Name        => Metadata.Name;
        public string Display     => Metadata.Display;
        public string Version     => Metadata.Version;
        public string Description => Metadata.Description;
        public string Author      => Metadata.Author;
        public string Email       => Metadata.Email;
        public PluginType Type    => Metadata.Type;
        public Requirements Requirements { get; }

        public abstract IReadOnlyList<string> SubscribedEvents { get; }

        public abstract void Initialize( IHostContext hostContext );
        public abstract void Shutdown();
        public abstract void ShowSettings();
    }
}
