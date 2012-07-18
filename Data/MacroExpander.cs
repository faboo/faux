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
using System.IO;

namespace Project
{
    public class MacroExpander
    {
        private static Dictionary<string, Func<MacroExpander, string>> macros =
            new Dictionary<string, Func<MacroExpander, string>>
            {
                { "{Project}", m => m.Project.Name },
                { "{ProjectPath}", m => m.Project.SavePath },
                { "{ProjectDir}", m => Path.GetDirectoryName(m.Project.SavePath) },
                { "{FilePath}", m => m.Target.RealPath },
                { "{FileName}", m => Path.GetFileName(m.Target.RealPath) },
                { "{FileDir}", m => Path.GetDirectoryName(m.Target.RealPath) },
                { "{NodeName}", m => m.Target.Name },
                { "{NodeType}", m => m.Target.Type.Name },
            };
        public static ICollection<string> Macros { get { return macros.Keys; } }

        public Current Project { get; set; }
        public File Target { get; set; }

        public string Expand(string source)
        {
            foreach(var macro in macros)
                source = source.Replace(macro.Key, macro.Value(this));

            return source;
        }
    }
}
