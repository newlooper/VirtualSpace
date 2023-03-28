/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text.Json;
using System.Windows.Forms;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using ConfigManager = VirtualSpace.Config.Manager;

namespace VirtualSpace
{
    public partial class AppController
    {
        private void InitRuleListView()
        {
            ///////////////////////////////////////
            // fix no ColumnHeader name issue
            lvc_Name.Name = nameof( lvc_Name );
            lvc_Created.Name = nameof( lvc_Created );
            lvc_Updated.Name = nameof( lvc_Updated );
        }

        private void ReadRules()
        {
            var profiles = ConfigManager.Configs.Profiles;
            cb_RuleProfiles.Items.Clear();
            foreach ( var profile in profiles )
            {
                cb_RuleProfiles.Items.Add( profile.Key );
            }

            cb_RuleProfiles.SelectedItem = ConfigManager.Configs.CurrentProfileName;

            var rules = Conditions.FetchRules();

            lv_Rules.ItemCheck -= lv_Rules_ItemCheck;
            lv_Rules.Items.Clear();
            foreach ( var rule in rules )
            {
                var item = LviByRule( rule );
                lv_Rules.Items.Add( item );
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

        private static ListViewItem LviByRule( RuleTemplate rule )
        {
            var item = new ListViewItem( rule.Name );
            item.SubItems.Add( $"{rule.Created:yyyy-MM-dd HH:mm:ss}" );
            item.SubItems.Add( $"{rule.Updated:yyyy-MM-dd HH:mm:ss}" );
            item.SubItems.Add( rule.Id.ToString() );
            item.Checked = rule.Enabled;
            return item;
        }

        private void btn_RuleNew_Click( object sender, EventArgs e )
        {
            OpenRuleDialog();
        }

        private void btn_RuleEdit_Click( object sender, EventArgs e )
        {
            if ( lv_Rules.SelectedItems.Count == 0 ) return;
            var selectedIndex = lv_Rules.SelectedIndices[0];
            OpenRuleDialog( selectedIndex );
        }

        private void btn_RuleClone_Click( object sender, EventArgs e )
        {
            if ( lv_Rules.SelectedItems.Count == 0 ) return;

            var rules = Conditions.FetchRules();
            if ( rules.Count == 0 ) return;

            var rule = rules[lv_Rules.SelectedIndices[0]];
            var time = DateTime.Now;

            var et = RefreshRuleId( Conditions.ParseExpressionTemplate( rule.Expression ) );

            var clone = new RuleTemplate
            {
                Name = rule.Name,
                Expression = JsonDocument.Parse( JsonSerializer.Serialize( et, Conditions.GetJsonSerializerOptions() ) ),
                Enabled = rule.Enabled,
                Action = rule.Action,
                Created = time,
                Updated = time
            };
            rules.Add( clone );
            lv_Rules.Items.Add( LviByRule( clone ) );
        }

        private void btn_RuleRemove_Click( object sender, EventArgs e )
        {
            DeleteSelectedItem();
        }

        private void DeleteSelectedItem()
        {
            if ( lv_Rules.SelectedItems.Count == 0 ) return;

            var rules = Conditions.FetchRules();
            if ( rules.Count == 0 ) return;

            rules.RemoveAt( lv_Rules.SelectedIndices[0] );
            lv_Rules.Items.RemoveAt( lv_Rules.SelectedIndices[0] );
            Conditions.SaveRules( ConfigManager.GetRulesPath(), rules );
        }

        private void OpenRuleDialog( int index = -1 )
        {
            var ruleForm = new RuleForm( index );
            ruleForm.Init();
            ruleForm.TopMost = true;
            ruleForm.ShowDialog();
        }

        private void lv_Rules_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( lv_Rules.SelectedItems.Count > 0 )
            {
                btn_RuleEdit.Enabled = true;
                btn_RuleClone.Enabled = true;
                btn_RuleRemove.Enabled = true;
            }
            else
            {
                btn_RuleEdit.Enabled = false;
                btn_RuleClone.Enabled = false;
                btn_RuleRemove.Enabled = false;
            }
        }

        private void lv_Rules_ItemCheck( object? sender, ItemCheckEventArgs e )
        {
            var rules = Conditions.FetchRules();
            if ( rules.Count == 0 ) return;

            var index = e.Index;
            rules[index].Enabled = e.NewValue == CheckState.Checked;
            Conditions.SaveRules( ConfigManager.GetRulesPath(), rules );
        }

        private void lv_Rules_VisibleChanged( object sender, EventArgs e )
        {
            lv_Rules.ItemCheck -= lv_Rules_ItemCheck;
            lv_Rules.ItemCheck += lv_Rules_ItemCheck;
        }

        private static ExpressionTemplate RefreshRuleId( ExpressionTemplate expressionTemplate )
        {
            expressionTemplate.id = Guid.NewGuid();
            if ( expressionTemplate.rules == null ) return expressionTemplate;

            foreach ( var rule in expressionTemplate.rules )
            {
                rule.id = Guid.NewGuid();
                if ( rule.rules != null ) RefreshRuleId( rule );
            }

            return expressionTemplate;
        }

        private void lv_Rules_KeyDown( object sender, KeyEventArgs e )
        {
            if ( Keys.Delete != e.KeyCode ) return;
            DeleteSelectedItem();
        }
    }
}