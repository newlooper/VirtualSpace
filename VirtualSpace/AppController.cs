/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using VirtualSpace.AppLogs;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController : Form, IAppController
    {
        private static readonly ComponentResourceManager Resources = new( typeof( AppController ) );
        private static          AppController            _instance;

        public AppController()
        {
            Application.EnableVisualStyles();

            var lang = ConfigManager.GetCurrentProfile().UI.Language;
            if ( Agent.ValidLangs.Keys.ToList().Contains( lang ) )
            {
                CultureInfo.CurrentCulture = new CultureInfo( lang );
                CultureInfo.CurrentUICulture = new CultureInfo( lang );
            }

            _instance = this;

            InitializeComponent();
            InitRuleListView();
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
            Show();
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
                        if ( item is ToolStripButton c )
                            SetToolStripButtonLang( c, lang );
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
                foreach ( ToolStripMenuItem c in item.DropDownItems )
                {
                    SetMenuItemLang( c, lang );
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
                ConfigManager.GetCurrentProfile().UI.Language = o.Name;
                ConfigManager.Save();

                foreach ( ToolStripMenuItem lang in langToolStripMenuItem.DropDownItems )
                {
                    lang.Checked = lang.Name == o.Name;
                }
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

        private void quitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            MainWindow.Quit();
        }

        private void AppController_FormClosing( object sender, FormClosingEventArgs e )
        {
            e.Cancel = true;
            Hide();
        }

        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var about = About.Create();

            about.ShowDialog();
        }

        private void tsb_general_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );

            mainTabs.SelectTab( 0 );
        }

        private void tsb_ui_Click( object sender, EventArgs e )
        {
            ts_PageNavButton_Click( sender, e );
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

        private void ts_PageNavButton_Click( object sender, EventArgs e )
        {
            foreach ( var item in ts_PageNav.Items )
            {
                var button = item as ToolStripButton;
                button.Checked = sender == item;
            }
        }

        private void AppController_Load( object sender, EventArgs e )
        {
            Logger.Info( "Current Language: " + CultureInfo.CurrentUICulture.DisplayName );
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            MainWindow.Quit();
        }

        private void settingsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            BringToTop();
        }
    }
}