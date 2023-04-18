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
using System.Windows;
using System.Windows.Controls;
using ControlPanel.Pages.Menus;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Config;

namespace ControlPanel.Pages;

public partial class Logs
{
    private static string _tbInfo    = string.Empty;
    private static string _tbDebug   = string.Empty;
    private static string _tbVerbose = string.Empty;
    private static string _tbEvent   = string.Empty;
    private static string _tbWarning = string.Empty;
    private static string _tbError   = string.Empty;

    private static Logs? _instance = null;

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
        get => _tbInfo;
        set
        {
            if ( value is null )
            {
                _tbInfo = string.Empty;
            }
            else
            {
                _tbInfo = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TbDebug
    {
        get => _tbDebug;
        set
        {
            if ( value is null )
            {
                _tbDebug = string.Empty;
            }
            else
            {
                _tbDebug = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TbVerbose
    {
        get => _tbVerbose;
        set
        {
            if ( value is null )
            {
                _tbVerbose = string.Empty;
            }
            else
            {
                _tbVerbose = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TbEvent
    {
        get => _tbEvent;
        set
        {
            if ( value is null )
            {
                _tbEvent = string.Empty;
            }
            else
            {
                _tbEvent = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TbWarning
    {
        get => _tbWarning;
        set
        {
            if ( value is null )
            {
                _tbWarning = string.Empty;
            }
            else
            {
                _tbWarning = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TbError
    {
        get => _tbError;
        set
        {
            if ( value is null )
            {
                _tbError = string.Empty;
            }
            else
            {
                _tbError = value;
                NotifyStaticPropertyChanged();
            }
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
                _tbInfo += message;
                TbInfo = _tbInfo;
                break;
            case "DEBUG":
                _tbDebug += message;
                TbDebug = _tbDebug;
                break;
            case "VERBOSE":
                _tbVerbose += message;
                TbVerbose = _tbVerbose;
                break;
            case "EVENT":
                _tbEvent += message;
                TbEvent = _tbEvent;
                break;
            case "WARNING":
                _tbWarning += message;
                TbWarning = _tbWarning;
                break;
            case "ERROR":
                _tbError += message;
                TbError = _tbError;
                break;
            default:
                _tbError += message;
                TbError = _tbError;
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
                    this[TcLogs.SelectedIndex] = string.Empty;
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
        TbInfo = string.Empty;
        TbDebug = string.Empty;
        TbVerbose = string.Empty;
        TbEvent = string.Empty;
        TbWarning = string.Empty;
        TbError = string.Empty;
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