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
            this.closeThisWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.langToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLogsInGuiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.panel_General = new System.Windows.Forms.Panel();
            this.lb_dummy_Placeholder = new System.Windows.Forms.Label();
            this.gb_Cluster = new System.Windows.Forms.GroupBox();
            this.chb_showVDIndexOnTrayIcon = new System.Windows.Forms.CheckBox();
            this.chb_notify_vd_changed = new System.Windows.Forms.CheckBox();
            this.chb_HideMainViewIfItsShown = new System.Windows.Forms.CheckBox();
            this.gb_Mouse = new System.Windows.Forms.GroupBox();
            this.tc_Mouse = new System.Windows.Forms.TabControl();
            this.tp_mouse_action = new System.Windows.Forms.TabPage();
            this.btn_mouse_save = new System.Windows.Forms.Button();
            this.lb_mouse_action = new System.Windows.Forms.Label();
            this.cb_mouse_func = new System.Windows.Forms.ComboBox();
            this.tv_mouse = new System.Windows.Forms.TreeView();
            this.gb_Hotkey = new System.Windows.Forms.GroupBox();
            this.tc_Keyboard = new System.Windows.Forms.TabControl();
            this.tp_hk_main = new System.Windows.Forms.TabPage();
            this.tb_hk_tip = new System.Windows.Forms.TextBox();
            this.ts_HotkeySave = new System.Windows.Forms.ToolStrip();
            this.tssb_hk_save_reg = new System.Windows.Forms.ToolStripSplitButton();
            this.tsmi_hk_save = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_hk_reg = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_hk_clear_save = new System.Windows.Forms.ToolStripMenuItem();
            this.lb_hk_func = new System.Windows.Forms.Label();
            this.cb_hk_key = new System.Windows.Forms.ComboBox();
            this.cb_hk_shift = new System.Windows.Forms.CheckBox();
            this.cb_hk_alt = new System.Windows.Forms.CheckBox();
            this.cb_hk_ctrl = new System.Windows.Forms.CheckBox();
            this.cb_hk_win = new System.Windows.Forms.CheckBox();
            this.tp_hk_extra = new System.Windows.Forms.TabPage();
            this.lb_hk_extra = new System.Windows.Forms.Label();
            this.tv_keyboard = new System.Windows.Forms.TreeView();
            this.gb_nav = new System.Windows.Forms.GroupBox();
            this.lb_nav_circle_h_type = new System.Windows.Forms.Label();
            this.cb_nav_circle_h_type = new System.Windows.Forms.ComboBox();
            this.cb_nav_circle_v = new System.Windows.Forms.CheckBox();
            this.cb_nav_circle_h = new System.Windows.Forms.CheckBox();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.MT_UI = new System.Windows.Forms.TabPage();
            this.panel_UI = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_ui_vd_view = new System.Windows.Forms.GroupBox();
            this.rb_vd_index_1 = new System.Windows.Forms.RadioButton();
            this.rb_vd_index_0 = new System.Windows.Forms.RadioButton();
            this.chb_show_vd_index = new System.Windows.Forms.CheckBox();
            this.chb_show_vd_name = new System.Windows.Forms.CheckBox();
            this.gb_DesktopArrangement = new System.Windows.Forms.GroupBox();
            this.tlp_DesktopArrangement = new System.Windows.Forms.TableLayoutPanel();
            this.btn_m7 = new System.Windows.Forms.Button();
            this.btn_m6 = new System.Windows.Forms.Button();
            this.btn_m5 = new System.Windows.Forms.Button();
            this.btn_m4 = new System.Windows.Forms.Button();
            this.btn_m3 = new System.Windows.Forms.Button();
            this.btn_m2 = new System.Windows.Forms.Button();
            this.btn_m1 = new System.Windows.Forms.Button();
            this.btn_m0 = new System.Windows.Forms.Button();
            this.lb_DesktopArrangementNote = new System.Windows.Forms.Label();
            this.MT_Rules = new System.Windows.Forms.TabPage();
            this.gb_Rules = new System.Windows.Forms.GroupBox();
            this.lv_Rules = new System.Windows.Forms.ListView();
            this.lvc_Name = new System.Windows.Forms.ColumnHeader();
            this.lvc_Created = new System.Windows.Forms.ColumnHeader();
            this.lvc_Updated = new System.Windows.Forms.ColumnHeader();
            this.btn_RuleEdit = new System.Windows.Forms.Button();
            this.btn_RuleClone = new System.Windows.Forms.Button();
            this.btn_RuleNew = new System.Windows.Forms.Button();
            this.btn_RuleRemove = new System.Windows.Forms.Button();
            this.gb_CurrentProfile = new System.Windows.Forms.GroupBox();
            this.cb_RuleProfiles = new System.Windows.Forms.ComboBox();
            this.MT_Plugins = new System.Windows.Forms.TabPage();
            this.gb_Plugins = new System.Windows.Forms.GroupBox();
            this.lv_Plugins = new System.Windows.Forms.ListView();
            this.lvc_PluginName = new System.Windows.Forms.ColumnHeader();
            this.lvc_PluginVersion = new System.Windows.Forms.ColumnHeader();
            this.lvc_PluginAuthor = new System.Windows.Forms.ColumnHeader();
            this.lvc_PluginEmail = new System.Windows.Forms.ColumnHeader();
            this.btn_PluginSettings = new System.Windows.Forms.Button();
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
            this.MT_General.SuspendLayout();
            this.panel_General.SuspendLayout();
            this.gb_Cluster.SuspendLayout();
            this.gb_Mouse.SuspendLayout();
            this.tc_Mouse.SuspendLayout();
            this.tp_mouse_action.SuspendLayout();
            this.gb_Hotkey.SuspendLayout();
            this.tc_Keyboard.SuspendLayout();
            this.tp_hk_main.SuspendLayout();
            this.ts_HotkeySave.SuspendLayout();
            this.tp_hk_extra.SuspendLayout();
            this.gb_nav.SuspendLayout();
            this.mainTabs.SuspendLayout();
            this.MT_UI.SuspendLayout();
            this.panel_UI.SuspendLayout();
            this.lb_ui_vd_view.SuspendLayout();
            this.gb_DesktopArrangement.SuspendLayout();
            this.tlp_DesktopArrangement.SuspendLayout();
            this.MT_Rules.SuspendLayout();
            this.gb_Rules.SuspendLayout();
            this.gb_CurrentProfile.SuspendLayout();
            this.MT_Plugins.SuspendLayout();
            this.gb_Plugins.SuspendLayout();
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
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.mainMenu.Name = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeThisWindowToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // closeThisWindowToolStripMenuItem
            // 
            resources.ApplyResources(this.closeThisWindowToolStripMenuItem, "closeThisWindowToolStripMenuItem");
            this.closeThisWindowToolStripMenuItem.Name = "closeThisWindowToolStripMenuItem";
            this.closeThisWindowToolStripMenuItem.Click += new System.EventHandler(this.closeThisWindowToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            resources.ApplyResources(this.quitToolStripMenuItem, "quitToolStripMenuItem");
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.langToolStripMenuItem,
            this.logsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
            // 
            // langToolStripMenuItem
            // 
            resources.ApplyResources(this.langToolStripMenuItem, "langToolStripMenuItem");
            this.langToolStripMenuItem.Name = "langToolStripMenuItem";
            // 
            // logsToolStripMenuItem
            // 
            resources.ApplyResources(this.logsToolStripMenuItem, "logsToolStripMenuItem");
            this.logsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLogsInGuiToolStripMenuItem,
            this.openLogFolderToolStripMenuItem});
            this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            // 
            // showLogsInGuiToolStripMenuItem
            // 
            resources.ApplyResources(this.showLogsInGuiToolStripMenuItem, "showLogsInGuiToolStripMenuItem");
            this.showLogsInGuiToolStripMenuItem.CheckOnClick = true;
            this.showLogsInGuiToolStripMenuItem.Name = "showLogsInGuiToolStripMenuItem";
            this.showLogsInGuiToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showLogsInGuiToolStripMenuItem_CheckedChanged);
            // 
            // openLogFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.openLogFolderToolStripMenuItem, "openLogFolderToolStripMenuItem");
            this.openLogFolderToolStripMenuItem.Name = "openLogFolderToolStripMenuItem";
            this.openLogFolderToolStripMenuItem.Click += new System.EventHandler(this.openLogFolderToolStripMenuItem_Click);
            // 
            // MT_Logs
            // 
            resources.ApplyResources(this.MT_Logs, "MT_Logs");
            this.MT_Logs.Controls.Add(this.logTabs);
            this.MT_Logs.Name = "MT_Logs";
            this.MT_Logs.UseVisualStyleBackColor = true;
            // 
            // logTabs
            // 
            resources.ApplyResources(this.logTabs, "logTabs");
            this.logTabs.Controls.Add(this.logTabInfo);
            this.logTabs.Controls.Add(this.logTabDebug);
            this.logTabs.Controls.Add(this.logTabEvent);
            this.logTabs.Controls.Add(this.logTabWarning);
            this.logTabs.Controls.Add(this.logTabError);
            this.logTabs.Name = "logTabs";
            this.logTabs.SelectedIndex = 0;
            this.logTabs.Click += new System.EventHandler(this.logTabs_Click);
            // 
            // logTabInfo
            // 
            resources.ApplyResources(this.logTabInfo, "logTabInfo");
            this.logTabInfo.Controls.Add(this.tbInfo);
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
            resources.ApplyResources(this.logTabDebug, "logTabDebug");
            this.logTabDebug.Controls.Add(this.tbDebug);
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
            resources.ApplyResources(this.logTabEvent, "logTabEvent");
            this.logTabEvent.Controls.Add(this.tbEvent);
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
            resources.ApplyResources(this.logTabWarning, "logTabWarning");
            this.logTabWarning.Controls.Add(this.tbWarning);
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
            resources.ApplyResources(this.logTabError, "logTabError");
            this.logTabError.Controls.Add(this.tbError);
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
            this.MT_General.Controls.Add(this.panel_General);
            this.MT_General.Name = "MT_General";
            this.MT_General.UseVisualStyleBackColor = true;
            // 
            // panel_General
            // 
            resources.ApplyResources(this.panel_General, "panel_General");
            this.panel_General.Controls.Add(this.lb_dummy_Placeholder);
            this.panel_General.Controls.Add(this.gb_Cluster);
            this.panel_General.Controls.Add(this.gb_Mouse);
            this.panel_General.Controls.Add(this.gb_Hotkey);
            this.panel_General.Controls.Add(this.gb_nav);
            this.panel_General.Name = "panel_General";
            // 
            // lb_dummy_Placeholder
            // 
            resources.ApplyResources(this.lb_dummy_Placeholder, "lb_dummy_Placeholder");
            this.lb_dummy_Placeholder.Name = "lb_dummy_Placeholder";
            // 
            // gb_Cluster
            // 
            resources.ApplyResources(this.gb_Cluster, "gb_Cluster");
            this.gb_Cluster.Controls.Add(this.chb_showVDIndexOnTrayIcon);
            this.gb_Cluster.Controls.Add(this.chb_notify_vd_changed);
            this.gb_Cluster.Controls.Add(this.chb_HideMainViewIfItsShown);
            this.gb_Cluster.Name = "gb_Cluster";
            this.gb_Cluster.TabStop = false;
            // 
            // chb_showVDIndexOnTrayIcon
            // 
            resources.ApplyResources(this.chb_showVDIndexOnTrayIcon, "chb_showVDIndexOnTrayIcon");
            this.chb_showVDIndexOnTrayIcon.Name = "chb_showVDIndexOnTrayIcon";
            this.chb_showVDIndexOnTrayIcon.UseVisualStyleBackColor = true;
            this.chb_showVDIndexOnTrayIcon.CheckedChanged += new System.EventHandler(this.chb_showVDIndexOnTrayIcon_CheckedChanged);
            // 
            // chb_notify_vd_changed
            // 
            resources.ApplyResources(this.chb_notify_vd_changed, "chb_notify_vd_changed");
            this.chb_notify_vd_changed.Name = "chb_notify_vd_changed";
            this.chb_notify_vd_changed.UseVisualStyleBackColor = true;
            this.chb_notify_vd_changed.CheckedChanged += new System.EventHandler(this.chb_notify_vd_changed_CheckedChanged);
            // 
            // chb_HideMainViewIfItsShown
            // 
            resources.ApplyResources(this.chb_HideMainViewIfItsShown, "chb_HideMainViewIfItsShown");
            this.chb_HideMainViewIfItsShown.Name = "chb_HideMainViewIfItsShown";
            this.chb_HideMainViewIfItsShown.UseVisualStyleBackColor = true;
            this.chb_HideMainViewIfItsShown.CheckedChanged += new System.EventHandler(this.chb_HideMainViewIfItsShown_CheckedChanged);
            // 
            // gb_Mouse
            // 
            resources.ApplyResources(this.gb_Mouse, "gb_Mouse");
            this.gb_Mouse.Controls.Add(this.tc_Mouse);
            this.gb_Mouse.Controls.Add(this.tv_mouse);
            this.gb_Mouse.Name = "gb_Mouse";
            this.gb_Mouse.TabStop = false;
            // 
            // tc_Mouse
            // 
            resources.ApplyResources(this.tc_Mouse, "tc_Mouse");
            this.tc_Mouse.Controls.Add(this.tp_mouse_action);
            this.tc_Mouse.Name = "tc_Mouse";
            this.tc_Mouse.SelectedIndex = 0;
            // 
            // tp_mouse_action
            // 
            resources.ApplyResources(this.tp_mouse_action, "tp_mouse_action");
            this.tp_mouse_action.Controls.Add(this.btn_mouse_save);
            this.tp_mouse_action.Controls.Add(this.lb_mouse_action);
            this.tp_mouse_action.Controls.Add(this.cb_mouse_func);
            this.tp_mouse_action.Name = "tp_mouse_action";
            this.tp_mouse_action.UseVisualStyleBackColor = true;
            // 
            // btn_mouse_save
            // 
            resources.ApplyResources(this.btn_mouse_save, "btn_mouse_save");
            this.btn_mouse_save.Name = "btn_mouse_save";
            this.btn_mouse_save.UseVisualStyleBackColor = true;
            this.btn_mouse_save.Click += new System.EventHandler(this.btn_mouse_save_Click);
            // 
            // lb_mouse_action
            // 
            resources.ApplyResources(this.lb_mouse_action, "lb_mouse_action");
            this.lb_mouse_action.Name = "lb_mouse_action";
            // 
            // cb_mouse_func
            // 
            resources.ApplyResources(this.cb_mouse_func, "cb_mouse_func");
            this.cb_mouse_func.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_mouse_func.FormattingEnabled = true;
            this.cb_mouse_func.Name = "cb_mouse_func";
            // 
            // tv_mouse
            // 
            resources.ApplyResources(this.tv_mouse, "tv_mouse");
            this.tv_mouse.Name = "tv_mouse";
            this.tv_mouse.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tv_mouse.Nodes")))});
            this.tv_mouse.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_mouse_AfterSelect);
            // 
            // gb_Hotkey
            // 
            resources.ApplyResources(this.gb_Hotkey, "gb_Hotkey");
            this.gb_Hotkey.Controls.Add(this.tc_Keyboard);
            this.gb_Hotkey.Controls.Add(this.tv_keyboard);
            this.gb_Hotkey.Name = "gb_Hotkey";
            this.gb_Hotkey.TabStop = false;
            // 
            // tc_Keyboard
            // 
            resources.ApplyResources(this.tc_Keyboard, "tc_Keyboard");
            this.tc_Keyboard.Controls.Add(this.tp_hk_main);
            this.tc_Keyboard.Controls.Add(this.tp_hk_extra);
            this.tc_Keyboard.Name = "tc_Keyboard";
            this.tc_Keyboard.SelectedIndex = 0;
            // 
            // tp_hk_main
            // 
            resources.ApplyResources(this.tp_hk_main, "tp_hk_main");
            this.tp_hk_main.Controls.Add(this.tb_hk_tip);
            this.tp_hk_main.Controls.Add(this.ts_HotkeySave);
            this.tp_hk_main.Controls.Add(this.lb_hk_func);
            this.tp_hk_main.Controls.Add(this.cb_hk_key);
            this.tp_hk_main.Controls.Add(this.cb_hk_shift);
            this.tp_hk_main.Controls.Add(this.cb_hk_alt);
            this.tp_hk_main.Controls.Add(this.cb_hk_ctrl);
            this.tp_hk_main.Controls.Add(this.cb_hk_win);
            this.tp_hk_main.Name = "tp_hk_main";
            this.tp_hk_main.UseVisualStyleBackColor = true;
            // 
            // tb_hk_tip
            // 
            resources.ApplyResources(this.tb_hk_tip, "tb_hk_tip");
            this.tb_hk_tip.Name = "tb_hk_tip";
            // 
            // ts_HotkeySave
            // 
            resources.ApplyResources(this.ts_HotkeySave, "ts_HotkeySave");
            this.ts_HotkeySave.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ts_HotkeySave.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ts_HotkeySave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssb_hk_save_reg});
            this.ts_HotkeySave.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.ts_HotkeySave.Name = "ts_HotkeySave";
            this.ts_HotkeySave.ShowItemToolTips = false;
            // 
            // tssb_hk_save_reg
            // 
            resources.ApplyResources(this.tssb_hk_save_reg, "tssb_hk_save_reg");
            this.tssb_hk_save_reg.BackColor = System.Drawing.SystemColors.Control;
            this.tssb_hk_save_reg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssb_hk_save_reg.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_hk_save,
            this.tsmi_hk_reg,
            this.tsmi_hk_clear_save});
            this.tssb_hk_save_reg.Margin = new System.Windows.Forms.Padding(0);
            this.tssb_hk_save_reg.Name = "tssb_hk_save_reg";
            this.tssb_hk_save_reg.ButtonClick += new System.EventHandler(this.tssb_hk_save_reg_ButtonClick);
            this.tssb_hk_save_reg.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tssb_hk_save_reg_DropDownItemClicked);
            // 
            // tsmi_hk_save
            // 
            resources.ApplyResources(this.tsmi_hk_save, "tsmi_hk_save");
            this.tsmi_hk_save.Name = "tsmi_hk_save";
            // 
            // tsmi_hk_reg
            // 
            resources.ApplyResources(this.tsmi_hk_reg, "tsmi_hk_reg");
            this.tsmi_hk_reg.Name = "tsmi_hk_reg";
            // 
            // tsmi_hk_clear_save
            // 
            resources.ApplyResources(this.tsmi_hk_clear_save, "tsmi_hk_clear_save");
            this.tsmi_hk_clear_save.Name = "tsmi_hk_clear_save";
            // 
            // lb_hk_func
            // 
            resources.ApplyResources(this.lb_hk_func, "lb_hk_func");
            this.lb_hk_func.Name = "lb_hk_func";
            // 
            // cb_hk_key
            // 
            resources.ApplyResources(this.cb_hk_key, "cb_hk_key");
            this.cb_hk_key.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_hk_key.FormattingEnabled = true;
            this.cb_hk_key.Items.AddRange(new object[] {
            resources.GetString("cb_hk_key.Items"),
            resources.GetString("cb_hk_key.Items1"),
            resources.GetString("cb_hk_key.Items2"),
            resources.GetString("cb_hk_key.Items3"),
            resources.GetString("cb_hk_key.Items4"),
            resources.GetString("cb_hk_key.Items5"),
            resources.GetString("cb_hk_key.Items6"),
            resources.GetString("cb_hk_key.Items7"),
            resources.GetString("cb_hk_key.Items8"),
            resources.GetString("cb_hk_key.Items9"),
            resources.GetString("cb_hk_key.Items10"),
            resources.GetString("cb_hk_key.Items11"),
            resources.GetString("cb_hk_key.Items12"),
            resources.GetString("cb_hk_key.Items13"),
            resources.GetString("cb_hk_key.Items14"),
            resources.GetString("cb_hk_key.Items15"),
            resources.GetString("cb_hk_key.Items16"),
            resources.GetString("cb_hk_key.Items17"),
            resources.GetString("cb_hk_key.Items18"),
            resources.GetString("cb_hk_key.Items19"),
            resources.GetString("cb_hk_key.Items20"),
            resources.GetString("cb_hk_key.Items21"),
            resources.GetString("cb_hk_key.Items22"),
            resources.GetString("cb_hk_key.Items23"),
            resources.GetString("cb_hk_key.Items24"),
            resources.GetString("cb_hk_key.Items25"),
            resources.GetString("cb_hk_key.Items26"),
            resources.GetString("cb_hk_key.Items27"),
            resources.GetString("cb_hk_key.Items28"),
            resources.GetString("cb_hk_key.Items29"),
            resources.GetString("cb_hk_key.Items30"),
            resources.GetString("cb_hk_key.Items31"),
            resources.GetString("cb_hk_key.Items32"),
            resources.GetString("cb_hk_key.Items33"),
            resources.GetString("cb_hk_key.Items34"),
            resources.GetString("cb_hk_key.Items35"),
            resources.GetString("cb_hk_key.Items36"),
            resources.GetString("cb_hk_key.Items37"),
            resources.GetString("cb_hk_key.Items38"),
            resources.GetString("cb_hk_key.Items39"),
            resources.GetString("cb_hk_key.Items40"),
            resources.GetString("cb_hk_key.Items41"),
            resources.GetString("cb_hk_key.Items42"),
            resources.GetString("cb_hk_key.Items43"),
            resources.GetString("cb_hk_key.Items44"),
            resources.GetString("cb_hk_key.Items45"),
            resources.GetString("cb_hk_key.Items46"),
            resources.GetString("cb_hk_key.Items47"),
            resources.GetString("cb_hk_key.Items48"),
            resources.GetString("cb_hk_key.Items49"),
            resources.GetString("cb_hk_key.Items50"),
            resources.GetString("cb_hk_key.Items51"),
            resources.GetString("cb_hk_key.Items52"),
            resources.GetString("cb_hk_key.Items53"),
            resources.GetString("cb_hk_key.Items54"),
            resources.GetString("cb_hk_key.Items55"),
            resources.GetString("cb_hk_key.Items56"),
            resources.GetString("cb_hk_key.Items57"),
            resources.GetString("cb_hk_key.Items58"),
            resources.GetString("cb_hk_key.Items59"),
            resources.GetString("cb_hk_key.Items60"),
            resources.GetString("cb_hk_key.Items61"),
            resources.GetString("cb_hk_key.Items62"),
            resources.GetString("cb_hk_key.Items63"),
            resources.GetString("cb_hk_key.Items64"),
            resources.GetString("cb_hk_key.Items65"),
            resources.GetString("cb_hk_key.Items66"),
            resources.GetString("cb_hk_key.Items67")});
            this.cb_hk_key.Name = "cb_hk_key";
            this.cb_hk_key.SelectedIndexChanged += new System.EventHandler(this.cb_hk_key_SelectedIndexChanged);
            // 
            // cb_hk_shift
            // 
            resources.ApplyResources(this.cb_hk_shift, "cb_hk_shift");
            this.cb_hk_shift.Name = "cb_hk_shift";
            this.cb_hk_shift.UseVisualStyleBackColor = true;
            this.cb_hk_shift.CheckedChanged += new System.EventHandler(this.cb_hk_shift_CheckedChanged);
            // 
            // cb_hk_alt
            // 
            resources.ApplyResources(this.cb_hk_alt, "cb_hk_alt");
            this.cb_hk_alt.Name = "cb_hk_alt";
            this.cb_hk_alt.UseVisualStyleBackColor = true;
            this.cb_hk_alt.CheckedChanged += new System.EventHandler(this.cb_hk_alt_CheckedChanged);
            // 
            // cb_hk_ctrl
            // 
            resources.ApplyResources(this.cb_hk_ctrl, "cb_hk_ctrl");
            this.cb_hk_ctrl.Name = "cb_hk_ctrl";
            this.cb_hk_ctrl.UseVisualStyleBackColor = true;
            this.cb_hk_ctrl.CheckedChanged += new System.EventHandler(this.cb_hk_ctrl_CheckedChanged);
            // 
            // cb_hk_win
            // 
            resources.ApplyResources(this.cb_hk_win, "cb_hk_win");
            this.cb_hk_win.Name = "cb_hk_win";
            this.cb_hk_win.UseVisualStyleBackColor = true;
            this.cb_hk_win.CheckedChanged += new System.EventHandler(this.cb_hk_win_CheckedChanged);
            // 
            // tp_hk_extra
            // 
            resources.ApplyResources(this.tp_hk_extra, "tp_hk_extra");
            this.tp_hk_extra.Controls.Add(this.lb_hk_extra);
            this.tp_hk_extra.Name = "tp_hk_extra";
            this.tp_hk_extra.UseVisualStyleBackColor = true;
            // 
            // lb_hk_extra
            // 
            resources.ApplyResources(this.lb_hk_extra, "lb_hk_extra");
            this.lb_hk_extra.Name = "lb_hk_extra";
            // 
            // tv_keyboard
            // 
            resources.ApplyResources(this.tv_keyboard, "tv_keyboard");
            this.tv_keyboard.Name = "tv_keyboard";
            this.tv_keyboard.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tv_keyboard.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tv_keyboard.Nodes1")))});
            this.tv_keyboard.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_keyboard_AfterSelect);
            // 
            // gb_nav
            // 
            resources.ApplyResources(this.gb_nav, "gb_nav");
            this.gb_nav.Controls.Add(this.lb_nav_circle_h_type);
            this.gb_nav.Controls.Add(this.cb_nav_circle_h_type);
            this.gb_nav.Controls.Add(this.cb_nav_circle_v);
            this.gb_nav.Controls.Add(this.cb_nav_circle_h);
            this.gb_nav.Name = "gb_nav";
            this.gb_nav.TabStop = false;
            // 
            // lb_nav_circle_h_type
            // 
            resources.ApplyResources(this.lb_nav_circle_h_type, "lb_nav_circle_h_type");
            this.lb_nav_circle_h_type.Name = "lb_nav_circle_h_type";
            // 
            // cb_nav_circle_h_type
            // 
            resources.ApplyResources(this.cb_nav_circle_h_type, "cb_nav_circle_h_type");
            this.cb_nav_circle_h_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_nav_circle_h_type.FormattingEnabled = true;
            this.cb_nav_circle_h_type.Name = "cb_nav_circle_h_type";
            this.cb_nav_circle_h_type.SelectedIndexChanged += new System.EventHandler(this.cb_nav_circle_h_type_SelectedIndexChanged);
            // 
            // cb_nav_circle_v
            // 
            resources.ApplyResources(this.cb_nav_circle_v, "cb_nav_circle_v");
            this.cb_nav_circle_v.Name = "cb_nav_circle_v";
            this.cb_nav_circle_v.UseVisualStyleBackColor = true;
            this.cb_nav_circle_v.CheckedChanged += new System.EventHandler(this.cb_nav_circle_v_CheckedChanged);
            // 
            // cb_nav_circle_h
            // 
            resources.ApplyResources(this.cb_nav_circle_h, "cb_nav_circle_h");
            this.cb_nav_circle_h.Name = "cb_nav_circle_h";
            this.cb_nav_circle_h.UseVisualStyleBackColor = true;
            this.cb_nav_circle_h.CheckedChanged += new System.EventHandler(this.cb_nav_circle_h_CheckedChanged);
            // 
            // mainTabs
            // 
            resources.ApplyResources(this.mainTabs, "mainTabs");
            this.mainTabs.Controls.Add(this.MT_General);
            this.mainTabs.Controls.Add(this.MT_UI);
            this.mainTabs.Controls.Add(this.MT_Rules);
            this.mainTabs.Controls.Add(this.MT_Plugins);
            this.mainTabs.Controls.Add(this.MT_Logs);
            this.mainTabs.Controls.Add(this.MT_About);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.SelectedIndexChanged += new System.EventHandler(this.mainTabs_SelectedIndexChanged);
            // 
            // MT_UI
            // 
            resources.ApplyResources(this.MT_UI, "MT_UI");
            this.MT_UI.Controls.Add(this.panel_UI);
            this.MT_UI.Name = "MT_UI";
            this.MT_UI.UseVisualStyleBackColor = true;
            // 
            // panel_UI
            // 
            resources.ApplyResources(this.panel_UI, "panel_UI");
            this.panel_UI.Controls.Add(this.label1);
            this.panel_UI.Controls.Add(this.lb_ui_vd_view);
            this.panel_UI.Controls.Add(this.gb_DesktopArrangement);
            this.panel_UI.Name = "panel_UI";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lb_ui_vd_view
            // 
            resources.ApplyResources(this.lb_ui_vd_view, "lb_ui_vd_view");
            this.lb_ui_vd_view.Controls.Add(this.rb_vd_index_1);
            this.lb_ui_vd_view.Controls.Add(this.rb_vd_index_0);
            this.lb_ui_vd_view.Controls.Add(this.chb_show_vd_index);
            this.lb_ui_vd_view.Controls.Add(this.chb_show_vd_name);
            this.lb_ui_vd_view.Name = "lb_ui_vd_view";
            this.lb_ui_vd_view.TabStop = false;
            // 
            // rb_vd_index_1
            // 
            resources.ApplyResources(this.rb_vd_index_1, "rb_vd_index_1");
            this.rb_vd_index_1.Name = "rb_vd_index_1";
            this.rb_vd_index_1.TabStop = true;
            this.rb_vd_index_1.UseVisualStyleBackColor = true;
            this.rb_vd_index_1.CheckedChanged += new System.EventHandler(this.rb_vd_index_1_CheckedChanged);
            // 
            // rb_vd_index_0
            // 
            resources.ApplyResources(this.rb_vd_index_0, "rb_vd_index_0");
            this.rb_vd_index_0.Name = "rb_vd_index_0";
            this.rb_vd_index_0.TabStop = true;
            this.rb_vd_index_0.UseVisualStyleBackColor = true;
            this.rb_vd_index_0.CheckedChanged += new System.EventHandler(this.rb_vd_index_0_CheckedChanged);
            // 
            // chb_show_vd_index
            // 
            resources.ApplyResources(this.chb_show_vd_index, "chb_show_vd_index");
            this.chb_show_vd_index.Name = "chb_show_vd_index";
            this.chb_show_vd_index.UseVisualStyleBackColor = true;
            this.chb_show_vd_index.CheckedChanged += new System.EventHandler(this.chb_show_vd_index_CheckedChanged);
            // 
            // chb_show_vd_name
            // 
            resources.ApplyResources(this.chb_show_vd_name, "chb_show_vd_name");
            this.chb_show_vd_name.Name = "chb_show_vd_name";
            this.chb_show_vd_name.UseVisualStyleBackColor = true;
            this.chb_show_vd_name.CheckedChanged += new System.EventHandler(this.chb_show_vd_name_CheckedChanged);
            // 
            // gb_DesktopArrangement
            // 
            resources.ApplyResources(this.gb_DesktopArrangement, "gb_DesktopArrangement");
            this.gb_DesktopArrangement.Controls.Add(this.tlp_DesktopArrangement);
            this.gb_DesktopArrangement.Controls.Add(this.lb_DesktopArrangementNote);
            this.gb_DesktopArrangement.Name = "gb_DesktopArrangement";
            this.gb_DesktopArrangement.TabStop = false;
            // 
            // tlp_DesktopArrangement
            // 
            resources.ApplyResources(this.tlp_DesktopArrangement, "tlp_DesktopArrangement");
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m7, 3, 1);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m6, 2, 1);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m5, 1, 1);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m4, 0, 1);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m3, 3, 0);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m2, 2, 0);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m1, 1, 0);
            this.tlp_DesktopArrangement.Controls.Add(this.btn_m0, 0, 0);
            this.tlp_DesktopArrangement.Name = "tlp_DesktopArrangement";
            // 
            // btn_m7
            // 
            resources.ApplyResources(this.btn_m7, "btn_m7");
            this.btn_m7.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m7.Name = "btn_m7";
            this.btn_m7.UseVisualStyleBackColor = false;
            this.btn_m7.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m6
            // 
            resources.ApplyResources(this.btn_m6, "btn_m6");
            this.btn_m6.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m6.Name = "btn_m6";
            this.btn_m6.UseVisualStyleBackColor = false;
            this.btn_m6.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m5
            // 
            resources.ApplyResources(this.btn_m5, "btn_m5");
            this.btn_m5.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m5.Name = "btn_m5";
            this.btn_m5.UseVisualStyleBackColor = false;
            this.btn_m5.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m4
            // 
            resources.ApplyResources(this.btn_m4, "btn_m4");
            this.btn_m4.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m4.Name = "btn_m4";
            this.btn_m4.UseVisualStyleBackColor = false;
            this.btn_m4.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m3
            // 
            resources.ApplyResources(this.btn_m3, "btn_m3");
            this.btn_m3.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m3.Name = "btn_m3";
            this.btn_m3.UseVisualStyleBackColor = false;
            this.btn_m3.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m2
            // 
            resources.ApplyResources(this.btn_m2, "btn_m2");
            this.btn_m2.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m2.Name = "btn_m2";
            this.btn_m2.UseVisualStyleBackColor = false;
            this.btn_m2.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m1
            // 
            resources.ApplyResources(this.btn_m1, "btn_m1");
            this.btn_m1.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_m1.Name = "btn_m1";
            this.btn_m1.UseVisualStyleBackColor = false;
            this.btn_m1.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // btn_m0
            // 
            resources.ApplyResources(this.btn_m0, "btn_m0");
            this.btn_m0.BackColor = System.Drawing.Color.MistyRose;
            this.btn_m0.Name = "btn_m0";
            this.btn_m0.UseVisualStyleBackColor = false;
            this.btn_m0.Click += new System.EventHandler(this.tlp_DesktopArrangement_SubControlClicked);
            // 
            // lb_DesktopArrangementNote
            // 
            resources.ApplyResources(this.lb_DesktopArrangementNote, "lb_DesktopArrangementNote");
            this.lb_DesktopArrangementNote.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lb_DesktopArrangementNote.Name = "lb_DesktopArrangementNote";
            // 
            // MT_Rules
            // 
            resources.ApplyResources(this.MT_Rules, "MT_Rules");
            this.MT_Rules.Controls.Add(this.gb_Rules);
            this.MT_Rules.Controls.Add(this.gb_CurrentProfile);
            this.MT_Rules.Name = "MT_Rules";
            this.MT_Rules.UseVisualStyleBackColor = true;
            // 
            // gb_Rules
            // 
            resources.ApplyResources(this.gb_Rules, "gb_Rules");
            this.gb_Rules.Controls.Add(this.lv_Rules);
            this.gb_Rules.Controls.Add(this.btn_RuleEdit);
            this.gb_Rules.Controls.Add(this.btn_RuleClone);
            this.gb_Rules.Controls.Add(this.btn_RuleNew);
            this.gb_Rules.Controls.Add(this.btn_RuleRemove);
            this.gb_Rules.Name = "gb_Rules";
            this.gb_Rules.TabStop = false;
            // 
            // lv_Rules
            // 
            resources.ApplyResources(this.lv_Rules, "lv_Rules");
            this.lv_Rules.CheckBoxes = true;
            this.lv_Rules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvc_Name,
            this.lvc_Created,
            this.lvc_Updated});
            this.lv_Rules.FullRowSelect = true;
            this.lv_Rules.GridLines = true;
            this.lv_Rules.HideSelection = false;
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
            // gb_CurrentProfile
            // 
            resources.ApplyResources(this.gb_CurrentProfile, "gb_CurrentProfile");
            this.gb_CurrentProfile.Controls.Add(this.cb_RuleProfiles);
            this.gb_CurrentProfile.Name = "gb_CurrentProfile";
            this.gb_CurrentProfile.TabStop = false;
            // 
            // cb_RuleProfiles
            // 
            resources.ApplyResources(this.cb_RuleProfiles, "cb_RuleProfiles");
            this.cb_RuleProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_RuleProfiles.FormattingEnabled = true;
            this.cb_RuleProfiles.Name = "cb_RuleProfiles";
            // 
            // MT_Plugins
            // 
            resources.ApplyResources(this.MT_Plugins, "MT_Plugins");
            this.MT_Plugins.Controls.Add(this.gb_Plugins);
            this.MT_Plugins.Name = "MT_Plugins";
            this.MT_Plugins.UseVisualStyleBackColor = true;
            // 
            // gb_Plugins
            // 
            resources.ApplyResources(this.gb_Plugins, "gb_Plugins");
            this.gb_Plugins.Controls.Add(this.lv_Plugins);
            this.gb_Plugins.Controls.Add(this.btn_PluginSettings);
            this.gb_Plugins.Name = "gb_Plugins";
            this.gb_Plugins.TabStop = false;
            // 
            // lv_Plugins
            // 
            resources.ApplyResources(this.lv_Plugins, "lv_Plugins");
            this.lv_Plugins.CheckBoxes = true;
            this.lv_Plugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvc_PluginName,
            this.lvc_PluginVersion,
            this.lvc_PluginAuthor,
            this.lvc_PluginEmail});
            this.lv_Plugins.FullRowSelect = true;
            this.lv_Plugins.GridLines = true;
            this.lv_Plugins.HideSelection = false;
            this.lv_Plugins.MultiSelect = false;
            this.lv_Plugins.Name = "lv_Plugins";
            this.lv_Plugins.UseCompatibleStateImageBehavior = false;
            this.lv_Plugins.View = System.Windows.Forms.View.Details;
            this.lv_Plugins.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lv_Plugins_ItemChecked);
            this.lv_Plugins.SelectedIndexChanged += new System.EventHandler(this.lv_Plugins_SelectedIndexChanged);
            // 
            // lvc_PluginName
            // 
            resources.ApplyResources(this.lvc_PluginName, "lvc_PluginName");
            // 
            // lvc_PluginVersion
            // 
            resources.ApplyResources(this.lvc_PluginVersion, "lvc_PluginVersion");
            // 
            // lvc_PluginAuthor
            // 
            resources.ApplyResources(this.lvc_PluginAuthor, "lvc_PluginAuthor");
            // 
            // lvc_PluginEmail
            // 
            resources.ApplyResources(this.lvc_PluginEmail, "lvc_PluginEmail");
            // 
            // btn_PluginSettings
            // 
            resources.ApplyResources(this.btn_PluginSettings, "btn_PluginSettings");
            this.btn_PluginSettings.Name = "btn_PluginSettings";
            this.btn_PluginSettings.UseVisualStyleBackColor = true;
            this.btn_PluginSettings.Click += new System.EventHandler(this.btn_PluginSettings_Click);
            // 
            // MT_About
            // 
            resources.ApplyResources(this.MT_About, "MT_About");
            this.MT_About.Controls.Add(this.lb_AppName);
            this.MT_About.Controls.Add(this.llb_Company);
            this.MT_About.Controls.Add(this.lbox_Env);
            this.MT_About.Controls.Add(this.lb_Copyright);
            this.MT_About.Controls.Add(this.lb_Version);
            this.MT_About.Controls.Add(this.lbVersion);
            this.MT_About.Controls.Add(this.pb_AboutLogo);
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
            resources.ApplyResources(this.lbox_Env, "lbox_Env");
            this.lbox_Env.FormattingEnabled = true;
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
            resources.ApplyResources(this.logCMS, "logCMS");
            this.logCMS.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.logCMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.logCMS.Name = "logCMS";
            // 
            // clearToolStripMenuItem
            // 
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // panel_Pages
            // 
            resources.ApplyResources(this.panel_Pages, "panel_Pages");
            this.panel_Pages.Controls.Add(this.mainTabs);
            this.panel_Pages.Name = "panel_Pages";
            // 
            // ts_PageNav
            // 
            resources.ApplyResources(this.ts_PageNav, "ts_PageNav");
            this.ts_PageNav.BackColor = System.Drawing.Color.White;
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
            resources.ApplyResources(this.panel_PageNav, "panel_PageNav");
            this.panel_PageNav.Controls.Add(this.ts_PageNav);
            this.panel_PageNav.Name = "panel_PageNav";
            // 
            // panel_mask
            // 
            resources.ApplyResources(this.panel_mask, "panel_mask");
            this.panel_mask.Name = "panel_mask";
            // 
            // mainStatusStrip
            // 
            resources.ApplyResources(this.mainStatusStrip, "mainStatusStrip");
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.SizingGrip = false;
            // 
            // niTray
            // 
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.ContextMenuStrip = this.trayMenu;
            // 
            // trayMenu
            // 
            resources.ApplyResources(this.trayMenu, "trayMenu");
            this.trayMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.trayMenu.Name = "trayMenu";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
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
            this.MT_General.ResumeLayout(false);
            this.panel_General.ResumeLayout(false);
            this.panel_General.PerformLayout();
            this.gb_Cluster.ResumeLayout(false);
            this.gb_Mouse.ResumeLayout(false);
            this.tc_Mouse.ResumeLayout(false);
            this.tp_mouse_action.ResumeLayout(false);
            this.tp_mouse_action.PerformLayout();
            this.gb_Hotkey.ResumeLayout(false);
            this.tc_Keyboard.ResumeLayout(false);
            this.tp_hk_main.ResumeLayout(false);
            this.tp_hk_main.PerformLayout();
            this.ts_HotkeySave.ResumeLayout(false);
            this.ts_HotkeySave.PerformLayout();
            this.tp_hk_extra.ResumeLayout(false);
            this.gb_nav.ResumeLayout(false);
            this.gb_nav.PerformLayout();
            this.mainTabs.ResumeLayout(false);
            this.MT_UI.ResumeLayout(false);
            this.panel_UI.ResumeLayout(false);
            this.panel_UI.PerformLayout();
            this.lb_ui_vd_view.ResumeLayout(false);
            this.lb_ui_vd_view.PerformLayout();
            this.gb_DesktopArrangement.ResumeLayout(false);
            this.tlp_DesktopArrangement.ResumeLayout(false);
            this.MT_Rules.ResumeLayout(false);
            this.gb_Rules.ResumeLayout(false);
            this.gb_CurrentProfile.ResumeLayout(false);
            this.MT_Plugins.ResumeLayout(false);
            this.gb_Plugins.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox gb_Rules;
        private System.Windows.Forms.GroupBox gb_CurrentProfile;
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
        private System.Windows.Forms.GroupBox gb_Plugins;
        private System.Windows.Forms.ListView lv_Plugins;
        private System.Windows.Forms.ColumnHeader lvc_PluginName;
        private System.Windows.Forms.ColumnHeader lvc_PluginVersion;
        private System.Windows.Forms.ColumnHeader lvc_PluginAuthor;
        private System.Windows.Forms.ColumnHeader lvc_PluginEmail;
        private System.Windows.Forms.Button btn_PluginSettings;
        private System.Windows.Forms.ToolStripMenuItem openLogFolderToolStripMenuItem;
        private System.Windows.Forms.GroupBox gb_nav;
        private System.Windows.Forms.CheckBox cb_nav_circle_v;
        private System.Windows.Forms.CheckBox cb_nav_circle_h;
        private System.Windows.Forms.Label lb_nav_circle_h_type;
        private System.Windows.Forms.ComboBox cb_nav_circle_h_type;
        private System.Windows.Forms.Panel panel_UI;
        private System.Windows.Forms.GroupBox gb_DesktopArrangement;
        private System.Windows.Forms.Label lb_DesktopArrangementNote;
        private System.Windows.Forms.TableLayoutPanel tlp_DesktopArrangement;
        private System.Windows.Forms.Button btn_m7;
        private System.Windows.Forms.Button btn_m6;
        private System.Windows.Forms.Button btn_m5;
        private System.Windows.Forms.Button btn_m4;
        private System.Windows.Forms.Button btn_m3;
        private System.Windows.Forms.Button btn_m2;
        private System.Windows.Forms.Button btn_m1;
        private System.Windows.Forms.Button btn_m0;
        private System.Windows.Forms.Panel panel_General;
        private System.Windows.Forms.GroupBox gb_Cluster;
        private System.Windows.Forms.GroupBox gb_Mouse;
        private System.Windows.Forms.GroupBox gb_Hotkey;
        private System.Windows.Forms.TreeView tv_keyboard;
        private System.Windows.Forms.TabControl tc_Keyboard;
        private System.Windows.Forms.TabPage tp_hk_main;
        private System.Windows.Forms.TextBox tb_hk_tip;
        private System.Windows.Forms.ComboBox cb_hk_key;
        private System.Windows.Forms.CheckBox cb_hk_shift;
        private System.Windows.Forms.CheckBox cb_hk_alt;
        private System.Windows.Forms.CheckBox cb_hk_ctrl;
        private System.Windows.Forms.CheckBox cb_hk_win;
        private System.Windows.Forms.TabPage tp_hk_extra;
        private System.Windows.Forms.Label lb_hk_func;
        private System.Windows.Forms.ToolStrip ts_HotkeySave;
        private System.Windows.Forms.ToolStripSplitButton tssb_hk_save_reg;
        private System.Windows.Forms.ToolStripMenuItem tsmi_hk_save;
        private System.Windows.Forms.ToolStripMenuItem tsmi_hk_reg;
        private System.Windows.Forms.ToolStripMenuItem tsmi_hk_clear_save;
        private System.Windows.Forms.Label lb_hk_extra;
        private System.Windows.Forms.ToolStripMenuItem closeThisWindowToolStripMenuItem;
        private System.Windows.Forms.Label lb_dummy_Placeholder;
        private System.Windows.Forms.TreeView tv_mouse;
        private System.Windows.Forms.TabControl tc_Mouse;
        private System.Windows.Forms.TabPage tp_mouse_action;
        private System.Windows.Forms.Button btn_mouse_save;
        private System.Windows.Forms.Label lb_mouse_action;
        private System.Windows.Forms.ComboBox cb_mouse_func;
        private System.Windows.Forms.GroupBox lb_ui_vd_view;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rb_vd_index_1;
        private System.Windows.Forms.RadioButton rb_vd_index_0;
        private System.Windows.Forms.CheckBox chb_show_vd_index;
        private System.Windows.Forms.CheckBox chb_show_vd_name;
        private System.Windows.Forms.CheckBox chb_HideMainViewIfItsShown;
        private System.Windows.Forms.CheckBox chb_notify_vd_changed;
        private System.Windows.Forms.CheckBox chb_showVDIndexOnTrayIcon;
    }
}