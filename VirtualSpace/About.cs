/* Copyright (C) 2022 Dylan Cheng (https://github.com/newlooper)

This file is part of VirtualSpace.

VirtualSpace is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

VirtualSpace is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with VirtualSpace. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualSpace
{
    public partial class About : Form
    {
        private About()
        {
            InitializeComponent();
        }

        public static About Create()
        {
            var about = new About();
            about.TopMost = true;
            about.lb_AppName.Text = ( (AssemblyProductAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof( AssemblyProductAttribute ),
                false ) ).Product;
            about.lb_Version.Text = ( (AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof( AssemblyFileVersionAttribute ),
                false ) ).Version;
            about.lb_Copyright.Text = ( (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof( AssemblyCopyrightAttribute ),
                false ) ).Copyright;
            about.llb_Company.Text = ( (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof( AssemblyCompanyAttribute ),
                false ) ).Company;
            about.lbox_Env.Items.Add( RuntimeInformation.FrameworkDescription );

            return about;
        }

        private void btn_OK_Click( object sender, EventArgs e )
        {
            Close();
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
    }
}