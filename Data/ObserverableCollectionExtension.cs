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
using System.Collections.ObjectModel;
using System.Collections;

namespace Project
{
    public static class ObserverableCollectionExtension
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Func<T,T,int> comparer)
        {
            List<T> sorted = collection.OrderBy(x => x, new Comparer<T>{ Function = comparer }).ToList();

            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static void InsertSorted<T>(this ObservableCollection<T> collection, T item, Func<T,T,int> comparer)
		{
            int index = 0;

            while (index < collection.Count() && Math.Sign(comparer(item, collection[index])) > 0)
                index += 1;

            collection.Insert(index, item);
		}

        private class Comparer<T> : IComparer<T>
        {
            public Func<T, T, int> Function { get; set; }

            public int Compare(T x, T y)
            {
                return Function(x, y);
            }
        }
    }
}
