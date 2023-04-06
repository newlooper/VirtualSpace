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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppController));
            mainMenu = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeThisWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            runAsAdministratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tsmiMainMenuRestart = new System.Windows.Forms.ToolStripMenuItem();
            tsmiMainMenuQuit = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            langToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showLogsInGuiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openLogFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MT_Logs = new System.Windows.Forms.TabPage();
            logTabs = new System.Windows.Forms.TabControl();
            logTabInfo = new System.Windows.Forms.TabPage();
            tbInfo = new System.Windows.Forms.TextBox();
            logTabDebug = new System.Windows.Forms.TabPage();
            tbDebug = new System.Windows.Forms.TextBox();
            logTabVerbose = new System.Windows.Forms.TabPage();
            tbVerbose = new System.Windows.Forms.TextBox();
            logTabEvent = new System.Windows.Forms.TabPage();
            tbEvent = new System.Windows.Forms.TextBox();
            logTabWarning = new System.Windows.Forms.TabPage();
            tbWarning = new System.Windows.Forms.TextBox();
            logTabError = new System.Windows.Forms.TabPage();
            tbError = new System.Windows.Forms.TextBox();
            MT_General = new System.Windows.Forms.TabPage();
            panel_General = new System.Windows.Forms.Panel();
            tab_General = new System.Windows.Forms.TabControl();
            tabPage_Genernal_Main = new System.Windows.Forms.TabPage();
            gb_profiles = new System.Windows.Forms.GroupBox();
            lb_profiles_note = new System.Windows.Forms.Label();
            btn_profile_del = new System.Windows.Forms.Button();
            btn_profile_rename = new System.Windows.Forms.Button();
            btn_profile_dup = new System.Windows.Forms.Button();
            cbb_profiles = new System.Windows.Forms.ComboBox();
            gb_storage = new System.Windows.Forms.GroupBox();
            lb_configRoot = new System.Windows.Forms.Label();
            btn_chooseConfigRoot = new System.Windows.Forms.Button();
            tb_configRoot = new System.Windows.Forms.TextBox();
            lb_note_configRoot = new System.Windows.Forms.Label();
            gb_general = new System.Windows.Forms.GroupBox();
            llb_TaskScheduler = new System.Windows.Forms.LinkLabel();
            lb_RunOnStartup = new System.Windows.Forms.Label();
            chb_RunOnStartup = new System.Windows.Forms.CheckBox();
            gb_nav = new System.Windows.Forms.GroupBox();
            lb_nav_circle_h_type = new System.Windows.Forms.Label();
            cb_nav_circle_h_type = new System.Windows.Forms.ComboBox();
            cb_nav_circle_v = new System.Windows.Forms.CheckBox();
            cb_nav_circle_h = new System.Windows.Forms.CheckBox();
            gb_Cluster = new System.Windows.Forms.GroupBox();
            rb_vdi_on_tray_style_2 = new System.Windows.Forms.RadioButton();
            rb_vdi_on_tray_style_1 = new System.Windows.Forms.RadioButton();
            rb_vdi_on_tray_style_0 = new System.Windows.Forms.RadioButton();
            chb_HideOnStart = new System.Windows.Forms.CheckBox();
            chb_showVDIndexOnTrayIcon = new System.Windows.Forms.CheckBox();
            chb_notify_vd_changed = new System.Windows.Forms.CheckBox();
            chb_HideMainViewIfItsShown = new System.Windows.Forms.CheckBox();
            tabPage_Genernal_Keyboard = new System.Windows.Forms.TabPage();
            tc_Keyboard = new System.Windows.Forms.TabControl();
            tp_hk_main = new System.Windows.Forms.TabPage();
            btn_hk_ClearAndSave = new System.Windows.Forms.Button();
            btn_hk_RegAndSave = new System.Windows.Forms.Button();
            tb_hk_tip = new System.Windows.Forms.TextBox();
            lb_hk_func = new System.Windows.Forms.Label();
            cb_hk_key = new System.Windows.Forms.ComboBox();
            cb_hk_shift = new System.Windows.Forms.CheckBox();
            cb_hk_alt = new System.Windows.Forms.CheckBox();
            cb_hk_ctrl = new System.Windows.Forms.CheckBox();
            cb_hk_win = new System.Windows.Forms.CheckBox();
            tp_hk_extra = new System.Windows.Forms.TabPage();
            lb_hk_extra = new System.Windows.Forms.Label();
            tv_keyboard = new System.Windows.Forms.TreeView();
            tabPage_Genernal_Mouse = new System.Windows.Forms.TabPage();
            lb_MouseOnTaskbarSwitchDesktop2 = new System.Windows.Forms.Label();
            tv_mouse = new System.Windows.Forms.TreeView();
            lb_MouseOnTaskbarSwitchDesktop1 = new System.Windows.Forms.Label();
            tc_Mouse = new System.Windows.Forms.TabControl();
            tp_mouse_action = new System.Windows.Forms.TabPage();
            btn_mouse_save = new System.Windows.Forms.Button();
            lb_mouse_action = new System.Windows.Forms.Label();
            cb_mouse_func = new System.Windows.Forms.ComboBox();
            chb_MouseOnTaskbarSwitchDesktop = new System.Windows.Forms.CheckBox();
            mainTabs = new System.Windows.Forms.TabControl();
            MT_UI = new System.Windows.Forms.TabPage();
            panel_UI = new System.Windows.Forms.Panel();
            lb_ui_vd_view = new System.Windows.Forms.GroupBox();
            rb_vd_index_1 = new System.Windows.Forms.RadioButton();
            rb_vd_index_0 = new System.Windows.Forms.RadioButton();
            chb_show_vd_index = new System.Windows.Forms.CheckBox();
            chb_show_vd_name = new System.Windows.Forms.CheckBox();
            gb_DesktopArrangement = new System.Windows.Forms.GroupBox();
            tlp_DesktopArrangement = new System.Windows.Forms.TableLayoutPanel();
            btn_m7 = new System.Windows.Forms.Button();
            btn_m6 = new System.Windows.Forms.Button();
            btn_m5 = new System.Windows.Forms.Button();
            btn_m4 = new System.Windows.Forms.Button();
            btn_m3 = new System.Windows.Forms.Button();
            btn_m2 = new System.Windows.Forms.Button();
            btn_m1 = new System.Windows.Forms.Button();
            btn_m0 = new System.Windows.Forms.Button();
            lb_DesktopArrangementNote = new System.Windows.Forms.Label();
            MT_Rules = new System.Windows.Forms.TabPage();
            gb_Rules = new System.Windows.Forms.GroupBox();
            lv_Rules = new System.Windows.Forms.ListView();
            lvc_Name = new System.Windows.Forms.ColumnHeader();
            lvc_Created = new System.Windows.Forms.ColumnHeader();
            lvc_Updated = new System.Windows.Forms.ColumnHeader();
            btn_RuleEdit = new System.Windows.Forms.Button();
            btn_RuleClone = new System.Windows.Forms.Button();
            btn_RuleNew = new System.Windows.Forms.Button();
            btn_RuleRemove = new System.Windows.Forms.Button();
            gb_CurrentProfile = new System.Windows.Forms.GroupBox();
            llb_goto_general = new System.Windows.Forms.LinkLabel();
            cb_RuleProfiles = new System.Windows.Forms.ComboBox();
            MT_Plugins = new System.Windows.Forms.TabPage();
            gb_Plugins = new System.Windows.Forms.GroupBox();
            lv_Plugins = new System.Windows.Forms.ListView();
            lvc_PluginName = new System.Windows.Forms.ColumnHeader();
            lvc_PluginVersion = new System.Windows.Forms.ColumnHeader();
            lvc_PluginAuthor = new System.Windows.Forms.ColumnHeader();
            lvc_PluginEmail = new System.Windows.Forms.ColumnHeader();
            btn_PluginSettings = new System.Windows.Forms.Button();
            MT_About = new System.Windows.Forms.TabPage();
            lb_AppName = new System.Windows.Forms.Label();
            llb_Company = new System.Windows.Forms.LinkLabel();
            lbox_Env = new System.Windows.Forms.ListBox();
            lb_Copyright = new System.Windows.Forms.Label();
            lb_Version = new System.Windows.Forms.Label();
            lbVersion = new System.Windows.Forms.Label();
            pb_AboutLogo = new System.Windows.Forms.PictureBox();
            logCMS = new System.Windows.Forms.ContextMenuStrip(components);
            clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            panel_Pages = new System.Windows.Forms.Panel();
            ts_PageNav = new System.Windows.Forms.ToolStrip();
            tsb_general = new System.Windows.Forms.ToolStripButton();
            tsb_ui = new System.Windows.Forms.ToolStripButton();
            tsb_rules = new System.Windows.Forms.ToolStripButton();
            tsb_plugins = new System.Windows.Forms.ToolStripButton();
            tsb_logs = new System.Windows.Forms.ToolStripButton();
            tsb_about = new System.Windows.Forms.ToolStripButton();
            panel_PageNav = new System.Windows.Forms.Panel();
            panel_mask = new System.Windows.Forms.Panel();
            mainStatusStrip = new System.Windows.Forms.StatusStrip();
            tssl_main_tips = new System.Windows.Forms.ToolStripStatusLabel();
            mainMenu.SuspendLayout();
            MT_Logs.SuspendLayout();
            logTabs.SuspendLayout();
            logTabInfo.SuspendLayout();
            logTabDebug.SuspendLayout();
            logTabVerbose.SuspendLayout();
            logTabEvent.SuspendLayout();
            logTabWarning.SuspendLayout();
            logTabError.SuspendLayout();
            MT_General.SuspendLayout();
            panel_General.SuspendLayout();
            tab_General.SuspendLayout();
            tabPage_Genernal_Main.SuspendLayout();
            gb_profiles.SuspendLayout();
            gb_storage.SuspendLayout();
            gb_general.SuspendLayout();
            gb_nav.SuspendLayout();
            gb_Cluster.SuspendLayout();
            tabPage_Genernal_Keyboard.SuspendLayout();
            tc_Keyboard.SuspendLayout();
            tp_hk_main.SuspendLayout();
            tp_hk_extra.SuspendLayout();
            tabPage_Genernal_Mouse.SuspendLayout();
            tc_Mouse.SuspendLayout();
            tp_mouse_action.SuspendLayout();
            mainTabs.SuspendLayout();
            MT_UI.SuspendLayout();
            panel_UI.SuspendLayout();
            lb_ui_vd_view.SuspendLayout();
            gb_DesktopArrangement.SuspendLayout();
            tlp_DesktopArrangement.SuspendLayout();
            MT_Rules.SuspendLayout();
            gb_Rules.SuspendLayout();
            gb_CurrentProfile.SuspendLayout();
            MT_Plugins.SuspendLayout();
            gb_Plugins.SuspendLayout();
            MT_About.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pb_AboutLogo).BeginInit();
            logCMS.SuspendLayout();
            panel_Pages.SuspendLayout();
            ts_PageNav.SuspendLayout();
            panel_PageNav.SuspendLayout();
            mainStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            resources.ApplyResources(mainMenu, "mainMenu");
            mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem });
            mainMenu.Name = "mainMenu";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(fileToolStripMenuItem, "fileToolStripMenuItem");
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { closeThisWindowToolStripMenuItem, runAsAdministratorToolStripMenuItem, toolStripSeparator1, tsmiMainMenuRestart, tsmiMainMenuQuit });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // closeThisWindowToolStripMenuItem
            // 
            resources.ApplyResources(closeThisWindowToolStripMenuItem, "closeThisWindowToolStripMenuItem");
            closeThisWindowToolStripMenuItem.Name = "closeThisWindowToolStripMenuItem";
            closeThisWindowToolStripMenuItem.Click += closeThisWindowToolStripMenuItem_Click;
            // 
            // runAsAdministratorToolStripMenuItem
            // 
            resources.ApplyResources(runAsAdministratorToolStripMenuItem, "runAsAdministratorToolStripMenuItem");
            runAsAdministratorToolStripMenuItem.Name = "runAsAdministratorToolStripMenuItem";
            runAsAdministratorToolStripMenuItem.Click += runAsAdministratorToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // tsmiMainMenuRestart
            // 
            resources.ApplyResources(tsmiMainMenuRestart, "tsmiMainMenuRestart");
            tsmiMainMenuRestart.Name = "tsmiMainMenuRestart";
            tsmiMainMenuRestart.Click += tsmiMainMenuRestart_Click;
            // 
            // tsmiMainMenuQuit
            // 
            resources.ApplyResources(tsmiMainMenuQuit, "tsmiMainMenuQuit");
            tsmiMainMenuQuit.Name = "tsmiMainMenuQuit";
            tsmiMainMenuQuit.Click += tsmiMainMenuQuit_Click;
            // 
            // optionsToolStripMenuItem
            // 
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { langToolStripMenuItem, logsToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.DropDownOpening += optionsToolStripMenuItem_DropDownOpening;
            // 
            // langToolStripMenuItem
            // 
            resources.ApplyResources(langToolStripMenuItem, "langToolStripMenuItem");
            langToolStripMenuItem.Name = "langToolStripMenuItem";
            // 
            // logsToolStripMenuItem
            // 
            resources.ApplyResources(logsToolStripMenuItem, "logsToolStripMenuItem");
            logsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { showLogsInGuiToolStripMenuItem, openLogFolderToolStripMenuItem });
            logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            // 
            // showLogsInGuiToolStripMenuItem
            // 
            resources.ApplyResources(showLogsInGuiToolStripMenuItem, "showLogsInGuiToolStripMenuItem");
            showLogsInGuiToolStripMenuItem.CheckOnClick = true;
            showLogsInGuiToolStripMenuItem.Name = "showLogsInGuiToolStripMenuItem";
            // 
            // openLogFolderToolStripMenuItem
            // 
            resources.ApplyResources(openLogFolderToolStripMenuItem, "openLogFolderToolStripMenuItem");
            openLogFolderToolStripMenuItem.Name = "openLogFolderToolStripMenuItem";
            openLogFolderToolStripMenuItem.Click += openLogFolderToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(helpToolStripMenuItem, "helpToolStripMenuItem");
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // MT_Logs
            // 
            resources.ApplyResources(MT_Logs, "MT_Logs");
            MT_Logs.Controls.Add(logTabs);
            MT_Logs.Name = "MT_Logs";
            MT_Logs.UseVisualStyleBackColor = true;
            // 
            // logTabs
            // 
            resources.ApplyResources(logTabs, "logTabs");
            logTabs.Controls.Add(logTabInfo);
            logTabs.Controls.Add(logTabDebug);
            logTabs.Controls.Add(logTabVerbose);
            logTabs.Controls.Add(logTabEvent);
            logTabs.Controls.Add(logTabWarning);
            logTabs.Controls.Add(logTabError);
            logTabs.Name = "logTabs";
            logTabs.SelectedIndex = 0;
            logTabs.Click += logTabs_Click;
            // 
            // logTabInfo
            // 
            resources.ApplyResources(logTabInfo, "logTabInfo");
            logTabInfo.Controls.Add(tbInfo);
            logTabInfo.Name = "logTabInfo";
            logTabInfo.UseVisualStyleBackColor = true;
            // 
            // tbInfo
            // 
            resources.ApplyResources(tbInfo, "tbInfo");
            tbInfo.Name = "tbInfo";
            tbInfo.ReadOnly = true;
            // 
            // logTabDebug
            // 
            resources.ApplyResources(logTabDebug, "logTabDebug");
            logTabDebug.Controls.Add(tbDebug);
            logTabDebug.Name = "logTabDebug";
            logTabDebug.UseVisualStyleBackColor = true;
            // 
            // tbDebug
            // 
            resources.ApplyResources(tbDebug, "tbDebug");
            tbDebug.Name = "tbDebug";
            tbDebug.ReadOnly = true;
            // 
            // logTabVerbose
            // 
            resources.ApplyResources(logTabVerbose, "logTabVerbose");
            logTabVerbose.Controls.Add(tbVerbose);
            logTabVerbose.Name = "logTabVerbose";
            logTabVerbose.UseVisualStyleBackColor = true;
            // 
            // tbVerbose
            // 
            resources.ApplyResources(tbVerbose, "tbVerbose");
            tbVerbose.Name = "tbVerbose";
            tbVerbose.ReadOnly = true;
            // 
            // logTabEvent
            // 
            resources.ApplyResources(logTabEvent, "logTabEvent");
            logTabEvent.Controls.Add(tbEvent);
            logTabEvent.Name = "logTabEvent";
            logTabEvent.UseVisualStyleBackColor = true;
            // 
            // tbEvent
            // 
            resources.ApplyResources(tbEvent, "tbEvent");
            tbEvent.Name = "tbEvent";
            tbEvent.ReadOnly = true;
            // 
            // logTabWarning
            // 
            resources.ApplyResources(logTabWarning, "logTabWarning");
            logTabWarning.Controls.Add(tbWarning);
            logTabWarning.Name = "logTabWarning";
            logTabWarning.UseVisualStyleBackColor = true;
            // 
            // tbWarning
            // 
            resources.ApplyResources(tbWarning, "tbWarning");
            tbWarning.Name = "tbWarning";
            tbWarning.ReadOnly = true;
            // 
            // logTabError
            // 
            resources.ApplyResources(logTabError, "logTabError");
            logTabError.Controls.Add(tbError);
            logTabError.Name = "logTabError";
            logTabError.UseVisualStyleBackColor = true;
            // 
            // tbError
            // 
            resources.ApplyResources(tbError, "tbError");
            tbError.Name = "tbError";
            tbError.ReadOnly = true;
            // 
            // MT_General
            // 
            resources.ApplyResources(MT_General, "MT_General");
            MT_General.Controls.Add(panel_General);
            MT_General.Name = "MT_General";
            MT_General.UseVisualStyleBackColor = true;
            // 
            // panel_General
            // 
            resources.ApplyResources(panel_General, "panel_General");
            panel_General.Controls.Add(tab_General);
            panel_General.Name = "panel_General";
            // 
            // tab_General
            // 
            resources.ApplyResources(tab_General, "tab_General");
            tab_General.Controls.Add(tabPage_Genernal_Main);
            tab_General.Controls.Add(tabPage_Genernal_Keyboard);
            tab_General.Controls.Add(tabPage_Genernal_Mouse);
            tab_General.Name = "tab_General";
            tab_General.SelectedIndex = 0;
            // 
            // tabPage_Genernal_Main
            // 
            resources.ApplyResources(tabPage_Genernal_Main, "tabPage_Genernal_Main");
            tabPage_Genernal_Main.Controls.Add(gb_profiles);
            tabPage_Genernal_Main.Controls.Add(gb_storage);
            tabPage_Genernal_Main.Controls.Add(gb_general);
            tabPage_Genernal_Main.Controls.Add(gb_nav);
            tabPage_Genernal_Main.Controls.Add(gb_Cluster);
            tabPage_Genernal_Main.Name = "tabPage_Genernal_Main";
            tabPage_Genernal_Main.UseVisualStyleBackColor = true;
            // 
            // gb_profiles
            // 
            resources.ApplyResources(gb_profiles, "gb_profiles");
            gb_profiles.Controls.Add(lb_profiles_note);
            gb_profiles.Controls.Add(btn_profile_del);
            gb_profiles.Controls.Add(btn_profile_rename);
            gb_profiles.Controls.Add(btn_profile_dup);
            gb_profiles.Controls.Add(cbb_profiles);
            gb_profiles.Name = "gb_profiles";
            gb_profiles.TabStop = false;
            // 
            // lb_profiles_note
            // 
            resources.ApplyResources(lb_profiles_note, "lb_profiles_note");
            lb_profiles_note.Name = "lb_profiles_note";
            // 
            // btn_profile_del
            // 
            resources.ApplyResources(btn_profile_del, "btn_profile_del");
            btn_profile_del.Name = "btn_profile_del";
            btn_profile_del.UseVisualStyleBackColor = true;
            btn_profile_del.Click += btn_profile_del_Click;
            // 
            // btn_profile_rename
            // 
            resources.ApplyResources(btn_profile_rename, "btn_profile_rename");
            btn_profile_rename.Name = "btn_profile_rename";
            btn_profile_rename.UseVisualStyleBackColor = true;
            btn_profile_rename.Click += btn_profile_rename_Click;
            // 
            // btn_profile_dup
            // 
            resources.ApplyResources(btn_profile_dup, "btn_profile_dup");
            btn_profile_dup.Name = "btn_profile_dup";
            btn_profile_dup.UseVisualStyleBackColor = true;
            btn_profile_dup.Click += btn_profile_dup_Click;
            // 
            // cbb_profiles
            // 
            resources.ApplyResources(cbb_profiles, "cbb_profiles");
            cbb_profiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbb_profiles.FormattingEnabled = true;
            cbb_profiles.Name = "cbb_profiles";
            // 
            // gb_storage
            // 
            resources.ApplyResources(gb_storage, "gb_storage");
            gb_storage.Controls.Add(lb_configRoot);
            gb_storage.Controls.Add(btn_chooseConfigRoot);
            gb_storage.Controls.Add(tb_configRoot);
            gb_storage.Controls.Add(lb_note_configRoot);
            gb_storage.Name = "gb_storage";
            gb_storage.TabStop = false;
            // 
            // lb_configRoot
            // 
            resources.ApplyResources(lb_configRoot, "lb_configRoot");
            lb_configRoot.Name = "lb_configRoot";
            // 
            // btn_chooseConfigRoot
            // 
            resources.ApplyResources(btn_chooseConfigRoot, "btn_chooseConfigRoot");
            btn_chooseConfigRoot.Name = "btn_chooseConfigRoot";
            btn_chooseConfigRoot.UseVisualStyleBackColor = true;
            btn_chooseConfigRoot.Click += btn_chooseConfigRoot_Click;
            // 
            // tb_configRoot
            // 
            resources.ApplyResources(tb_configRoot, "tb_configRoot");
            tb_configRoot.Name = "tb_configRoot";
            tb_configRoot.ReadOnly = true;
            // 
            // lb_note_configRoot
            // 
            resources.ApplyResources(lb_note_configRoot, "lb_note_configRoot");
            lb_note_configRoot.Name = "lb_note_configRoot";
            // 
            // gb_general
            // 
            resources.ApplyResources(gb_general, "gb_general");
            gb_general.Controls.Add(llb_TaskScheduler);
            gb_general.Controls.Add(lb_RunOnStartup);
            gb_general.Controls.Add(chb_RunOnStartup);
            gb_general.Name = "gb_general";
            gb_general.TabStop = false;
            // 
            // llb_TaskScheduler
            // 
            resources.ApplyResources(llb_TaskScheduler, "llb_TaskScheduler");
            llb_TaskScheduler.Name = "llb_TaskScheduler";
            llb_TaskScheduler.TabStop = true;
            llb_TaskScheduler.LinkClicked += llb_TaskScheduler_LinkClicked;
            // 
            // lb_RunOnStartup
            // 
            resources.ApplyResources(lb_RunOnStartup, "lb_RunOnStartup");
            lb_RunOnStartup.Name = "lb_RunOnStartup";
            // 
            // chb_RunOnStartup
            // 
            resources.ApplyResources(chb_RunOnStartup, "chb_RunOnStartup");
            chb_RunOnStartup.Name = "chb_RunOnStartup";
            chb_RunOnStartup.UseVisualStyleBackColor = true;
            chb_RunOnStartup.CheckedChanged += chb_RunOnStartup_CheckedChanged;
            chb_RunOnStartup.VisibleChanged += chb_RunOnStartup_VisibleChanged;
            // 
            // gb_nav
            // 
            resources.ApplyResources(gb_nav, "gb_nav");
            gb_nav.Controls.Add(lb_nav_circle_h_type);
            gb_nav.Controls.Add(cb_nav_circle_h_type);
            gb_nav.Controls.Add(cb_nav_circle_v);
            gb_nav.Controls.Add(cb_nav_circle_h);
            gb_nav.Name = "gb_nav";
            gb_nav.TabStop = false;
            // 
            // lb_nav_circle_h_type
            // 
            resources.ApplyResources(lb_nav_circle_h_type, "lb_nav_circle_h_type");
            lb_nav_circle_h_type.Name = "lb_nav_circle_h_type";
            // 
            // cb_nav_circle_h_type
            // 
            resources.ApplyResources(cb_nav_circle_h_type, "cb_nav_circle_h_type");
            cb_nav_circle_h_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_nav_circle_h_type.FormattingEnabled = true;
            cb_nav_circle_h_type.Name = "cb_nav_circle_h_type";
            // 
            // cb_nav_circle_v
            // 
            resources.ApplyResources(cb_nav_circle_v, "cb_nav_circle_v");
            cb_nav_circle_v.Name = "cb_nav_circle_v";
            cb_nav_circle_v.UseVisualStyleBackColor = true;
            // 
            // cb_nav_circle_h
            // 
            resources.ApplyResources(cb_nav_circle_h, "cb_nav_circle_h");
            cb_nav_circle_h.Name = "cb_nav_circle_h";
            cb_nav_circle_h.UseVisualStyleBackColor = true;
            // 
            // gb_Cluster
            // 
            resources.ApplyResources(gb_Cluster, "gb_Cluster");
            gb_Cluster.Controls.Add(rb_vdi_on_tray_style_2);
            gb_Cluster.Controls.Add(rb_vdi_on_tray_style_1);
            gb_Cluster.Controls.Add(rb_vdi_on_tray_style_0);
            gb_Cluster.Controls.Add(chb_HideOnStart);
            gb_Cluster.Controls.Add(chb_showVDIndexOnTrayIcon);
            gb_Cluster.Controls.Add(chb_notify_vd_changed);
            gb_Cluster.Controls.Add(chb_HideMainViewIfItsShown);
            gb_Cluster.Name = "gb_Cluster";
            gb_Cluster.TabStop = false;
            // 
            // rb_vdi_on_tray_style_2
            // 
            resources.ApplyResources(rb_vdi_on_tray_style_2, "rb_vdi_on_tray_style_2");
            rb_vdi_on_tray_style_2.Name = "rb_vdi_on_tray_style_2";
            rb_vdi_on_tray_style_2.TabStop = true;
            rb_vdi_on_tray_style_2.UseVisualStyleBackColor = true;
            // 
            // rb_vdi_on_tray_style_1
            // 
            resources.ApplyResources(rb_vdi_on_tray_style_1, "rb_vdi_on_tray_style_1");
            rb_vdi_on_tray_style_1.Name = "rb_vdi_on_tray_style_1";
            rb_vdi_on_tray_style_1.TabStop = true;
            rb_vdi_on_tray_style_1.UseVisualStyleBackColor = true;
            // 
            // rb_vdi_on_tray_style_0
            // 
            resources.ApplyResources(rb_vdi_on_tray_style_0, "rb_vdi_on_tray_style_0");
            rb_vdi_on_tray_style_0.Name = "rb_vdi_on_tray_style_0";
            rb_vdi_on_tray_style_0.TabStop = true;
            rb_vdi_on_tray_style_0.UseVisualStyleBackColor = true;
            // 
            // chb_HideOnStart
            // 
            resources.ApplyResources(chb_HideOnStart, "chb_HideOnStart");
            chb_HideOnStart.Name = "chb_HideOnStart";
            chb_HideOnStart.UseVisualStyleBackColor = true;
            // 
            // chb_showVDIndexOnTrayIcon
            // 
            resources.ApplyResources(chb_showVDIndexOnTrayIcon, "chb_showVDIndexOnTrayIcon");
            chb_showVDIndexOnTrayIcon.Name = "chb_showVDIndexOnTrayIcon";
            chb_showVDIndexOnTrayIcon.UseVisualStyleBackColor = true;
            // 
            // chb_notify_vd_changed
            // 
            resources.ApplyResources(chb_notify_vd_changed, "chb_notify_vd_changed");
            chb_notify_vd_changed.Name = "chb_notify_vd_changed";
            chb_notify_vd_changed.UseVisualStyleBackColor = true;
            // 
            // chb_HideMainViewIfItsShown
            // 
            resources.ApplyResources(chb_HideMainViewIfItsShown, "chb_HideMainViewIfItsShown");
            chb_HideMainViewIfItsShown.Name = "chb_HideMainViewIfItsShown";
            chb_HideMainViewIfItsShown.UseVisualStyleBackColor = true;
            // 
            // tabPage_Genernal_Keyboard
            // 
            resources.ApplyResources(tabPage_Genernal_Keyboard, "tabPage_Genernal_Keyboard");
            tabPage_Genernal_Keyboard.Controls.Add(tc_Keyboard);
            tabPage_Genernal_Keyboard.Controls.Add(tv_keyboard);
            tabPage_Genernal_Keyboard.Name = "tabPage_Genernal_Keyboard";
            tabPage_Genernal_Keyboard.UseVisualStyleBackColor = true;
            // 
            // tc_Keyboard
            // 
            resources.ApplyResources(tc_Keyboard, "tc_Keyboard");
            tc_Keyboard.Controls.Add(tp_hk_main);
            tc_Keyboard.Controls.Add(tp_hk_extra);
            tc_Keyboard.Name = "tc_Keyboard";
            tc_Keyboard.SelectedIndex = 0;
            // 
            // tp_hk_main
            // 
            resources.ApplyResources(tp_hk_main, "tp_hk_main");
            tp_hk_main.Controls.Add(btn_hk_ClearAndSave);
            tp_hk_main.Controls.Add(btn_hk_RegAndSave);
            tp_hk_main.Controls.Add(tb_hk_tip);
            tp_hk_main.Controls.Add(lb_hk_func);
            tp_hk_main.Controls.Add(cb_hk_key);
            tp_hk_main.Controls.Add(cb_hk_shift);
            tp_hk_main.Controls.Add(cb_hk_alt);
            tp_hk_main.Controls.Add(cb_hk_ctrl);
            tp_hk_main.Controls.Add(cb_hk_win);
            tp_hk_main.Name = "tp_hk_main";
            tp_hk_main.UseVisualStyleBackColor = true;
            // 
            // btn_hk_ClearAndSave
            // 
            resources.ApplyResources(btn_hk_ClearAndSave, "btn_hk_ClearAndSave");
            btn_hk_ClearAndSave.Name = "btn_hk_ClearAndSave";
            btn_hk_ClearAndSave.UseVisualStyleBackColor = true;
            btn_hk_ClearAndSave.Click += btn_hk_ClearAndSave_Click;
            // 
            // btn_hk_RegAndSave
            // 
            resources.ApplyResources(btn_hk_RegAndSave, "btn_hk_RegAndSave");
            btn_hk_RegAndSave.Name = "btn_hk_RegAndSave";
            btn_hk_RegAndSave.UseVisualStyleBackColor = true;
            btn_hk_RegAndSave.Click += btn_hk_RegAndSave_Click;
            // 
            // tb_hk_tip
            // 
            resources.ApplyResources(tb_hk_tip, "tb_hk_tip");
            tb_hk_tip.Name = "tb_hk_tip";
            // 
            // lb_hk_func
            // 
            resources.ApplyResources(lb_hk_func, "lb_hk_func");
            lb_hk_func.Name = "lb_hk_func";
            // 
            // cb_hk_key
            // 
            resources.ApplyResources(cb_hk_key, "cb_hk_key");
            cb_hk_key.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_hk_key.FormattingEnabled = true;
            cb_hk_key.Items.AddRange(new object[] { resources.GetString("cb_hk_key.Items"), resources.GetString("cb_hk_key.Items1"), resources.GetString("cb_hk_key.Items2"), resources.GetString("cb_hk_key.Items3"), resources.GetString("cb_hk_key.Items4"), resources.GetString("cb_hk_key.Items5"), resources.GetString("cb_hk_key.Items6"), resources.GetString("cb_hk_key.Items7"), resources.GetString("cb_hk_key.Items8"), resources.GetString("cb_hk_key.Items9"), resources.GetString("cb_hk_key.Items10"), resources.GetString("cb_hk_key.Items11"), resources.GetString("cb_hk_key.Items12"), resources.GetString("cb_hk_key.Items13"), resources.GetString("cb_hk_key.Items14"), resources.GetString("cb_hk_key.Items15"), resources.GetString("cb_hk_key.Items16"), resources.GetString("cb_hk_key.Items17"), resources.GetString("cb_hk_key.Items18"), resources.GetString("cb_hk_key.Items19"), resources.GetString("cb_hk_key.Items20"), resources.GetString("cb_hk_key.Items21"), resources.GetString("cb_hk_key.Items22"), resources.GetString("cb_hk_key.Items23"), resources.GetString("cb_hk_key.Items24"), resources.GetString("cb_hk_key.Items25"), resources.GetString("cb_hk_key.Items26"), resources.GetString("cb_hk_key.Items27"), resources.GetString("cb_hk_key.Items28"), resources.GetString("cb_hk_key.Items29"), resources.GetString("cb_hk_key.Items30"), resources.GetString("cb_hk_key.Items31"), resources.GetString("cb_hk_key.Items32"), resources.GetString("cb_hk_key.Items33"), resources.GetString("cb_hk_key.Items34"), resources.GetString("cb_hk_key.Items35"), resources.GetString("cb_hk_key.Items36"), resources.GetString("cb_hk_key.Items37"), resources.GetString("cb_hk_key.Items38"), resources.GetString("cb_hk_key.Items39"), resources.GetString("cb_hk_key.Items40"), resources.GetString("cb_hk_key.Items41"), resources.GetString("cb_hk_key.Items42"), resources.GetString("cb_hk_key.Items43"), resources.GetString("cb_hk_key.Items44"), resources.GetString("cb_hk_key.Items45"), resources.GetString("cb_hk_key.Items46"), resources.GetString("cb_hk_key.Items47"), resources.GetString("cb_hk_key.Items48"), resources.GetString("cb_hk_key.Items49"), resources.GetString("cb_hk_key.Items50"), resources.GetString("cb_hk_key.Items51"), resources.GetString("cb_hk_key.Items52"), resources.GetString("cb_hk_key.Items53"), resources.GetString("cb_hk_key.Items54"), resources.GetString("cb_hk_key.Items55"), resources.GetString("cb_hk_key.Items56"), resources.GetString("cb_hk_key.Items57"), resources.GetString("cb_hk_key.Items58"), resources.GetString("cb_hk_key.Items59"), resources.GetString("cb_hk_key.Items60"), resources.GetString("cb_hk_key.Items61"), resources.GetString("cb_hk_key.Items62"), resources.GetString("cb_hk_key.Items63"), resources.GetString("cb_hk_key.Items64"), resources.GetString("cb_hk_key.Items65"), resources.GetString("cb_hk_key.Items66"), resources.GetString("cb_hk_key.Items67") });
            cb_hk_key.Name = "cb_hk_key";
            cb_hk_key.SelectedIndexChanged += cb_hk_key_SelectedIndexChanged;
            // 
            // cb_hk_shift
            // 
            resources.ApplyResources(cb_hk_shift, "cb_hk_shift");
            cb_hk_shift.Name = "cb_hk_shift";
            cb_hk_shift.UseVisualStyleBackColor = true;
            cb_hk_shift.CheckedChanged += cb_hk_shift_CheckedChanged;
            // 
            // cb_hk_alt
            // 
            resources.ApplyResources(cb_hk_alt, "cb_hk_alt");
            cb_hk_alt.Name = "cb_hk_alt";
            cb_hk_alt.UseVisualStyleBackColor = true;
            cb_hk_alt.CheckedChanged += cb_hk_alt_CheckedChanged;
            // 
            // cb_hk_ctrl
            // 
            resources.ApplyResources(cb_hk_ctrl, "cb_hk_ctrl");
            cb_hk_ctrl.Name = "cb_hk_ctrl";
            cb_hk_ctrl.UseVisualStyleBackColor = true;
            cb_hk_ctrl.CheckedChanged += cb_hk_ctrl_CheckedChanged;
            // 
            // cb_hk_win
            // 
            resources.ApplyResources(cb_hk_win, "cb_hk_win");
            cb_hk_win.Name = "cb_hk_win";
            cb_hk_win.UseVisualStyleBackColor = true;
            cb_hk_win.CheckedChanged += cb_hk_win_CheckedChanged;
            // 
            // tp_hk_extra
            // 
            resources.ApplyResources(tp_hk_extra, "tp_hk_extra");
            tp_hk_extra.Controls.Add(lb_hk_extra);
            tp_hk_extra.Name = "tp_hk_extra";
            tp_hk_extra.UseVisualStyleBackColor = true;
            // 
            // lb_hk_extra
            // 
            resources.ApplyResources(lb_hk_extra, "lb_hk_extra");
            lb_hk_extra.Name = "lb_hk_extra";
            // 
            // tv_keyboard
            // 
            resources.ApplyResources(tv_keyboard, "tv_keyboard");
            tv_keyboard.Name = "tv_keyboard";
            tv_keyboard.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { (System.Windows.Forms.TreeNode)resources.GetObject("tv_keyboard.Nodes"), (System.Windows.Forms.TreeNode)resources.GetObject("tv_keyboard.Nodes1"), (System.Windows.Forms.TreeNode)resources.GetObject("tv_keyboard.Nodes2") });
            tv_keyboard.AfterSelect += tv_keyboard_AfterSelect;
            // 
            // tabPage_Genernal_Mouse
            // 
            resources.ApplyResources(tabPage_Genernal_Mouse, "tabPage_Genernal_Mouse");
            tabPage_Genernal_Mouse.Controls.Add(lb_MouseOnTaskbarSwitchDesktop2);
            tabPage_Genernal_Mouse.Controls.Add(tv_mouse);
            tabPage_Genernal_Mouse.Controls.Add(lb_MouseOnTaskbarSwitchDesktop1);
            tabPage_Genernal_Mouse.Controls.Add(tc_Mouse);
            tabPage_Genernal_Mouse.Controls.Add(chb_MouseOnTaskbarSwitchDesktop);
            tabPage_Genernal_Mouse.Name = "tabPage_Genernal_Mouse";
            tabPage_Genernal_Mouse.UseVisualStyleBackColor = true;
            // 
            // lb_MouseOnTaskbarSwitchDesktop2
            // 
            resources.ApplyResources(lb_MouseOnTaskbarSwitchDesktop2, "lb_MouseOnTaskbarSwitchDesktop2");
            lb_MouseOnTaskbarSwitchDesktop2.Name = "lb_MouseOnTaskbarSwitchDesktop2";
            // 
            // tv_mouse
            // 
            resources.ApplyResources(tv_mouse, "tv_mouse");
            tv_mouse.Name = "tv_mouse";
            tv_mouse.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { (System.Windows.Forms.TreeNode)resources.GetObject("tv_mouse.Nodes") });
            tv_mouse.AfterSelect += tv_mouse_AfterSelect;
            // 
            // lb_MouseOnTaskbarSwitchDesktop1
            // 
            resources.ApplyResources(lb_MouseOnTaskbarSwitchDesktop1, "lb_MouseOnTaskbarSwitchDesktop1");
            lb_MouseOnTaskbarSwitchDesktop1.Name = "lb_MouseOnTaskbarSwitchDesktop1";
            // 
            // tc_Mouse
            // 
            resources.ApplyResources(tc_Mouse, "tc_Mouse");
            tc_Mouse.Controls.Add(tp_mouse_action);
            tc_Mouse.Name = "tc_Mouse";
            tc_Mouse.SelectedIndex = 0;
            // 
            // tp_mouse_action
            // 
            resources.ApplyResources(tp_mouse_action, "tp_mouse_action");
            tp_mouse_action.Controls.Add(btn_mouse_save);
            tp_mouse_action.Controls.Add(lb_mouse_action);
            tp_mouse_action.Controls.Add(cb_mouse_func);
            tp_mouse_action.Name = "tp_mouse_action";
            tp_mouse_action.UseVisualStyleBackColor = true;
            // 
            // btn_mouse_save
            // 
            resources.ApplyResources(btn_mouse_save, "btn_mouse_save");
            btn_mouse_save.Name = "btn_mouse_save";
            btn_mouse_save.UseVisualStyleBackColor = true;
            btn_mouse_save.Click += btn_mouse_save_Click;
            // 
            // lb_mouse_action
            // 
            resources.ApplyResources(lb_mouse_action, "lb_mouse_action");
            lb_mouse_action.Name = "lb_mouse_action";
            // 
            // cb_mouse_func
            // 
            resources.ApplyResources(cb_mouse_func, "cb_mouse_func");
            cb_mouse_func.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_mouse_func.FormattingEnabled = true;
            cb_mouse_func.Name = "cb_mouse_func";
            // 
            // chb_MouseOnTaskbarSwitchDesktop
            // 
            resources.ApplyResources(chb_MouseOnTaskbarSwitchDesktop, "chb_MouseOnTaskbarSwitchDesktop");
            chb_MouseOnTaskbarSwitchDesktop.Name = "chb_MouseOnTaskbarSwitchDesktop";
            chb_MouseOnTaskbarSwitchDesktop.UseVisualStyleBackColor = true;
            // 
            // mainTabs
            // 
            resources.ApplyResources(mainTabs, "mainTabs");
            mainTabs.Controls.Add(MT_General);
            mainTabs.Controls.Add(MT_UI);
            mainTabs.Controls.Add(MT_Rules);
            mainTabs.Controls.Add(MT_Plugins);
            mainTabs.Controls.Add(MT_Logs);
            mainTabs.Controls.Add(MT_About);
            mainTabs.Name = "mainTabs";
            mainTabs.SelectedIndex = 0;
            mainTabs.SelectedIndexChanged += mainTabs_SelectedIndexChanged;
            // 
            // MT_UI
            // 
            resources.ApplyResources(MT_UI, "MT_UI");
            MT_UI.Controls.Add(panel_UI);
            MT_UI.Name = "MT_UI";
            MT_UI.UseVisualStyleBackColor = true;
            // 
            // panel_UI
            // 
            resources.ApplyResources(panel_UI, "panel_UI");
            panel_UI.Controls.Add(lb_ui_vd_view);
            panel_UI.Controls.Add(gb_DesktopArrangement);
            panel_UI.Name = "panel_UI";
            // 
            // lb_ui_vd_view
            // 
            resources.ApplyResources(lb_ui_vd_view, "lb_ui_vd_view");
            lb_ui_vd_view.Controls.Add(rb_vd_index_1);
            lb_ui_vd_view.Controls.Add(rb_vd_index_0);
            lb_ui_vd_view.Controls.Add(chb_show_vd_index);
            lb_ui_vd_view.Controls.Add(chb_show_vd_name);
            lb_ui_vd_view.Name = "lb_ui_vd_view";
            lb_ui_vd_view.TabStop = false;
            // 
            // rb_vd_index_1
            // 
            resources.ApplyResources(rb_vd_index_1, "rb_vd_index_1");
            rb_vd_index_1.Name = "rb_vd_index_1";
            rb_vd_index_1.TabStop = true;
            rb_vd_index_1.UseVisualStyleBackColor = true;
            // 
            // rb_vd_index_0
            // 
            resources.ApplyResources(rb_vd_index_0, "rb_vd_index_0");
            rb_vd_index_0.Name = "rb_vd_index_0";
            rb_vd_index_0.TabStop = true;
            rb_vd_index_0.UseVisualStyleBackColor = true;
            // 
            // chb_show_vd_index
            // 
            resources.ApplyResources(chb_show_vd_index, "chb_show_vd_index");
            chb_show_vd_index.Name = "chb_show_vd_index";
            chb_show_vd_index.UseVisualStyleBackColor = true;
            // 
            // chb_show_vd_name
            // 
            resources.ApplyResources(chb_show_vd_name, "chb_show_vd_name");
            chb_show_vd_name.Name = "chb_show_vd_name";
            chb_show_vd_name.UseVisualStyleBackColor = true;
            // 
            // gb_DesktopArrangement
            // 
            resources.ApplyResources(gb_DesktopArrangement, "gb_DesktopArrangement");
            gb_DesktopArrangement.Controls.Add(tlp_DesktopArrangement);
            gb_DesktopArrangement.Controls.Add(lb_DesktopArrangementNote);
            gb_DesktopArrangement.Name = "gb_DesktopArrangement";
            gb_DesktopArrangement.TabStop = false;
            // 
            // tlp_DesktopArrangement
            // 
            resources.ApplyResources(tlp_DesktopArrangement, "tlp_DesktopArrangement");
            tlp_DesktopArrangement.Controls.Add(btn_m7, 3, 1);
            tlp_DesktopArrangement.Controls.Add(btn_m6, 2, 1);
            tlp_DesktopArrangement.Controls.Add(btn_m5, 1, 1);
            tlp_DesktopArrangement.Controls.Add(btn_m4, 0, 1);
            tlp_DesktopArrangement.Controls.Add(btn_m3, 3, 0);
            tlp_DesktopArrangement.Controls.Add(btn_m2, 2, 0);
            tlp_DesktopArrangement.Controls.Add(btn_m1, 1, 0);
            tlp_DesktopArrangement.Controls.Add(btn_m0, 0, 0);
            tlp_DesktopArrangement.Name = "tlp_DesktopArrangement";
            // 
            // btn_m7
            // 
            resources.ApplyResources(btn_m7, "btn_m7");
            btn_m7.BackColor = System.Drawing.Color.Gainsboro;
            btn_m7.Name = "btn_m7";
            btn_m7.UseVisualStyleBackColor = false;
            btn_m7.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m6
            // 
            resources.ApplyResources(btn_m6, "btn_m6");
            btn_m6.BackColor = System.Drawing.Color.Gainsboro;
            btn_m6.Name = "btn_m6";
            btn_m6.UseVisualStyleBackColor = false;
            btn_m6.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m5
            // 
            resources.ApplyResources(btn_m5, "btn_m5");
            btn_m5.BackColor = System.Drawing.Color.Gainsboro;
            btn_m5.Name = "btn_m5";
            btn_m5.UseVisualStyleBackColor = false;
            btn_m5.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m4
            // 
            resources.ApplyResources(btn_m4, "btn_m4");
            btn_m4.BackColor = System.Drawing.Color.Gainsboro;
            btn_m4.Name = "btn_m4";
            btn_m4.UseVisualStyleBackColor = false;
            btn_m4.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m3
            // 
            resources.ApplyResources(btn_m3, "btn_m3");
            btn_m3.BackColor = System.Drawing.Color.Gainsboro;
            btn_m3.Name = "btn_m3";
            btn_m3.UseVisualStyleBackColor = false;
            btn_m3.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m2
            // 
            resources.ApplyResources(btn_m2, "btn_m2");
            btn_m2.BackColor = System.Drawing.Color.Gainsboro;
            btn_m2.Name = "btn_m2";
            btn_m2.UseVisualStyleBackColor = false;
            btn_m2.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m1
            // 
            resources.ApplyResources(btn_m1, "btn_m1");
            btn_m1.BackColor = System.Drawing.Color.Gainsboro;
            btn_m1.Name = "btn_m1";
            btn_m1.UseVisualStyleBackColor = false;
            btn_m1.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // btn_m0
            // 
            resources.ApplyResources(btn_m0, "btn_m0");
            btn_m0.BackColor = System.Drawing.Color.MistyRose;
            btn_m0.Name = "btn_m0";
            btn_m0.UseVisualStyleBackColor = false;
            btn_m0.Click += tlp_DesktopArrangement_SubControlClicked;
            // 
            // lb_DesktopArrangementNote
            // 
            resources.ApplyResources(lb_DesktopArrangementNote, "lb_DesktopArrangementNote");
            lb_DesktopArrangementNote.ForeColor = System.Drawing.SystemColors.ControlText;
            lb_DesktopArrangementNote.Name = "lb_DesktopArrangementNote";
            // 
            // MT_Rules
            // 
            resources.ApplyResources(MT_Rules, "MT_Rules");
            MT_Rules.Controls.Add(gb_Rules);
            MT_Rules.Controls.Add(gb_CurrentProfile);
            MT_Rules.Name = "MT_Rules";
            MT_Rules.UseVisualStyleBackColor = true;
            // 
            // gb_Rules
            // 
            resources.ApplyResources(gb_Rules, "gb_Rules");
            gb_Rules.Controls.Add(lv_Rules);
            gb_Rules.Controls.Add(btn_RuleEdit);
            gb_Rules.Controls.Add(btn_RuleClone);
            gb_Rules.Controls.Add(btn_RuleNew);
            gb_Rules.Controls.Add(btn_RuleRemove);
            gb_Rules.Name = "gb_Rules";
            gb_Rules.TabStop = false;
            // 
            // lv_Rules
            // 
            resources.ApplyResources(lv_Rules, "lv_Rules");
            lv_Rules.CheckBoxes = true;
            lv_Rules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { lvc_Name, lvc_Created, lvc_Updated });
            lv_Rules.FullRowSelect = true;
            lv_Rules.GridLines = true;
            lv_Rules.MultiSelect = false;
            lv_Rules.Name = "lv_Rules";
            lv_Rules.UseCompatibleStateImageBehavior = false;
            lv_Rules.View = System.Windows.Forms.View.Details;
            lv_Rules.SelectedIndexChanged += lv_Rules_SelectedIndexChanged;
            lv_Rules.VisibleChanged += lv_Rules_VisibleChanged;
            lv_Rules.KeyDown += lv_Rules_KeyDown;
            // 
            // lvc_Name
            // 
            resources.ApplyResources(lvc_Name, "lvc_Name");
            // 
            // lvc_Created
            // 
            resources.ApplyResources(lvc_Created, "lvc_Created");
            // 
            // lvc_Updated
            // 
            resources.ApplyResources(lvc_Updated, "lvc_Updated");
            // 
            // btn_RuleEdit
            // 
            resources.ApplyResources(btn_RuleEdit, "btn_RuleEdit");
            btn_RuleEdit.Name = "btn_RuleEdit";
            btn_RuleEdit.UseVisualStyleBackColor = true;
            btn_RuleEdit.Click += btn_RuleEdit_Click;
            // 
            // btn_RuleClone
            // 
            resources.ApplyResources(btn_RuleClone, "btn_RuleClone");
            btn_RuleClone.Name = "btn_RuleClone";
            btn_RuleClone.UseVisualStyleBackColor = true;
            btn_RuleClone.Click += btn_RuleClone_Click;
            // 
            // btn_RuleNew
            // 
            resources.ApplyResources(btn_RuleNew, "btn_RuleNew");
            btn_RuleNew.Name = "btn_RuleNew";
            btn_RuleNew.UseVisualStyleBackColor = true;
            btn_RuleNew.Click += btn_RuleNew_Click;
            // 
            // btn_RuleRemove
            // 
            resources.ApplyResources(btn_RuleRemove, "btn_RuleRemove");
            btn_RuleRemove.Name = "btn_RuleRemove";
            btn_RuleRemove.UseVisualStyleBackColor = true;
            btn_RuleRemove.Click += btn_RuleRemove_Click;
            // 
            // gb_CurrentProfile
            // 
            resources.ApplyResources(gb_CurrentProfile, "gb_CurrentProfile");
            gb_CurrentProfile.Controls.Add(llb_goto_general);
            gb_CurrentProfile.Controls.Add(cb_RuleProfiles);
            gb_CurrentProfile.Name = "gb_CurrentProfile";
            gb_CurrentProfile.TabStop = false;
            // 
            // llb_goto_general
            // 
            resources.ApplyResources(llb_goto_general, "llb_goto_general");
            llb_goto_general.Name = "llb_goto_general";
            llb_goto_general.TabStop = true;
            llb_goto_general.LinkClicked += llb_goto_general_LinkClicked;
            // 
            // cb_RuleProfiles
            // 
            resources.ApplyResources(cb_RuleProfiles, "cb_RuleProfiles");
            cb_RuleProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_RuleProfiles.FormattingEnabled = true;
            cb_RuleProfiles.Name = "cb_RuleProfiles";
            // 
            // MT_Plugins
            // 
            resources.ApplyResources(MT_Plugins, "MT_Plugins");
            MT_Plugins.Controls.Add(gb_Plugins);
            MT_Plugins.Name = "MT_Plugins";
            MT_Plugins.UseVisualStyleBackColor = true;
            // 
            // gb_Plugins
            // 
            resources.ApplyResources(gb_Plugins, "gb_Plugins");
            gb_Plugins.Controls.Add(lv_Plugins);
            gb_Plugins.Controls.Add(btn_PluginSettings);
            gb_Plugins.Name = "gb_Plugins";
            gb_Plugins.TabStop = false;
            // 
            // lv_Plugins
            // 
            resources.ApplyResources(lv_Plugins, "lv_Plugins");
            lv_Plugins.CheckBoxes = true;
            lv_Plugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { lvc_PluginName, lvc_PluginVersion, lvc_PluginAuthor, lvc_PluginEmail });
            lv_Plugins.FullRowSelect = true;
            lv_Plugins.GridLines = true;
            lv_Plugins.MultiSelect = false;
            lv_Plugins.Name = "lv_Plugins";
            lv_Plugins.UseCompatibleStateImageBehavior = false;
            lv_Plugins.View = System.Windows.Forms.View.Details;
            lv_Plugins.SelectedIndexChanged += lv_Plugins_SelectedIndexChanged;
            // 
            // lvc_PluginName
            // 
            resources.ApplyResources(lvc_PluginName, "lvc_PluginName");
            // 
            // lvc_PluginVersion
            // 
            resources.ApplyResources(lvc_PluginVersion, "lvc_PluginVersion");
            // 
            // lvc_PluginAuthor
            // 
            resources.ApplyResources(lvc_PluginAuthor, "lvc_PluginAuthor");
            // 
            // lvc_PluginEmail
            // 
            resources.ApplyResources(lvc_PluginEmail, "lvc_PluginEmail");
            // 
            // btn_PluginSettings
            // 
            resources.ApplyResources(btn_PluginSettings, "btn_PluginSettings");
            btn_PluginSettings.Name = "btn_PluginSettings";
            btn_PluginSettings.UseVisualStyleBackColor = true;
            btn_PluginSettings.Click += btn_PluginSettings_Click;
            // 
            // MT_About
            // 
            resources.ApplyResources(MT_About, "MT_About");
            MT_About.Controls.Add(lb_AppName);
            MT_About.Controls.Add(llb_Company);
            MT_About.Controls.Add(lbox_Env);
            MT_About.Controls.Add(lb_Copyright);
            MT_About.Controls.Add(lb_Version);
            MT_About.Controls.Add(lbVersion);
            MT_About.Controls.Add(pb_AboutLogo);
            MT_About.Name = "MT_About";
            MT_About.UseVisualStyleBackColor = true;
            MT_About.Paint += MT_About_Paint;
            // 
            // lb_AppName
            // 
            resources.ApplyResources(lb_AppName, "lb_AppName");
            lb_AppName.Name = "lb_AppName";
            // 
            // llb_Company
            // 
            resources.ApplyResources(llb_Company, "llb_Company");
            llb_Company.Name = "llb_Company";
            llb_Company.TabStop = true;
            llb_Company.LinkClicked += llb_Company_LinkClicked;
            // 
            // lbox_Env
            // 
            resources.ApplyResources(lbox_Env, "lbox_Env");
            lbox_Env.FormattingEnabled = true;
            lbox_Env.Name = "lbox_Env";
            // 
            // lb_Copyright
            // 
            resources.ApplyResources(lb_Copyright, "lb_Copyright");
            lb_Copyright.Name = "lb_Copyright";
            // 
            // lb_Version
            // 
            resources.ApplyResources(lb_Version, "lb_Version");
            lb_Version.Name = "lb_Version";
            // 
            // lbVersion
            // 
            resources.ApplyResources(lbVersion, "lbVersion");
            lbVersion.Name = "lbVersion";
            // 
            // pb_AboutLogo
            // 
            resources.ApplyResources(pb_AboutLogo, "pb_AboutLogo");
            pb_AboutLogo.Name = "pb_AboutLogo";
            pb_AboutLogo.TabStop = false;
            // 
            // logCMS
            // 
            resources.ApplyResources(logCMS, "logCMS");
            logCMS.ImageScalingSize = new System.Drawing.Size(24, 24);
            logCMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { clearToolStripMenuItem });
            logCMS.Name = "logCMS";
            // 
            // clearToolStripMenuItem
            // 
            resources.ApplyResources(clearToolStripMenuItem, "clearToolStripMenuItem");
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // panel_Pages
            // 
            resources.ApplyResources(panel_Pages, "panel_Pages");
            panel_Pages.Controls.Add(mainTabs);
            panel_Pages.Name = "panel_Pages";
            // 
            // ts_PageNav
            // 
            resources.ApplyResources(ts_PageNav, "ts_PageNav");
            ts_PageNav.BackColor = System.Drawing.Color.White;
            ts_PageNav.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            ts_PageNav.ImageScalingSize = new System.Drawing.Size(24, 24);
            ts_PageNav.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tsb_general, tsb_ui, tsb_rules, tsb_plugins, tsb_logs, tsb_about });
            ts_PageNav.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            ts_PageNav.Name = "ts_PageNav";
            // 
            // tsb_general
            // 
            resources.ApplyResources(tsb_general, "tsb_general");
            tsb_general.Name = "tsb_general";
            tsb_general.Padding = new System.Windows.Forms.Padding(10);
            tsb_general.Tag = "";
            tsb_general.Click += tsb_general_Click;
            // 
            // tsb_ui
            // 
            resources.ApplyResources(tsb_ui, "tsb_ui");
            tsb_ui.Name = "tsb_ui";
            tsb_ui.Padding = new System.Windows.Forms.Padding(10);
            tsb_ui.Tag = "";
            tsb_ui.Click += tsb_ui_Click;
            // 
            // tsb_rules
            // 
            resources.ApplyResources(tsb_rules, "tsb_rules");
            tsb_rules.Name = "tsb_rules";
            tsb_rules.Padding = new System.Windows.Forms.Padding(10);
            tsb_rules.Tag = "";
            tsb_rules.Click += tsb_rules_Click;
            // 
            // tsb_plugins
            // 
            resources.ApplyResources(tsb_plugins, "tsb_plugins");
            tsb_plugins.Name = "tsb_plugins";
            tsb_plugins.Padding = new System.Windows.Forms.Padding(10);
            tsb_plugins.Tag = "";
            tsb_plugins.Click += tsb_plugins_Click;
            // 
            // tsb_logs
            // 
            resources.ApplyResources(tsb_logs, "tsb_logs");
            tsb_logs.Name = "tsb_logs";
            tsb_logs.Padding = new System.Windows.Forms.Padding(10);
            tsb_logs.Tag = "";
            tsb_logs.Click += tsb_logs_Click;
            // 
            // tsb_about
            // 
            resources.ApplyResources(tsb_about, "tsb_about");
            tsb_about.Name = "tsb_about";
            tsb_about.Padding = new System.Windows.Forms.Padding(10);
            tsb_about.Tag = "";
            tsb_about.Click += tsb_about_Click;
            // 
            // panel_PageNav
            // 
            resources.ApplyResources(panel_PageNav, "panel_PageNav");
            panel_PageNav.Controls.Add(ts_PageNav);
            panel_PageNav.Name = "panel_PageNav";
            // 
            // panel_mask
            // 
            resources.ApplyResources(panel_mask, "panel_mask");
            panel_mask.Name = "panel_mask";
            // 
            // mainStatusStrip
            // 
            resources.ApplyResources(mainStatusStrip, "mainStatusStrip");
            mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssl_main_tips });
            mainStatusStrip.Name = "mainStatusStrip";
            mainStatusStrip.SizingGrip = false;
            // 
            // tssl_main_tips
            // 
            resources.ApplyResources(tssl_main_tips, "tssl_main_tips");
            tssl_main_tips.Name = "tssl_main_tips";
            // 
            // AppController
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(mainStatusStrip);
            Controls.Add(panel_mask);
            Controls.Add(panel_PageNav);
            Controls.Add(panel_Pages);
            Controls.Add(mainMenu);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MainMenuStrip = mainMenu;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AppController";
            ShowInTaskbar = false;
            FormClosing += AppController_FormClosing;
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            MT_Logs.ResumeLayout(false);
            logTabs.ResumeLayout(false);
            logTabInfo.ResumeLayout(false);
            logTabInfo.PerformLayout();
            logTabDebug.ResumeLayout(false);
            logTabDebug.PerformLayout();
            logTabVerbose.ResumeLayout(false);
            logTabVerbose.PerformLayout();
            logTabEvent.ResumeLayout(false);
            logTabEvent.PerformLayout();
            logTabWarning.ResumeLayout(false);
            logTabWarning.PerformLayout();
            logTabError.ResumeLayout(false);
            logTabError.PerformLayout();
            MT_General.ResumeLayout(false);
            panel_General.ResumeLayout(false);
            tab_General.ResumeLayout(false);
            tabPage_Genernal_Main.ResumeLayout(false);
            gb_profiles.ResumeLayout(false);
            gb_storage.ResumeLayout(false);
            gb_storage.PerformLayout();
            gb_general.ResumeLayout(false);
            gb_general.PerformLayout();
            gb_nav.ResumeLayout(false);
            gb_nav.PerformLayout();
            gb_Cluster.ResumeLayout(false);
            gb_Cluster.PerformLayout();
            tabPage_Genernal_Keyboard.ResumeLayout(false);
            tc_Keyboard.ResumeLayout(false);
            tp_hk_main.ResumeLayout(false);
            tp_hk_main.PerformLayout();
            tp_hk_extra.ResumeLayout(false);
            tabPage_Genernal_Mouse.ResumeLayout(false);
            tabPage_Genernal_Mouse.PerformLayout();
            tc_Mouse.ResumeLayout(false);
            tp_mouse_action.ResumeLayout(false);
            tp_mouse_action.PerformLayout();
            mainTabs.ResumeLayout(false);
            MT_UI.ResumeLayout(false);
            panel_UI.ResumeLayout(false);
            panel_UI.PerformLayout();
            lb_ui_vd_view.ResumeLayout(false);
            lb_ui_vd_view.PerformLayout();
            gb_DesktopArrangement.ResumeLayout(false);
            tlp_DesktopArrangement.ResumeLayout(false);
            MT_Rules.ResumeLayout(false);
            gb_Rules.ResumeLayout(false);
            gb_CurrentProfile.ResumeLayout(false);
            gb_CurrentProfile.PerformLayout();
            MT_Plugins.ResumeLayout(false);
            gb_Plugins.ResumeLayout(false);
            MT_About.ResumeLayout(false);
            MT_About.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pb_AboutLogo).EndInit();
            logCMS.ResumeLayout(false);
            panel_Pages.ResumeLayout(false);
            ts_PageNav.ResumeLayout(false);
            ts_PageNav.PerformLayout();
            panel_PageNav.ResumeLayout(false);
            panel_PageNav.PerformLayout();
            mainStatusStrip.ResumeLayout(false);
            mainStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem tsmiMainMenuQuit;
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
        private System.Windows.Forms.Label lb_hk_extra;
        private System.Windows.Forms.ToolStripMenuItem closeThisWindowToolStripMenuItem;
        private System.Windows.Forms.TreeView tv_mouse;
        private System.Windows.Forms.TabControl tc_Mouse;
        private System.Windows.Forms.TabPage tp_mouse_action;
        private System.Windows.Forms.Button btn_mouse_save;
        private System.Windows.Forms.Label lb_mouse_action;
        private System.Windows.Forms.ComboBox cb_mouse_func;
        private System.Windows.Forms.GroupBox lb_ui_vd_view;
        private System.Windows.Forms.RadioButton rb_vd_index_1;
        private System.Windows.Forms.RadioButton rb_vd_index_0;
        private System.Windows.Forms.CheckBox chb_show_vd_index;
        private System.Windows.Forms.CheckBox chb_show_vd_name;
        private System.Windows.Forms.CheckBox chb_HideMainViewIfItsShown;
        private System.Windows.Forms.CheckBox chb_notify_vd_changed;
        private System.Windows.Forms.CheckBox chb_showVDIndexOnTrayIcon;
        private System.Windows.Forms.CheckBox chb_HideOnStart;
        private System.Windows.Forms.ToolStripMenuItem runAsAdministratorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox gb_general;
        private System.Windows.Forms.Label lb_RunOnStartup;
        private System.Windows.Forms.CheckBox chb_RunOnStartup;
        private System.Windows.Forms.LinkLabel llb_TaskScheduler;
        private System.Windows.Forms.Button btn_hk_RegAndSave;
        private System.Windows.Forms.Button btn_hk_ClearAndSave;
        private System.Windows.Forms.CheckBox chb_MouseOnTaskbarSwitchDesktop;
        private System.Windows.Forms.Label lb_MouseOnTaskbarSwitchDesktop2;
        private System.Windows.Forms.Label lb_MouseOnTaskbarSwitchDesktop1;
        private System.Windows.Forms.TabControl tab_General;
        private System.Windows.Forms.TabPage tabPage_Genernal_Main;
        private System.Windows.Forms.TabPage tabPage_Genernal_Keyboard;
        private System.Windows.Forms.TabPage tabPage_Genernal_Mouse;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabPage logTabVerbose;
        private System.Windows.Forms.TextBox tbVerbose;
        private System.Windows.Forms.RadioButton rb_vdi_on_tray_style_2;
        private System.Windows.Forms.RadioButton rb_vdi_on_tray_style_1;
        private System.Windows.Forms.RadioButton rb_vdi_on_tray_style_0;
        private System.Windows.Forms.GroupBox gb_storage;
        private System.Windows.Forms.Button btn_chooseConfigRoot;
        private System.Windows.Forms.TextBox tb_configRoot;
        private System.Windows.Forms.Label lb_note_configRoot;
        private System.Windows.Forms.Label lb_configRoot;
        private System.Windows.Forms.ToolStripMenuItem tsmiMainMenuRestart;
        private System.Windows.Forms.GroupBox gb_profiles;
        private System.Windows.Forms.ComboBox cbb_profiles;
        private System.Windows.Forms.Button btn_profile_del;
        private System.Windows.Forms.Button btn_profile_rename;
        private System.Windows.Forms.Button btn_profile_dup;
        private System.Windows.Forms.ToolStripStatusLabel tssl_main_tips;
        private System.Windows.Forms.LinkLabel llb_goto_general;
        private System.Windows.Forms.Label lb_profiles_note;
    }
}