using System.Runtime.InteropServices;

namespace VirtualSpace.Helpers
{
    public static class Shell32
    {
        // ByValTStr in SHSTOCKICONINFO is not supported by LibraryImport marshalling.
        [DllImport( "Shell32.dll" )]
        public static extern int SHGetStockIconInfo( SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii );
    }
}
