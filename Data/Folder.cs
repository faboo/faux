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
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows;

namespace Project
{
    public class Folder : Node, IXmlSerializable
    {
		public static readonly DependencyProperty ContentsProperty = DependencyProperty.Register("Contents", typeof(ObservableCollection<Node>), typeof(Folder));
        private bool changing = false;

		public ObservableCollection<Node> Contents
		{
			get { return (ObservableCollection<Node>)GetValue(ContentsProperty); }
			set { SetValue(ContentsProperty, value); }
		}

        public Folder()
        {
            Type = Settings.Current.Types.First(t => t.Name == "Folder");
            Contents = new ObservableCollection<Node>();
        }

        public Folder(OtherFilesFolder source)
        {
            Type = Settings.Current.Types.First(t => t.Name == "Folder");
            Contents = new ObservableCollection<Node>();
            Name = source.Name;

            foreach (var node in source.Contents)
                Add(node);
        }

        public override void CalculateTypes()
        {
            foreach (Node node in this.Contents.ToArray())
            {
                //if (node is Folder && !(node is OtherFilesFolder))
                    node.CalculateTypes();
                //else if(node is File)
                //    (node as File).CalculateType();
            }
        }

        public override void SetProject(Current project) {
            base.SetProject(project);

            foreach(Node node in Contents)
                node.SetProject(project);
        }

        private void OnContentsChanged(object sender, EventArgs args)
        {
            if (!changing && !(sender is System.Windows.Media.Imaging.BitmapImage))
            {
                changing = true;
                Contents.Sort(CompareNodes);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(
                    ContentsProperty,
                    Contents,
                    Contents));
                changing = false;
            }
        }

        public virtual void Add(Node node)
        {
            if (node is OtherFile)
                node = new File(node as OtherFile);
            else if (node is OtherFilesFolder)
                node = new Folder(node as OtherFilesFolder);

            AddCore(node);
        }

        protected void AddCore(Node node)
        {
            node.SetProject(Project);
            node.Changed += OnContentsChanged;
            Contents.InsertSorted(node, CompareNodes);
            OnPropertyChanged(new DependencyPropertyChangedEventArgs(
                ContentsProperty,
                Contents,
                Contents));
        }

        public void Remove(Node node){
            node.Changed -= OnContentsChanged;
            Contents.Remove(node);
            OnPropertyChanged(new DependencyPropertyChangedEventArgs(
                ContentsProperty,
                Contents,
                Contents));
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new Folder();
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
                else if (reader.Name == "Contents")
                {
                    reader.ReadStartElement();
                    ReadContents(reader);
                    reader.ReadEndElement();
                }
                else
                {
                    reader.Read();
                }
            }

            reader.Read();
        }

        private void ReadContents(XmlReader reader)
        {
            XmlSerializer des = null;
            XmlSerializer folderDes = new XmlSerializer(typeof(Folder));
            XmlSerializer fileDes = new XmlSerializer(typeof(File));

            while (reader.IsStartElement())
            {
                if (reader.Name == "Folder")
                    des = folderDes;
                else // (reader.Name == "File")
                    des = fileDes;

                Add((Node)des.Deserialize(reader));
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer folderDes = new XmlSerializer(typeof(Folder));
            XmlSerializer fileDes = new XmlSerializer(typeof(File));

            writer.WriteElementString("Name", Name);
            
            writer.WriteStartElement("Contents");
            foreach (Node node in Contents)
            {
                if (node.GetType() == typeof(Folder))
                    folderDes.Serialize(writer, node);
                else if(node.GetType() == typeof(File))
                    fileDes.Serialize(writer, node);
            }
            writer.WriteEndElement();
        }

        private int CompareNodes(Node left, Node right)
        {
            if (left is OtherFilesFolder && right is OtherFile)
                return -1;
            else if (left is OtherFilesFolder && !(right is OtherFilesFolder))
                return 1;
            else if (right is OtherFilesFolder && left is OtherFile)
                return 1;
            else if (right is OtherFilesFolder && !(left is OtherFilesFolder))
                return -1;
            else if (left is Folder && !(right is Folder))
                return -1;
            else if (left is File && !(right is File))
                return 1;
            else
                return left.Name.CompareTo(right.Name);
        }
    }
}
