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
using System.Xml.Serialization;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace Project
{
    public abstract class Node : Freezable, IXmlSerializable
    {
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Node), new FrameworkPropertyMetadata(OnNameChanged));
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(Node));
        //public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Folder), typeof(Node), new FrameworkPropertyMetadata(OnParentChanged));

		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
        [XmlIgnore]
		public Type Type
		{
			get { return (Type)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
        [XmlIgnore]
        public Current Project { get; private set; }
        [XmlIgnore]
        public virtual bool Moveable {
            get { return true; }
        }
        [XmlIgnore]
        public virtual Folder Parent { get; set; }
		/*{
            get { return (Folder)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}*/
        [XmlIgnore]
        public virtual ImageSource Overlay {
            get { return null; }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                if (reader.Name == "Name")
                {
                    Name = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.Read();
                }
            }
            
            reader.ReadEndElement();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);
        }

        public virtual void SetProject(Current project) {
            Project = project;
        }

        public virtual void CalculateTypes() {
        }

        protected virtual void OnNameChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        protected virtual void OnParentChanged(DependencyPropertyChangedEventArgs args) {
            if(args.OldValue != null) {
                (args.OldValue as Folder).Remove(this);
            }
            if(Parent != null) {
                Parent.Add(this);
            }
        }

        private static void OnNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((Node)sender).OnNameChanged(args);
        }

        private static void OnParentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            // something screws up the dispatching when sending a Contents changes event from Folder
            //if(args.OldValue != args.NewValue)
            //    ((Node)sender).OnParentChanged(args);
        }

        protected void Fork(ProcessStartInfo info)
        {
            Process launcher = new Process();

            launcher.StartInfo = info;
            launcher.StartInfo.ErrorDialog = true;
            launcher.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();

            launcher.Start();
            launcher.Dispose();
        }

        public static Node FromPath(Current project, string path) {
            if(project.FileInPath(path)) {
                if(Directory.Exists(path)) {
                    return new Folder {
                        Name = Path.GetFileName(path),
                        Project = project,
                    };
                }
                else {
                    return new File {
                        Name = Path.GetFileName(path),
                        RealPath = project.GetFileSubPath(path),
                        Project = project,
                    };
                }
            }
            else {
                if(Directory.Exists(path)) {
                    var folder = new ExternalFolder {
                        Name = Path.GetFileName(path),
                        RealPath = path,
                        Project = project,
                    };

                    folder.Update();

                    return folder;
                }
                else {
                    return new ExternalFile {
                        Name = Path.GetFileName(path),
                        RealPath = path,
                        Project = project,
                    };
                }
            }
        }
    }
}
