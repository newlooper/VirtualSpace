// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

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
        internal static IVirtualDesktopManager             VirtualDesktopManager;
        internal static IVirtualDesktopPinnedApps          VirtualDesktopPinnedApps;
        public static   IApplicationViewCollection         ApplicationViewCollection;

        static DesktopManager()
        {
            if ( Environment.OSVersion.Version is {Major: 10, Build: >= 22000} )
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

        public static event EventHandler<IVirtualDesktop>?                         Created;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>?          DestroyBegin;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>?          DestroyFailed;
        public static event EventHandler<VirtualDesktopDestroyEventArgs>?          Destroyed;
        public static event EventHandler<VirtualDesktopChangedEventArgs>?          CurrentChanged;
        public static event EventHandler<VirtualDesktopMovedEventArgs>?            Moved;
        public static event EventHandler<VirtualDesktopRenamedEventArgs>?          Renamed;
        public static event EventHandler<VirtualDesktopWallpaperChangedEventArgs>? WallpaperChanged;

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

        private class EventProxy : IVirtualDesktopNotification
        {
            public void ViewVirtualDesktopChanged( IApplicationView pView )
            {
                // throw new NotImplementedException();
            }

            public void CurrentVirtualDesktopChanged( IVirtualDesktop pDesktopOld, IVirtualDesktop pDesktopNew )
            {
                CurrentChanged?.Invoke( this, new VirtualDesktopChangedEventArgs( pDesktopOld, pDesktopNew ) );
            }

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


            public void VirtualDesktopMoved( IVirtualDesktop pDesktop, int nIndexFrom, int nIndexTo )
            {
                Moved?.Invoke( this, new VirtualDesktopMovedEventArgs( pDesktop, nIndexFrom, nIndexTo ) );
            }

#if NET5_0_OR_GREATER
            public void VirtualDesktopRenamed( IVirtualDesktop pDesktop, IntPtr newName )
            {
                var name = MarshalString.FromAbi( newName );
                Renamed?.Invoke( this, new VirtualDesktopRenamedEventArgs( pDesktop, name ) );
            }
#else
            public void VirtualDesktopRenamed( IVirtualDesktop pDesktop, string chName )
            {
                Renamed?.Invoke( this, new VirtualDesktopRenamedEventArgs( pDesktop, chName ) );
            }
#endif

#if NET5_0_OR_GREATER
            public void VirtualDesktopWallpaperChanged( IVirtualDesktop pDesktop, IntPtr newPath )
            {
                var path = MarshalString.FromAbi( newPath );
                WallpaperChanged?.Invoke( this, new VirtualDesktopWallpaperChangedEventArgs( pDesktop, path ) );
            }
#else
            public void VirtualDesktopWallpaperChanged( IVirtualDesktop pDesktop, string chPath )
            {
                WallpaperChanged?.Invoke( this, new VirtualDesktopWallpaperChangedEventArgs( pDesktop, chPath ) );
            }
#endif
            public void VirtualDesktopSwitched( IVirtualDesktop pDesktop )
            {
                // throw new NotImplementedException();
            }

            public void RemoteVirtualDesktopConnected( IVirtualDesktop pDesktop )
            {
                // throw new NotImplementedException();
            }
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

    public class VirtualDesktopWallpaperChangedEventArgs : EventArgs
    {
        public VirtualDesktopWallpaperChangedEventArgs( IVirtualDesktop desktop, string path )
        {
            Desktop = desktop;
            Path = path;
        }

        public IVirtualDesktop Desktop { get; }
        public string          Path    { get; }
    }


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

    public class VirtualDesktopMovedEventArgs : EventArgs
    {
        public VirtualDesktopMovedEventArgs( IVirtualDesktop desktop, int oldIndex, int newIndex )
        {
            Desktop = desktop;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public IVirtualDesktop Desktop  { get; }
        public int             OldIndex { get; }
        public int             NewIndex { get; }
    }

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