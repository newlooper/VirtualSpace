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
using System.IO;
using System.Reflection;
using System.Windows;
using VirtualSpace.AppLogs;
using VirtualSpace.Config;
using VirtualSpace.Helpers;

namespace VirtualSpace
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            LogManager.InitLogger( Path.Combine( Manager.GetConfigRoot(), Const.Settings.LogsFolder ) );

            AppDomain.CurrentDomain.AssemblyResolve += AutoResolver;
            var app = new App
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            app.Run();
        }

        private static Assembly? AutoResolver( object? sender, ResolveEventArgs eventArgs )
        {
            string       dllName;
            const string resName = ".Resources.";
            const string dllExt  = ".dll";

            var programName       = Assembly.GetExecutingAssembly().GetName().Name;
            var shortAssemblyName = new AssemblyName( eventArgs.Name ).Name;

            if ( shortAssemblyName?.EndsWith( ".resources" ) == true )
                return null;

            switch ( shortAssemblyName )
            {
                case "VirtualDesktop10": // must same as the <AssemblyName> which VirtualDesktopWrapper dependent, not <Aliases> in VirtualDesktopWrapper.csproj
                    Logger.Debug( "[Init]Load VirtualDesktop10 lib" );
                    dllName = programName + resName + "VirtualDesktop10" + dllExt;
                    break;
                case "VirtualDesktop11_24H2": // must same as the <AssemblyName> which VirtualDesktopWrapper dependent, not <Aliases> in VirtualDesktopWrapper.csproj
                    var ver = SysInfo.OSVersion;
                    switch ( ver.Build )
                    {
                        case <= 22489:
                            Logger.Debug( "[Init]Load VirtualDesktop11 lib 21H2" );
                            dllName = programName + resName + "VirtualDesktop11_21H2" + dllExt;
                            break;
                        case 22621:
                            Logger.Debug( "[Init]Load VirtualDesktop11 lib 22H2" );
                            dllName = ver.Revision switch
                            {
                                < 2215 => programName + resName + "VirtualDesktop11" + dllExt,
                                < 3085 => programName + resName + "VirtualDesktop11_23H2" + dllExt,
                                _ => programName + resName + "VirtualDesktop11_22H2_3085" + dllExt
                            };

                            break;
                        case 22631:
                            Logger.Debug( "[Init]Load VirtualDesktop11 lib 23H2" );
                            if ( ver.Revision >= 3085 )
                                dllName = programName + resName + "VirtualDesktop11_23H2_3085" + dllExt;
                            else
                                dllName = programName + resName + "VirtualDesktop11_23H2" + dllExt;

                            break;
                        case 26100:
                            Logger.Debug( "[Init]Load VirtualDesktop11 lib 24H2" );
                            if ( ver.Revision >= 2152 )
                                dllName = programName + resName + "VirtualDesktop11_24H2" + dllExt;
                            else
                                dllName = programName + resName + "VirtualDesktop11_23H2" + dllExt;

                            break;
                        default:
                            Logger.Debug( "[Init]Load VirtualDesktop11 lib 24H2" );
                            dllName = programName + resName + "VirtualDesktop11_24H2" + dllExt;
                            break;
                    }

                    break;
                default:
                    Logger.Debug( $"[Init]Load {shortAssemblyName} lib" );
                    dllName = programName + resName + shortAssemblyName + dllExt;
                    break;
            }

            using var stream      = typeof( Program ).Assembly.GetManifestResourceStream( dllName );
            var       rawAssembly = new byte[stream!.Length];
            stream.ReadExactly( rawAssembly );
            // try
            // {
            //     var filepath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, shortAssemblyName + dllExt );
            //     File.WriteAllBytesAsync( filepath, rawAssembly );
            // }
            // catch
            // {
            //     // ignored
            // }

            return Assembly.Load( rawAssembly );
        }
    }
}