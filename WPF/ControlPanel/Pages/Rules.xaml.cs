/* Copyright (C) 2021 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.ViewModels;
using MaterialDesignThemes.Wpf;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;

namespace ControlPanel.Pages;

public partial class Rules
{
    private static Rules? _instance;

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
        HandleClick();

        UserControlRuleEditor.RuleListItemsSource = RuleList.ItemsSource as FullObservableCollection<RuleTemplate>;
    }

    public static void ReloadRules()
    {
        _instance.RuleList.ItemsSource = RulesViewModel.Instance.Rules;
        _instance._needRefresh = true;
    }

    private void HandleClick()
    {
        AddHandler( Button.ClickEvent, new RoutedEventHandler( ClickEventFromSubControl ) );
    }

    private void ClickEventFromSubControl( object sender, RoutedEventArgs e )
    {
        if ( e.OriginalSource is Button btn )
        {
            switch ( btn.Name )
            {
                case "btnSave":
                case "btnCloseDefBox":

                    DrawerHost.IsBottomDrawerOpen = false;
                    e.Handled = true;
                    break;
            }
        }
    }

    public static Rules Create( string headerKey, PackIconKind iconKind )
    {
        return _instance ??= new Rules( headerKey, iconKind );
    }

    private void RuleList_OnSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
        UserControlRuleEditor.RuleDefBox.DataContext = RuleList.SelectedItem; // 笑看风云变

        btnEditRule.IsEnabled = RuleList.SelectedItems.Count > 0;
        btnCloneRule.IsEnabled = RuleList.SelectedItems.Count > 0;
        btnDeleteRule.IsEnabled = RuleList.SelectedItems.Count > 0;
    }

    private void BtnEditRule_OnClick( object sender, RoutedEventArgs e )
    {
        var r = RuleList.SelectedItem as RuleTemplate;
        if ( r == null ) return;

        UserControlRuleEditor.RuleDate.Visibility = Visibility.Visible;

        DrawerHost.IsBottomDrawerOpen = true;
    }

    private void BtnNewRule_OnClick( object sender, RoutedEventArgs e )
    {
        RuleList.SelectedIndex = -1;

        UserControlRuleEditor.RuleDefBox.DataContext = new RuleTemplate // 平地起风云
        {
            Id = Guid.Empty,
            Enabled = true,
            Action = new Behavior()
        };
        UserControlRuleEditor.RuleDate.Visibility = Visibility.Hidden;

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
            Expression = JsonDocument.Parse( JsonSerializer.Serialize( et, RulesViewModel.WriteOptions ) ),
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
}