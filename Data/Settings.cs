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
using System.Collections.ObjectModel;
using System.Windows;
using System.Deployment.Application;

namespace Project
{
    [XmlRootAttribute("Settings", IsNullable = false)]
    public class Settings
    {
        private static string path = "Settings.xml";
        private static Settings current = null;

        public String LastProject { get; set; }

        [XmlArrayItem("Type")]
        public ObservableCollection<Type> Types { get; set; }

        [XmlArrayItem("Command")]
        public ObservableCollection<Command> Commands { get; set; }

        private static string GetAppDataPath()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
                return ApplicationDeployment.CurrentDeployment.DataDirectory;
            else
                return Application.Current.GetUserAppDataPath();
        }

        private static Stream GetSettingsStream()
        {
            Stream stream = Application.GetResourceStream(new Uri("/"+path, UriKind.Relative)).Stream;
            string localPath = GetAppDataPath() + path;

            if (System.IO.File.Exists(localPath))
                stream = new FileStream(localPath, FileMode.Open, FileAccess.Read);

            return stream;
        }

        public static Settings Current
        {
            get
            {
                if (current == null)
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Settings));

                    using (Stream settings = GetSettingsStream())
                    {
                        current = (Settings)deserializer.Deserialize(settings);
                        settings.Close();
                    }

                    foreach (Type type in BuiltinTypes.Types)
                        if (!current.Types.Contains(type))
                            current.Types.Add(type);
                }

                return current;
            }
        }

        public static void Save()
        {
            string appDir = Application.Current.GetUserAppDataPath();
            string localPath = appDir + path;
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            if (!Directory.Exists(appDir))
                Directory.CreateDirectory(appDir);

            using (FileStream settings = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(settings, current);
                settings.Close();
            }
        }
    }
}
