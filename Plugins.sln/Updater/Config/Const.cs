// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of Updater.
//
// Updater is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// Updater is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with Updater. If not, see <https://www.gnu.org/licenses/>.

namespace Updater.Config;

public static class Const
{
    public const string HttpHeaderUserAgent    = "User-Agent";
    public const string HttpUserAgent          = "C# App";
    public const string GitHubApiLatestRelease = "https://api.github.com/repos/newlooper/VirtualSpace/releases/latest";
    public const string PatternTag             = @"""tag_name"": ?""(.*?)""";
    public const string PatternVersion         = @"\d+\.\d+\.\d+(\.\d+)?";
    public const string PatternDownloadUrl     = @"""browser_download_url"": ?""(.*?)""";
    public const string DownloadZip            = "download.zip";
    public const int    WaitInterval           = 500;
}