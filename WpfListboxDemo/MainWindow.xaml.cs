using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            _comboBoxSelectedTabBackground.ItemsSource = typeof(Brushes).GetProperties();
            _comboBoxUnselectedTabBackground.ItemsSource = typeof(Brushes).GetProperties();
            _tabHeaderTop.CloseTabRequest += _tabHeaderTop_CloseTabRequest; 
            _tabHeaderBottom.CloseTabRequest += _tabHeaderBottom_CloseTabRequest;

            DataContext = new MainWindowModel();
            (DataContext as MainWindowModel).SelectedHeader = (DataContext as MainWindowModel).ListBoxItems[2];

            _comboBoxSelectedTabBackground.SelectedIndex = 0;
            _comboBoxUnselectedTabBackground.SelectedIndex = 17;

            _comboBoxSelectedTabBackground_SelectionChanged(null, null);
            _comboBoxUnselectedTabBackground_SelectionChanged(null, null);
        }

        private void _tabHeaderBottom_CloseTabRequest(object sender, System.EventArgs e)
        {
            if ((DataContext as MainWindowModel).ListBoxItems.Contains(sender as TabHeaderItem))
            {
                int index = (DataContext as MainWindowModel).ListBoxItems.IndexOf(sender as TabHeaderItem);
                (DataContext as MainWindowModel).ListBoxItems.RemoveAt(index);
                if (index >= (DataContext as MainWindowModel).ListBoxItems.Count)
                {
                    --index;
                }
                if (index > -1)
                {
                    (DataContext as MainWindowModel).SelectedHeader = (DataContext as MainWindowModel).ListBoxItems[index];
                }
            }
        }

        private void _tabHeaderTop_CloseTabRequest(object sender, System.EventArgs e)
        {
            _tabHeaderBottom_CloseTabRequest(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _buttonFile_Click(object sender, RoutedEventArgs e)
        {
            MainWindowModel viewModel = (DataContext as MainWindowModel);
            if (viewModel == null)
            {
                return;
            }

            ContextMenu contextMenu = new ContextMenu();
            foreach (var item in viewModel.ListBoxItems)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = item.Label;
                menuItem.IsChecked = item == viewModel.SelectedHeader;
                menuItem.Command = new Command(delegate { viewModel.SelectedHeader = item; }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private Brush ConvertSelectedItemToBrush(ComboBox comboBox)
        {
            var converter = new System.Windows.Media.BrushConverter();
            return (Brush)converter.ConvertFromString(((System.Reflection.PropertyInfo)(comboBox.SelectedItem)).Name);
        }

        private void _comboBoxSelectedTabBackground_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as MainWindowModel).SelectedTabBackground = ConvertSelectedItemToBrush(_comboBoxSelectedTabBackground); ;
        }

        private void _comboBoxUnselectedTabBackground_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as MainWindowModel).UnselectedTabBackground = ConvertSelectedItemToBrush(_comboBoxUnselectedTabBackground); ;
        }

        private void _buttonChooseFont_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog dlg = new System.Windows.Forms.FontDialog();
            dlg.Font = new System.Drawing.Font((DataContext as MainWindowModel).FontFamily, (float)(DataContext as MainWindowModel).FontSize, dlg.Font.Style);
            dlg.ShowEffects = false;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                (DataContext as MainWindowModel).FontFamily = dlg.Font.Name;
                (DataContext as MainWindowModel).FontSize = dlg.Font.Size;
            }
        }
    }
}
