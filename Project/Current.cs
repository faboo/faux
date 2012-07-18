using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace Project
{
    [XmlRootAttribute("Project", IsNullable = false)]
    public class Current : Freezable
    {
        [XmlIgnore]
        public string Name { get; set; }
        [XmlIgnore]
        public string SavePath { get; set; }
        public ProjectFolder BaseFolder { get; set; }
        [XmlArrayItem("Type")]
        public List<Type> TypeOverrides { get; set; }

        public Current()
        {
            TypeOverrides = new List<Type>();
        }

        public Current(string file)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Current));
            Current read = null;

            SavePath = file;

            using(FileStream project = new FileStream(file, FileMode.Open, FileAccess.Read)){
                read = (Current)deserializer.Deserialize(project);
                project.Close();
            }

            this.Name = Path.GetFileNameWithoutExtension(file);
            this.BaseFolder = new ProjectFolder(){
                Name = this.Name,
                Contents = read.BaseFolder.Contents
            };
            this.TypeOverrides = read.TypeOverrides;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new Current();
        }
    }
}
