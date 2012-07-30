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

        private Point? lastTreeClick = null;
        private TreeViewItem selectedContainer = null;
        private TreeViewItem draggedItem, target;
        private ScrollViewer scrollViewer = null;

        static NodeTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeTree), new FrameworkPropertyMetadata(typeof(NodeTree)));
        }

        public NodeTree()
        {
            AllowDrop = true;
            EventManager.RegisterClassHandler(
                typeof(NodeTree),
                TreeViewItem.SelectedEvent,
                (RoutedEventHandler)((s,a) => selectedContainer = (TreeViewItem)a.OriginalSource),
                true);

            CommandBindings.Add(new CommandBinding(
                ProjectCommands.Rename,
                OnRenameExecuted,
                (s, e) => e.CanExecute = SelectedItem != null));
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

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private bool CheckDropTarget(TreeViewItem sourceItem, TreeViewItem targetItem)
        {
            return
                sourceItem != null &&
                targetItem != null &&
                !Object.ReferenceEquals(sourceItem.Header, targetItem.Header);
        }

        private void CopyItem(TreeViewItem sourceItem, TreeViewItem targetItem)
        {
            try
            {
                //adding dragged TreeViewItem in target TreeViewItem
                Node sourceNode = (Node)sourceItem.Header;

                if (!(targetItem.Header is Folder))
                    targetItem = targetItem.FindVisualParent<TreeViewItem>(o => o.Header is Folder);

                ((Folder)targetItem.Header).Add(sourceNode);

                //finding Parent TreeViewItem of dragged TreeViewItem 
                TreeViewItem parentItem = sourceItem.FindVisualParent<TreeViewItem>(o => o.Header is Folder && o != sourceItem);
                // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                if (parentItem == null)
                    ((ObservableCollection<Node>)ItemsSource).Remove(sourceNode);
                else
                    ((Folder)parentItem.Header).Remove(sourceNode);
            }
            catch
            {

            }
        }

        protected override void OnDragOver(DragEventArgs args)
        {
            if (lastTreeClick != null)
            {
                try
                {
                    Point currentPosition = args.GetPosition(this);

                    if (args.OriginalSource is UIElement &&
                        ((Math.Abs(currentPosition.X - lastTreeClick.Value.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - lastTreeClick.Value.Y) > 10.0)))
                    {
                        // Verify that this is a valid drop and then store the drop target
                        TreeViewItem item = GetNearestContainer(args.OriginalSource as UIElement);
                        if (CheckDropTarget(draggedItem, item))
                            args.Effects = DragDropEffects.Move;
                        else
                            args.Effects = DragDropEffects.None;
                    }

                    if (currentPosition.Y < 20)
                    {
                        args.Effects = DragDropEffects.Scroll;
                        ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - 7);
                    }
                    else if (currentPosition.Y + 20 > ScrollViewer.ViewportHeight)
                    {
                        args.Effects = DragDropEffects.Scroll;
                        ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + 7);
                    }

                    args.Handled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected override void OnDrop(DragEventArgs args)
        {
            if (lastTreeClick != null)
            {
                try
                {
                    args.Effects = DragDropEffects.None;
                    args.Handled = true;

                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem targetItem = GetNearestContainer(args.OriginalSource as UIElement);
                    if (targetItem != null && draggedItem != null)
                    {
                        target = targetItem;
                        args.Effects = DragDropEffects.Move;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        {
            Point position = args.GetPosition(this);

            if (args.ChangedButton == MouseButton.Left && this.InputHitTest(position).FindVisualParent<TreeViewItem>(o => true) != null)
            {
                lastTreeClick = position;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            lastTreeClick = null;
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            try
            {
                if (args.LeftButton == MouseButtonState.Pressed && lastTreeClick != null)
                {
                    Point currentPosition = args.GetPosition(this);

                    if (Math.Abs(currentPosition.X - lastTreeClick.Value.X) > 10.0 ||
                        Math.Abs(currentPosition.Y - lastTreeClick.Value.Y) > 10.0)
                    {
                        draggedItem = selectedContainer;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(this, this.SelectedValue,
                                DragDropEffects.Move);

                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (target != null))
                            {
                                // A Move drop was accepted
                                if (!Object.ReferenceEquals(draggedItem, target))
                                {
                                    CopyItem(draggedItem, target);
                                }
                            }

                            target = null;
                            draggedItem = null;
                            lastTreeClick = null;
                        }
                    }
                }
                else
                {
                    lastTreeClick = null;
                }
            }
            catch
            {
            }
        }

        private void OnRenameExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            Rename rename = selectedContainer.FindVisualChild<Rename>();

            if (rename != null)
                rename.Show();
        }
    }
}
