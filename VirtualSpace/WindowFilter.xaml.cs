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
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using VirtualSpace.Config;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop;

namespace VirtualSpace
{
    public partial class WindowFilter : Window
    {
        private static WindowFilter? _instance;
        private static IntPtr        _handle;
        private static string        _lastKeyword = string.Empty;

        private static readonly Timer FilterTimer = new()
        {
            Enabled = true,
            Interval = Manager.Configs.Cluster.WindowFilterKeywordScanningInterval
        };

        private WindowFilter()
        {
            InitializeComponent();
        }

        public static WindowFilter GetInstance( IntPtr handle )
        {
            if ( _instance == null )
            {
                _instance = new WindowFilter
                {
                    Height = Const.Window.FILTER_BAR_HEIGHT
                };
                new WindowInteropHelper( _instance ).EnsureHandle();
            }

            User32.SetWindowLongPtr( new HandleRef( _instance, _handle ),
                (int)GetWindowLongFields.GWL_HWNDPARENT,
                handle.ToInt32()
            );

            FilterTimer.Elapsed += FilterTimerOnElapsed;

            return _instance;
        }

        private static void FilterTimerOnElapsed( object? sender, ElapsedEventArgs e )
        {
            if ( _lastKeyword == Keyword ) return;
            _lastKeyword = Keyword;
            VirtualDesktopManager.ShowVisibleWindowsForDesktops();
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            _handle = new WindowInteropHelper( this ).EnsureHandle();
        }

        public void SetFocus()
        {
            User32.SetForegroundWindow( _handle );
            tbFilter.Focus();

            FilterTimer.Start();
        }

        public void ClearAndHide( bool clearKeyword = true )
        {
            FilterTimer.Stop();
            if ( clearKeyword )
            {
                _lastKeyword = string.Empty;
                tbFilter.Clear();
            }

            if ( clearKeyword )
            {
                Close();
                _instance = null;
            }
            else
            {
                Hide();
            }
        }

        public static string Keyword
        {
            get
            {
                if ( _instance == null ) return string.Empty;
                if ( _instance.tbFilter.CheckAccess() )
                {
                    return _instance.tbFilter.Text;
                }

                return _instance.Dispatcher.Invoke( () => _instance.tbFilter.Text );
            }
        }
    }
}