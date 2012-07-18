using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Diagnostics;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Current), typeof(MainWindow));

		public Current Project
		{
			get { return (Current)GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}

        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            Settings load = Settings.Current;
            try
            {
                if((Application.Current as App).Args.Length > 0)
                    Open((Application.Current as App).Args[0]);
                else if (!String.IsNullOrWhiteSpace(Settings.Current.LastProject))
                    Open(Settings.Current.LastProject);
            }
            catch
            {
            }
        }

        protected override void OnClosed(EventArgs args)
        {
            Settings.Save();
            base.OnClosed(args);
        }

        private void Open(string file)
        {
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(file));
            Project = new Current(file);
            Settings.Current.LastProject = file;
        }

        protected override void OnDragOver(DragEventArgs args)
        {
            if (args.Data.GetFormats().Contains(DataFormats.FileDrop))
            {
                string file = ((string[])args.Data.GetData(DataFormats.FileDrop, true))[0];
                args.Effects = DragDropEffects.All;
                args.Handled = file.EndsWith(".project");
            }
            else
            {
                args.Handled = false;
                args.Effects = DragDropEffects.None;
            }
            args.Handled = true;
        }

        protected override void OnDrop(DragEventArgs args)
        {
            object dropData = args.Data.GetData(DataFormats.FileDrop, true);

            if (dropData != null)
            {
                string file = ((string[])dropData)[0];

                Settings.Current.LastProject = file;
                Open(file);

                args.Handled = true;
            }
        }

        private void OnFileLaunch(object sender, MouseButtonEventArgs args)
        {
            object node = ((TreeViewItem)args.Source).DataContext;

            if (node is OtherFile)
                StartFile((OtherFile)node);
            else if (node is File)
                LaunchFile((File)node);
            else
                base.OnMouseDoubleClick(args);
        }

        private void ExecuteSave(object sender, ExecutedRoutedEventArgs args)
        {
            try
            {
                Project.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Failed to Save Project", MessageBoxButton.OK);
            }
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = Project != null;
        }

        private void ExecuteNew(object sender, ExecutedRoutedEventArgs args)
        {
            Folder container = args.Parameter as Folder;

            container.Add(new Folder() { Name = "New Folder" });
        }

        private void CanNew(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = Project != null;
        }

        private void ExecuteExplore(object sender, ExecutedRoutedEventArgs args)
        {
            Process.Start(System.IO.Path.GetDirectoryName(Project.SavePath));
        }

        private void CanExplore(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = Project != null;
        }

        private void ExecuteEditTypes(object sender, ExecutedRoutedEventArgs args)
        {
            TypesEditor dialog = new TypesEditor();

            dialog.Owner = this;
            dialog.Closed += (s, a) => RecalculateTypes();
            dialog.Show();
        }

        private void ExecuteLaunch(object sender, ExecutedRoutedEventArgs args)
        {
            LaunchFile((File)args.Parameter);
        }

        private void ExecuteStart(object sender, ExecutedRoutedEventArgs args)
        {
            StartFile((Node)args.Parameter);
        }

        private void ExecuteCommand(object sender, ExecutedRoutedEventArgs args)
        {
            Process.Start("cmd");
        }

        private void ExecuteRefresh(object sender, ExecutedRoutedEventArgs args)
        {
            AllFiles allFiles = new AllFiles(Project.BaseFolder);

            (args.Parameter as OtherFilesFolder).Update(allFiles);
        }

        private void LaunchFile(File node)
        {
            MacroExpander macros = new MacroExpander { Project = Project };
            node.Launch(macros);
        }

        private void StartFile(Node node)
        {
            if(node is File)
                (node as File).Start();
            else if (node is OtherFile)
                (node as OtherFile).Start();
            else if (node is OtherFilesFolder)
                (node as OtherFilesFolder).Start();
        }

        private void RecalculateTypes()
        {
            Project.RecalculateTypes();
        }
    }
}
