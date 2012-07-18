﻿//Faux, Copyright (C) 2012 Ray Wallace
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
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace Project
{
    [XmlRootAttribute("Project", IsNullable = false)]
    public class Current : Freezable
    {
		public static readonly DependencyProperty BaseFolderProperty = DependencyProperty.Register("BaseFolder", typeof(ProjectFolder), typeof(Current));

        [XmlIgnore]
        public string Name { get; set; }
        [XmlIgnore]
        public string SavePath { get; set; }
		public ProjectFolder BaseFolder
		{
			get { return (ProjectFolder)GetValue(BaseFolderProperty); }
			set { SetValue(BaseFolderProperty, value); }
		}
        [XmlArrayItem("Type")]
        public List<Type> TypeOverrides { get; set; }

        private FileSystemWatcher watch = null;
        private bool loading = true;

        public Current()
        {
            TypeOverrides = new List<Type>();
        }

        public Current(string file)
        {
            Name = Path.GetFileNameWithoutExtension(file);
            SavePath = file;

            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Current));
                Current read = null;

                using (FileStream project = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    read = (Current)deserializer.Deserialize(project);
                    project.Close();
                }

                BaseFolder = read.BaseFolder;
                BaseFolder.Name = Name;
                read.BaseFolder = null;
                TypeOverrides = read.TypeOverrides;
            }
            catch
            {
                BaseFolder = new ProjectFolder()
                {
                    Name = Name,
                    Contents = new ObservableCollection<Node>()
                };
            }

            UpdateOtherFiles();

            watch = new FileSystemWatcher(Path.GetDirectoryName(SavePath));
            loading = false;
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Current));
                String temp = SavePath + "._tmp";

                using (FileStream project = new FileStream(temp, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(project, this);
                    project.Close();
                }

                System.IO.File.Replace(temp, SavePath, SavePath + "~");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save project because:\n" + ex.Message, "Project Save Error", MessageBoxButton.OK);
            }
        }

        public void RecalculateTypes()
        {
            BaseFolder.CalculateTypes();
        }

        protected override void OnChanged()
        {
            base.OnChanged();
            if(!loading)
                Save();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new Current();
        }

        private void UpdateOtherFiles()
        {
            OtherFilesFolder off = (OtherFilesFolder)BaseFolder.Contents.FirstOrDefault(f => f is OtherFilesFolder);
            AllFiles allFiles = new AllFiles(BaseFolder);

            if(off == null){
                off = new OtherFilesFolder();
                BaseFolder.Add(off);
            }
            else{
                off.Contents.Clear();
            }

            off.Update(allFiles);
        }

        private void UpdateOtherFiles(OtherFilesFolder folder, AllFiles allFiles, string savePathDir)
        {
            string pathOffset = Path.GetDirectoryName(SavePath);
            string realPath = null;

            foreach (string file in Directory.GetFiles(savePathDir))
            {
                realPath = file.Substring(pathOffset.Length + (pathOffset.EndsWith("\\")? 0 : 1));
                if (!allFiles.Any(f =>
                    Path.Equals(f.RealPath, realPath)))
                    folder.Add(new OtherFile()
                    {
                        Name = Path.GetFileName(file),
                        RealPath = realPath
                    });
            }
            foreach (string dir in Directory.GetDirectories(savePathDir))
            {
                OtherFilesFolder child = new OtherFilesFolder()
                {
                    Name = Path.GetFileName(dir),
                };
                UpdateOtherFiles(child, allFiles, dir+"\\");
                folder.Add(child);
            }
        }
    }
}
