// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LinqExpressionBuilder;
using VirtualSpace.Config.Events.Entity;
using VirtualSpace.Config.Events.Expression;
using VirtualSpace.Helpers;
using VirtualSpace.VirtualDesktop.Api;

namespace ControlPanel.ViewModels;

public class RulesViewModel : ViewModelBase
{
    private static RulesViewModel?                        _instance;
    public         FullObservableCollection<RuleTemplate> Rules;

    private RulesViewModel()
    {
        _instance = this;
        Rules = new FullObservableCollection<RuleTemplate>( Conditions.FetchRules() );
        Rules.CollectionChanged += RulesOnListChanged;
    }

    public static RulesViewModel Instance => _instance ??= new RulesViewModel();

    public static void ReloadRules()
    {
        if ( _instance == null ) return;

        _instance.Rules.CollectionChanged -= _instance.RulesOnListChanged;
        _instance.Rules = new FullObservableCollection<RuleTemplate>( Conditions.FetchRules() );
        _instance.Rules.CollectionChanged += _instance.RulesOnListChanged;
        Pages.Rules.ReloadRules();
    }

    private void RulesOnListChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        Conditions.SaveRules( Rules.ToList() );
    }

    public static List<object> Operators => GetOperators();

    private static List<object> GetOperators()
    {
        return new List<object>
        {
            new {Value = Keywords.Eq[0], text = ""},
            new {Value = Keywords.StartsWith[0], Text = ""},
            new {Value = Keywords.EndsWith[0], Text = ""},
            new {Value = Keywords.Contains[0], Text = ""},
            new {Value = Keywords.RegexIsMatch[0], Text = ""}
        };
    }

    public static List<object> Screens => SysInfo.GetAllScreens();

    public static List<object> Desktops
    {
        get
        {
            var desktops = new List<object>();
            for ( var i = 0; i < DesktopWrapper.Count; i++ ) // system's order
            {
                desktops.Add( new {Value = i, Text = DesktopWrapper.DesktopNameFromIndex( i )} );
            }

            return desktops;
        }
    }
}