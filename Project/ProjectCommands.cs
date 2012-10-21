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
using System.Windows.Input;

namespace Project
{
    public static class ProjectCommands
    {
        public static readonly RoutedUICommand Rename = new RoutedUICommand("Rename", "Rename", typeof(ProjectCommands),
            new InputGestureCollection{
                new KeyGesture(Key.F2)
            });
        public static readonly RoutedUICommand EditTypes = new RoutedUICommand("Edit Types...", "EditTypes", typeof(ProjectCommands));
        public static readonly RoutedUICommand EditCommands = new RoutedUICommand("Edit Commands...", "EditCommands", typeof(ProjectCommands));
        public static readonly RoutedUICommand Launch = new RoutedUICommand("Launch", "Launch", typeof(ProjectCommands));
        public static readonly RoutedUICommand Start = new RoutedUICommand("Start", "Start", typeof(ProjectCommands));
        public static readonly RoutedUICommand Command = new RoutedUICommand("Command", "Command Project", typeof(ProjectCommands));
        public static readonly RoutedUICommand LaunchCommand = new RoutedUICommand("LaunchCommand", "Launch...", typeof(ProjectCommands));
        public static readonly RoutedUICommand Refresh = new RoutedUICommand("Refresh", "Refresh", typeof(ProjectCommands));
        public static readonly RoutedUICommand SearchIcons = new RoutedUICommand("SearchIcons", "Search...", typeof(ProjectCommands));
    }
}
