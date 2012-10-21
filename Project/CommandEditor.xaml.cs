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

namespace Project {
    /// <summary>
    /// Interaction logic for CommandEditor.xaml
    /// </summary>
    public partial class CommandEditor : UserControl {
        public CommandEditor() {
            String macroTips = "Available macros:\n";

            InitializeComponent();

            foreach (string macro in MacroExpander.Macros)
                macroTips = macroTips + macro + "\n";

            argsHelp.Text = macroTips;
        }

        private void OnArgQuestionClicked(object sender, RoutedEventArgs e) {
            argsToolTip.IsOpen = !argsToolTip.IsOpen;
        }
    }
}
