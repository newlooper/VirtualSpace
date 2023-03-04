// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
//
// This file is part of Updater.
//
// Updater is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// Updater is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with Updater. If not, see <https://www.gnu.org/licenses/>.

using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Updater.Config;
using VirtualSpace.Commons;
using VirtualSpace.Plugin;

namespace Updater
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static ResourceManager Langs = new(
            Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Langs.WinFormStrings",
            typeof( MainForm ).Assembly );

        protected override void OnLoad( EventArgs e )
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad( e );
        }

        private void MainForm_Load( object sender, EventArgs e )
        {
            var pipeMessage = new PipeMessage
            {
                Type = PipeMessageType.PLUGIN_UPDATER,
                Name = PluginManager.PluginInfo.Name,
                Handle = Handle.ToInt32()
            };

            try
            {
                IpcPipeClient.PluginCheckIn<HostInfo>(
                    pipeMessage,
                    () => { MessageBox.Show( Langs.GetString( "Error.CheckHost" ) ); },
                    Application.Exit,
                    CheckUpdate
                );
            }
            catch
            {
                Application.Exit();
            }
        }

        private async void CheckUpdate( HostInfo hostInfo )
        {
            while ( true )
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add( Const.HttpHeaderUserAgent, Const.HttpUserAgent );

                var jsonReleaseInfo = await httpClient.GetStringAsync( Const.GitHubApiLatestRelease );
                var mTag            = Regex.Match( jsonReleaseInfo, Const.PatternTag );
                if ( !mTag.Success ) break;

                var mVer = Regex.Match( mTag.Groups[1].Value, Const.PatternVersion );
                if ( !mVer.Success ) break;

                var latestVersion = new Version( mVer.Groups[0].Value );
                if ( latestVersion <= hostInfo.Version ) break;

                var dialogResult = MessageBox.Show( string.Format( Langs.GetString( "Updater.Confirm" ), hostInfo.Product, latestVersion, hostInfo.Version ),
                    Langs.GetString( "Updater.Confirm.Title" ),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information );
                if ( !dialogResult.Equals( DialogResult.Yes ) && !dialogResult.Equals( DialogResult.OK ) ) break;

                var versionPostfix = Regex.Replace( hostInfo.InfoVersion, Const.PatternVersion, "" );

                var mc     = Regex.Matches( jsonReleaseInfo, Const.PatternDownloadUrl );
                var zipUrl = "";
                foreach ( Match m in mc )
                {
                    var url = m.Groups[1].Value;
                    if ( url.Contains( versionPostfix ) && url.Contains( "noplugin" ) )
                    {
                        zipUrl = url;
                        break;
                    }
                }

                if ( string.IsNullOrEmpty( zipUrl ) ) break;

                var downloadZip = Path.Combine( PluginManager.GetAppFolder(), Const.DownloadZip );
                if ( File.Exists( downloadZip ) ) File.Delete( downloadZip );

                var progress = new Progress<float>();
                progress.ProgressChanged += ( s, progressValue ) => { progbarDownload.Value = (int)progressValue; };

                Visible = true;

                await using var file = new FileStream( downloadZip, FileMode.Create, FileAccess.Write, FileShare.None );
                await httpClient.DownloadDataAsync( zipUrl, file, progress );
                file.Close();

                while ( progbarDownload.Value < 100 )
                {
                    await Task.Delay( Const.WaitInterval );
                }

                Visible = false;

                var exeInZip = hostInfo.Product + ".exe";
                var backup   = hostInfo.AppPath + ".bak";

                if ( File.Exists( backup ) ) File.Delete( backup );

                using var zip = ZipFile.Open( downloadZip, ZipArchiveMode.Read );
                foreach ( var entry in zip.Entries )
                {
                    if ( !entry.Name.Contains( exeInZip ) ) continue;
                    File.Move( hostInfo.AppPath, backup );
                    entry.ExtractToFile( hostInfo.AppPath );

                    IpcPipeClient.NotifyHostRestart();

                    break;
                }

                break;
            }

            Application.Exit();
        }
    }
}