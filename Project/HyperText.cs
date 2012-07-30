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
using System.Diagnostics;

namespace Project
{
    public class HyperText : Button
    {
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Uri), typeof(HyperText), new FrameworkPropertyMetadata(OnTextChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(HyperText), new FrameworkPropertyMetadata(OnTextChanged));

        static HyperText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperText), new FrameworkPropertyMetadata(typeof(HyperText)));
        }

		public Uri Target
		{
			get { return (Uri)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		public String Text
		{
			get { return (String)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as HyperText).OnTextChanged();
        }

        private void OnTextChanged()
        {
			if(!String.IsNullOrWhiteSpace(Text))
				Content = Text;
            else if (Target != null)
                Content = Target.ToString();
		}

        protected override void OnClick()
        {
            Process launcher = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Target.AbsoluteUri,
                    ErrorDialog = true,
                }
            };

            base.OnClick();

            launcher.Start();
            launcher.Dispose();
        }
    }
}
