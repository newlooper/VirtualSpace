// Author: Markus Scholtes, 2021
// Version 1.9, 2021-10-08
// Version for Windows 10 1809 to 21H1
// Compile with:
// C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe VirtualDesktop.cs
// Based on http://stackoverflow.com/a/32417530, Windows 10 SDK, github project Grabacr07/VirtualDesktop and own research

/////////////////////////////////////////////////
// Dylan Cheng (https://github.com/newlooper)
// added Notifications about desktop
// added conditional compile for C#/WinRT breaking change on .NET 5.0+

using System;
using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using WinRT;
#endif

namespace VirtualDesktop
{
    public static class DesktopManager
    {
        private static  DisposableNotification             _disposableNotification;
        private static  IVirtualDesktopNotificationService VirtualDesktopNotificationService;
        internal static IVirtualDesktopManagerInternal     VirtualDesktopManagerInternal;
        internal static IVirtualDesktopManagerInternal2    VirtualDesktopManagerInternal2;
        internal static IVirtualDesktopManager             VirtualDesktopManager;
        internal static IVirtualDesktopPinnedApps          VirtualDesktopPinnedApps;
        public static   IApplicationViewCollection         ApplicationViewCollection;

        static DesktopManager()
        {
            if ( Environment.OSVersion.Version is {Major: 10, Build: < 22000} )
                Init();
        }

        public static void ResetDesktopManager()
        {
            Init();
        }

        private static void Init()
        {
            var shell = (IServiceProvider10)Activator.CreateInstance( Type.GetTypeFromCLSID( Guids.CLSID_ImmersiveShell ) );

            VirtualDesktopManager = (IVirtualDesktopManager)Activator.CreateInstance(
                Type.GetTypeFromCLSID( Guids.CLSID_VirtualDesktopManager ) );

            VirtualDesktopManagerInternal = (IVirtualDesktopManagerInternal)shell.QueryService(
                Guids.CLSID_VirtualDesktopManagerInternal,
                typeof( IVirtualDesktopManagerInternal ).GUID );
            try
            {
                VirtualDesktopManagerInternal2 = (IVirtualDesktopManagerInternal2)shell.QueryService(
                    Guids.CLSID_VirtualDesktopManagerInternal,
                    typeof( IVirtualDesktopManagerInternal2 ).GUID );
            }
            catch
            {
                VirtualDesktopManagerInternal2 = null;
            }

            ApplicationViewCollection = (IApplicationViewCollection)shell.QueryService(
                typeof( IApplicationViewCollection ).GUID,
                typeof( IApplicationViewCollection ).GUID );
            VirtualDesktopPinnedApps = (IVirtualDesktopPinnedApps)shell.QueryService(
                Guids.CLSID_VirtualDesktopPinnedApps,
                typeof( IVirtualDesktopPinnedApps ).GUID );
            VirtualDesktopNotificationService = (IVirtualDesktopNotificationService)shell.QueryService(
                Guids.CLSID_VirtualDesktopNotificationService,
                typeof( IVirtualDesktopNotificationService ).GUID );

            _disposableNotification = new DisposableNotification();
            _disposableNotification.DwCookie = VirtualDesktopNotificationService.Register( new EventProxy() );
        }

        public static int GetDesktopCount()
        {
            try
            {
                return VirtualDesktopManagerInternal.GetCount();
            }
            catch
            {
                ResetDesktopManager();
                return VirtualDesktopManagerInternal.GetCount();
            }
        }

        public static IVirtualDesktop GetDesktop( int index )
        {
            // get desktop with index
            var count = GetDesktopCount();
            if ( index < 0 || index >= count ) throw new ArgumentOutOfRangeException( nameof( index ) );
            VirtualDesktopManagerInternal.GetDesktops( out var desktops );
            desktops.GetAt( index, typeof( IVirtualDesktop ).GUID, out var objDesktop );
            Marshal.ReleaseComObject( desktops );
            return (IVirtualDesktop)objDesktop;
        }

        internal static int GetDesktopIndex( IVirtualDesktop desktop )
        {
            // get index of desktop
            var count    = GetDesktopCount();
            var index    = -1;
            var idSearch = desktop.GetId();
            VirtualDesktopManagerInternal.GetDesktops( out var desktops );
            for ( var i = 0; i < count; i++ )
            {
                desktops.GetAt( i, typeof( IVirtualDesktop ).GUID, out var objDesktop );
                if ( idSearch.CompareTo( ( (IVirtualDesktop)objDesktop ).GetId() ) == 0 )
                {
                    index = i;
                    break;
                }
            }

            Marshal.ReleaseComObject( desktops );
            return index;
        }

        internal static IApplicationView GetApplicationView( this IntPtr hWnd )
        {
            // get application view to window handle
            ApplicationViewCollection.GetViewForHWnd( hWnd, out var view );
            return view;
        }

        internal static string GetAppId( IntPtr hWnd )
        {
            // get Application ID to window handle
            hWnd.GetApplicationView().GetAppUserModelId( out var appId );
            return appId;
        }

        public static int GetViewCount()
        {
            ApplicationViewCollection.GetViews( out var objectArray );
            objectArray.GetCount( out var count );
            Marshal.ReleaseComObject( objectArray );
            return count;
        }

        public static event EventHandler<IVirtualDesktop>?                Created;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyBegin;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>? DestroyFailed;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>? Destroyed;
        public static event EventHandler<VirtualDesktopChangedEventArgs>? CurrentChanged;

        private class DisposableNotification : IDisposable
        {
            private bool _isDisposed;
            public  uint DwCookie { private get; set; }

            public void Dispose()
            {
                if ( _isDisposed ) return;

                VirtualDesktopNotificationService.Unregister( DwCookie );
                _isDisposed = true;
            }
        }

        // public static event EventHandler<VirtualDesktopRenamedEventArgs>? Renamed;
        // public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs>? WallpaperChanged;

        private class EventProxy : IVirtualDesktopNotification
        {
            public void VirtualDesktopCreated( IVirtualDesktop pDesktop )
            {
                Created?.Invoke( this, pDesktop );
            }

            public void VirtualDesktopDestroyBegin( IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback )
            {
                DestroyBegin?.Invoke( this, new VirtualDesktopDestroyEventArgs( pDesktopDestroyed, pDesktopFallback ) );
            }

            public void VirtualDesktopDestroyFailed( IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback )
            {
                DestroyFailed?.Invoke( this, new VirtualDesktopDestroyEventArgs( pDesktopDestroyed, pDesktopFallback ) );
            }

            public void VirtualDesktopDestroyed( IVirtualDesktop pDesktopDestroyed, IVirtualDesktop pDesktopFallback )
            {
                Destroyed?.Invoke( this, new VirtualDesktopDestroyEventArgs( pDesktopDestroyed, pDesktopFallback ) );
            }

            public void ViewVirtualDesktopChanged( IApplicationView pView )
            {
                // throw new NotImplementedException();
            }

            public void CurrentVirtualDesktopChanged( IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew )
            {
                CurrentChanged?.Invoke( this, new VirtualDesktopChangedEventArgs( pDesktopOld, pDesktopNew ) );
            }

            // public void VirtualDesktopRenamed( IVirtualDesktop pDesktop, string chName )
            // {
            //     Renamed?.Invoke( this, new VirtualDesktopRenamedEventArgs( pDesktop, chName ) );
            // }
        }
    }

    public class VirtualDesktopRenamedEventArgs : EventArgs
    {
        public VirtualDesktopRenamedEventArgs( IVirtualDesktop desktop, string name )
        {
            Desktop = desktop;
            Name = name;
        }

        public IVirtualDesktop Desktop { get; }
        public string          Name    { get; }
    }

    // public class VirtualDesktopWallpaperChangedEventArgs : EventArgs
    // {
    //     public VirtualDesktopWallpaperChangedEventArgs( IVirtualDesktop desktop, string path )
    //     {
    //         Desktop = desktop;
    //         Path = path;
    //     }
    //
    //     public IVirtualDesktop Desktop { get; }
    //     public string          Path    { get; }
    // }

    public class VirtualDesktopChangedEventArgs : EventArgs
    {
        public VirtualDesktopChangedEventArgs( IVirtualDesktop oldDesktop, IVirtualDesktop newDesktop )
        {
            OldDesktop = oldDesktop;
            NewDesktop = newDesktop;
        }

        public IVirtualDesktop OldDesktop { get; }
        public IVirtualDesktop NewDesktop { get; }
    }

    // public class VirtualDesktopMovedEventArgs : EventArgs
    // {
    //     public VirtualDesktopMovedEventArgs( IVirtualDesktop desktop, int oldIndex, int newIndex )
    //     {
    //         Desktop = desktop;
    //         OldIndex = oldIndex;
    //         NewIndex = newIndex;
    //     }
    //
    //     public IVirtualDesktop Desktop  { get; }
    //     public int             OldIndex { get; }
    //     public int             NewIndex { get; }
    // }

    public class VirtualDesktopDestroyEventArgs : EventArgs
    {
        public VirtualDesktopDestroyEventArgs( IVirtualDesktop destroyed, IVirtualDesktop fallback )
        {
            Destroyed = destroyed;
            Fallback = fallback;
        }

        public IVirtualDesktop Destroyed { get; }
        public IVirtualDesktop Fallback  { get; }
    }
}