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
using VirtualSpace.Config.Events.Entity;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController : Form
    {
        private static readonly ComponentResourceManager Resources = new( typeof( AppController ) );
        private static          AppController            _instance;

        public AppController()
        {
            Application.EnableVisualStyles();

            var lang = ConfigManager.GetCurrentProfile().UI.Language;
            Logger.Info( "Language Setting In Profile: " + lang );
            if ( Agent.ValidLangs.Keys.ToList().Contains( lang ) )
            {
                CultureInfo.CurrentCulture = new CultureInfo( lang );
                CultureInfo.CurrentUICulture = new CultureInfo( lang );
            }

            Logger.Info( "Current Language: " + CultureInfo.CurrentUICulture.DisplayName );

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

        public static void SetAllLang( string lang )
        {
            CultureInfo.CurrentCulture = new CultureInfo( lang );
            CultureInfo.CurrentUICulture = new CultureInfo( lang );

            Logger.Info( "Change Language: " + CultureInfo.CurrentUICulture.DisplayName );

            void Invoker()
            {
                SetControlLang( _instance.logCMS, lang );
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

            if ( control is MenuStrip strip )
            {
                Resources.ApplyResources( control, strip.Name, ci );
                if ( strip.Items.Count > 0 )
                {
                    foreach ( ToolStripMenuItem c in strip.Items )
                    {
                        SetMenuItemLang( c, lang );
                    }
                }
            }
            else if ( control is ContextMenuStrip ms )
            {
                Resources.ApplyResources( control, ms.Name, ci );
                if ( ms.Items.Count > 0 )
                {
                    foreach ( ToolStripMenuItem c in ms.Items )
                    {
                        SetMenuItemLang( c, lang );
                    }
                }
            }
            else if ( control is ListView lv )
            {
                foreach ( ColumnHeader ch in lv.Columns )
                {
                    Resources.ApplyResources( ch, ch.Name, ci );
                }
            }

            foreach ( Control c in control.Controls )
            {
                Resources.ApplyResources( c, c.Name, ci );
                SetControlLang( c, lang );
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

        public static void UpdateRuleListView( int index, RuleTemplate rule )
        {
            if ( index == -1 )
            {
                AddRule( rule );
            }
            else
            {
                UpdateRule( index, rule );
            }
        }

        private static void AddRule( RuleTemplate rule )
        {
            _instance.lv_Rules.Items.Add( LviByRule( rule ) );
        }

        private static void UpdateRule( int index, RuleTemplate rule )
        {
            _instance.lv_Rules.Items[index] = LviByRule( rule );
        }

        public void BringToTop()
        {
            TopMost = true;
            ReadRules();
            Show();
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
    }
}