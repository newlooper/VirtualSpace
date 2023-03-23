/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.ViewModels;
using LinqExpressionBuilder;
using MaterialDesignThemes.Wpf;
using VirtualSpace;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Helpers;

namespace ControlPanel.Pages;

public partial class Rules
{
    private static Rules?        _instance;
    private        RuleTemplate? _newRule;

    private Rules()
    {
        _instance = this;
        InitializeComponent();
    }

    private Rules( string headerKey, PackIconKind iconKind ) : this()
    {
        var mdc = (MenuContainerViewModel)MenuContainer.DataContext;
        mdc.HeaderKey = headerKey;
        mdc.IconKind = iconKind;

        ReloadRules();
    }

    public static void ReloadRules()
    {
        _instance.RuleList.ItemsSource = RulesViewModel.Instance.Rules;
        _instance._needRefresh = true;
    }

    public static Rules Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Rules( headerKey, iconKind );
    }

    private void RuleList_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        RuleDefBox.DataContext = RuleList.SelectedItem; // 笑看风云变

        btnEditRule.IsEnabled = RuleList.SelectedItems.Count > 0;
        btnCloneRule.IsEnabled = RuleList.SelectedItems.Count > 0;
        btnDeleteRule.IsEnabled = RuleList.SelectedItems.Count > 0;
    }

    private void BtnNewRule_OnClick( object sender, RoutedEventArgs e )
    {
        RuleList.SelectedIndex = -1;
        _newRule = new RuleTemplate
        {
            Id = Guid.Empty,
            Enabled = true,
            Action = new Behavior()
        };

        RuleDefBox.DataContext = _newRule; // 平地起风云

        RuleDate.Visibility = Visibility.Hidden;
        DrawerHost.IsBottomDrawerOpen = true;
    }

    private void BtnEditRule_OnClick( object sender, RoutedEventArgs e )
    {
        var r = RuleList.SelectedItem as RuleTemplate;
        if ( r == null ) return;

        RuleDate.Visibility = Visibility.Visible;
        DrawerHost.IsBottomDrawerOpen = true;
    }

    private void BtnCloneRule_OnClick( object sender, RoutedEventArgs e )
    {
        var r = RuleList.SelectedItem as RuleTemplate;
        if ( r == null ) return;

        var foc  = RuleList.ItemsSource as FullObservableCollection<RuleTemplate>;
        var time = DateTime.Now;

        var et = RefreshRuleIds( Conditions.ParseExpressionTemplate( r.Expression ) );

        var clone = new RuleTemplate
        {
            Name = r.Name,
            Expression = JsonDocument.Parse( JsonSerializer.Serialize( et, WriteOptions ) ),
            Enabled = r.Enabled,
            Tag = r.Tag,
            Action = r.Action!.Clone(),
            Created = time,
            Updated = time
        };

        foc.Add( clone );
    }

    private void BtnDeleteRule_OnClick( object sender, RoutedEventArgs e )
    {
        var r = RuleList.SelectedItem as RuleTemplate;
        if ( r == null ) return;

        var foc = RuleList.ItemsSource as FullObservableCollection<RuleTemplate>;
        foc.Remove( r );
    }

    private void Cbb_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        if ( sender is not ComboBox {IsLoaded: true} cbb || cbb.SelectedValue is null ) return;

        var field = cbb.Name.Split( "_" )[1];

        var r = RuleList.SelectedItem as RuleTemplate;
        if ( r == null ) return;
        var exp = Conditions.ParseExpressionTemplate( r.Expression );
        foreach ( var rule in exp.rules.Where( rule => rule.field == field ) )
        {
            rule.@operator = cbb.SelectedValue.ToString();
            break;
        }

        r.Expression = JsonDocument.Parse( JsonSerializer.Serialize( exp, WriteOptions ) );
    }

    private static ExpressionTemplate RefreshRuleIds( ExpressionTemplate expressionTemplate )
    {
        expressionTemplate.id = Guid.NewGuid();
        if ( expressionTemplate.rules == null ) return expressionTemplate;

        foreach ( var rule in expressionTemplate.rules )
        {
            rule.id = Guid.NewGuid();
            if ( rule.rules != null ) RefreshRuleIds( rule );
        }

        return expressionTemplate;
    }

    private void BtnSave_OnClick( object sender, RoutedEventArgs e )
    {
        var bd = tbName.GetBindingExpression( TextBox.TextProperty );
        bd?.UpdateSource();

        bd = tbWeight.GetBindingExpression( TextBox.TextProperty );
        bd?.UpdateSource();

        if ( bd?.ValidationError is null )
        {
            var r = RuleList.SelectedItem as RuleTemplate ?? _newRule;

            if ( chb_Title.IsChecked == false &&
                 chb_ProcessName.IsChecked == false &&
                 chb_ProcessPath.IsChecked == false &&
                 chb_CommandLine.IsChecked == false &&
                 chb_WinInScreen.IsChecked == false &&
                 chb_WndClass.IsChecked == false )
            {
                Snackbar.MessageQueue?.Enqueue(
                    Agent.Langs.GetString( "Rule.AtLeastOne" ),
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds( 5 ) );
                return;
            }

            var exp = new ExpressionTemplate
            {
                condition = Keywords.And,
                rules = new List<ExpressionTemplate>(),
            };

            if ( r.Expression != null )
            {
                exp.id = Conditions.ParseExpressionTemplate( r.Expression ).id;
            }

            try
            {
                BuildRule( chb_Title, cbb_Title, tb_Title, exp );
                BuildRule( chb_ProcessName, cbb_ProcessName, tb_ProcessName, exp );
                BuildRule( chb_ProcessPath, cbb_ProcessPath, tb_ProcessPath, exp );
                BuildRule( chb_CommandLine, cbb_CommandLine, tb_CommandLine, exp );
                BuildRule( chb_WndClass, cbb_WndClass, tb_WndClass, exp );
                BuildRule( chb_WinInScreen, cbb_WinInScreen, null, exp );
            }
            catch ( Exception ex )
            {
                Snackbar.MessageQueue?.Enqueue(
                    Agent.Langs.GetString( ex.Message ),
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds( 5 ) );
                return;
            }

            r.Expression = JsonDocument.Parse( JsonSerializer.Serialize( exp, WriteOptions ) );

            var action = r.Action;
            if ( chb_MoveToDesktop.IsChecked == true )
            {
                action.MoveToDesktop = int.Parse( cbb_MoveToDesktop.SelectedValue.ToString() );
            }

            action.FollowWindow = (bool)chb_FollowWindow.IsChecked;
            action.PinWindow = (bool)chb_PinWindow.IsChecked;
            action.PinApp = (bool)chb_PinApp.IsChecked;
            action.HideFromView = (bool)chb_HideFromView.IsChecked;

            if ( chb_MoveToScreen.IsChecked == true )
            {
                action.MoveToScreen = int.Parse( cbb_MoveToScreen.SelectedValue.ToString() );
            }

            if ( r.Id == Guid.Empty )
            {
                r.Id = Guid.NewGuid();
                r.Created = DateTime.Now;
                r.Updated = r.Created;
                var foc = RuleList.ItemsSource as FullObservableCollection<RuleTemplate>;
                foc.Add( r );
            }
            else
            {
                r.Updated = DateTime.Now;
            }

            _newRule = null;
            DrawerHost.IsBottomDrawerOpen = false;
        }
    }

    private void BuildRule( CheckBox cb, ComboBox cbb, TextBox? tb, ExpressionTemplate exp )
    {
        if ( cb.IsChecked != true ) return;

        var V   = new Value();
        var opt = cbb.SelectedValue.ToString();
        if ( tb is null )
        {
            opt = Keywords.Eq[0];
            V = new Value {V = cbb.SelectedValue.ToString()};
        }
        else
        {
            if ( opt == Keywords.RegexIsMatch[0] && !StringHelper.IsValidRegex( tb.Text ) )
            {
                throw new Exception( "Rule.InvalidRegex" );
            }

            V = new Value {V = tb.Text};
        }

        var rule = new ExpressionTemplate
        {
            type = Keywords.String,
            field = cb.Name.Split( "_" )[1],
            @operator = opt,
            value = V
        };

        exp.rules.Add( rule );
    }

    private void BtnCloseDefBox_OnClick( object sender, RoutedEventArgs e )
    {
        _newRule = null;
        DrawerHost.IsBottomDrawerOpen = false;
    }

    private static readonly JsonSerializerOptions? WriteOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}