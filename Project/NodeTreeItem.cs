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
        }

        protected override void OnDragOver(DragEventArgs args) {
            if(Node is Folder) {
                if(args.Data.GetDataPresent("Project.File")) {
                    args.Effects = DragDropEffects.Move;
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.Folder")) {
                    args.Effects = DragDropEffects.Move;
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.OtherFile")) {
                    args.Effects = DragDropEffects.Link;
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.OtherFilesFolder")) {
                    args.Effects = DragDropEffects.Link;
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent(DataFormats.FileDrop)
                    && !((string[])args.Data.GetData(DataFormats.FileDrop, true))[0]
                        .EndsWith(".project")) {

                    args.Effects = DragDropEffects.Link;
                    args.Handled = true;
                }
                else {
                    base.OnDragEnter(args);
                }
            }
            else {
                base.OnDragEnter(args);
            }
        }

        protected override void OnDrop(DragEventArgs args) {
            if(Node is Folder) {
                Node node = args.Data.GetData("Project.File") as Node
                    ?? args.Data.GetData("Project.Folder") as Node
                    ?? args.Data.GetData("Project.OtherFile") as Node
                    ?? args.Data.GetData("Project.OtherFilesFolder") as Node
                    ?? args.Data.GetData("Project.ExternalFile") as Node
                    ?? args.Data.GetData("Project.ExternalFolder") as Node;

                if(node != null && node.Moveable && (this.Node as Folder).ChangeableContents) {
                    (this.Node as Folder).Add(node);
                    args.Handled = true;
                }
                /*if(args.Data.GetDataPresent("Project.File")) {
                    File node = (File)args.Data.GetData("Project.File");

                    (this.Node as Folder).Add(node);
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.Folder")) {
                    Folder node = (Folder)args.Data.GetData("Project.Folder");

                    (this.Node as Folder).Add(node);
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.OtherFile")) {
                    OtherFile node = (OtherFile)args.Data.GetData("Project.OtherFile");

                    node.Parent.Remove(node);
                    (this.Node as Folder).Add(node);
                    args.Handled = true;
                }
                else if(args.Data.GetDataPresent("Project.OtherFilesFolder")) {
                    OtherFilesFolder node = (OtherFilesFolder)args.Data.GetData("Project.OtherFilesFolder");

                    node.Parent.Remove(node);
                    (this.Node as Folder).Add(node);
                    args.Handled = true;
                }*/
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
                else {
                    base.OnDrop(args);
                }
            }
            else {
                base.OnDrop(args);
            }
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
