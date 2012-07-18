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

namespace Project
{
    public static class BuiltinTypes
    {
        public static List<Type> Types = new List<Type>{
            new Type {
                Name = "Folder",
                IconPath = "pack://application:,,,/Icons/Folder.ico",
                Builtin = true
            },
            new Type {
                Name = "Other File",
                IconPath = "pack://application:,,,/Icons/File.ico",
                Builtin = true
            },
            new Type {
                Name = "Other Files Folder",
                IconPath = "pack://application:,,,/Icons/Other Files Folder.ico",
                Builtin = true
            },
            new Type {
                Name = "Unknown File",
                IconPath = "pack://application:,,,/Icons/File.ico",
                Builtin = true
            },
        };
    }
}
