/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using VirtualSpace.VirtualDesktop.Api;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace.VirtualDesktop
{
    internal static partial class VirtualDesktopManager
    {
        private static Dictionary<int, int>? _vdToMatrixMap;
        private static Dictionary<int, int>? _matrixToVdMap;

        public static void RebuildMatrixMap( int rowsCols )
        {
            var matrixDefault = new int[rowsCols, rowsCols];
            var index         = 0;
            for ( var i = 0; i < rowsCols; i++ )
            {
                for ( var j = 0; j < rowsCols; j++ )
                {
                    matrixDefault[i, j] = index++;
                }
            }

            _vdToMatrixMap = new Dictionary<int, int>();
            _matrixToVdMap = new Dictionary<int, int>();

            if ( Math.Pow( rowsCols, 2 ) != DesktopWrapper.Count )
            {
                ConfigManager.CurrentProfile.UI.DesktopArrangement = 0;
                ConfigManager.Save();
            }

            AppController.CheckDesktopArrangement( ConfigManager.CurrentProfile.UI.DesktopArrangement.ToString() );

            var da = ConfigManager.CurrentProfile.UI.DesktopArrangement;

            index = 0;
            for ( var i = 0; i < rowsCols; i++ )
            {
                for ( var j = 0; j < rowsCols; j++ )
                {
                    switch ( da )
                    {
                        case 0:
                            _vdToMatrixMap.Add( matrixDefault[i, j], index ); // TopLeft To BottomRight H
                            _matrixToVdMap.Add( index, matrixDefault[i, j] );
                            break;
                        case 1:
                            _vdToMatrixMap.Add( matrixDefault[i, rowsCols - 1 - j], index ); // TopRight To BottomLeft H
                            _matrixToVdMap.Add( index, matrixDefault[i, rowsCols - 1 - j] );
                            break;
                        case 2:
                            _vdToMatrixMap.Add( matrixDefault[rowsCols - 1 - i, j], index ); // BottomLeft To TopRight H
                            _matrixToVdMap.Add( index, matrixDefault[rowsCols - 1 - i, j] );
                            break;
                        case 3:
                            _vdToMatrixMap.Add( matrixDefault[rowsCols - 1 - i, rowsCols - 1 - j], index ); // BottomRight To TopLeft H
                            _matrixToVdMap.Add( index, matrixDefault[rowsCols - 1 - i, rowsCols - 1 - j] );
                            break;
                        case 4:
                            _vdToMatrixMap.Add( matrixDefault[j, i], index ); // TopLeft To BottomRight V
                            _matrixToVdMap.Add( index, matrixDefault[j, i] );
                            break;
                        case 5:
                            _vdToMatrixMap.Add( matrixDefault[rowsCols - 1 - j, i], index ); // TopRight To BottomLeft V
                            _matrixToVdMap.Add( index, matrixDefault[rowsCols - 1 - j, i] );
                            break;
                        case 6:
                            _vdToMatrixMap.Add( matrixDefault[j, rowsCols - 1 - i], index ); // BottomLeft To TopRight V
                            _matrixToVdMap.Add( index, matrixDefault[j, rowsCols - 1 - i] );
                            break;
                        case 7:
                            _vdToMatrixMap.Add( matrixDefault[rowsCols - 1 - j, rowsCols - 1 - i], index ); // BottomRight To TopLeft V
                            _matrixToVdMap.Add( index, matrixDefault[rowsCols - 1 - j, rowsCols - 1 - i] );
                            break;
                        default:
                            _vdToMatrixMap.Add( matrixDefault[i, j], index ); // TopLeft To BottomRight H
                            _matrixToVdMap.Add( index, matrixDefault[i, j] );
                            break;
                    }

                    index++;
                }
            }
        }

        public static int GetMatrixIndexByVdIndex( int vdIndex )
        {
            return _vdToMatrixMap[vdIndex];
        }

        public static int GetVdIndexByMatrixIndex( int matrixIndex )
        {
            return _matrixToVdMap[matrixIndex];
        }
    }
}