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
using System.IO;
using System.Text.Json;

namespace VirtualSpace.Plugin
{
    public class PluginManager
    {
        public static T? LoadFromJson<T>( string infoFile )
        {
            using var fs     = new FileStream( infoFile, FileMode.Open, FileAccess.ReadWrite );
            var       buffer = new byte[fs.Length];
            fs.Read( buffer, 0, (int)fs.Length );
            var utf8Reader = new Utf8JsonReader( buffer );
            return JsonSerializer.Deserialize<T>( ref utf8Reader );
        }

        public static bool CheckRequirements( Requirements? req )
        {
            var check   = false;
            var version = Environment.OSVersion.Version;

            if ( version.Major >= req?.WinVer.Min.Major && version.Build >= req.WinVer.Min.Build )
                check = true;

            if ( req?.WinVer.Max != null && ( version.Major > req.WinVer.Max.Major || version.Build > req.WinVer.Max.Build ) )
                check = false;

            return check;
        }
    }
}