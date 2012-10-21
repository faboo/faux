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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace Project
{
    public class File : Node
    {
        public static readonly DependencyProperty RealPathProperty = DependencyProperty.Register("RealPath", typeof(string), typeof(File), new FrameworkPropertyMetadata(OnRealPathChanged));

        public File()
        {
            Init();
        }

        public File(OtherFile source)
        {
            Name = source.Name;
            RealPath = source.RealPath;
            Init();
        }

        public string RealPath {
            get { return (string)GetValue(RealPathProperty); }
            set { SetValue(RealPathProperty, value); }
        }

        private void Init()
        {
            Type = Settings.Current.Types.FirstOrDefault(t => t.Name == "Unknown File");
            if(RealPath != null)
                CalculateTypes();
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                else if (reader.Name == "RealPath")
                {
                    RealPath = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("RealPath", RealPath);
        }

        protected override void OnNameChanged(DependencyPropertyChangedEventArgs args)
        {
            if(RealPath == null)
                RealPath = Name;
        }

		protected void OnRealPathChanged(DependencyPropertyChangedEventArgs args){
            CalculateTypes();
		}

        public override void CalculateTypes()
        {
            Type = null;

            if(Project != null)
                foreach(var type in Project.TypeOverrides)
                    if(type.Pattern != null && Regex.IsMatch(RealPath, type.Pattern)) {
                        Type = type;
                        break;
                    }

            if(Type == null)
                foreach (var type in Settings.Current.Types)
                    if (type.Pattern != null && Regex.IsMatch(RealPath, type.Pattern))
                    {
                        Type = type;
                        break;
                    }

            if(Type == null)
                Type = Settings.Current.Types.FirstOrDefault(t => t.Name == "Unknown File");
        }


        public void Launch(MacroExpander macros)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            macros.Target = this;

            if (Type != null && Type.Launcher != null)
            {
                if (String.IsNullOrWhiteSpace(Type.LauncherArgs))
                    info.Arguments = RealPath;
                else
                    info.Arguments = macros.Expand(Type.LauncherArgs);
                info.FileName = Type.Launcher;
            }
            else
            {
                info.FileName = RealPath;
            }

            Fork(info);
        }

        public void Start()
        {
            Fork(new ProcessStartInfo{
                FileName = RealPath
            });
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new File();
        }

        private static void OnRealPathChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((File)source).OnRealPathChanged(args);
        }
    }
}
