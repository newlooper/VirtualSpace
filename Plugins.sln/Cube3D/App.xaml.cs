using System.Windows;
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
            var pluginInfo = Config.PluginInfo;
            if ( pluginInfo == null || string.IsNullOrEmpty( pluginInfo.Name ) )
            {
                MessageBox.Show( "plugin.json invalid." );
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