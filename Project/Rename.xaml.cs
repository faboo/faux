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
        public Rename()
        {
            InitializeComponent();
            text.LostKeyboardFocus += (s, a) =>
                Visibility = Visibility.Collapsed;
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
    }
}
