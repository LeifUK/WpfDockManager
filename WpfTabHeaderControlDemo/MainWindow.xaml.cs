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
            _tabHeader1.CloseTabRequest += _tabHeader1_CloseTabRequest;
            _tabHeader2.CloseTabRequest += _tabHeader2_CloseTabRequest;
            _tabHeader3.CloseTabRequest += _tabHeader3_CloseTabRequest;
            _tabHeader1.ItemsChanged += _tabHeader1_ItemsChanged;
            _tabHeader2.ItemsChanged += _tabHeader2_ItemsChanged;
            _tabHeader3.ItemsChanged += _tabHeader3_ItemsChanged;

            DataContext = new MainWindowModel();
            (DataContext as MainWindowModel).SelectedHeader = (DataContext as MainWindowModel).ListBoxItems[2];

            _comboBoxSelectedTabBackground.SelectedIndex = 0;
            _comboBoxUnselectedTabBackground.SelectedIndex = 17;

            _comboBoxSelectedTabBackground_SelectionChanged(null, null);
            _comboBoxUnselectedTabBackground_SelectionChanged(null, null);
        }

        private void ItemsChanged(WpfControlLibrary.TabHeaderControl tabHeaderControl)
        {
            MainWindowModel viewModel = (DataContext as MainWindowModel);
            if (viewModel == null)
            {
                return;
            }
            System.Collections.ObjectModel.ObservableCollection<TabHeaderItem> listBoxItems = new System.Collections.ObjectModel.ObservableCollection<TabHeaderItem>();
            foreach (var item in tabHeaderControl.Items)
            {
                listBoxItems.Add(item as TabHeaderItem);
            }
            viewModel.ListBoxItems = listBoxItems;
        }

        private void _tabHeader1_ItemsChanged(object sender, System.EventArgs e)
        {
            ItemsChanged(_tabHeader1);
        }

        private void _tabHeader2_ItemsChanged(object sender, System.EventArgs e)
        {
            ItemsChanged(_tabHeader2);
        }

        private void _tabHeader3_ItemsChanged(object sender, System.EventArgs e)
        {
            ItemsChanged(_tabHeader3);
        }

        private void _tabHeader1_CloseTabRequest(object sender, System.EventArgs e)
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

        private void _tabHeader2_CloseTabRequest(object sender, System.EventArgs e)
        {
            _tabHeader1_CloseTabRequest(sender, e);
        }

        private void _tabHeader3_CloseTabRequest(object sender, System.EventArgs e)
        {
            _tabHeader1_CloseTabRequest(sender, e);
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

        private void _buttonFile1_Click(object sender, RoutedEventArgs e)
        {
            _buttonFile_Click(sender, e);
        }

        private void _buttonFile2_Click(object sender, RoutedEventArgs e)
        {
            _buttonFile_Click(sender, e);
        }

        private void _buttonFile3_Click(object sender, RoutedEventArgs e)
        {
            _buttonFile_Click(sender, e);
        }

        int clickIndex = -1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowModel mainWindowModel = (DataContext as MainWindowModel);
            if ((clickIndex > -1) && (clickIndex < mainWindowModel.ListBoxItems.Count))
            {
                mainWindowModel.ListBoxItems.RemoveAt(clickIndex);
            }
            if (clickIndex > 0)
            {
                --clickIndex;
            }
            if (mainWindowModel.ListBoxItems.Count > 0)
            {
                _tabHeader3.SelectedIndex = clickIndex;
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListBoxItem listBoxItem = sender as System.Windows.Controls.ListBoxItem;
            if (listBoxItem == null)
            {
                return;
            }

            TabHeaderItem tabHeaderItem = listBoxItem.DataContext as TabHeaderItem;
            if (tabHeaderItem == null)
            {
                return;
            }

            clickIndex = (DataContext as MainWindowModel).ListBoxItems.IndexOf(tabHeaderItem);
        }
    }
}
