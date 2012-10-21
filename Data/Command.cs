using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace Project
{
    public class Command: Freezable {
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Command));
		public static readonly DependencyProperty ProgramProperty = DependencyProperty.Register("Program", typeof(string), typeof(Command));
		public static readonly DependencyProperty ArgumentsProperty = DependencyProperty.Register("Arguments", typeof(string), typeof(Command));
		public static readonly DependencyProperty InCmdProperty = DependencyProperty.Register("InCmd", typeof(bool), typeof(Command));

		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public string Program
		{
			get { return (string)GetValue(ProgramProperty); }
			set { SetValue(ProgramProperty, value); }
		}
		public string Arguments
		{
			get { return (string)GetValue(ArgumentsProperty); }
			set { SetValue(ArgumentsProperty, value); }
		}
		public bool InCmd
		{
			get { return (bool)GetValue(InCmdProperty); }
			set { SetValue(InCmdProperty, value); }
		}

        public void Launch(Node selectedNode, MacroExpander macros) {
            ProcessStartInfo info = new ProcessStartInfo();
            macros.Target = selectedNode as File;

            info.FileName = Program;
            info.Arguments = macros.Expand(Arguments);

            Fork(info);
        }

        protected void Fork(ProcessStartInfo info) {
            Process launcher = new Process();

            launcher.StartInfo = info;
            launcher.StartInfo.ErrorDialog = true;
            launcher.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();

            launcher.Start();
            launcher.Dispose();
        }

        protected override Freezable CreateInstanceCore() {
            return new Command();
        }
    }
}
