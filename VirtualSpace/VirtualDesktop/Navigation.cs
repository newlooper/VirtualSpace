// Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;
using VirtualSpace.Config;

namespace VirtualSpace.VirtualDesktop
{
    public static class Navigation
    {
        public static int CalculateTargetIndex( int vdCount, int fromIndex, Keys dir, Config.Entity.Navigation nav )
        {
            var rows     = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
            var cols     = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
            var maxIndex = vdCount - 1;

            var targetIndex = -1;

            ////////////////////////////////////////
            // 边界保护 / 循环判断
            // 按行放置方式
            switch ( dir )
            {
                case Keys.Left:
                    if ( fromIndex % cols == 0 ) // 第一列，左侧已无元素
                    {
                        if ( nav.CirculationH ) // 启用水平循环
                        {
                            switch ( nav.CirculationHType )
                            {
                                case Const.NavHTypeNextRow: // 跨行循环
                                    targetIndex = fromIndex == 0 ? maxIndex : fromIndex - 1;
                                    break;
                                case Const.NavHTypeSameRow: // 行内循环
                                    targetIndex = fromIndex + cols - 1;
                                    targetIndex = targetIndex > maxIndex ? maxIndex : targetIndex;
                                    break;
                            }
                        }
                        else // 未启用水平循环，原地不动
                        {
                            targetIndex = fromIndex;
                        }
                    }
                    else
                    {
                        targetIndex = fromIndex - 1;
                    }

                    break;
                case Keys.Right:
                    if ( ( fromIndex + 1 ) % cols == 0 || fromIndex == maxIndex ) // 最后一列 或 最后一个桌面，右侧已无元素
                    {
                        if ( nav.CirculationH )
                        {
                            switch ( nav.CirculationHType )
                            {
                                case Const.NavHTypeNextRow:
                                    targetIndex = fromIndex == maxIndex ? 0 : fromIndex + 1;
                                    break;
                                case Const.NavHTypeSameRow:
                                    targetIndex = fromIndex / cols * cols; // ( fromIndex / cols ) 取整得到所在行号(0基)，行号与列数的积为行首索引
                                    break;
                            }
                        }
                        else
                        {
                            targetIndex = fromIndex;
                        }
                    }
                    else
                    {
                        targetIndex = fromIndex + 1;
                    }

                    break;
                case Keys.Up:
                    if ( fromIndex < cols ) // 第一行，上面已无元素
                    {
                        if ( nav.CirculationV ) // 启用垂直循环
                        {
                            targetIndex = fromIndex + ( rows - 1 ) * cols;
                            while ( targetIndex > maxIndex )
                                targetIndex -= cols;
                        }
                        else
                        {
                            targetIndex = fromIndex;
                        }
                    }
                    else
                    {
                        targetIndex = fromIndex - cols;
                    }

                    break;
                case Keys.Down:
                    if ( fromIndex >= ( rows - 1 ) * cols ) // 最后一行，下面已无元素
                    {
                        if ( nav.CirculationV ) // 启用垂直循环
                        {
                            targetIndex = fromIndex - ( rows - 1 ) * cols;
                        }
                        else
                        {
                            targetIndex = fromIndex;
                        }
                    }
                    else
                    {
                        targetIndex = fromIndex + cols;
                        if ( targetIndex > maxIndex )
                        {
                            targetIndex = nav.CirculationV ? targetIndex % rows : fromIndex;
                        }
                    }

                    break;
            }

            return targetIndex;
        }
    }
}