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
using System.Threading;
using System.Net;
using System.Xml;

namespace Project
{
    /// <summary>
    /// Interaction logic for IconBrowser.xaml
    /// </summary>
    public partial class IconBrowser : Window
    {
        private const int MaxSize = 64;
        private const int MinSize = 24;
        private const string API_KEY = "f12356947d88588eda891c844ab4e836";

		public static readonly DependencyProperty IconsProperty = DependencyProperty.Register("Icons", typeof(ObservableCollection<Icon>), typeof(IconBrowser));
		public static readonly DependencyProperty SearchTermsProperty = DependencyProperty.Register("SearchTerms", typeof(string), typeof(IconBrowser));
		public static readonly DependencyProperty SelectedIconProperty = DependencyProperty.Register("SelectedIcon", typeof(Icon), typeof(IconBrowser));

		public ObservableCollection<Icon> Icons
		{
			get { return (ObservableCollection<Icon>)GetValue(IconsProperty); }
			set { SetValue(IconsProperty, value); }
		}
		public string SearchTerms
		{
			get { return (string)GetValue(SearchTermsProperty); }
			set { SetValue(SearchTermsProperty, value); }
		}
		public Icon SelectedIcon
		{
			get { return (Icon)GetValue(SelectedIconProperty); }
			set { SetValue(SelectedIconProperty, value); }
		}

        public IconBrowser()
        {
            SearchTerms = "";
            Icons = new ObservableCollection<Project.Icon>();
            InitializeComponent();
        }

		private void ExecuteSearch(object sender, ExecutedRoutedEventArgs args){
			string terms = SearchTerms;

			ThreadPool.QueueUserWorkItem(o =>
					SearchIcons(terms));

            args.Handled = true;
		}

        private void CanSearch(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = !String.IsNullOrWhiteSpace(SearchTerms);
        }

		private void SearchIcons(string terms){
            string searchUrl = String.Format(
                "http://www.iconfinder.com/xml/search/?q={0}&p=0&c=25&min={1}&max={2}&api_key={3}",
                terms.Replace(' ', '+'),
                MinSize,
                MaxSize,
                API_KEY);
            WebClient web = new WebClient();
            string strResponse = web.DownloadString(searchUrl);
            XmlDocument xmlResponse = new XmlDocument();

            xmlResponse.LoadXml(strResponse);

            foreach (var image in xmlResponse.SelectNodes("/results/iconmatches/icon/image"))
            {
                if (image is XmlElement)
                    AddIcon((image as XmlElement).InnerText);
            }
		}

        private void AddIcon(string location)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                Icons.Add(new Icon { URL = new Uri(location) })));
        }

        private void ExecuteSelect(object sender, ExecutedRoutedEventArgs args)
        {
            DialogResult = true;
            Close();
            args.Handled = true;
        }

        private void CanSelect(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = SelectedIcon != null;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class Icon : DependencyObject {
		public static readonly DependencyProperty URLProperty = DependencyProperty.Register("URL", typeof(Uri), typeof(Icon), new FrameworkPropertyMetadata(OnUrlChanged));
		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(Icon));

		public Uri URL
		{
			get { return (Uri)GetValue(URLProperty); }
			set { SetValue(URLProperty, value); }
		}
		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

        private void OnUrlChanged()
        {
            BitmapImage icon;

            try
            {
                icon = new BitmapImage();

                icon.BeginInit();
                icon.UriSource = URL;

                icon.EndInit();
                Image = icon;
            }
            catch
            {
            }

        }

		private static void OnUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args){
            (sender as Icon).OnUrlChanged();
		}
    }
}
