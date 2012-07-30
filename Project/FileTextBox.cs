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
using System.Windows.Controls;
using System.Windows;

namespace Project
{
    public class FileTextBox : TextBox
    {
        public FileTextBox()
        {
            AllowDrop = true;
            ToolTip = "Drag a file into the box";
        }

        protected override void OnDragOver(DragEventArgs args)
        {
            if (args.Data.GetFormats(true).Contains("Text") ||
                args.Data.GetFormats(true).Contains("FileNameW"))
            {
                args.Effects = DragDropEffects.Copy;
            }
        }

        protected override void OnDrop(DragEventArgs args)
        {
            args.Effects = DragDropEffects.Move;
            if(args.Data.GetFormats(true).Contains("FileNameW"))
                this.Text = (args.Data.GetData("FileNameW") as String[])[0];
            else
                this.Text = args.Data.GetData("Text") as String;
        }
    }
}
