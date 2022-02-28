/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Button = System.Windows.Controls.Button;

namespace Cube3D
{
    public partial class MainWindow
    {
        private static readonly int _animationDuration = 500;

        private static readonly Dictionary<Keys, RotateTransform3D> RotateTransDirections = new Dictionary<Keys, RotateTransform3D>
        {
            {
                Keys.Left, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 0 ) )
                {
                    CenterX = _meshWidth / 2,
                    CenterZ = -_meshWidth / 2
                }
            },
            {
                Keys.Right, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, -1, 0 ), 0 ) )
                {
                    CenterX = _meshWidth / 2,
                    CenterZ = -_meshWidth / 2
                }
            },
            {
                Keys.Up, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 1, 0, 0 ), 0 ) )
                {
                    CenterY = _meshHeight / 2,
                    CenterZ = -_meshHeight / 2
                }
            },
            {
                Keys.Down, new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( -1, 0, 0 ), 0 ) )
                {
                    CenterY = _meshHeight / 2,
                    CenterZ = -_meshHeight / 2
                }
            }
        };

        private readonly DoubleAnimation _animationCube = new DoubleAnimation
        {
            From = 0,
            To = 90,
            Duration = new Duration( TimeSpan.FromMilliseconds( _animationDuration ) ),
            FillBehavior = FillBehavior.Stop
        };

        private readonly ThicknessAnimation _animationNotifyGrid = new ThicknessAnimation
        {
            Duration = new Duration( TimeSpan.FromMilliseconds( _animationDuration ) ),
            FillBehavior = FillBehavior.Stop
        };

        private readonly Transform3DGroup  _transGroup = new Transform3DGroup();
        private          RotateTransform3D _rotateTrans;
        private          int               _runningAnimationCount;

        private void AnimationCompleted( object sender, EventArgs e )
        {
            Interlocked.Decrement( ref _runningAnimationCount );
            if ( _runningAnimationCount == 0 )
                ShowHide();
        }

        private void CubeAnimation( Keys dir )
        {
            switch ( dir )
            {
                case Keys.Left:
                    _rotateTrans = RotateTransDirections[Keys.Left];
                    break;
                case Keys.Right:
                    _rotateTrans = RotateTransDirections[Keys.Right];
                    break;
                case Keys.Up:
                    _rotateTrans = RotateTransDirections[Keys.Up];
                    break;
                case Keys.Down:
                    _rotateTrans = RotateTransDirections[Keys.Down];
                    break;
            }

            if ( _transGroup.Children.Count == 0 )
            {
                _transGroup.Children.Add( _rotateTrans );
            }
            else
            {
                _transGroup.Children[0] = _rotateTrans;
            }

            CubeModel3DGroup.Transform = _transGroup;

            // _animationCube.EasingFunction = new CircleEase();
            _rotateTrans.Rotation.BeginAnimation( AxisAngleRotation3D.AngleProperty, _animationCube );
            Interlocked.Increment( ref _runningAnimationCount );

            // var animationCamera = new Point3DAnimation 
            // {
            //     From = new Point3D( MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z + 0.1),
            //     To = new Point3D( MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z + 0.5),
            //     Duration = new Duration( TimeSpan.FromMilliseconds( _animationDuration / 2 ) ),
            //     FillBehavior = FillBehavior.Stop,
            //     AutoReverse = true
            // };
            //
            // MainCamera.BeginAnimation( ProjectionCamera.PositionProperty , animationCamera );
        }

        private void NotificationGridAnimation( int fromIndex, int toIndex, int vdCount )
        {
            var oneCell = (Button)NotifyGrid.Children[fromIndex]; // 用 0 也行，UniformGrid 的 cell 尺寸一样

            var rowsCols = (int)Math.Ceiling( Math.Sqrt( vdCount ) );
            var fromRow  = fromIndex / rowsCols; // divide
            var fromCol  = fromIndex % rowsCols; // modulus 
            var toRow    = toIndex / rowsCols; // divide
            var toCol    = toIndex % rowsCols; // modulus 

            // Debug.WriteLine( $"Row: {toRow}, Column:{toCol}" );
            var buttonMargin = (Thickness)Resources["NotifyButtonMargin"];

            var cellCenterToCell00Distance = rowsCols - 1; // 默认 margin all=0 在中心，需计算与左上角的单位距离
            var cell00Margin = new Thickness // 左上角 cell 的 margin，行索引=列索引=子元素索引=0，便于计算
            {
                Left = -buttonMargin.Left - cellCenterToCell00Distance * oneCell.ActualWidth,
                Top = -buttonMargin.Top - cellCenterToCell00Distance * oneCell.ActualHeight
            };

            ///////////////////////////////////////////////////////////////////
            // 所有非 0 索引的 cell，其 margin 都基于左上角 cell 计算，籍此进行定位
            CurrentIndicator.Margin = new Thickness // 当前索引作为动画起始 cell
            {
                Left = cell00Margin.Left + 2 * fromCol * oneCell.ActualWidth, // 宽度对应列系数
                Top = cell00Margin.Top + 2 * fromRow * oneCell.ActualHeight // 高度对应行系数
            };

            var targetCellMargin = new Thickness // 目标 cell 作为动画结束 cell
            {
                Left = cell00Margin.Left + 2 * toCol * oneCell.ActualWidth, // 宽度对应列系数
                Top = cell00Margin.Top + 2 * toRow * oneCell.ActualHeight // 高度对应行系数
            };

            ////////////////////////////////////////////////////////////////
            // animation
            _animationNotifyGrid.From = CurrentIndicator.Margin;
            _animationNotifyGrid.To = targetCellMargin;
            // _animationNotifyGrid.EasingFunction = new CircleEase();
            CurrentIndicator.BeginAnimation( MarginProperty, _animationNotifyGrid );
            Interlocked.Increment( ref _runningAnimationCount );
        }
    }
}