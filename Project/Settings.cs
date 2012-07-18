using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Project
{
    [XmlRootAttribute("Settings", IsNullable = false)]
    public class Settings
    {
        private static string path = "Settings.xml";
        private static Settings current = null;

        [XmlArrayItem("Type")]
        public List<Type> Types { get; set; }

        public static Settings Current
        {
            get
            {
                if (current == null)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Settings));

                    using (FileStream settings = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        current = (Settings)deserializer.Deserialize(settings);
                        settings.Close();
                    }
                }

                return current;
            }
        }
    }
}
