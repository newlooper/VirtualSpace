﻿/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LinqExpressionBuilder;
using VirtualSpace.Config;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Helpers;
using Process = System.Diagnostics.Process;

namespace VirtualSpace
{
    public partial class RuleForm : Form
    {
        private readonly Dictionary<string, Guid> _editIds = new();
        private readonly int                      _editIndex;
        private          IVirtualDesktopInfo      _vdi;

        public RuleForm()
        {
            InitializeComponent();
            _editIndex = -1;
        }

        public RuleForm( int index )
        {
            InitializeComponent();
            _editIndex = index;
        }

        public void Init( IVirtualDesktopInfo vdi )
        {
            _vdi = vdi;
            InitOperators();
            SetFormValues();
        }

        private void InitOperators()
        {
            OperatorList( cbb_Title );
            OperatorList( cbb_ProcessName );
            OperatorList( cbb_ProcessPath );
            OperatorList( cbb_WndClass );

            //////////////////////////
            // all virtual desktops
            var count    = _vdi.GetDesktopCount();
            var desktops = new List<object>();
            for ( var i = 0; i < count; i++ )
            {
                desktops.Add( new {Value = i, Text = _vdi.DesktopNameFromIndex( i )} );
            }

            WinForms.SetComboBoxDataSource( cbb_MoveToDesktop, desktops );

            //////////////////////////
            // all screens
            var screens    = new List<object>();
            var allScreens = Screen.AllScreens;
            for ( var i = 0; i < allScreens.Length; i++ )
            {
                screens.Add( new {Value = i, Text = $"{allScreens[i].DeviceName}  ({allScreens[i].DeviceFriendlyName()})"} );
            }

            WinForms.SetComboBoxDataSource( cbb_WinInScreen, screens );
            WinForms.SetComboBoxDataSource( cbb_MoveToScreen, new List<object>( screens ) ); // avoid share a same list
        }

        private static void OperatorList( ComboBox cbb )
        {
            var items = new List<object>
            {
                new {Value = Keywords.Eq[0], Text = Agent.Langs.GetString( "Rule.Op.Eq" )},
                new {Value = Keywords.StartsWith[0], Text = Agent.Langs.GetString( "Rule.Op.Ssw" )},
                new {Value = Keywords.EndsWith[0], Text = Agent.Langs.GetString( "Rule.Op.Esw" )},
                new {Value = Keywords.Contains[0], Text = Agent.Langs.GetString( "Rule.Op.Sc" )},
                new {Value = Keywords.RegexIsMatch[0], Text = Agent.Langs.GetString( "Rule.Op.Regex" )}
            };
            WinForms.SetComboBoxDataSource( cbb, items );
        }

        private void SetFormValues()
        {
            if ( _editIndex < 0 ) return;

            _editIds.Clear();

            var ruleList = Conditions.FetchRules();
            if ( ruleList.Count == 0 ) return;

            cb_Enabled.Checked = ruleList[_editIndex].Enabled;
            tb_Name.Text = ruleList[_editIndex].Name;
            var root  = Conditions.ParseExpressionTemplate( ruleList[_editIndex].Expression );
            var array = new List<ExpressionTemplate>();

            if ( root.rules != null )
            {
                array.AddRange( root.rules );
            }
            else
            {
                array.Add( root );
            }

            foreach ( var rule in array )
            {
                var type      = rule.type;
                var field     = rule.field;
                var @operator = rule.@operator;
                var valueNode = rule.value;
                var id        = rule.id;

                switch ( field )
                {
                    case RuleFields.Title:
                        cb_Title.Checked = true;
                        cbb_Title.SelectedValue = @operator;
                        tb_Title.Text = valueNode.V;
                        _editIds[RuleFields.Title] = id;
                        break;
                    case RuleFields.ProcessName:
                        cb_ProcessName.Checked = true;
                        cbb_ProcessName.SelectedValue = @operator;
                        tb_ProcessName.Text = valueNode.V;
                        _editIds[RuleFields.ProcessName] = id;
                        break;
                    case RuleFields.ProcessPath:
                        cb_ProcessPath.Checked = true;
                        cbb_ProcessPath.SelectedValue = @operator;
                        tb_ProcessPath.Text = valueNode.V;
                        _editIds[RuleFields.ProcessPath] = id;
                        break;
                    case RuleFields.WndClass:
                        cb_WndClass.Checked = true;
                        cbb_WndClass.SelectedValue = @operator;
                        tb_WndClass.Text = valueNode.V;
                        _editIds[RuleFields.WndClass] = id;
                        break;
                    case RuleFields.WinInScreen:
                        cb_WinInScreen.Checked = true;
                        cbb_WinInScreen.SelectedValue = int.Parse( valueNode.V );
                        _editIds[RuleFields.WinInScreen] = id;
                        break;
                }

                var action = ruleList[_editIndex].Action;

                cb_PinWindow.Checked = action.PinWindow;
                cb_PinApp.Checked = action.PinApp;
                cb_HideFromView.Checked = action.HideFromView;

                cb_MoveToDesktop.Checked = action.MoveToDesktop >= 0;
                cbb_MoveToDesktop.SelectedValue = action.MoveToDesktop;
                cb_FollowWindow.Checked = action.FollowWindow;

                cb_MoveToScreen.Checked = action.MoveToScreen >= 0;
                cbb_MoveToScreen.SelectedValue = action.MoveToScreen;
            }

            lbCreated.Text = $"{ruleList[_editIndex].Created:yyyy-MM-dd HH:mm:ss}";
            lbUpdated.Text = $"{ruleList[_editIndex].Updated:yyyy-MM-dd HH:mm:ss}";
        }

        public void SetFormValuesByWindow( IntPtr handle )
        {
            tb_Name.Text = Agent.Langs.GetString( "Rule.New" );

            cb_Title.Checked = true;
            var sbTitle = new StringBuilder( Const.WindowTitleMaxLength );
            User32.GetWindowText( handle, sbTitle, sbTitle.Capacity );
            tb_Title.Text = sbTitle.ToString();

            _ = User32.GetWindowThreadProcessId( handle, out var pId );
            var process = Process.GetProcessById( pId );
            cb_ProcessName.Checked = true;
            tb_ProcessName.Text = process.ProcessName;

            try
            {
                tb_ProcessPath.Text = process.MainModule?.FileName;
            }
            catch ( Exception ex )
            {
                cb_ProcessPath.Checked = false;
                cb_ProcessPath.Enabled = false;
                tb_ProcessPath.Text = ex.Message;
            }

            var sbCName = new StringBuilder( Const.WindowClassMaxLength );
            User32.GetClassName( handle, sbCName, sbCName.Capacity );
            tb_WndClass.Text = sbCName.ToString();

            var allScreens = Screen.AllScreens;
            var screen     = Screen.FromHandle( handle );
            for ( var i = 0; i < allScreens.Length; i++ )
            {
                if ( screen.DeviceName == allScreens[i].DeviceName )
                {
                    cbb_WinInScreen.SelectedValue = i;
                    break;
                }
            }
        }

        private void btn_RuleSave_Click( object sender, EventArgs e )
        {
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            if ( string.IsNullOrEmpty( tb_Name.Text ) )
            {
                toolStripStatusLabel1.Text = lb_Name.Text;
                toolStripStatusLabel2.Text = Agent.Langs.GetString( "Rule.NameRequired" );
                return;
            }

            if ( ( cb_Title.Checked ||
                   cb_ProcessName.Checked ||
                   cb_ProcessPath.Checked ||
                   cb_WndClass.Checked ||
                   cb_WinInScreen.Checked ) == false )
            {
                toolStripStatusLabel2.Text = Agent.Langs.GetString( "Rule.AtLeastOne" );
                return;
            }

            var ruleList = Conditions.FetchRules();

            var rule = new RuleTemplate();
            var exp = new ExpressionTemplate
            {
                condition = Keywords.And,
                rules = new List<ExpressionTemplate>()
            };

            if ( _editIndex > -1 )
            {
                rule.Id = ruleList[_editIndex].Id;
                exp.id = Conditions.ParseExpressionTemplate( ruleList[_editIndex].Expression ).id;
            }

            try
            {
                BuildRule( cb_Title, cbb_Title, tb_Title, exp );
                BuildRule( cb_ProcessName, cbb_ProcessName, tb_ProcessName, exp );
                BuildRule( cb_ProcessPath, cbb_ProcessPath, tb_ProcessPath, exp );
                BuildRule( cb_WndClass, cbb_WndClass, tb_WndClass, exp );
                BuildRule( cb_WinInScreen, cbb_WinInScreen, null, exp );
            }
            catch ( Exception ex )
            {
                toolStripStatusLabel2.Text = ex.Message;
                return;
            }

            var action = new Behavior();
            if ( cb_MoveToDesktop.Checked )
            {
                action.MoveToDesktop = int.Parse( cbb_MoveToDesktop.SelectedValue.ToString() );
            }

            if ( cb_MoveToScreen.Checked )
            {
                action.MoveToScreen = int.Parse( cbb_MoveToScreen.SelectedValue.ToString() );
            }

            action.FollowWindow = cb_FollowWindow.Checked;
            action.PinWindow = cb_PinWindow.Checked;
            action.PinApp = cb_PinApp.Checked;
            action.HideFromView = cb_HideFromView.Checked;

            var time = DateTime.Now;
            rule.Name = tb_Name.Text;
            rule.Expression = JsonDocument.Parse( JsonSerializer.Serialize( exp, Conditions.GetJsonSerializerOptions() ) );
            rule.Action = action;
            rule.Enabled = cb_Enabled.Checked;
            rule.Updated = time;

            if ( _editIndex == -1 )
            {
                rule.Created = time;
                ruleList.Add( rule );
            }
            else
            {
                rule.Created = ruleList[_editIndex].Created;
                ruleList[_editIndex] = rule;
            }

            AppController.UpdateRuleListView( _editIndex, rule );

            Close();
        }

        private void BuildRule( CheckBox cb, ComboBox cbb, TextBox? tb, ExpressionTemplate exp )
        {
            if ( cb.Checked )
            {
                var V   = new Value();
                var opt = cbb.SelectedValue.ToString();
                if ( tb is null )
                {
                    opt = Keywords.Eq[0];
                    V = new Value {V = cbb.SelectedValue.ToString()};
                }
                else
                {
                    if ( opt == Keywords.RegexIsMatch[0] && !IsValidRegex( tb.Text ) )
                    {
                        throw new Exception( "Invalid Regex." );
                    }

                    V = new Value {V = tb.Text};
                }

                var name = cb.Name[3..]; // 偷懒的做法，依赖控件名称，因此控件更名时需要注意不要破坏对应关系

                var rule = new ExpressionTemplate
                {
                    type = Keywords.String,
                    field = cb.Name[3..],
                    @operator = opt,
                    value = V
                };
                if ( _editIds.ContainsKey( name ) )
                {
                    rule.id = _editIds[name];
                }

                exp.rules.Add( rule );
            }
        }

        private static bool IsValidRegex( string pattern )
        {
            if ( string.IsNullOrWhiteSpace( pattern ) ) return false;

            try
            {
                _ = Regex.Match( "", pattern );
            }
            catch ( ArgumentException )
            {
                return false;
            }

            return true;
        }

        private void checkBox_CheckedChanged( object sender, EventArgs e )
        {
            if ( cb_Title.Checked )
            {
                cbb_Title.Enabled = true;
                tb_Title.Enabled = true;
            }
            else
            {
                cbb_Title.Enabled = false;
                tb_Title.Enabled = false;
            }

            if ( cb_ProcessName.Checked )
            {
                cbb_ProcessName.Enabled = true;
                tb_ProcessName.Enabled = true;
            }
            else
            {
                cbb_ProcessName.Enabled = false;
                tb_ProcessName.Enabled = false;
            }

            if ( cb_ProcessPath.Checked )
            {
                cbb_ProcessPath.Enabled = true;
                tb_ProcessPath.Enabled = true;
            }
            else
            {
                cbb_ProcessPath.Enabled = false;
                tb_ProcessPath.Enabled = false;
            }

            if ( cb_WndClass.Checked )
            {
                cbb_WndClass.Enabled = true;
                tb_WndClass.Enabled = true;
            }
            else
            {
                cbb_WndClass.Enabled = false;
                tb_WndClass.Enabled = false;
            }

            if ( cb_MoveToDesktop.Checked )
            {
                cbb_MoveToDesktop.Enabled = true;
                cb_FollowWindow.Enabled = true;
            }
            else
            {
                cbb_MoveToDesktop.Enabled = false;
                cb_FollowWindow.Enabled = false;
            }

            if ( cb_WinInScreen.Checked )
            {
                cbb_WinInScreen.Enabled = true;
            }
            else
            {
                cbb_WinInScreen.Enabled = false;
            }

            if ( cb_MoveToScreen.Checked )
            {
                cbb_MoveToScreen.Enabled = true;
            }
            else
            {
                cbb_MoveToScreen.Enabled = false;
            }
        }
    }
}