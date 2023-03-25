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
        private static readonly ResourceManager Langs = new(
            Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Langs.WinFormStrings",
            typeof( MainForm ).Assembly );

        public MainForm()
        {
            InitializeComponent();
        }

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
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add( Const.HttpHeaderUserAgent, Const.HttpUserAgent );

                var jsonReleaseInfo = await httpClient.GetStringAsync( Const.GitHubApiLatestRelease );
                var mTag            = Regex.Match( jsonReleaseInfo, Const.PatternTag );
                if ( !mTag.Success ) throw new Exception( "release info not found." );

                var mVer = Regex.Match( mTag.Groups[1].Value, Const.PatternVersion );
                if ( !mVer.Success ) throw new Exception( "version info not found." );

                var latestVersion = new Version( mVer.Groups[0].Value );
                if ( latestVersion <= hostInfo.Version ) throw new Exception( "no update." );

                var dialogResult = MessageBox.Show( string.Format( Langs.GetString( "Updater.Confirm" ), hostInfo.Product, latestVersion, hostInfo.Version ),
                    Langs.GetString( "Updater.Confirm.Title" ),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information );
                if ( !dialogResult.Equals( DialogResult.Yes ) && !dialogResult.Equals( DialogResult.OK ) ) throw new Exception( "user canceled." );

                var mcUrls = Regex.Matches( jsonReleaseInfo, Const.PatternDownloadUrl );
                var zipUrl = "";
                foreach ( Match mUrl in mcUrls )
                {
                    var url = mUrl.Groups[1].Value;
                    if ( url.Contains( "noplugin" ) )
                    {
                        zipUrl = url;
                        break;
                    }
                }

                if ( string.IsNullOrEmpty( zipUrl ) ) throw new Exception( "file url not found." );

                Visible = true;

                var downloadZip = Path.Combine( PluginManager.GetAppFolder(), Const.DownloadZip );

                var progress = new Progress<float>();
                progress.ProgressChanged += ( s, progressValue ) => { progbarDownload.Value = (int)progressValue; };

                lb_progress.Text = Langs.GetString( "Progress.Downloading" );
                if ( File.Exists( downloadZip ) ) File.Delete( downloadZip );

                await using var file = new FileStream( downloadZip, FileMode.Create, FileAccess.Write, FileShare.None );
                await httpClient.DownloadDataAsync( zipUrl, file, progress );
                file.Close();

                while ( progbarDownload.Value < 100 )
                {
                    await Task.Delay( Const.WaitInterval );
                }

                lb_progress.Text = Langs.GetString( "Progress.Clean" );
                await Task.Delay( Const.WaitInterval );

                var exeInZip = hostInfo.Product + ".exe";
                var backup   = hostInfo.AppPath + ".bak";

                if ( File.Exists( backup ) ) File.Delete( backup );

                lb_progress.Text = Langs.GetString( "Progress.Extract" );
                await Task.Delay( Const.WaitInterval );

                using var zip = ZipFile.Open( downloadZip, ZipArchiveMode.Read );
                foreach ( var entry in zip.Entries )
                {
                    if ( !entry.Name.Contains( exeInZip ) ) continue;
                    File.Move( hostInfo.AppPath, backup );
                    entry.ExtractToFile( hostInfo.AppPath );

                    lb_progress.Text = Langs.GetString( "Progress.NotifyHostRestart" );
                    await Task.Delay( Const.WaitInterval );
                    IpcPipeClient.NotifyHostRestart();

                    break;
                }

                Visible = false;
            }
            catch
            {
                // ignored
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}