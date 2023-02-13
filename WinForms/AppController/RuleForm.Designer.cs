/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace
{
    partial class RuleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuleForm));
            this.gb_Basic = new System.Windows.Forms.GroupBox();
            this.cb_Enabled = new System.Windows.Forms.CheckBox();
            this.tb_Name = new System.Windows.Forms.TextBox();
            this.lb_Name = new System.Windows.Forms.Label();
            this.gb_Rules = new System.Windows.Forms.GroupBox();
            this.tb_CommandLine = new System.Windows.Forms.TextBox();
            this.cbb_CommandLine = new System.Windows.Forms.ComboBox();
            this.cb_CommandLine = new System.Windows.Forms.CheckBox();
            this.cbb_WinInScreen = new System.Windows.Forms.ComboBox();
            this.cb_WinInScreen = new System.Windows.Forms.CheckBox();
            this.tb_WndClass = new System.Windows.Forms.TextBox();
            this.cbb_WndClass = new System.Windows.Forms.ComboBox();
            this.cb_WndClass = new System.Windows.Forms.CheckBox();
            this.tb_ProcessPath = new System.Windows.Forms.TextBox();
            this.cbb_ProcessPath = new System.Windows.Forms.ComboBox();
            this.cb_ProcessPath = new System.Windows.Forms.CheckBox();
            this.tb_ProcessName = new System.Windows.Forms.TextBox();
            this.cbb_ProcessName = new System.Windows.Forms.ComboBox();
            this.cb_ProcessName = new System.Windows.Forms.CheckBox();
            this.tb_Title = new System.Windows.Forms.TextBox();
            this.cbb_Title = new System.Windows.Forms.ComboBox();
            this.cb_Title = new System.Windows.Forms.CheckBox();
            this.gb_Actions = new System.Windows.Forms.GroupBox();
            this.cbb_MoveToScreen = new System.Windows.Forms.ComboBox();
            this.cb_MoveToScreen = new System.Windows.Forms.CheckBox();
            this.cb_HideFromView = new System.Windows.Forms.CheckBox();
            this.cb_PinApp = new System.Windows.Forms.CheckBox();
            this.cb_PinWindow = new System.Windows.Forms.CheckBox();
            this.cb_FollowWindow = new System.Windows.Forms.CheckBox();
            this.cbb_MoveToDesktop = new System.Windows.Forms.ComboBox();
            this.cb_MoveToDesktop = new System.Windows.Forms.CheckBox();
            this.btn_RuleSave = new System.Windows.Forms.Button();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.gb_Info = new System.Windows.Forms.GroupBox();
            this.lbUpdated = new System.Windows.Forms.Label();
            this.lbCreated = new System.Windows.Forms.Label();
            this.lb_Updated = new System.Windows.Forms.Label();
            this.lb_Created = new System.Windows.Forms.Label();
            this.gb_Basic.SuspendLayout();
            this.gb_Rules.SuspendLayout();
            this.gb_Actions.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.gb_Info.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_Basic
            // 
            resources.ApplyResources(this.gb_Basic, "gb_Basic");
            this.gb_Basic.Controls.Add(this.cb_Enabled);
            this.gb_Basic.Controls.Add(this.tb_Name);
            this.gb_Basic.Controls.Add(this.lb_Name);
            this.gb_Basic.Name = "gb_Basic";
            this.gb_Basic.TabStop = false;
            // 
            // cb_Enabled
            // 
            resources.ApplyResources(this.cb_Enabled, "cb_Enabled");
            this.cb_Enabled.Checked = true;
            this.cb_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_Enabled.Name = "cb_Enabled";
            this.cb_Enabled.UseVisualStyleBackColor = true;
            // 
            // tb_Name
            // 
            resources.ApplyResources(this.tb_Name, "tb_Name");
            this.tb_Name.BackColor = System.Drawing.SystemColors.Info;
            this.tb_Name.Name = "tb_Name";
            // 
            // lb_Name
            // 
            resources.ApplyResources(this.lb_Name, "lb_Name");
            this.lb_Name.Name = "lb_Name";
            // 
            // gb_Rules
            // 
            resources.ApplyResources(this.gb_Rules, "gb_Rules");
            this.gb_Rules.Controls.Add(this.tb_CommandLine);
            this.gb_Rules.Controls.Add(this.cbb_CommandLine);
            this.gb_Rules.Controls.Add(this.cb_CommandLine);
            this.gb_Rules.Controls.Add(this.cbb_WinInScreen);
            this.gb_Rules.Controls.Add(this.cb_WinInScreen);
            this.gb_Rules.Controls.Add(this.tb_WndClass);
            this.gb_Rules.Controls.Add(this.cbb_WndClass);
            this.gb_Rules.Controls.Add(this.cb_WndClass);
            this.gb_Rules.Controls.Add(this.tb_ProcessPath);
            this.gb_Rules.Controls.Add(this.cbb_ProcessPath);
            this.gb_Rules.Controls.Add(this.cb_ProcessPath);
            this.gb_Rules.Controls.Add(this.tb_ProcessName);
            this.gb_Rules.Controls.Add(this.cbb_ProcessName);
            this.gb_Rules.Controls.Add(this.cb_ProcessName);
            this.gb_Rules.Controls.Add(this.tb_Title);
            this.gb_Rules.Controls.Add(this.cbb_Title);
            this.gb_Rules.Controls.Add(this.cb_Title);
            this.gb_Rules.Name = "gb_Rules";
            this.gb_Rules.TabStop = false;
            // 
            // tb_CommandLine
            // 
            resources.ApplyResources(this.tb_CommandLine, "tb_CommandLine");
            this.tb_CommandLine.Name = "tb_CommandLine";
            // 
            // cbb_CommandLine
            // 
            resources.ApplyResources(this.cbb_CommandLine, "cbb_CommandLine");
            this.cbb_CommandLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_CommandLine.FormattingEnabled = true;
            this.cbb_CommandLine.Name = "cbb_CommandLine";
            // 
            // cb_CommandLine
            // 
            resources.ApplyResources(this.cb_CommandLine, "cb_CommandLine");
            this.cb_CommandLine.Name = "cb_CommandLine";
            this.cb_CommandLine.UseVisualStyleBackColor = true;
            this.cb_CommandLine.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // cbb_WinInScreen
            // 
            resources.ApplyResources(this.cbb_WinInScreen, "cbb_WinInScreen");
            this.cbb_WinInScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_WinInScreen.FormattingEnabled = true;
            this.cbb_WinInScreen.Name = "cbb_WinInScreen";
            // 
            // cb_WinInScreen
            // 
            resources.ApplyResources(this.cb_WinInScreen, "cb_WinInScreen");
            this.cb_WinInScreen.Name = "cb_WinInScreen";
            this.cb_WinInScreen.UseVisualStyleBackColor = true;
            this.cb_WinInScreen.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tb_WndClass
            // 
            resources.ApplyResources(this.tb_WndClass, "tb_WndClass");
            this.tb_WndClass.Name = "tb_WndClass";
            // 
            // cbb_WndClass
            // 
            resources.ApplyResources(this.cbb_WndClass, "cbb_WndClass");
            this.cbb_WndClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_WndClass.FormattingEnabled = true;
            this.cbb_WndClass.Name = "cbb_WndClass";
            // 
            // cb_WndClass
            // 
            resources.ApplyResources(this.cb_WndClass, "cb_WndClass");
            this.cb_WndClass.Name = "cb_WndClass";
            this.cb_WndClass.UseVisualStyleBackColor = true;
            this.cb_WndClass.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tb_ProcessPath
            // 
            resources.ApplyResources(this.tb_ProcessPath, "tb_ProcessPath");
            this.tb_ProcessPath.Name = "tb_ProcessPath";
            // 
            // cbb_ProcessPath
            // 
            resources.ApplyResources(this.cbb_ProcessPath, "cbb_ProcessPath");
            this.cbb_ProcessPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_ProcessPath.FormattingEnabled = true;
            this.cbb_ProcessPath.Name = "cbb_ProcessPath";
            // 
            // cb_ProcessPath
            // 
            resources.ApplyResources(this.cb_ProcessPath, "cb_ProcessPath");
            this.cb_ProcessPath.Name = "cb_ProcessPath";
            this.cb_ProcessPath.UseVisualStyleBackColor = true;
            this.cb_ProcessPath.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tb_ProcessName
            // 
            resources.ApplyResources(this.tb_ProcessName, "tb_ProcessName");
            this.tb_ProcessName.Name = "tb_ProcessName";
            // 
            // cbb_ProcessName
            // 
            resources.ApplyResources(this.cbb_ProcessName, "cbb_ProcessName");
            this.cbb_ProcessName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_ProcessName.FormattingEnabled = true;
            this.cbb_ProcessName.Name = "cbb_ProcessName";
            // 
            // cb_ProcessName
            // 
            resources.ApplyResources(this.cb_ProcessName, "cb_ProcessName");
            this.cb_ProcessName.Name = "cb_ProcessName";
            this.cb_ProcessName.UseVisualStyleBackColor = true;
            this.cb_ProcessName.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // tb_Title
            // 
            resources.ApplyResources(this.tb_Title, "tb_Title");
            this.tb_Title.Name = "tb_Title";
            // 
            // cbb_Title
            // 
            resources.ApplyResources(this.cbb_Title, "cbb_Title");
            this.cbb_Title.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_Title.FormattingEnabled = true;
            this.cbb_Title.Name = "cbb_Title";
            // 
            // cb_Title
            // 
            resources.ApplyResources(this.cb_Title, "cb_Title");
            this.cb_Title.Name = "cb_Title";
            this.cb_Title.UseVisualStyleBackColor = true;
            this.cb_Title.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // gb_Actions
            // 
            resources.ApplyResources(this.gb_Actions, "gb_Actions");
            this.gb_Actions.Controls.Add(this.cbb_MoveToScreen);
            this.gb_Actions.Controls.Add(this.cb_MoveToScreen);
            this.gb_Actions.Controls.Add(this.cb_HideFromView);
            this.gb_Actions.Controls.Add(this.cb_PinApp);
            this.gb_Actions.Controls.Add(this.cb_PinWindow);
            this.gb_Actions.Controls.Add(this.cb_FollowWindow);
            this.gb_Actions.Controls.Add(this.cbb_MoveToDesktop);
            this.gb_Actions.Controls.Add(this.cb_MoveToDesktop);
            this.gb_Actions.Name = "gb_Actions";
            this.gb_Actions.TabStop = false;
            // 
            // cbb_MoveToScreen
            // 
            resources.ApplyResources(this.cbb_MoveToScreen, "cbb_MoveToScreen");
            this.cbb_MoveToScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_MoveToScreen.FormattingEnabled = true;
            this.cbb_MoveToScreen.Name = "cbb_MoveToScreen";
            // 
            // cb_MoveToScreen
            // 
            resources.ApplyResources(this.cb_MoveToScreen, "cb_MoveToScreen");
            this.cb_MoveToScreen.Name = "cb_MoveToScreen";
            this.cb_MoveToScreen.UseVisualStyleBackColor = true;
            this.cb_MoveToScreen.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // cb_HideFromView
            // 
            resources.ApplyResources(this.cb_HideFromView, "cb_HideFromView");
            this.cb_HideFromView.Name = "cb_HideFromView";
            this.cb_HideFromView.UseVisualStyleBackColor = true;
            // 
            // cb_PinApp
            // 
            resources.ApplyResources(this.cb_PinApp, "cb_PinApp");
            this.cb_PinApp.Name = "cb_PinApp";
            this.cb_PinApp.UseVisualStyleBackColor = true;
            // 
            // cb_PinWindow
            // 
            resources.ApplyResources(this.cb_PinWindow, "cb_PinWindow");
            this.cb_PinWindow.Name = "cb_PinWindow";
            this.cb_PinWindow.UseVisualStyleBackColor = true;
            // 
            // cb_FollowWindow
            // 
            resources.ApplyResources(this.cb_FollowWindow, "cb_FollowWindow");
            this.cb_FollowWindow.Checked = true;
            this.cb_FollowWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_FollowWindow.Name = "cb_FollowWindow";
            this.cb_FollowWindow.UseVisualStyleBackColor = true;
            // 
            // cbb_MoveToDesktop
            // 
            resources.ApplyResources(this.cbb_MoveToDesktop, "cbb_MoveToDesktop");
            this.cbb_MoveToDesktop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_MoveToDesktop.FormattingEnabled = true;
            this.cbb_MoveToDesktop.Name = "cbb_MoveToDesktop";
            // 
            // cb_MoveToDesktop
            // 
            resources.ApplyResources(this.cb_MoveToDesktop, "cb_MoveToDesktop");
            this.cb_MoveToDesktop.Name = "cb_MoveToDesktop";
            this.cb_MoveToDesktop.UseVisualStyleBackColor = true;
            this.cb_MoveToDesktop.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // btn_RuleSave
            // 
            resources.ApplyResources(this.btn_RuleSave, "btn_RuleSave");
            this.btn_RuleSave.Name = "btn_RuleSave";
            this.btn_RuleSave.UseVisualStyleBackColor = true;
            this.btn_RuleSave.Click += new System.EventHandler(this.btn_RuleSave_Click);
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusBar.Name = "statusBar";
            this.statusBar.SizingGrip = false;
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            // 
            // gb_Info
            // 
            resources.ApplyResources(this.gb_Info, "gb_Info");
            this.gb_Info.Controls.Add(this.lbUpdated);
            this.gb_Info.Controls.Add(this.lbCreated);
            this.gb_Info.Controls.Add(this.lb_Updated);
            this.gb_Info.Controls.Add(this.lb_Created);
            this.gb_Info.Name = "gb_Info";
            this.gb_Info.TabStop = false;
            // 
            // lbUpdated
            // 
            resources.ApplyResources(this.lbUpdated, "lbUpdated");
            this.lbUpdated.Name = "lbUpdated";
            // 
            // lbCreated
            // 
            resources.ApplyResources(this.lbCreated, "lbCreated");
            this.lbCreated.Name = "lbCreated";
            // 
            // lb_Updated
            // 
            resources.ApplyResources(this.lb_Updated, "lb_Updated");
            this.lb_Updated.Name = "lb_Updated";
            // 
            // lb_Created
            // 
            resources.ApplyResources(this.lb_Created, "lb_Created");
            this.lb_Created.Name = "lb_Created";
            // 
            // RuleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gb_Info);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btn_RuleSave);
            this.Controls.Add(this.gb_Actions);
            this.Controls.Add(this.gb_Rules);
            this.Controls.Add(this.gb_Basic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RuleForm";
            this.gb_Basic.ResumeLayout(false);
            this.gb_Basic.PerformLayout();
            this.gb_Rules.ResumeLayout(false);
            this.gb_Rules.PerformLayout();
            this.gb_Actions.ResumeLayout(false);
            this.gb_Actions.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.gb_Info.ResumeLayout(false);
            this.gb_Info.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_Basic;
        private System.Windows.Forms.CheckBox cb_Enabled;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.Label lb_Name;
        private System.Windows.Forms.GroupBox gb_Rules;
        private System.Windows.Forms.ComboBox cbb_Title;
        private System.Windows.Forms.CheckBox cb_Title;
        private System.Windows.Forms.TextBox tb_Title;
        private System.Windows.Forms.TextBox tb_WndClass;
        private System.Windows.Forms.ComboBox cbb_WndClass;
        private System.Windows.Forms.CheckBox cb_WndClass;
        private System.Windows.Forms.TextBox tb_ProcessPath;
        private System.Windows.Forms.ComboBox cbb_ProcessPath;
        private System.Windows.Forms.CheckBox cb_ProcessPath;
        private System.Windows.Forms.TextBox tb_ProcessName;
        private System.Windows.Forms.ComboBox cbb_ProcessName;
        private System.Windows.Forms.CheckBox cb_ProcessName;
        private System.Windows.Forms.GroupBox gb_Actions;
        private System.Windows.Forms.CheckBox cb_PinApp;
        private System.Windows.Forms.CheckBox cb_PinWindow;
        private System.Windows.Forms.CheckBox cb_FollowWindow;
        private System.Windows.Forms.ComboBox cbb_MoveToDesktop;
        private System.Windows.Forms.CheckBox cb_MoveToDesktop;
        private System.Windows.Forms.Button btn_RuleSave;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox gb_Info;
        private System.Windows.Forms.Label lb_Updated;
        private System.Windows.Forms.Label lb_Created;
        private System.Windows.Forms.Label lbUpdated;
        private System.Windows.Forms.Label lbCreated;
        private System.Windows.Forms.CheckBox cb_HideFromView;
        private System.Windows.Forms.ComboBox cbb_WinInScreen;
        private System.Windows.Forms.CheckBox cb_WinInScreen;
        private System.Windows.Forms.ComboBox cbb_MoveToScreen;
        private System.Windows.Forms.CheckBox cb_MoveToScreen;
        private System.Windows.Forms.TextBox tb_CommandLine;
        private System.Windows.Forms.ComboBox cbb_CommandLine;
        private System.Windows.Forms.CheckBox cb_CommandLine;
    }
}