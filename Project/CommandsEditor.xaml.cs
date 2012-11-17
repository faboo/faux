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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Project {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CommandsEditor : Window {
        public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register("Commands", typeof(IList<Command>), typeof(CommandsEditor));

		public IList<Command> Commands
		{
            get { return (IList<Command>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
		}

        public CommandsEditor() {
            InitializeComponent();
        }

        private void OnNewExecuted(object sender, ExecutedRoutedEventArgs args) {
            Command newCommand = new Command();
            Commands.Add(newCommand);
            commands.SelectedItem = newCommand;
        }

        private void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs args) {
            Commands.Remove(commands.SelectedItem as Command);
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs args) {
            if(IsInitialized)
                args.CanExecute = commands.SelectedItem != null;
        }
    }
}
