using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Collections.Specialized;
using System.IO;

namespace Project {
    public class NodeTreeItem : TreeViewItem {
        bool clickedIn = false;
        Point clickStarted;

        public Node Node {
            get { return DataContext as Node; }
        }

        public NodeTreeItem() {
            AllowDrop = true;

            CommandBindings.Add(new CommandBinding(
                ProjectCommands.Rename,
                ExecuteRename));
            CommandBindings.Add(new CommandBinding(
                ProjectCommands.Properties,
                ExecuteProperties));
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.New,
                ExecuteNew));
        }

        private Node DragDataAsNode(IDataObject data) {
            return data.GetData("Project.File") as Node
                ?? data.GetData("Project.Folder") as Node
                ?? data.GetData("Project.OtherFile") as Node
                ?? data.GetData("Project.OtherFilesFolder") as Node
                ?? data.GetData("Project.ExternalFile") as Node
                ?? data.GetData("Project.ExternalFolder") as Node;
        }
        
        private void ExecuteRename(object sender, ExecutedRoutedEventArgs args)
        {
            Rename rename = this.FindVisualChild<Rename>();

            if (rename != null)
                rename.Show();
        }

        private void ExecuteProperties(object sender, ExecutedRoutedEventArgs args) {
            File file = DataContext as File;
            WindowsInterop.ShowFileProperties(file.RealPath);
        }

        private void ExecuteNew(object sender, ExecutedRoutedEventArgs args) {
            Folder container = args.Parameter as Folder;

            container.Add(new Folder() { Name = "New Folder" });
        }

        private void CanNew(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = DataContext is Folder;
        }

        protected override void OnDragOver(DragEventArgs args) {
            if(Node is Folder && (Node as Folder).ChangeableContents) {
                Node dragging = DragDataAsNode(args.Data);

                if(dragging != null && dragging.Moveable &&
                    (!(dragging is Folder) || !(dragging as Folder).Contains(Node))) {

                    if(dragging is File || dragging is Folder) {
                        args.Effects = DragDropEffects.Move;
                        args.Handled = true;
                    }
                    else if(dragging is OtherFilesFolder || dragging is OtherFile) {
                        args.Effects = DragDropEffects.Link;
                        args.Handled = true;
                    }
                }
                else if(args.Data.GetDataPresent(DataFormats.FileDrop)
                    && !((string[])args.Data.GetData(DataFormats.FileDrop, true))[0]
                        .EndsWith(".project")) {

                    args.Effects = DragDropEffects.Link;
                    args.Handled = true;
                }
            }

            if(!args.Handled)
                base.OnDragEnter(args);
        }

        protected override void OnDrop(DragEventArgs args) {
            if(Node is Folder && (this.Node as Folder).ChangeableContents) {
                Node dragging = DragDataAsNode(args.Data);

                if(dragging != null && dragging.Moveable &&
                    (!(dragging is Folder) || !(dragging as Folder).Contains(Node))) {
                    
                    (this.Node as Folder).Add(dragging);
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent(DataFormats.FileDrop)
                    && !(args.Data.GetData(DataFormats.FileDrop, true) as string[]).
                        All(f =>
                            f.EndsWith(".project"))) {

                    foreach(string filename in (args.Data.GetData(DataFormats.FileDrop, true) as string[])
                            .Where(f => !f.EndsWith(".project"))) {
                        (Node as Folder).Add(Node.FromPath(Node.Project, filename));
                    }

                    args.Handled = true;
                }
            }

            if(!args.Handled)
                base.OnDrop(args);
        }

        protected override void OnMouseMove(MouseEventArgs args) {
            if(args.LeftButton == MouseButtonState.Pressed
                && clickedIn) {
                Point cursorAt = args.GetPosition(this);

                if(Math.Abs(cursorAt.X - clickStarted.X) > 5 ||
                    Math.Abs(cursorAt.Y - clickStarted.Y) > 5) {
                    DataObject dragData = new DataObject(
                        typeof(Node),
                        Node);
                    string root = System.IO.Path.GetDirectoryName(
                        Node.Project.SavePath);

                    if(Node is File)
                        dragData.SetFileDropList(new StringCollection {
                                Path.Combine(root, (DataContext as File).RealPath)
                            });
                    else if(Node is OtherFile)
                        dragData.SetFileDropList(new StringCollection {
                                Path.Combine(root, (DataContext as OtherFile).RealPath)
                            });
                    else if(Node is ExternalFile)
                        dragData.SetFileDropList(new StringCollection {
                                (DataContext as ExternalFile).RealPath
                            });
                    else if(Node is ExternalFolder)
                        dragData.SetFileDropList(new StringCollection {
                                (DataContext as ExternalFolder).RealPath
                            });

                    DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move | DragDropEffects.Link);

                    args.Handled = true;
                }
            }
            else {
                clickedIn = false;
                base.OnMouseMove(args);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs args) {
            if(args.ChangedButton == MouseButton.Left && args.ClickCount == 1) {
                clickStarted = args.GetPosition(this);
                clickedIn = true;
            }
            base.OnMouseDown(args);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            clickedIn = false;
            base.OnMouseUp(e);
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new NodeTreeItem();
        }
    }
}
