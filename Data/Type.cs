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
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Windows;
using TAFactory.IconPack;
using System.Drawing;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace Project
{
    public class Type : Freezable, IComparable, IEquatable<Type>
    {
        public static readonly List<Type> AllTypes = new List<Type>();
        private static readonly string DefaultIconPath =
            "pack://application:,,,/Icons/DefaultIcon.ico";

		public static readonly DependencyProperty PatternProperty = DependencyProperty.Register("Pattern", typeof(string), typeof(Type));
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Type));
		public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(Type), new FrameworkPropertyMetadata(OnIconPathChanged));
		public static readonly DependencyProperty LauncherProperty = DependencyProperty.Register("Launcher", typeof(string), typeof(Type), new FrameworkPropertyMetadata(OnLauncherChanged));
		public static readonly DependencyProperty LauncherArgsProperty = DependencyProperty.Register("LauncherArgs", typeof(string), typeof(Type));
		private static readonly DependencyPropertyKey IconImagePropertyKey = DependencyProperty.RegisterReadOnly("IconImage", typeof(ImageSource), typeof(Type), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty IconImageProperty = IconImagePropertyKey.DependencyProperty;

        public bool Builtin { get; set; }
		public string Pattern
		{
			get { return (string)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public string IconPath
		{
			get { return (string)GetValue(IconPathProperty); }
			set { SetValue(IconPathProperty, value); }
		}
		public string Launcher
		{
			get { return (string)GetValue(LauncherProperty); }
			set { SetValue(LauncherProperty, value); }
		}
		public string LauncherArgs
		{
			get { return (string)GetValue(LauncherArgsProperty); }
			set { SetValue(LauncherArgsProperty, value); }
		}
       // public SDictionary<string,string> Launchers { get; set; }
        [XmlIgnore]
		public ImageSource IconImage
		{
			get { return (ImageSource)GetValue(IconImageProperty); }
			private set { SetValue(IconImagePropertyKey, value); }
		}

        public Type()
        {
            AllTypes.Add(this);
            Builtin = false;
            //Launchers = new SDictionary<string, string>();
            SetIconImageSource();
        }

        public Type(bool register)
        {
            if (register)
                AllTypes.Add(this);
            Builtin = false;
            //Launchers = new SDictionary<string, string>();
            SetIconImageSource();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new Type(false);
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        
        private ImageSource ToImageSource(Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Exception("Failed to delete native bitmap");
            }

            return wpfBitmap;
        }

        private ImageSource GetFileIcon(string path)
        {
            BitmapImage ic = new BitmapImage();

            ic.BeginInit();
            ic.UriSource = new Uri(path);

            ic.EndInit();

            return ic;
        }

		private void SetIconImageSource(){
			ImageSource icon;

            try
            {
                if (!String.IsNullOrWhiteSpace(IconPath))
                    icon = GetFileIcon(IconPath);
                else if (!String.IsNullOrWhiteSpace(Launcher))
                    icon = ToImageSource(IconHelper.ExtractIcon(Launcher, 0));
                else
                    icon = GetFileIcon(DefaultIconPath);
            }
            catch
            {
                icon = GetFileIcon(DefaultIconPath);
            }

			IconImage = icon;
		}

        public static int Compare(Type left, Type right)
        {
            return left.Name.CompareTo(right.Name);
        }

		private static void OnIconPathChanged(DependencyObject source, DependencyPropertyChangedEventArgs args){
			(source as Type).SetIconImageSource();
		}

		private static void OnLauncherChanged(DependencyObject source, DependencyPropertyChangedEventArgs args){
			(source as Type).SetIconImageSource();
		}

        public int CompareTo(object obj)
        {
            return Compare(this, obj as Type);
        }

        public bool Equals(Type other)
        {
            return this.CompareTo(other) == 0;
        }
    }
}
