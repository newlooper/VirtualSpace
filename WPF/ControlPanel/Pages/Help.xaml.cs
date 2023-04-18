// Copyright (C) 2023 Dylan Cheng (https://github.com/newlooper)
// 
// This file is part of VirtualSpace.
// 
// VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ControlPanel.Pages;

public partial class Help : UserControl
{
    private static Help? _instance = null;

    private Help()
    {
        InitializeComponent();
        AppInfo();
    }

    public static Help Instance => _instance ??= new Help();

    private void Hyperlink_OnClick( object sender, RoutedEventArgs e )
    {
        var hl  = (Hyperlink)sender;
        var url = hl.NavigateUri.ToString();

        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start( psi );
    }

    private void AppInfo()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        try
        {
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

            llb_CompanyUri.NavigateUri = new Uri( ( (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                entryAssembly,
                typeof( AssemblyCompanyAttribute ),
                false ) ).Company );
            llb_CompanyText.Text = llb_CompanyUri.NavigateUri.ToString();
        }
        catch
        {
            // ignored
        }

        if ( lbox_Env.Items.Count == 0 )
            lbox_Env.Items.Add( RuntimeInformation.FrameworkDescription );
    }
}