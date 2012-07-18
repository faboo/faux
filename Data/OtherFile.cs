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
using System.Windows;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Project
{
    public class OtherFile : Node
    {
        public static readonly DependencyProperty RealPathProperty = DependencyProperty.Register("RealPath", typeof(string), typeof(OtherFile));
        
        [XmlIgnore]
        public string RealPath
        {
            get { return (string)GetValue(RealPathProperty); }
            set { SetValue(RealPathProperty, value); }
        }

        public OtherFile()
        {
            Type = Settings.Current.Types.First(t => t.Name == "Other File");
        }

        protected override Freezable CreateInstanceCore()
        {
            return new OtherFile();
        }

        public void Start()
        {
            Fork(new ProcessStartInfo
            {
                FileName = RealPath
            });
        }
    }
}
