using System.Windows;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace ControlPanel
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static RegValueMonitor _rkm;

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            LogManager.InitLogger( Const.Settings.LogsFolder );
            Manager.Init();
            _rkm = new RegValueMonitor( "HKEY_USERS",
                @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize",
                "AppsUseLightTheme" );
        }
    }
}