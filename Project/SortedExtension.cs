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
using System.Windows.Markup;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Collections;

namespace Project
{
    [MarkupExtensionReturnType(typeof(CollectionViewSource))]
    public class SortedExtension : MarkupExtension
    {
        private static DependencyProperty SourceProperty = DependencyProperty.Register("SortedExtension", typeof(ICollection), typeof(SortedExtension));
        private CollectionViewSource view = null;
        private DependencyObject sourceContainer = new DependencyObject();

        public string GroupPath { get; set; }
        public string SortPath { get; set; }
        public ListSortDirection Direction { get; set; }
        public IEnumerable SourceBinding { get; set; }

        public SortedExtension()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (view == null)
            {
                view = new CollectionViewSource();

                view.Source = SourceBinding;

                if (SortPath != null)
                    view.SortDescriptions.Add(new SortDescription
                    {
                        Direction = Direction,
                        PropertyName = SortPath,
                    });
                if(GroupPath != null)
                    view.GroupDescriptions.Add(new PropertyGroupDescription
                    {
                        PropertyName = GroupPath,
                    });
            }

            return view;
        }
    }
}
