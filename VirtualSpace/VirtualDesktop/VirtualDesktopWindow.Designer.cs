/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace.VirtualDesktop
{
    partial class VirtualDesktopWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            ReleaseThumbnails();
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            
            this.SuspendLayout();
            // 
            // VirtualDesktopWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 600);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "VirtualDesktopWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "__VirtualDesktopWindow!";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VirtualDesktopWindow_Closing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualDesktopWindow_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VirtualDesktopWindow_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualDesktopWindow_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.pbWallpaper_Paint);
            this.ResumeLayout(false);

        }

        #endregion

    }
}