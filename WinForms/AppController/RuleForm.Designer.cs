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
            if (disposing && (components != null)) {
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
            gb_Basic = new System.Windows.Forms.GroupBox();
            cb_Enabled = new System.Windows.Forms.CheckBox();
            tb_Name = new System.Windows.Forms.TextBox();
            lb_Name = new System.Windows.Forms.Label();
            gb_Rules = new System.Windows.Forms.GroupBox();
            tb_CommandLine = new System.Windows.Forms.TextBox();
            cbb_CommandLine = new System.Windows.Forms.ComboBox();
            cb_CommandLine = new System.Windows.Forms.CheckBox();
            cbb_WinInScreen = new System.Windows.Forms.ComboBox();
            cb_WinInScreen = new System.Windows.Forms.CheckBox();
            tb_WndClass = new System.Windows.Forms.TextBox();
            cbb_WndClass = new System.Windows.Forms.ComboBox();
            cb_WndClass = new System.Windows.Forms.CheckBox();
            tb_ProcessPath = new System.Windows.Forms.TextBox();
            cbb_ProcessPath = new System.Windows.Forms.ComboBox();
            cb_ProcessPath = new System.Windows.Forms.CheckBox();
            tb_ProcessName = new System.Windows.Forms.TextBox();
            cbb_ProcessName = new System.Windows.Forms.ComboBox();
            cb_ProcessName = new System.Windows.Forms.CheckBox();
            tb_Title = new System.Windows.Forms.TextBox();
            cbb_Title = new System.Windows.Forms.ComboBox();
            cb_Title = new System.Windows.Forms.CheckBox();
            gb_Actions = new System.Windows.Forms.GroupBox();
            cbb_MoveToScreen = new System.Windows.Forms.ComboBox();
            cb_MoveToScreen = new System.Windows.Forms.CheckBox();
            cb_HideFromView = new System.Windows.Forms.CheckBox();
            cb_PinApp = new System.Windows.Forms.CheckBox();
            cb_PinWindow = new System.Windows.Forms.CheckBox();
            cb_FollowWindow = new System.Windows.Forms.CheckBox();
            cbb_MoveToDesktop = new System.Windows.Forms.ComboBox();
            cb_MoveToDesktop = new System.Windows.Forms.CheckBox();
            btn_RuleSave = new System.Windows.Forms.Button();
            statusBar = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            gb_Info = new System.Windows.Forms.GroupBox();
            lbUpdated = new System.Windows.Forms.Label();
            lbCreated = new System.Windows.Forms.Label();
            lb_Updated = new System.Windows.Forms.Label();
            lb_Created = new System.Windows.Forms.Label();
            gb_Basic.SuspendLayout();
            gb_Rules.SuspendLayout();
            gb_Actions.SuspendLayout();
            statusBar.SuspendLayout();
            gb_Info.SuspendLayout();
            SuspendLayout();
            // 
            // gb_Basic
            // 
            resources.ApplyResources(gb_Basic, "gb_Basic");
            gb_Basic.Controls.Add(cb_Enabled);
            gb_Basic.Controls.Add(tb_Name);
            gb_Basic.Controls.Add(lb_Name);
            gb_Basic.Name = "gb_Basic";
            gb_Basic.TabStop = false;
            // 
            // cb_Enabled
            // 
            resources.ApplyResources(cb_Enabled, "cb_Enabled");
            cb_Enabled.Checked = true;
            cb_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_Enabled.Name = "cb_Enabled";
            cb_Enabled.UseVisualStyleBackColor = true;
            // 
            // tb_Name
            // 
            resources.ApplyResources(tb_Name, "tb_Name");
            tb_Name.BackColor = System.Drawing.SystemColors.Info;
            tb_Name.Name = "tb_Name";
            // 
            // lb_Name
            // 
            resources.ApplyResources(lb_Name, "lb_Name");
            lb_Name.Name = "lb_Name";
            // 
            // gb_Rules
            // 
            resources.ApplyResources(gb_Rules, "gb_Rules");
            gb_Rules.Controls.Add(tb_CommandLine);
            gb_Rules.Controls.Add(cbb_CommandLine);
            gb_Rules.Controls.Add(cb_CommandLine);
            gb_Rules.Controls.Add(cbb_WinInScreen);
            gb_Rules.Controls.Add(cb_WinInScreen);
            gb_Rules.Controls.Add(tb_WndClass);
            gb_Rules.Controls.Add(cbb_WndClass);
            gb_Rules.Controls.Add(cb_WndClass);
            gb_Rules.Controls.Add(tb_ProcessPath);
            gb_Rules.Controls.Add(cbb_ProcessPath);
            gb_Rules.Controls.Add(cb_ProcessPath);
            gb_Rules.Controls.Add(tb_ProcessName);
            gb_Rules.Controls.Add(cbb_ProcessName);
            gb_Rules.Controls.Add(cb_ProcessName);
            gb_Rules.Controls.Add(tb_Title);
            gb_Rules.Controls.Add(cbb_Title);
            gb_Rules.Controls.Add(cb_Title);
            gb_Rules.Name = "gb_Rules";
            gb_Rules.TabStop = false;
            // 
            // tb_CommandLine
            // 
            resources.ApplyResources(tb_CommandLine, "tb_CommandLine");
            tb_CommandLine.Name = "tb_CommandLine";
            // 
            // cbb_CommandLine
            // 
            resources.ApplyResources(cbb_CommandLine, "cbb_CommandLine");
            cbb_CommandLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_CommandLine.FormattingEnabled = true;
            cbb_CommandLine.Name = "cbb_CommandLine";
            // 
            // cb_CommandLine
            // 
            resources.ApplyResources(cb_CommandLine, "cb_CommandLine");
            cb_CommandLine.Name = "cb_CommandLine";
            cb_CommandLine.UseVisualStyleBackColor = true;
            cb_CommandLine.CheckedChanged += checkBox_CheckedChanged;
            // 
            // cbb_WinInScreen
            // 
            resources.ApplyResources(cbb_WinInScreen, "cbb_WinInScreen");
            cbb_WinInScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_WinInScreen.FormattingEnabled = true;
            cbb_WinInScreen.Name = "cbb_WinInScreen";
            // 
            // cb_WinInScreen
            // 
            resources.ApplyResources(cb_WinInScreen, "cb_WinInScreen");
            cb_WinInScreen.Name = "cb_WinInScreen";
            cb_WinInScreen.UseVisualStyleBackColor = true;
            cb_WinInScreen.CheckedChanged += checkBox_CheckedChanged;
            // 
            // tb_WndClass
            // 
            resources.ApplyResources(tb_WndClass, "tb_WndClass");
            tb_WndClass.Name = "tb_WndClass";
            // 
            // cbb_WndClass
            // 
            resources.ApplyResources(cbb_WndClass, "cbb_WndClass");
            cbb_WndClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_WndClass.FormattingEnabled = true;
            cbb_WndClass.Name = "cbb_WndClass";
            // 
            // cb_WndClass
            // 
            resources.ApplyResources(cb_WndClass, "cb_WndClass");
            cb_WndClass.Name = "cb_WndClass";
            cb_WndClass.UseVisualStyleBackColor = true;
            cb_WndClass.CheckedChanged += checkBox_CheckedChanged;
            // 
            // tb_ProcessPath
            // 
            resources.ApplyResources(tb_ProcessPath, "tb_ProcessPath");
            tb_ProcessPath.Name = "tb_ProcessPath";
            // 
            // cbb_ProcessPath
            // 
            resources.ApplyResources(cbb_ProcessPath, "cbb_ProcessPath");
            cbb_ProcessPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_ProcessPath.FormattingEnabled = true;
            cbb_ProcessPath.Name = "cbb_ProcessPath";
            // 
            // cb_ProcessPath
            // 
            resources.ApplyResources(cb_ProcessPath, "cb_ProcessPath");
            cb_ProcessPath.Name = "cb_ProcessPath";
            cb_ProcessPath.UseVisualStyleBackColor = true;
            cb_ProcessPath.CheckedChanged += checkBox_CheckedChanged;
            // 
            // tb_ProcessName
            // 
            resources.ApplyResources(tb_ProcessName, "tb_ProcessName");
            tb_ProcessName.Name = "tb_ProcessName";
            // 
            // cbb_ProcessName
            // 
            resources.ApplyResources(cbb_ProcessName, "cbb_ProcessName");
            cbb_ProcessName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_ProcessName.FormattingEnabled = true;
            cbb_ProcessName.Name = "cbb_ProcessName";
            // 
            // cb_ProcessName
            // 
            resources.ApplyResources(cb_ProcessName, "cb_ProcessName");
            cb_ProcessName.Name = "cb_ProcessName";
            cb_ProcessName.UseVisualStyleBackColor = true;
            cb_ProcessName.CheckedChanged += checkBox_CheckedChanged;
            // 
            // tb_Title
            // 
            resources.ApplyResources(tb_Title, "tb_Title");
            tb_Title.Name = "tb_Title";
            // 
            // cbb_Title
            // 
            resources.ApplyResources(cbb_Title, "cbb_Title");
            cbb_Title.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_Title.FormattingEnabled = true;
            cbb_Title.Name = "cbb_Title";
            // 
            // cb_Title
            // 
            resources.ApplyResources(cb_Title, "cb_Title");
            cb_Title.Name = "cb_Title";
            cb_Title.UseVisualStyleBackColor = true;
            cb_Title.CheckedChanged += checkBox_CheckedChanged;
            // 
            // gb_Actions
            // 
            resources.ApplyResources(gb_Actions, "gb_Actions");
            gb_Actions.Controls.Add(cbb_MoveToScreen);
            gb_Actions.Controls.Add(cb_MoveToScreen);
            gb_Actions.Controls.Add(cb_HideFromView);
            gb_Actions.Controls.Add(cb_PinApp);
            gb_Actions.Controls.Add(cb_PinWindow);
            gb_Actions.Controls.Add(cb_FollowWindow);
            gb_Actions.Controls.Add(cbb_MoveToDesktop);
            gb_Actions.Controls.Add(cb_MoveToDesktop);
            gb_Actions.Name = "gb_Actions";
            gb_Actions.TabStop = false;
            // 
            // cbb_MoveToScreen
            // 
            resources.ApplyResources(cbb_MoveToScreen, "cbb_MoveToScreen");
            cbb_MoveToScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_MoveToScreen.FormattingEnabled = true;
            cbb_MoveToScreen.Name = "cbb_MoveToScreen";
            // 
            // cb_MoveToScreen
            // 
            resources.ApplyResources(cb_MoveToScreen, "cb_MoveToScreen");
            cb_MoveToScreen.Name = "cb_MoveToScreen";
            cb_MoveToScreen.UseVisualStyleBackColor = true;
            cb_MoveToScreen.CheckedChanged += checkBox_CheckedChanged;
            // 
            // cb_HideFromView
            // 
            resources.ApplyResources(cb_HideFromView, "cb_HideFromView");
            cb_HideFromView.Name = "cb_HideFromView";
            cb_HideFromView.UseVisualStyleBackColor = true;
            // 
            // cb_PinApp
            // 
            resources.ApplyResources(cb_PinApp, "cb_PinApp");
            cb_PinApp.Name = "cb_PinApp";
            cb_PinApp.UseVisualStyleBackColor = true;
            // 
            // cb_PinWindow
            // 
            resources.ApplyResources(cb_PinWindow, "cb_PinWindow");
            cb_PinWindow.Name = "cb_PinWindow";
            cb_PinWindow.UseVisualStyleBackColor = true;
            // 
            // cb_FollowWindow
            // 
            resources.ApplyResources(cb_FollowWindow, "cb_FollowWindow");
            cb_FollowWindow.Checked = true;
            cb_FollowWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_FollowWindow.Name = "cb_FollowWindow";
            cb_FollowWindow.UseVisualStyleBackColor = true;
            // 
            // cbb_MoveToDesktop
            // 
            resources.ApplyResources(cbb_MoveToDesktop, "cbb_MoveToDesktop");
            cbb_MoveToDesktop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_MoveToDesktop.FormattingEnabled = true;
            cbb_MoveToDesktop.Name = "cbb_MoveToDesktop";
            // 
            // cb_MoveToDesktop
            // 
            resources.ApplyResources(cb_MoveToDesktop, "cb_MoveToDesktop");
            cb_MoveToDesktop.Name = "cb_MoveToDesktop";
            cb_MoveToDesktop.UseVisualStyleBackColor = true;
            cb_MoveToDesktop.CheckedChanged += checkBox_CheckedChanged;
            // 
            // btn_RuleSave
            // 
            resources.ApplyResources(btn_RuleSave, "btn_RuleSave");
            btn_RuleSave.Name = "btn_RuleSave";
            btn_RuleSave.UseVisualStyleBackColor = true;
            btn_RuleSave.Click += btn_RuleSave_Click;
            // 
            // statusBar
            // 
            resources.ApplyResources(statusBar, "statusBar");
            statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2 });
            statusBar.Name = "statusBar";
            statusBar.SizingGrip = false;
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(toolStripStatusLabel1, "toolStripStatusLabel1");
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            resources.ApplyResources(toolStripStatusLabel2, "toolStripStatusLabel2");
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            // 
            // gb_Info
            // 
            resources.ApplyResources(gb_Info, "gb_Info");
            gb_Info.Controls.Add(lbUpdated);
            gb_Info.Controls.Add(lbCreated);
            gb_Info.Controls.Add(lb_Updated);
            gb_Info.Controls.Add(lb_Created);
            gb_Info.Name = "gb_Info";
            gb_Info.TabStop = false;
            // 
            // lbUpdated
            // 
            resources.ApplyResources(lbUpdated, "lbUpdated");
            lbUpdated.Name = "lbUpdated";
            // 
            // lbCreated
            // 
            resources.ApplyResources(lbCreated, "lbCreated");
            lbCreated.Name = "lbCreated";
            // 
            // lb_Updated
            // 
            resources.ApplyResources(lb_Updated, "lb_Updated");
            lb_Updated.Name = "lb_Updated";
            // 
            // lb_Created
            // 
            resources.ApplyResources(lb_Created, "lb_Created");
            lb_Created.Name = "lb_Created";
            // 
            // RuleForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gb_Info);
            Controls.Add(statusBar);
            Controls.Add(btn_RuleSave);
            Controls.Add(gb_Actions);
            Controls.Add(gb_Rules);
            Controls.Add(gb_Basic);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RuleForm";
            gb_Basic.ResumeLayout(false);
            gb_Basic.PerformLayout();
            gb_Rules.ResumeLayout(false);
            gb_Rules.PerformLayout();
            gb_Actions.ResumeLayout(false);
            gb_Actions.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            gb_Info.ResumeLayout(false);
            gb_Info.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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