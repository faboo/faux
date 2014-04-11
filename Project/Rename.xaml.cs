using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Interaction logic for Rename.xaml
    /// </summary>
    public partial class Rename : UserControl
    {
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(String), typeof(Rename));

		public String FileName
		{
            get { return (String)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
		}

        static Rename() {
            DataContextProperty.OverrideMetadata(typeof(Rename),
                new FrameworkPropertyMetadata(OnDataContextChanged));
        }

        public Rename()
        {
            InitializeComponent();
            text.LostKeyboardFocus += (s, a) =>
                FinishEditing();
            text.GotKeyboardFocus += (s, a) =>
                Visibility = Visibility.Visible;
            text.PreviewKeyDown += (s, a) =>{
                if(a.Key == Key.Enter || a.Key == Key.Return)
                    Visibility = Visibility.Collapsed;
            };
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            text.Focus();
            text.SelectAll();
        }

        private void FinishEditing() {
            Window owner = this.FindVisualParent<Window>(w => true);
            Node node = (Node)DataContext; 
            
            Visibility = Visibility.Collapsed;

            if(node is File &&
                MessageBox.Show(owner, "Change real filename too?", owner.Title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) {
                (node as File).Rename(FileName);
            }
            else {
                node.Name = FileName;
            }
        }

        private static void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            Rename control = (Rename)obj;

            control.FileName = ((Node)control.DataContext).Name;
        }
    }
}
