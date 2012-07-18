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
using System.IO;
using System.Diagnostics;

namespace Project
{
    public class OtherFilesFolder : Folder
    {
        public static readonly DependencyProperty RealPathProperty = DependencyProperty.Register("RealPath", typeof(string), typeof(OtherFilesFolder));

        public string RealPath
        {
            get { return (string)GetValue(RealPathProperty); }
            set { SetValue(RealPathProperty, value); }
        }

        public OtherFilesFolder()
        {
            Type offType = Settings.Current.Types.FirstOrDefault(t => t.Name == "Other Files Folder");

            Name = "Other Files";
            RealPath = "";
            if (offType != null)
                Type = offType;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                reader.Read();
            }

            reader.Read();
        }

        public override void Add(Node node)
        {
            AddCore(node);
        }

        public void Update(AllFiles allFiles)
        {
            Update(allFiles, RealPath);
        }

        protected void Update(AllFiles allFiles, string realPathDir)
        {
            string readDir = Path.Combine(Directory.GetCurrentDirectory(), realPathDir);
            string realPath = null;

            this.Contents.Clear();

            foreach (string file in Directory.GetFiles(readDir))
            {
                realPath = Path.Combine(realPathDir, Path.GetFileName(file));
                if (!allFiles.Any(f =>
                    Path.Equals(f.RealPath, realPath)))
                    this.Add(new OtherFile()
                    {
                        Name = Path.GetFileName(file),
                        RealPath = realPath,
                    });
            }
            foreach (string dir in Directory.GetDirectories(readDir))
            {
                realPath = Path.Combine(realPathDir, Path.GetFileName(dir));
                OtherFilesFolder child = new OtherFilesFolder()
                {
                    Name = Path.GetFileName(dir),
                    RealPath = realPath,
                };
                child.Update(allFiles,  Path.Combine(realPathDir, child.Name));
                this.Add(child);
            }
        }

        public void Start()
        {
            Fork(new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory()+"\\"+RealPath
            });
        }
    }
}
