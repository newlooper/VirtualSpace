/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace VirtualSpace
{
    public static class Agent
    {
        public const string FallbackLanguage = "en";

        public static readonly Dictionary<string, string> ValidLangs = new()
        {
            { "en", "English" },
            { "zh-Hans", "中文(简体)" },
            { "zh-Hant", "中文(繁體)" }
        };

        public static readonly ResourceManager Langs = new(
            Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Langs.WinFormStrings",
            typeof( Agent ).Assembly );

        public static readonly ResourceManager Images = new(
            Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Images.Images",
            typeof( Agent ).Assembly );

        /// <summary>
        /// Maps a culture (default: process UI culture) to a product language in <see cref="ValidLangs"/>.
        /// Unsupported cultures fall back to <see cref="FallbackLanguage"/>.
        /// </summary>
        public static string ResolveUiLanguage( CultureInfo? culture = null )
        {
            culture ??= CultureInfo.CurrentUICulture;

            for ( var c = culture; c.Name.Length > 0; c = c.Parent )
            {
                if ( ValidLangs.ContainsKey( c.Name ) )
                    return c.Name;
            }

            var iso = culture.TwoLetterISOLanguageName;
            if ( ValidLangs.ContainsKey( iso ) )
                return iso;

            if ( !iso.Equals( "zh", StringComparison.OrdinalIgnoreCase ) )
                return FallbackLanguage;
            
            var name = culture.Name;
            if ( name.Contains( "Hant", StringComparison.OrdinalIgnoreCase ) ||
                 name.Contains( "TW", StringComparison.OrdinalIgnoreCase ) ||
                 name.Contains( "HK", StringComparison.OrdinalIgnoreCase ) ||
                 name.Contains( "MO", StringComparison.OrdinalIgnoreCase ) ||
                 name.Contains( "CHT", StringComparison.OrdinalIgnoreCase ) )
                return "zh-Hant";

            return "zh-Hans";
        }
    }
}
