//Faux, Copyright (C) 2012 Ray Wallace
//
//This program is free software; you can redistribute it and/or modify it under
//the terms of the GNU General Public License as published by the Free Software
//Foundation version 2 of the Licens.
//
//This program is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
//details.
//
//You should have received a copy of the GNU General Public License along with
//this program; if not, write to the Free Software Foundation, Inc., 51 Franklin
//Street, Fifth Floor, Boston, MA  02110-1301, USA.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;

namespace Project
{
    public static class ApplicationExtension
    {
        public static string GetUserAppDataPath(this Application app)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            object[] customAttrs =
                assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            AssemblyCompanyAttribute company =
                          ((AssemblyCompanyAttribute)(customAttrs[0]));

            return String.Format(
                @"{0}\{1}\{2}\",
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                    company.Company,
                    assembly.GetName().Version.ToString());
        }
    }
}
