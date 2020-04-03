using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDockManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel.MainViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _layoutManager.Initialise();

            _layoutManager.SaveLayout(out System.Xml.XmlDocument xmlDocument, "C:\\Temp\\Layout.xml");
            _layoutManager.Clear();
            _layoutManager.LoadLayout(out System.Xml.XmlDocument xmlDocument_Loaded, "C:\\Temp\\Layout.xml");
            _layoutManager.SaveLayout(out System.Xml.XmlDocument xmlDocument_saved, "C:\\Temp\\Layout_2.xml");
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_layoutManager != null)
            {
                _layoutManager.Shutdown();
            }
        }
    }
}
