/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using VirtualSpace.Helpers;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController : Form, IAppController
    {
        private static readonly ComponentResourceManager Resources = new( typeof( AppController ) );
        private static          AppController            _instance;
        private                 IntPtr                   _mainWindowHandle;

        public AppController()
        {
            Application.EnableVisualStyles();

            var lang = ConfigManager.CurrentProfile.UI.Language;
            if ( Agent.ValidLangs.Keys.ToList().Contains( lang ) )
            {
                CultureInfo.CurrentCulture = new CultureInfo( lang );
                CultureInfo.CurrentUICulture = new CultureInfo( lang );
            }

            _instance = this;

            InitializeComponent();

            InitRuleListView();
            InitPluginListView();
            InitUiConfig();
            InitClusterConfig();
            InitMouseConfig();

            CheckAdmin();

            PickLogAndWrite();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                cp.Style = unchecked(cp.Style | (int)0x80000000); // WS_POPUP
                return cp;
            }
        }

        public void BringToTop()
        {
            TopMost = true;

            ReadRules();
            ReadNavConfig();
            KeyboardTopNodeExpand();
            MouseTopNodeExpand();

            Show();
        }

        public void SetMainWindowHandle( IntPtr handle )
        {
            _mainWindowHandle = handle;
        }

        private void CheckAdmin()
        {
            if ( SysInfo.IsAdministrator() )
            {
                runAsAdministratorToolStripMenuItem.Visible = false;
                return;
            }

            var iconResult = new User32.SHSTOCKICONINFO();
            iconResult.cbSize = (uint)Marshal.SizeOf( iconResult );

            _ = User32.SHGetStockIconInfo(
                User32.SHSTOCKICONID.SIID_SHIELD,
                User32.SHGSI.SHGSI_ICON | User32.SHGSI.SHGSI_SMALLICON,
                ref iconResult );
            runAsAdministratorToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            var icon = Bitmap.FromHicon( iconResult.hIcon );
            icon.MakeTransparent();
            runAsAdministratorToolStripMenuItem.Image = icon;
        }

        public static void SetAllLang( string lang )
        {
            CultureInfo.CurrentCulture = new CultureInfo( lang );
            CultureInfo.CurrentUICulture = new CultureInfo( lang );

            Logger.Info( "Change Language: " + CultureInfo.CurrentUICulture.DisplayName );

            void Invoker()
            {
                SetControlLang( _instance.logCMS, lang );
                SetControlLang( _instance.trayMenu, lang );

                _instance.tv_keyboard.Nodes.Clear();
                _instance.tv_keyboard.Nodes.AddRange( new[]
                {
                    (TreeNode)Resources.GetObject( "tv_keyboard.Nodes" ),
                    (TreeNode)Resources.GetObject( "tv_keyboard.Nodes1" )
                } );
                _instance.KeyboardTopNodeExpand();

                _instance.tv_mouse.Nodes.Clear();
                _instance.tv_mouse.Nodes.AddRange( new[]
                {
                    (TreeNode)Resources.GetObject( "tv_mouse.Nodes" )
                } );
                _instance.MouseTopNodeExpand();

                SetControlLang( _instance, lang );
            }

            if ( _instance.InvokeRequired )
            {
                _instance.Invoke( (MethodInvoker)Invoker );
            }
            else
            {
                Invoker();
            }
        }

        public static void SetControlLang( Control control, string lang )
        {
            var ci = new CultureInfo( lang );

            switch ( control )
            {
                case MenuStrip strip:
                {
                    Resources.ApplyResources( control, strip.Name, ci );
                    foreach ( var item in strip.Items )
                    {
                        if ( item is ToolStripMenuItem c )
                            SetMenuItemLang( c, lang );
                    }

                    break;
                }
                case ContextMenuStrip ms:
                {
                    Resources.ApplyResources( control, ms.Name, ci );
                    foreach ( var item in ms.Items )
                    {
                        if ( item is ToolStripMenuItem c )
                            SetMenuItemLang( c, lang );
                    }

                    break;
                }
                case ListView lv:
                {
                    foreach ( var item in lv.Columns )
                    {
                        if ( item is ColumnHeader ch )
                            Resources.ApplyResources( ch, ch.Name, ci );
                    }

                    break;
                }
                case ToolStrip ts:
                {
                    foreach ( var item in ts.Items )
                    {
                        switch ( item )
                        {
                            case ToolStripButton tsb:
                                SetToolStripButtonLang( tsb, lang );
                                break;
                            case ToolStripSplitButton tssb:
                                Resources.ApplyResources( tssb, tssb.Name, ci );
                                foreach ( var m in tssb.DropDownItems )
                                {
                                    SetMenuItemLang( m as ToolStripMenuItem, lang );
                                }

                                break;
                        }
                    }

                    break;
                }
            }

            foreach ( var item in control.Controls )
            {
                if ( item is Control c )
                {
                    Resources.ApplyResources( c, c.Name, ci );
                    SetControlLang( c, lang );
                }
            }
        }

        private static void SetMenuItemLang( ToolStripMenuItem item, string lang )
        {
            var ci = new CultureInfo( lang );
            Resources.ApplyResources( item, item.Name, ci );
            if ( item.DropDownItems.Count > 0 )
            {
                foreach ( ToolStripItem c in item.DropDownItems )
                {
                    if ( c is ToolStripMenuItem menuItem )
                    {
                        SetMenuItemLang( menuItem, lang );
                    }
                }
            }
        }

        private static void SetToolStripButtonLang( ToolStripButton item, string lang )
        {
            var ci = new CultureInfo( lang );
            Resources.ApplyResources( item, item.Name, ci );
        }

        private void optionsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            void UpdateCheckState( object? s, EventArgs evt )
            {
                var o = (ToolStripMenuItem)s;
                SetAllLang( o.Name );
                ConfigManager.CurrentProfile.UI.Language = o.Name;
                ConfigManager.Save();

                foreach ( ToolStripMenuItem lang in langToolStripMenuItem.DropDownItems )
                {
                    lang.Checked = lang.Name == o.Name;
                }

                ReadNavConfig();
            }

            langToolStripMenuItem.DropDownItems.Clear();
            foreach ( var langKV in Agent.ValidLangs )
            {
                var langItem = new ToolStripMenuItem
                {
                    Checked = CultureInfo.CurrentUICulture.Name == langKV.Key,
                    Name = langKV.Key,
                    Text = langKV.Value
                };
                langItem.Click += UpdateCheckState;
                langToolStripMenuItem.DropDownItems.Add( langItem );
            }
        }

        private void tsmiMainMenuQuit_Click( object sender, EventArgs e )
        {
            User32.PostMessage( _mainWindowHandle, WinMsg.WM_CLOSE, 0, 0 );
        }

        private void AppController_FormClosing( object sender, FormClosingEventArgs e )
        {
            e.Cancel = true;
            Hide();
        }

        private void tsb_general_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
            mainTabs.SelectTab( 0 );
        }

        private void tsb_ui_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );

            RenderDesktopArrangementButtons( ConfigManager.CurrentProfile.UI.DesktopArrangement.ToString() );

            mainTabs.SelectTab( 1 );
        }

        private void tsb_rules_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
            mainTabs.SelectTab( 2 );
        }

        private void tsb_plugins_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
            mainTabs.SelectTab( 3 );
        }

        private void tsb_logs_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
            mainTabs.SelectTab( 4 );
        }

        private void tsb_about_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
            mainTabs.SelectTab( 5 );
        }

        private void ts_PageNavButton_Click( object sender, EventArgs e )
        {
            foreach ( var item in ts_PageNav.Items )
            {
                var button = item as ToolStripButton;
                button.Checked = sender == item;
            }
        }

        private void settingsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            BringToTop();
        }

        private void llb_Company_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            string url;
            if ( e.Link.LinkData != null )
                url = e.Link.LinkData.ToString();
            else
                url = llb_Company.Text.Substring( e.Link.Start, e.Link.Length );

            if ( !url.Contains( "://" ) )
                url = "https://" + url;

            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start( psi );
            llb_Company.LinkVisited = true;
        }

        private void MT_About_Paint( object sender, PaintEventArgs e )
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            lb_AppName.Text = ( (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyProductAttribute ),
                false ) ).Product;
            lb_Version.Text = ( (AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyInformationalVersionAttribute ),
                false ) ).InformationalVersion;
            lb_Copyright.Text = ( (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyCopyrightAttribute ),
                false ) ).Copyright;
            llb_Company.Text = ( (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyCompanyAttribute ),
                false ) ).Company;

            if ( lbox_Env.Items.Count == 0 )
                lbox_Env.Items.Add( RuntimeInformation.FrameworkDescription );
        }

        private void openLogFolderToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var logFolder = Path.Combine( ConfigManager.AppFolder, "Logs" );
            if ( Directory.Exists( logFolder ) )
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = logFolder,
                    FileName = "explorer.exe"
                };

                Process.Start( startInfo );
            }
        }

        private void closeThisWindowToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Hide();
        }

        private void runAsAdministratorToolStripMenuItem_Click( object sender, EventArgs e )
        {
            User32.PostMessage( _mainWindowHandle, WinMsg.WM_HOTKEY, UserMessage.RunAsAdministrator, 0 );
        }
    }
}