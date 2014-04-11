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
        public FreezableCollection<Type> TypeOverrides { get; set; }
        [XmlArrayItem("Command")]
        public FreezableCollection<Command> Commands { get; set; }

        private FileSystemWatcher watch = null;
        private bool loading = true;
        private bool shouldSave = true;

        public Current()
        {
            TypeOverrides = new FreezableCollection<Type>();
            Commands = new FreezableCollection<Command>();
        }

        public Current(string file, bool shouldSave)
        {
            Name = Path.GetFileNameWithoutExtension(file);
            SavePath = file;
            this.shouldSave = shouldSave;

            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Current));
                Current read = null;

                using (FileStream project = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    read = (Current)deserializer.Deserialize(project);
                    project.Close();
                }

                TypeOverrides = read.TypeOverrides;
                TypeOverrides.Changed += (o, a) =>
                    OnChanged();
                Commands = read.Commands;
                Commands.Changed += (o, a) =>
                    OnChanged();
                BaseFolder = read.BaseFolder;
                BaseFolder.SetProject(this);
                BaseFolder.Name = Name;
                BaseFolder.CalculateTypes();
                read.BaseFolder = null;
            }
            catch
            {
                BaseFolder = new ProjectFolder()
                {
                    Name = Name,
                    Contents = new ObservableCollection<Node>()
                };
            }

            EnsureProjectFiles();
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
            if(!loading && shouldSave)
                Save();
        }

        protected override Freezable CreateInstanceCore()
        {
            var current = new Current();
            current.shouldSave = shouldSave;
            return current;
        }

        private void EnsureProjectFiles() {
            ProjectFilesFolder pff = (ProjectFilesFolder)BaseFolder.Contents.FirstOrDefault(f => f is ProjectFilesFolder);

            if(pff == null) {
                var files = BaseFolder.Contents.Where(f => !(f is ProjectFilesFolder || f is OtherFilesFolder)).ToArray();

                if(files.Length == 1 && files[0] is Folder && files[0].Name.Equals("Project Files")) {
                    pff = new ProjectFilesFolder(files[0] as Folder);
                    BaseFolder.Remove(files[0]);
                }
                else {
                    pff = new ProjectFilesFolder();

                    foreach(var file in files) {
                        pff.Add(file);
                        BaseFolder.Remove(file);
                    }
                }
                
                BaseFolder.Add(pff);
            }
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

        public bool FileInPath(string path) {
            //Need to add the \ to the end because IsBaseOf is *weird*.
            Uri project = new Uri(Path.GetDirectoryName(SavePath)+'\\');

            return project.IsBaseOf(new Uri(path));
            /*
            return Path.GetDirectoryName(path).StartsWith(
                Path.GetDirectoryName(SavePath));
             */
        }

        public string GetFileSubPath(string path) {
            if(FileInPath(path)) {
                int savePathSize = Path.GetDirectoryName(SavePath).Length + 1;

                return path.Substring(savePathSize);
            }
            else {
                return path;
            }
        }
    }
}
