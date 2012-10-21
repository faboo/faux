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

namespace Project
{
    public abstract class Node : Freezable, IXmlSerializable
    {
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Node), new FrameworkPropertyMetadata(OnNameChanged));
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(Node));

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
        public Current Project { get; set; }

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

        private static void OnNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((Node)sender).OnNameChanged(args);
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
    }
}
