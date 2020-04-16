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

namespace WpfListboxDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowModel();
            _labelSelectedItem.Content = (DataContext as MainWindowModel).ListBoxItems[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _labelSelectedItem.Content = _ListBox.SelectedItem;
        }
    }
}
