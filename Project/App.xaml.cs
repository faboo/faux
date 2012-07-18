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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string[] Args { get; private set; }

        protected override void OnStartup(StartupEventArgs args)
        {
            if (AppDomain.CurrentDomain.SetupInformation
                .ActivationArguments != null &&
                AppDomain.CurrentDomain.SetupInformation
                .ActivationArguments.ActivationData != null)
                Args = AppDomain.CurrentDomain.SetupInformation
                    .ActivationArguments.ActivationData;
            else
                Args = args.Args;

            DispatcherUnhandledException += OnDispatcherUnhandledException;

            base.OnStartup(args);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error occurred: " + e.Exception.ToString()+"\n", "Project Error");
            e.Handled = true;
        }
    }
}
