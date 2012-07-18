using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Windows;

namespace Project
{
    public class Type : Freezable
    {
        public static readonly List<Type> AllTypes = new List<Type>();

        private BitmapImage iconCache = null;

        public Regex Pattern { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public string Launcher { get; set; }
        public ImageSource IconImage {
            get {
                if (iconCache == null)
                {
                    iconCache = new BitmapImage();

                    iconCache.BeginInit();
                    iconCache.UriSource = new Uri(IconPath);
                    iconCache.EndInit();
                }

                return iconCache;
            }
        }

        public Type()
        {
            AllTypes.Add(this);
        }

        public Type(bool register)
        {
            if (register)
                AllTypes.Add(this);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new Type(false);
        }
    }
}
