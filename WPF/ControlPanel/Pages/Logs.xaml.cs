/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.Pages.Menus;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Config;

namespace ControlPanel.Pages;

public partial class Logs
{
    private static readonly StringBuilder SbDebug   = new();
    private static readonly StringBuilder SbVerbose = new();
    private static readonly StringBuilder SbEvent   = new();
    private static readonly StringBuilder SbWarning = new();
    private static readonly StringBuilder SbError   = new();
    private static readonly StringBuilder SbInfo    = new();

    private static Logs? _instance;

    private Logs()
    {
        InitializeComponent();
    }

    private Logs( string headerKey, PackIconKind iconKind ) : this()
    {
        TcLogs.DataContext = this;

        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.InjectContent = new LogsMenu();
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;
    }

    public string this[ int index ]
    {
        set
        {
            switch ( index )
            {
                case 0:
                    TbInfo = value;
                    break;
                case 1:
                    TbDebug = value;
                    break;
                case 2:
                    TbVerbose = value;
                    break;
                case 3:
                    TbEvent = value;
                    break;
                case 4:
                    TbWarning = value;
                    break;
                case 5:
                    TbError = value;
                    break;
            }
        }
    }

    public static string TbInfo
    {
        get => SbInfo.ToString();
        set
        {
            if ( value is null )
            {
                SbInfo.Clear();
            }
            else
            {
                SbInfo.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static string TbDebug
    {
        get => SbDebug.ToString();
        set
        {
            if ( value is null )
            {
                SbDebug.Clear();
            }
            else
            {
                SbDebug.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static string TbVerbose
    {
        get => SbVerbose.ToString();
        set
        {
            if ( value is null )
            {
                SbVerbose.Clear();
            }
            else
            {
                SbVerbose.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static string TbEvent
    {
        get => SbEvent.ToString();
        set
        {
            if ( value is null )
            {
                SbEvent.Clear();
            }
            else
            {
                SbEvent.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static string TbWarning
    {
        get => SbWarning.ToString();
        set
        {
            if ( value is null )
            {
                SbWarning.Clear();
            }
            else
            {
                SbWarning.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static string TbError
    {
        get => SbError.ToString();
        set
        {
            if ( value is null )
            {
                SbError.Clear();
            }
            else
            {
                SbError.Append( value );
            }

            NotifyStaticPropertyChanged();
        }
    }

    public static Logs Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Logs( headerKey, iconKind );
    }

    public static void Append( string message, string type )
    {
        switch ( type )
        {
            case "INFO":
                TbInfo = message;
                break;
            case "DEBUG":
                TbDebug = message;
                break;
            case "VERBOSE":
                TbVerbose = message;
                break;
            case "EVENT":
                TbEvent = message;
                break;
            case "WARNING":
                TbWarning = message;
                break;
            case "ERROR":
                TbError = message;
                break;
            default:
                TbError = message;
                break;
        }
    }

    public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

    private static void NotifyStaticPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        StaticPropertyChanged?.Invoke( null, new PropertyChangedEventArgs( propertyName ) );
    }

    private void Clear_Click( object sender, RoutedEventArgs e )
    {
        if ( sender is MenuItem mi )
        {
            if ( mi.CommandParameter is ContextMenu cm )
            {
                if ( cm.PlacementTarget is TabItem t )
                {
                    t.IsSelected = true;
                    this[TcLogs.SelectedIndex] = null;
                }
            }
        }
    }

    private void TabItem_OnContextMenuOpening( object sender, ContextMenuEventArgs e )
    {
        if ( e.Source is TabItem t )
        {
            t.IsSelected = true;
        }
    }

    public static void ClearAll()
    {
        TbInfo = null;
        TbDebug = null;
        TbVerbose = null;
        TbEvent = null;
        TbWarning = null;
        TbError = null;
    }

    public static void OpenLogsDir()
    {
        var logFolder = Path.Combine( Manager.AppRootFolder, Const.Settings.LogsFolder );
        if ( !Directory.Exists( logFolder ) ) return;
        var startInfo = new ProcessStartInfo
        {
            Arguments = logFolder,
            FileName = "explorer.exe"
        };

        Process.Start( startInfo );
    }
}