using System.Windows;
using Cube3D.Config;
using VirtualSpace.Plugin;

namespace Cube3D
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            var pluginInfo = ConfigManager.PluginInfo;
            if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Name ) )
            {
                MessageBox.Show( $"{Const.PluginInfoFile} invalid." );
                Current.Shutdown();
            }

            if ( !PluginManager.CheckRequirements( pluginInfo.Requirements ) )
            {
                MessageBox.Show( "Plugin Error.\nThe system does not meet the Requirements." );
                Current.Shutdown();
            }
        }
    }
}