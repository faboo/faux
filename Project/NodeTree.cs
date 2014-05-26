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
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace Project
{
    public class NodeTree : TreeView
    {
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Current), typeof(NodeTree));

        private TreeViewItem selectedContainer = null;
        private ScrollViewer scrollViewer = null;

        static NodeTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeTree), new FrameworkPropertyMetadata(typeof(NodeTree)));
        }

        public NodeTree()
        {
            EventManager.RegisterClassHandler(
                typeof(NodeTree),
                TreeViewItem.SelectedEvent,
                (RoutedEventHandler)((s,a) => selectedContainer = (TreeViewItem)a.OriginalSource),
                true);
/*
            CommandBindings.Add(new CommandBinding(
                ProjectCommands.Rename,
                OnRenameExecuted,
                (s, e) => e.CanExecute = SelectedItem != null));
            CommandBindings.Add(new CommandBinding(
                ProjectCommands.Properties,
                OnPropertiesExecuted,
                (s, e) => e.CanExecute = SelectedItem != null));
 */
        }

		public Current Project
		{
			get { return (Current)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}

        private ScrollViewer ScrollViewer
        {
            get
            {
                if (scrollViewer == null)
                {
                    DependencyObject border = VisualTreeHelper.GetChild(this, 0);
                    if (border != null)
                    {
                        scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                    }
                }

                return scrollViewer;
            }
        }

        protected override void OnDragOver(DragEventArgs args) {
            var position = args.GetPosition(this);

            if(position.Y < 12) {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - 10);
            }
            else if(position.Y > this.ActualHeight - 12) {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + 10);
            }
            else {
                base.OnDragOver(args);
            }
        }

        private void OnRenameExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            Rename rename = selectedContainer.FindVisualChild<Rename>();

            if (rename != null)
                rename.Show();
        }

        private void OnPropertiesExecuted(object sender, ExecutedRoutedEventArgs args) {
            File file = selectedContainer.DataContext as File;
            WindowsInterop.ShowFileProperties(file.RealPath);
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new NodeTreeItem();
        }
    }
}
