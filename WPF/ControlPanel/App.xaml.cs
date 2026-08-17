using System;
using System.Windows;
using System.Windows.Markup;

namespace ControlPanel;

public partial class App : Application, IComponentConnector
{
    private bool _contentLoaded;

    public void InitializeComponent()
    {
        if ( _contentLoaded ) return;
        _contentLoaded = true;
        Resources.MergedDictionaries.Add( new ResourceDictionary
        {
            Source = new Uri( "/ControlPanel;component/ControlPanel.xaml", UriKind.Relative )
        } );
    }

    void IComponentConnector.Connect( int connectionId, object target )
    {
    }
}
