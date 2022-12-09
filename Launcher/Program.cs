using System.Diagnostics;

namespace Launcher
{
    internal class Program
    {
        static void Main( string[] args )
        {
            var version = Environment.OSVersion.Version;
            if ( version.Major >= 10 )
            {
                var exe = version.Build switch
                {
                    >= 17763 and < 22000 => "VirtualSpace10.exe",
                    >= 22000 and < 22489 => "VirtualSpace.21H2.exe",
                    >= 22489 and < 25000 => "VirtualSpace.22H2.exe",
                    _ => ""
                };

                if ( string.IsNullOrEmpty( exe ) ) return;

                var psi = new ProcessStartInfo
                {
                    FileName = Path.Combine( System.Reflection.Assembly.GetExecutingAssembly().Location, exe )
                };

                try
                {
                    Process.Start( psi );
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}