/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

namespace VirtualSpace
{
    partial class AppController
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
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppController));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.langToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLogsInGuiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MT_Logs = new System.Windows.Forms.TabPage();
            this.logTabs = new System.Windows.Forms.TabControl();
            this.logTabInfo = new System.Windows.Forms.TabPage();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.logTabDebug = new System.Windows.Forms.TabPage();
            this.tbDebug = new System.Windows.Forms.TextBox();
            this.logTabEvent = new System.Windows.Forms.TabPage();
            this.tbEvent = new System.Windows.Forms.TextBox();
            this.logTabWarning = new System.Windows.Forms.TabPage();
            this.tbWarning = new System.Windows.Forms.TextBox();
            this.logTabError = new System.Windows.Forms.TabPage();
            this.tbError = new System.Windows.Forms.TextBox();
            this.MT_General = new System.Windows.Forms.TabPage();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.MT_UI = new System.Windows.Forms.TabPage();
            this.MT_Rules = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lv_Rules = new System.Windows.Forms.ListView();
            this.lvc_Name = new System.Windows.Forms.ColumnHeader();
            this.lvc_Created = new System.Windows.Forms.ColumnHeader();
            this.lvc_Updated = new System.Windows.Forms.ColumnHeader();
            this.btn_RuleEdit = new System.Windows.Forms.Button();
            this.btn_RuleClone = new System.Windows.Forms.Button();
            this.btn_RuleNew = new System.Windows.Forms.Button();
            this.btn_RuleRemove = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_RuleProfiles = new System.Windows.Forms.ComboBox();
            this.MT_Plugins = new System.Windows.Forms.TabPage();
            this.MT_About = new System.Windows.Forms.TabPage();
            this.lb_AppName = new System.Windows.Forms.Label();
            this.llb_Company = new System.Windows.Forms.LinkLabel();
            this.lbox_Env = new System.Windows.Forms.ListBox();
            this.lb_Copyright = new System.Windows.Forms.Label();
            this.lb_Version = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.pb_AboutLogo = new System.Windows.Forms.PictureBox();
            this.logCMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_Pages = new System.Windows.Forms.Panel();
            this.ts_PageNav = new System.Windows.Forms.ToolStrip();
            this.tsb_general = new System.Windows.Forms.ToolStripButton();
            this.tsb_ui = new System.Windows.Forms.ToolStripButton();
            this.tsb_rules = new System.Windows.Forms.ToolStripButton();
            this.tsb_plugins = new System.Windows.Forms.ToolStripButton();
            this.tsb_logs = new System.Windows.Forms.ToolStripButton();
            this.tsb_about = new System.Windows.Forms.ToolStripButton();
            this.panel_PageNav = new System.Windows.Forms.Panel();
            this.panel_mask = new System.Windows.Forms.Panel();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.MT_Logs.SuspendLayout();
            this.logTabs.SuspendLayout();
            this.logTabInfo.SuspendLayout();
            this.logTabDebug.SuspendLayout();
            this.logTabEvent.SuspendLayout();
            this.logTabWarning.SuspendLayout();
            this.logTabError.SuspendLayout();
            this.mainTabs.SuspendLayout();
            this.MT_Rules.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.MT_About.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AboutLogo)).BeginInit();
            this.logCMS.SuspendLayout();
            this.panel_Pages.SuspendLayout();
            this.ts_PageNav.SuspendLayout();
            this.panel_PageNav.SuspendLayout();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Name = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            resources.ApplyResources(this.quitToolStripMenuItem, "quitToolStripMenuItem");
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.langToolStripMenuItem,
            this.logsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
            // 
            // langToolStripMenuItem
            // 
            this.langToolStripMenuItem.Name = "langToolStripMenuItem";
            resources.ApplyResources(this.langToolStripMenuItem, "langToolStripMenuItem");
            // 
            // logsToolStripMenuItem
            // 
            this.logsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLogsInGuiToolStripMenuItem});
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            resources.ApplyResources(this.logsToolStripMenuItem, "logsToolStripMenuItem");
            // 
            // showLogsInGuiToolStripMenuItem
            // 
            this.showLogsInGuiToolStripMenuItem.CheckOnClick = true;
            this.showLogsInGuiToolStripMenuItem.Name = "showLogsInGuiToolStripMenuItem";
            resources.ApplyResources(this.showLogsInGuiToolStripMenuItem, "showLogsInGuiToolStripMenuItem");
            this.showLogsInGuiToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showLogsInGuiToolStripMenuItem_CheckedChanged);
            // 
            // MT_Logs
            // 
            this.MT_Logs.Controls.Add(this.logTabs);
            resources.ApplyResources(this.MT_Logs, "MT_Logs");
            this.MT_Logs.Name = "MT_Logs";
            this.MT_Logs.UseVisualStyleBackColor = true;
            // 
            // logTabs
            // 
            this.logTabs.Controls.Add(this.logTabInfo);
            this.logTabs.Controls.Add(this.logTabDebug);
            this.logTabs.Controls.Add(this.logTabEvent);
            this.logTabs.Controls.Add(this.logTabWarning);
            this.logTabs.Controls.Add(this.logTabError);
            resources.ApplyResources(this.logTabs, "logTabs");
            this.logTabs.Name = "logTabs";
            this.logTabs.SelectedIndex = 0;
            this.logTabs.Click += new System.EventHandler(this.logTabs_Click);
            // 
            // logTabInfo
            // 
            this.logTabInfo.Controls.Add(this.tbInfo);
            resources.ApplyResources(this.logTabInfo, "logTabInfo");
            this.logTabInfo.Name = "logTabInfo";
            this.logTabInfo.UseVisualStyleBackColor = true;
            // 
            // tbInfo
            // 
            resources.ApplyResources(this.tbInfo, "tbInfo");
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            // 
            // logTabDebug
            // 
            this.logTabDebug.Controls.Add(this.tbDebug);
            resources.ApplyResources(this.logTabDebug, "logTabDebug");
            this.logTabDebug.Name = "logTabDebug";
            this.logTabDebug.UseVisualStyleBackColor = true;
            // 
            // tbDebug
            // 
            resources.ApplyResources(this.tbDebug, "tbDebug");
            this.tbDebug.Name = "tbDebug";
            this.tbDebug.ReadOnly = true;
            // 
            // logTabEvent
            // 
            this.logTabEvent.Controls.Add(this.tbEvent);
            resources.ApplyResources(this.logTabEvent, "logTabEvent");
            this.logTabEvent.Name = "logTabEvent";
            this.logTabEvent.UseVisualStyleBackColor = true;
            // 
            // tbEvent
            // 
            resources.ApplyResources(this.tbEvent, "tbEvent");
            this.tbEvent.Name = "tbEvent";
            this.tbEvent.ReadOnly = true;
            // 
            // logTabWarning
            // 
            this.logTabWarning.Controls.Add(this.tbWarning);
            resources.ApplyResources(this.logTabWarning, "logTabWarning");
            this.logTabWarning.Name = "logTabWarning";
            this.logTabWarning.UseVisualStyleBackColor = true;
            // 
            // tbWarning
            // 
            resources.ApplyResources(this.tbWarning, "tbWarning");
            this.tbWarning.Name = "tbWarning";
            this.tbWarning.ReadOnly = true;
            // 
            // logTabError
            // 
            this.logTabError.Controls.Add(this.tbError);
            resources.ApplyResources(this.logTabError, "logTabError");
            this.logTabError.Name = "logTabError";
            this.logTabError.UseVisualStyleBackColor = true;
            // 
            // tbError
            // 
            resources.ApplyResources(this.tbError, "tbError");
            this.tbError.Name = "tbError";
            this.tbError.ReadOnly = true;
            // 
            // MT_General
            // 
            resources.ApplyResources(this.MT_General, "MT_General");
            this.MT_General.Name = "MT_General";
            this.MT_General.UseVisualStyleBackColor = true;
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.MT_General);
            this.mainTabs.Controls.Add(this.MT_UI);
            this.mainTabs.Controls.Add(this.MT_Rules);
            this.mainTabs.Controls.Add(this.MT_Plugins);
            this.mainTabs.Controls.Add(this.MT_Logs);
            this.mainTabs.Controls.Add(this.MT_About);
            resources.ApplyResources(this.mainTabs, "mainTabs");
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            // 
            // MT_UI
            // 
            resources.ApplyResources(this.MT_UI, "MT_UI");
            this.MT_UI.Name = "MT_UI";
            this.MT_UI.UseVisualStyleBackColor = true;
            // 
            // MT_Rules
            // 
            this.MT_Rules.Controls.Add(this.groupBox2);
            this.MT_Rules.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.MT_Rules, "MT_Rules");
            this.MT_Rules.Name = "MT_Rules";
            this.MT_Rules.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lv_Rules);
            this.groupBox2.Controls.Add(this.btn_RuleEdit);
            this.groupBox2.Controls.Add(this.btn_RuleClone);
            this.groupBox2.Controls.Add(this.btn_RuleNew);
            this.groupBox2.Controls.Add(this.btn_RuleRemove);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lv_Rules
            // 
            this.lv_Rules.CheckBoxes = true;
            this.lv_Rules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvc_Name,
            this.lvc_Created,
            this.lvc_Updated});
            this.lv_Rules.FullRowSelect = true;
            this.lv_Rules.GridLines = true;
            this.lv_Rules.HideSelection = false;
            resources.ApplyResources(this.lv_Rules, "lv_Rules");
            this.lv_Rules.MultiSelect = false;
            this.lv_Rules.Name = "lv_Rules";
            this.lv_Rules.UseCompatibleStateImageBehavior = false;
            this.lv_Rules.View = System.Windows.Forms.View.Details;
            this.lv_Rules.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lv_Rules_ItemChecked);
            this.lv_Rules.SelectedIndexChanged += new System.EventHandler(this.lv_Rules_SelectedIndexChanged);
            // 
            // lvc_Name
            // 
            resources.ApplyResources(this.lvc_Name, "lvc_Name");
            // 
            // lvc_Created
            // 
            resources.ApplyResources(this.lvc_Created, "lvc_Created");
            // 
            // lvc_Updated
            // 
            resources.ApplyResources(this.lvc_Updated, "lvc_Updated");
            // 
            // btn_RuleEdit
            // 
            resources.ApplyResources(this.btn_RuleEdit, "btn_RuleEdit");
            this.btn_RuleEdit.Name = "btn_RuleEdit";
            this.btn_RuleEdit.UseVisualStyleBackColor = true;
            this.btn_RuleEdit.Click += new System.EventHandler(this.btn_RuleEdit_Click);
            // 
            // btn_RuleClone
            // 
            resources.ApplyResources(this.btn_RuleClone, "btn_RuleClone");
            this.btn_RuleClone.Name = "btn_RuleClone";
            this.btn_RuleClone.UseVisualStyleBackColor = true;
            this.btn_RuleClone.Click += new System.EventHandler(this.btn_RuleClone_Click);
            // 
            // btn_RuleNew
            // 
            resources.ApplyResources(this.btn_RuleNew, "btn_RuleNew");
            this.btn_RuleNew.Name = "btn_RuleNew";
            this.btn_RuleNew.UseVisualStyleBackColor = true;
            this.btn_RuleNew.Click += new System.EventHandler(this.btn_RuleNew_Click);
            // 
            // btn_RuleRemove
            // 
            resources.ApplyResources(this.btn_RuleRemove, "btn_RuleRemove");
            this.btn_RuleRemove.Name = "btn_RuleRemove";
            this.btn_RuleRemove.UseVisualStyleBackColor = true;
            this.btn_RuleRemove.Click += new System.EventHandler(this.btn_RuleRemove_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_RuleProfiles);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cb_RuleProfiles
            // 
            this.cb_RuleProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_RuleProfiles.FormattingEnabled = true;
            resources.ApplyResources(this.cb_RuleProfiles, "cb_RuleProfiles");
            this.cb_RuleProfiles.Name = "cb_RuleProfiles";
            // 
            // MT_Plugins
            // 
            resources.ApplyResources(this.MT_Plugins, "MT_Plugins");
            this.MT_Plugins.Name = "MT_Plugins";
            this.MT_Plugins.UseVisualStyleBackColor = true;
            // 
            // MT_About
            // 
            this.MT_About.Controls.Add(this.lb_AppName);
            this.MT_About.Controls.Add(this.llb_Company);
            this.MT_About.Controls.Add(this.lbox_Env);
            this.MT_About.Controls.Add(this.lb_Copyright);
            this.MT_About.Controls.Add(this.lb_Version);
            this.MT_About.Controls.Add(this.lbVersion);
            this.MT_About.Controls.Add(this.pb_AboutLogo);
            resources.ApplyResources(this.MT_About, "MT_About");
            this.MT_About.Name = "MT_About";
            this.MT_About.UseVisualStyleBackColor = true;
            this.MT_About.Paint += new System.Windows.Forms.PaintEventHandler(this.MT_About_Paint);
            // 
            // lb_AppName
            // 
            resources.ApplyResources(this.lb_AppName, "lb_AppName");
            this.lb_AppName.Name = "lb_AppName";
            // 
            // llb_Company
            // 
            resources.ApplyResources(this.llb_Company, "llb_Company");
            this.llb_Company.Name = "llb_Company";
            this.llb_Company.TabStop = true;
            this.llb_Company.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llb_Company_LinkClicked);
            // 
            // lbox_Env
            // 
            this.lbox_Env.FormattingEnabled = true;
            resources.ApplyResources(this.lbox_Env, "lbox_Env");
            this.lbox_Env.Name = "lbox_Env";
            // 
            // lb_Copyright
            // 
            resources.ApplyResources(this.lb_Copyright, "lb_Copyright");
            this.lb_Copyright.Name = "lb_Copyright";
            // 
            // lb_Version
            // 
            resources.ApplyResources(this.lb_Version, "lb_Version");
            this.lb_Version.Name = "lb_Version";
            // 
            // lbVersion
            // 
            resources.ApplyResources(this.lbVersion, "lbVersion");
            this.lbVersion.Name = "lbVersion";
            // 
            // pb_AboutLogo
            // 
            resources.ApplyResources(this.pb_AboutLogo, "pb_AboutLogo");
            this.pb_AboutLogo.Name = "pb_AboutLogo";
            this.pb_AboutLogo.TabStop = false;
            // 
            // logCMS
            // 
            this.logCMS.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.logCMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.logCMS.Name = "logCMS";
            resources.ApplyResources(this.logCMS, "logCMS");
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // panel_Pages
            // 
            this.panel_Pages.Controls.Add(this.mainTabs);
            resources.ApplyResources(this.panel_Pages, "panel_Pages");
            this.panel_Pages.Name = "panel_Pages";
            // 
            // ts_PageNav
            // 
            this.ts_PageNav.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.ts_PageNav, "ts_PageNav");
            this.ts_PageNav.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ts_PageNav.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ts_PageNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_general,
            this.tsb_ui,
            this.tsb_rules,
            this.tsb_plugins,
            this.tsb_logs,
            this.tsb_about});
            this.ts_PageNav.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.ts_PageNav.Name = "ts_PageNav";
            // 
            // tsb_general
            // 
            resources.ApplyResources(this.tsb_general, "tsb_general");
            this.tsb_general.Name = "tsb_general";
            this.tsb_general.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_general.Tag = "";
            this.tsb_general.Click += new System.EventHandler(this.tsb_general_Click);
            // 
            // tsb_ui
            // 
            resources.ApplyResources(this.tsb_ui, "tsb_ui");
            this.tsb_ui.Name = "tsb_ui";
            this.tsb_ui.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_ui.Tag = "";
            this.tsb_ui.Click += new System.EventHandler(this.tsb_ui_Click);
            // 
            // tsb_rules
            // 
            resources.ApplyResources(this.tsb_rules, "tsb_rules");
            this.tsb_rules.Name = "tsb_rules";
            this.tsb_rules.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_rules.Tag = "";
            this.tsb_rules.Click += new System.EventHandler(this.tsb_rules_Click);
            // 
            // tsb_plugins
            // 
            resources.ApplyResources(this.tsb_plugins, "tsb_plugins");
            this.tsb_plugins.Name = "tsb_plugins";
            this.tsb_plugins.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_plugins.Tag = "";
            this.tsb_plugins.Click += new System.EventHandler(this.tsb_plugins_Click);
            // 
            // tsb_logs
            // 
            resources.ApplyResources(this.tsb_logs, "tsb_logs");
            this.tsb_logs.Name = "tsb_logs";
            this.tsb_logs.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_logs.Tag = "";
            this.tsb_logs.Click += new System.EventHandler(this.tsb_logs_Click);
            // 
            // tsb_about
            // 
            resources.ApplyResources(this.tsb_about, "tsb_about");
            this.tsb_about.Name = "tsb_about";
            this.tsb_about.Padding = new System.Windows.Forms.Padding(10);
            this.tsb_about.Tag = "";
            this.tsb_about.Click += new System.EventHandler(this.tsb_about_Click);
            // 
            // panel_PageNav
            // 
            this.panel_PageNav.Controls.Add(this.ts_PageNav);
            resources.ApplyResources(this.panel_PageNav, "panel_PageNav");
            this.panel_PageNav.Name = "panel_PageNav";
            // 
            // panel_mask
            // 
            resources.ApplyResources(this.panel_mask, "panel_mask");
            this.panel_mask.Name = "panel_mask";
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            resources.ApplyResources(this.mainStatusStrip, "mainStatusStrip");
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.SizingGrip = false;
            // 
            // niTray
            // 
            this.niTray.ContextMenuStrip = this.trayMenu;
            resources.ApplyResources(this.niTray, "niTray");
            // 
            // trayMenu
            // 
            this.trayMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.trayMenu.Name = "trayMenu";
            resources.ApplyResources(this.trayMenu, "trayMenu");
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // AppController
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.panel_mask);
            this.Controls.Add(this.panel_PageNav);
            this.Controls.Add(this.panel_Pages);
            this.Controls.Add(this.mainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mainMenu;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppController";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppController_FormClosing);
            this.Load += new System.EventHandler(this.AppController_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.MT_Logs.ResumeLayout(false);
            this.logTabs.ResumeLayout(false);
            this.logTabInfo.ResumeLayout(false);
            this.logTabInfo.PerformLayout();
            this.logTabDebug.ResumeLayout(false);
            this.logTabDebug.PerformLayout();
            this.logTabEvent.ResumeLayout(false);
            this.logTabEvent.PerformLayout();
            this.logTabWarning.ResumeLayout(false);
            this.logTabWarning.PerformLayout();
            this.logTabError.ResumeLayout(false);
            this.logTabError.PerformLayout();
            this.mainTabs.ResumeLayout(false);
            this.MT_Rules.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.MT_About.ResumeLayout(false);
            this.MT_About.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_AboutLogo)).EndInit();
            this.logCMS.ResumeLayout(false);
            this.panel_Pages.ResumeLayout(false);
            this.ts_PageNav.ResumeLayout(false);
            this.ts_PageNav.PerformLayout();
            this.panel_PageNav.ResumeLayout(false);
            this.panel_PageNav.PerformLayout();
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TabPage MT_Logs;
        private System.Windows.Forms.TabPage MT_General;
        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabControl logTabs;
        private System.Windows.Forms.TabPage logTabDebug;
        private System.Windows.Forms.TabPage logTabInfo;
        private System.Windows.Forms.TabPage logTabWarning;
        private System.Windows.Forms.TabPage logTabError;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.TextBox tbWarning;
        private System.Windows.Forms.TextBox tbError;
        private System.Windows.Forms.TextBox tbDebug;
        private System.Windows.Forms.ContextMenuStrip logCMS;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.TabPage logTabEvent;
        private System.Windows.Forms.TextBox tbEvent;
        private System.Windows.Forms.Panel panel_Pages;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem langToolStripMenuItem;
        private System.Windows.Forms.TabPage MT_UI;
        private System.Windows.Forms.TabPage MT_Rules;
        private System.Windows.Forms.ComboBox cb_RuleProfiles;
        private System.Windows.Forms.ListView lv_Rules;
        private System.Windows.Forms.ColumnHeader lvc_Name;
        private System.Windows.Forms.ColumnHeader lvc_Created;
        private System.Windows.Forms.ColumnHeader lvc_Updated;
        private System.Windows.Forms.Button btn_RuleRemove;
        private System.Windows.Forms.Button btn_RuleEdit;
        private System.Windows.Forms.Button btn_RuleClone;
        private System.Windows.Forms.Button btn_RuleNew;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showLogsInGuiToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ts_PageNav;
        private System.Windows.Forms.ToolStripButton tsb_general;
        private System.Windows.Forms.ToolStripButton tsb_ui;
        private System.Windows.Forms.ToolStripButton tsb_rules;
        private System.Windows.Forms.ToolStripButton tsb_plugins;
        private System.Windows.Forms.ToolStripButton tsb_logs;
        private System.Windows.Forms.Panel panel_PageNav;
        private System.Windows.Forms.Panel panel_mask;
        private System.Windows.Forms.TabPage MT_Plugins;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabPage MT_About;
        private System.Windows.Forms.LinkLabel llb_Company;
        private System.Windows.Forms.ListBox lbox_Env;
        private System.Windows.Forms.Label lb_Copyright;
        private System.Windows.Forms.Label lb_Version;
        private System.Windows.Forms.Label lbVersion;
        private System.Windows.Forms.PictureBox pb_AboutLogo;
        private System.Windows.Forms.ToolStripButton tsb_about;
        private System.Windows.Forms.Label lb_AppName;
    }
}