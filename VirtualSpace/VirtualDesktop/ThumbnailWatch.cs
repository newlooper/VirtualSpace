/* Copyright (C) 2026 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Config.Entity;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace VirtualSpace.VirtualDesktop;

public partial class VirtualDesktopWindow
{
    internal static bool IsDragInProgress => _dragState != 0;

    internal IReadOnlyList<VisibleWindow> ShownWindows => _visibleWindows;

    internal bool ShowsHandles( List<VisibleWindow> expected )
    {
        if ( expected.Count != _visibleWindows.Count )
        {
            return false;
        }

        foreach ( var window in _visibleWindows )
        {
            var found = false;
            foreach ( var candidate in expected )
            {
                if ( candidate.Handle != window.Handle )
                {
                    continue;
                }

                found = true;
                break;
            }

            if ( !found )
            {
                return false;
            }
        }

        return true;
    }

    internal static void RefitThumbnail( VisibleWindow window )
    {
        if ( window.Thumb == IntPtr.Zero ||
             window.ThumbCell.Right <= window.ThumbCell.Left ||
             window.ThumbCell.Bottom <= window.ThumbCell.Top )
        {
            return;
        }

        DWM_THUMBNAIL_PROPERTIES props;
        if ( VirtualDesktopManager.Ui.ThumbCaptionMode.ShowsCaption() )
        {
            var image = CaptionImageRect( window.ThumbCell );
            props = ScaleCenter( window.Thumb, image );
            PinThumbnailUnderCaption( ref props, window.ThumbCell, image );
        }
        else
        {
            props = ScaleCenter( window.Thumb, window.ThumbCell );
        }
        window.SetValidArea( props );
        UpdateThumbnail( window.Thumb, props );
    }

    internal void ReregisterThumbnail( VisibleWindow window )
    {
        if ( window.Thumb != IntPtr.Zero )
        {
            _            = DwmApi.DwmUnregisterThumbnail( window.Thumb );
            window.Thumb = IntPtr.Zero;
        }

        if ( DwmApi.DwmRegisterThumbnail( Handle, window.Handle, out var thumb ) != 0 || thumb == IntPtr.Zero )
        {
            return;
        }

        window.Thumb = thumb;
        RefitThumbnail( window );
    }
}

internal static partial class VirtualDesktopManager
{
    // 只在主视图显示且 TrackVisibleWindows 打开时跑。关掉开关会停掉定时器，不再占 Dispatcher。
    // 稳定期盖住启动后的空源、短暂 cloak 和短暂不可见。壳层 cloak 表示窗口在其他虚拟桌面，仍然显示。
    // 稳定期结束后，源连续 INVALID_SOURCE_STREAK 次没有像素才抑制；隐藏或重度 cloak 的窗口连续缺席 ABSENT_SOURCE_STREAK 次才拿掉。
    private const int THUMBNAIL_WATCH_INTERVAL_MS = 400;
    private const int THUMBNAIL_SETTLE_MS         = 2000;
    private const int INVALID_SOURCE_STREAK       = 2;
    private const int ABSENT_SOURCE_STREAK        = 4;
    private const int DWM_CLOAKED_SHELL           = 2;

    private static readonly HashSet<IntPtr>          SuppressedSources = [];
    private static readonly Dictionary<IntPtr, int>  SourceStreak      = [];
    private static readonly Dictionary<IntPtr, int>  AbsentStreak      = [];
    private static readonly Dictionary<IntPtr, SIZE> SourceSizes       = [];
    private static readonly Dictionary<IntPtr, long> ThumbnailSeenAt   = [];
    private static          DispatcherTimer?         _thumbWatch;
    private static          int                      _thumbWatchProcessId;
    private static          bool                     _thumbWatchBusy;

    /// <summary>
    ///     开关或主视图可见性变化时调用。需要追踪才创建定时器，并立刻扫一次，避免干等第一个间隔。
    /// </summary>
    internal static void SyncThumbnailWatch()
    {
        if ( !Manager.Configs.Cluster.TrackVisibleWindows || !MainWindow.IsShowing() )
        {
            StopThumbnailWatch();
            return;
        }

        var startNow = _thumbWatch is null;
        StartThumbnailWatch( _thumbWatchProcessId );
        if ( startNow && _thumbWatch is not null )
        {
            OnThumbnailWatch( null, EventArgs.Empty );
        }
    }

    private static void StartThumbnailWatch( int processId )
    {
        _thumbWatchProcessId = processId;
        if ( _thumbWatch is not null || !MainWindow.IsShowing() )
        {
            return;
        }

        var dispatcher = MainWindow.GetMainWindow().Dispatcher;
        _thumbWatch = new DispatcherTimer(
            TimeSpan.FromMilliseconds( THUMBNAIL_WATCH_INTERVAL_MS ),
            DispatcherPriority.Background,
            OnThumbnailWatch,
            dispatcher );
    }

    private static void StopThumbnailWatch()
    {
        _thumbWatch?.Stop();
        _thumbWatch     = null;
        _thumbWatchBusy = false;
        SuppressedSources.Clear();
        SourceStreak.Clear();
        AbsentStreak.Clear();
        SourceSizes.Clear();
        ThumbnailSeenAt.Clear();
    }

    /// <summary>
    ///     先记无效源，再探测已抑制的源能否恢复，成员变了才整表重排。
    ///     成员没变时按源尺寸重贴比例。物理显示器变了才重排该虚拟桌面的缩略图和标题栏。标题变化不重排。拖动进行中直接跳过。
    /// </summary>
    private static void OnThumbnailWatch( object? sender, EventArgs e )
    {
        if ( _thumbWatch is null || _thumbWatchBusy )
        {
            return;
        }

        if ( !MainWindow.IsShowing() )
        {
            StopThumbnailWatch();
            return;
        }

        if ( VirtualDesktopWindow.IsDragInProgress )
        {
            return;
        }

        _thumbWatchBusy = true;
        try
        {
            var targets = ResolveThumbnailTargets( null );
            NoteUnhealthySources( targets );
            RecoverSuppressed( targets );

            if ( ThumbnailAssignmentsDiffer( targets, _thumbWatchProcessId ) )
            {
                if ( VirtualDesktopWindow.IsDragInProgress )
                {
                    return;
                }

                ApplyThumbnailSnapshot( null, _thumbWatchProcessId, recordError: false );
                return;
            }

            RelayoutChangedScreens( targets );
            UpdateStableThumbnailAspects( targets );
            PruneThumbnailCaches( targets );
        }
        catch ( Exception ex )
        {
            Logger.Error( $"ThumbnailWatch: {ex.Message}" );
        }
        finally
        {
            _thumbWatchBusy = false;
        }
    }

    private static void ApplyThumbnailSnapshot( List<VirtualDesktopWindow>? vdwList, int processId, bool recordError = true )
    {
        var allVisibleWindows = GetVisibleWindows();
        Logger.Verbose( $"VisibleWindows/ApplicationViews: {allVisibleWindows.Count.ToString()}/{DesktopManagerWrapper.GetViewCount().ToString()}" );

        vdwList = ResolveThumbnailTargets( vdwList );
        var plan = MatchVisibleWindows( allVisibleWindows, vdwList, processId, log: true, recordError );

        foreach ( var virtualDesktopWindow in vdwList )
        {
            virtualDesktopWindow.ClearVisibleWindows();
        }

        foreach ( var pair in plan )
        {
            foreach ( var win in pair.Value )
            {
                pair.Key.AddWindow( win );
            }
        }

        foreach ( var vdw in vdwList )
        {
            vdw.ShowThumbnails();
        }
    }

    /// <summary>
    ///     只比较句柄集合。比较前先把短暂缺席的窗口补回计划，避免换屏或启动抖动被当成成员变化。
    /// </summary>
    private static bool ThumbnailAssignmentsDiffer( List<VirtualDesktopWindow> vdwList, int processId )
    {
        var plan = MatchVisibleWindows( GetVisibleWindows(), vdwList, processId, log: false, recordError: false );
        HoldTransientAbsences( vdwList, plan );
        foreach ( var vdw in vdwList )
        {
            if ( !plan.TryGetValue( vdw, out var expected ) )
            {
                expected = [];
            }

            if ( !vdw.ShowsHandles( expected ) )
            {
                return true;
            }
        }

        return false;
    }

    private static List<VirtualDesktopWindow> ResolveThumbnailTargets( List<VirtualDesktopWindow>? vdwList )
    {
        if ( vdwList is null && TryGetOnlyOneInMainView( out var onlyOne ) )
        {
            return [onlyOne];
        }

        return vdwList ?? _virtualDesktops;
    }

    /// <summary>
    ///     按虚拟桌面把可见窗口分到格子。查不到桌面身份时，已在显示的句柄留在原格，不写入永久忽略列表。
    /// </summary>
    private static Dictionary<VirtualDesktopWindow, List<VisibleWindow>> MatchVisibleWindows(
        List<VisibleWindow>        allVisibleWindows,
        List<VirtualDesktopWindow> vdwList,
        int                        processId,
        bool                       log,
        bool                       recordError )
    {
        var plan = new Dictionary<VirtualDesktopWindow, List<VisibleWindow>>( vdwList.Count );
        foreach ( var vdw in vdwList )
        {
            plan[vdw] = [];
        }

        foreach ( var win in allVisibleWindows.Where( win =>
                     string.IsNullOrEmpty( WindowFilter.Keyword ) ||
                     win.Title.Contains( WindowFilter.Keyword, StringComparison.CurrentCultureIgnoreCase ) ) )
        {
            try
            {
                if ( processId != 0 )
                {
                    _ = User32.GetWindowThreadProcessId( win.Handle, out var pId );
                    if ( processId != pId )
                    {
                        continue;
                    }
                }

                if ( ShouldSkipSource( win.Handle ) )
                {
                    continue;
                }

                if ( DesktopWrapper.IsWindowPinned( win.Handle ) ||
                     DesktopWrapper.IsApplicationPinned( win.Handle ) )
                {
                    if ( log )
                    {
                        Logger.Debug( $"Desktop[ALL]{win.Title} Is Pinned." );
                    }

                    foreach ( var vdw in vdwList )
                    {
                        plan[vdw].Add( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
                    }

                    continue;
                }

                if ( !TryGetOwnerId( win, out var ownerId ) )
                {
                    KeepShownWindow( vdwList, plan, win );
                    continue;
                }

                if ( vdwList.Count == _virtualDesktops.Count )
                {
                    var owner = vdwList.Find( v => v.VdId == ownerId );
                    if ( owner is null )
                    {
                        continue;
                    }

                    plan[owner].Add( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
                    if ( log )
                    {
                        Logger.Debug( $"Desktop[{owner.VdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( owner.VdIndex )}) CONTAINS {win.Title}" );
                    }
                }
                else
                {
                    foreach ( var vdw in vdwList.Where( vdw => vdw.VdId == ownerId ) )
                    {
                        plan[vdw].Add( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
                        if ( log )
                        {
                            Logger.Debug( $"Desktop[{vdw.VdIndex.ToString()}]({DesktopWrapper.DesktopNameFromIndex( vdw.VdIndex )}) CONTAINS {win.Title}" );
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                if ( !log || win.Classname == Const.ApplicationFrameWindow )
                {
                    continue;
                }

                if ( recordError && win.Classname != Const.ApplicationFrameWindow )
                {
                    Logger.Warning( $"{ex.Message} ∵ {win.Title}({win.Handle.ToString( "X2" )}), WndClass: {win.Classname}" );
                }

                KeepShownWindow( vdwList, plan, win );
            }
        }

        return plan;
    }

    /// <summary>
    ///     UWP 帧窗口的桌面身份在子窗口 Windows.UI.Core.CoreWindow 上，只查帧本身会失败。
    /// </summary>
    private static bool TryGetOwnerId( VisibleWindow win, out Guid ownerId )
    {
        if ( TryDesktopId( win.Handle, out ownerId ) )
        {
            return true;
        }

        if ( win.Classname != Const.ApplicationFrameWindow )
        {
            return false;
        }

        var core = FindCoreWindow( win.Handle );
        return core != IntPtr.Zero && TryDesktopId( core, out ownerId );
    }

    private static bool TryDesktopId( IntPtr handle, out Guid ownerId )
    {
        try
        {
            ownerId = DesktopWrapper.GuidFromWindow( handle );
            return ownerId != Guid.Empty;
        }
        catch ( Exception )
        {
            ownerId = Guid.Empty;
            return false;
        }
    }

    private static IntPtr FindCoreWindow( IntPtr frame )
    {
        var found = IntPtr.Zero;
        var name  = new char[Const.WindowClassMaxLength];
        User32.EnumChildWindows( frame, ( child, _ ) =>
        {
            var length = User32.GetClassName( child, name, name.Length );
            if ( length <= 0 || new string( name, 0, length ) != Const.WindowsUiCoreWindow )
            {
                return true;
            }

            found = child;
            return false;
        }, 0 );

        return found;
    }

    /// <summary>
    ///     桌面身份暂时查不到时，把已经出现在某个格子里的句柄放回该格。
    /// </summary>
    private static void KeepShownWindow(
        List<VirtualDesktopWindow>                            vdwList,
        Dictionary<VirtualDesktopWindow, List<VisibleWindow>> plan,
        VisibleWindow                                         win )
    {
        foreach ( var vdw in vdwList )
        {
            var shown = false;
            foreach ( var window in vdw.ShownWindows )
            {
                if ( window.Handle != win.Handle )
                {
                    continue;
                }

                shown = true;
                break;
            }

            if ( !shown )
            {
                continue;
            }

            foreach ( var planned in plan[vdw] )
            {
                if ( planned.Handle == win.Handle )
                {
                    return;
                }
            }

            plan[vdw].Add( new VisibleWindow( win.Title, win.Classname, win.Handle ) );
            return;
        }
    }

    /// <summary>
    ///     壳层 cloak 不是无效源。只有稳定期结束后仍画不出像素、已被抑制的句柄才跳过。
    /// </summary>
    private static bool ShouldSkipSource( IntPtr handle )
    {
        if ( IsShellCloaked( handle ) )
        {
            return false;
        }

        return SuppressedSources.Contains( handle );
    }

    private static bool IsShellCloaked( IntPtr handle )
    {
        _ = DwmApi.DwmGetWindowAttribute( handle,
            (uint)DwmApi.DwmWindowAttribute.DWMWA_CLOAKED,
            out var cloaked,
            sizeof( uint ) );

        return cloaked == DWM_CLOAKED_SHELL;
    }

    /// <summary>
    ///     源尺寸为 0 时先重新注册一次。稳定期内不累计失败；结束后连续 INVALID_SOURCE_STREAK 次仍没有像素，才放进抑制列表。
    /// </summary>
    private static void NoteUnhealthySources( List<VirtualDesktopWindow> targets )
    {
        foreach ( var vdw in targets )
        {
            foreach ( var window in vdw.ShownWindows )
            {
                ThumbnailSeenAt.TryAdd( window.Handle, Environment.TickCount64 );

                if ( IsShellCloaked( window.Handle ) )
                {
                    SourceStreak.Remove( window.Handle );
                    SuppressedSources.Remove( window.Handle );
                    continue;
                }

                if ( ThumbHasPixels( window, out _ ) )
                {
                    SourceStreak.Remove( window.Handle );
                    continue;
                }

                if ( !SourceStreak.TryGetValue( window.Handle, out var value ) )
                {
                    vdw.ReregisterThumbnail( window );
                    if ( ThumbHasPixels( window, out _ ) )
                    {
                        continue;
                    }

                    value                       = 0;
                    SourceStreak[window.Handle] = value;
                }

                if ( IsThumbnailSettling( window.Handle ) )
                {
                    continue;
                }

                var streak = value + 1;
                SourceStreak[window.Handle] = streak;
                if ( streak >= INVALID_SOURCE_STREAK )
                {
                    SuppressedSources.Add( window.Handle );
                }
            }
        }
    }

    /// <summary>
    ///     句柄还挂在格子上时不探测。离开格子后临时注册一张缩略图，源尺寸非 0 才解除抑制。
    /// </summary>
    private static void RecoverSuppressed( List<VirtualDesktopWindow> targets )
    {
        if ( SuppressedSources.Count == 0 )
        {
            return;
        }

        var shown = new HashSet<IntPtr>();
        foreach ( var vdw in targets )
        {
            foreach ( var window in vdw.ShownWindows )
            {
                shown.Add( window.Handle );
            }
        }

        foreach ( var handle in SuppressedSources.ToArray() )
        {
            if ( !User32.IsWindow( handle ) )
            {
                SuppressedSources.Remove( handle );
                SourceStreak.Remove( handle );
                continue;
            }

            if ( shown.Contains( handle ) )
            {
                continue;
            }

            if ( ProbeHasPixels( handle, targets ) )
            {
                SuppressedSources.Remove( handle );
                SourceStreak.Remove( handle );
            }
        }
    }

    /// <summary>
    ///     源宽高变了才按原格子重贴。窗口在屏幕上的位置变化不改缩略图布局。
    /// </summary>
    private static void UpdateStableThumbnailAspects( List<VirtualDesktopWindow> targets )
    {
        foreach ( var vdw in targets )
        {
            var refit = false;
            foreach ( var window in vdw.ShownWindows )
            {
                if ( !ThumbHasPixels( window, out var size ) )
                {
                    continue;
                }

                if ( !SourceSizes.TryGetValue( window.Handle, out var previous ) )
                {
                    SourceSizes[window.Handle] = size;
                    continue;
                }

                if ( previous.cx == size.cx && previous.cy == size.cy )
                {
                    continue;
                }

                SourceSizes[window.Handle] = size;
                VirtualDesktopWindow.RefitThumbnail( window );
                refit = true;
            }

            if ( refit && VirtualDesktopManager.Ui.ThumbCaptionMode.ShowsCaption() )
            {
                vdw.RefreshThumbCaptions();
            }
        }
    }

    private static void PruneThumbnailCaches( List<VirtualDesktopWindow> targets )
    {
        var live = new HashSet<IntPtr>();
        foreach ( var vdw in targets )
        {
            foreach ( var window in vdw.ShownWindows )
            {
                live.Add( window.Handle );
            }
        }

        foreach ( var handle in SourceSizes.Keys.ToArray() )
        {
            if ( !live.Contains( handle ) )
            {
                SourceSizes.Remove( handle );
            }
        }

        foreach ( var handle in SourceStreak.Keys.ToArray() )
        {
            if ( !live.Contains( handle ) && !SuppressedSources.Contains( handle ) )
            {
                SourceStreak.Remove( handle );
            }
        }

        foreach ( var handle in AbsentStreak.Keys.ToArray() )
        {
            if ( !live.Contains( handle ) )
            {
                AbsentStreak.Remove( handle );
            }
        }

        foreach ( var handle in ThumbnailSeenAt.Keys.ToArray() )
        {
            if ( !live.Contains( handle ) && !User32.IsWindow( handle ) )
            {
                ThumbnailSeenAt.Remove( handle );
            }
        }
    }

    /// <summary>
    ///     从第一次出现在格子里算起的 THUMBNAIL_SETTLE_MS 毫秒。启动过程中的空源和可见性抖动都落在这段时间里。
    /// </summary>
    private static bool IsThumbnailSettling( IntPtr handle )
    {
        return ThumbnailSeenAt.TryGetValue( handle, out var seenAt ) &&
               Environment.TickCount64 - seenAt < THUMBNAIL_SETTLE_MS;
    }

    /// <summary>
    ///     本轮匹配里不见的句柄：窗口已销毁则立刻放弃。稳定期内，或仍可见且没有重度 cloak 时，留在当前格子。
    ///     已抑制的源不挽留。其余隐藏窗口只再保留 ABSENT_SOURCE_STREAK 次扫描。
    /// </summary>
    private static void HoldTransientAbsences(
        List<VirtualDesktopWindow>                            targets,
        Dictionary<VirtualDesktopWindow, List<VisibleWindow>> plan )
    {
        var expected = new HashSet<IntPtr>();
        foreach ( var windows in plan.Values )
        {
            foreach ( var window in windows )
            {
                expected.Add( window.Handle );
            }
        }

        foreach ( var vdw in targets )
        {
            foreach ( var window in vdw.ShownWindows )
            {
                if ( expected.Contains( window.Handle ) )
                {
                    AbsentStreak.Remove( window.Handle );
                    continue;
                }

                if ( !User32.IsWindow( window.Handle ) )
                {
                    AbsentStreak.Remove( window.Handle );
                    ThumbnailSeenAt.Remove( window.Handle );
                    continue;
                }

                if ( IsThumbnailSettling( window.Handle ) )
                {
                    AbsentStreak.Remove( window.Handle );
                    plan[vdw].Add( new VisibleWindow( window.Title, window.Classname, window.Handle ) );
                    continue;
                }

                if ( SuppressedSources.Contains( window.Handle ) )
                {
                    AbsentStreak.Remove( window.Handle );
                    continue;
                }

                if ( User32.IsWindowVisible( window.Handle ) && !Filters.IsCloaked( window.Handle ) )
                {
                    AbsentStreak.Remove( window.Handle );
                    plan[vdw].Add( new VisibleWindow( window.Title, window.Classname, window.Handle ) );
                    continue;
                }

                var streak = AbsentStreak.GetValueOrDefault( window.Handle ) + 1;
                AbsentStreak[window.Handle] = streak;
                if ( streak >= ABSENT_SOURCE_STREAK )
                {
                    continue;
                }

                plan[vdw].Add( new VisibleWindow( window.Title, window.Classname, window.Handle ) );
            }
        }
    }

    private static bool ThumbHasPixels( VisibleWindow window, out SIZE size )
    {
        size = default;
        if ( window.Thumb == IntPtr.Zero )
        {
            return false;
        }

        return DwmApi.DwmQueryThumbnailSourceSize( window.Thumb, out size ) == 0 && size is { cx: > 0, cy: > 0 };
    }

    private static bool ProbeHasPixels( IntPtr source, List<VirtualDesktopWindow> targets )
    {
        var dest = IntPtr.Zero;
        foreach ( var vdw in targets )
        {
            if ( vdw.Handle == IntPtr.Zero )
            {
                continue;
            }

            dest = vdw.Handle;
            break;
        }

        if ( dest == IntPtr.Zero ||
             DwmApi.DwmRegisterThumbnail( dest, source, out var thumb ) != 0 ||
             thumb == IntPtr.Zero )
        {
            return false;
        }

        try
        {
            return DwmApi.DwmQueryThumbnailSourceSize( thumb, out var size ) == 0 && size is { cx: > 0, cy: > 0 };
        }
        finally
        {
            _ = DwmApi.DwmUnregisterThumbnail( thumb );
        }
    }
}