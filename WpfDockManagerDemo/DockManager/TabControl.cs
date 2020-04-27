using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    class TabControl : Grid
    {
        public TabControl()
        {
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            _tabHeaderControl = new WpfControlLibrary.TabHeaderControl();
            Children.Add(_tabHeaderControl);
            Grid.SetRow(_tabHeaderControl, 1);
            Grid.SetColumn(_tabHeaderControl, 0);
            _tabHeaderControl.UnselectedTabBackground = System.Windows.Media.Brushes.MidnightBlue;
            _tabHeaderControl.SelectedTabBackground = System.Windows.Media.Brushes.LightSalmon;

            Border border = new Border();
            Children.Add(border);
            Grid.SetRow(border, 1);
            Grid.SetRowSpan(border, 2);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 4);
            Grid.SetZIndex(border, -1);
            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            border.Background = System.Windows.Media.Brushes.Gainsboro;

            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
            _tabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;

            _button = new Button();
            Children.Add(_button);
            Grid.SetRow(_button, 1);
            Grid.SetColumn(_button, 2);
            _button.Click += _button_Click;
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _button.Style = (System.Windows.Style)res["MenuButtonStyle"];
        }

        private void _tabHeaderControl_CloseTabRequest(object sender, System.EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            System.Collections.Generic.KeyValuePair<UserControl, IDocument> item = (System.Collections.Generic.KeyValuePair<UserControl, IDocument>)sender;
            if (item.Value.CanClose)
            {
                if (item.Value.HasChanged)
                {
                    System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("There are unsaved changes in the document. Do you wish to save the changes before closing?", "Close " + item.Value.Title, System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                    
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        item.Value.Save();
                    }
                    
                    if (dialogResult != System.Windows.Forms.DialogResult.Cancel)
                    {
                        int index = _items.IndexOf(item);
                        RemoveAt(index);
                        TabClosed?.Invoke(null, null);
                    }
                }
            }
        }

        private void _button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            int i = 0;
            foreach (var item in _items)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = item.Value.Title;
                menuItem.IsChecked = item.Key == _selectedUserControl;
                menuItem.CommandParameter = i;
                ++i;
                menuItem.Command = new Command(delegate { _tabHeaderControl.SelectedIndex = (int) menuItem.CommandParameter; }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        public event System.EventHandler SelectionChanged;
        public event System.EventHandler TabClosed;

        private void _tabHeaderControl_SelectionChanged(object sender, System.EventArgs e)
        {
            if (_selectedUserControl != null)
            {
                Children.Remove(_selectedUserControl);
                _selectedUserControl = null;
            }
            
            if ((_tabHeaderControl.SelectedIndex > -1) && (_tabHeaderControl.SelectedIndex < _items.Count))
            {
                _selectedUserControl = _items[_tabHeaderControl.SelectedIndex].Key;
                Children.Add(_selectedUserControl);
            }

            SelectionChanged?.Invoke(sender, e);
        }

        WpfControlLibrary.TabHeaderControl _tabHeaderControl;
        Button _button;

        private UserControl _selectedUserControl;
        private System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl,IDocument>> _items;

        public void AddUserControl(UserControl userControl)
        {
            if (userControl == null)
            {
                throw new System.Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": userControl is null");
            }

            IDocument iDocument = userControl.DataContext as IDocument;
            if (iDocument == null)
            {
                throw new System.Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": userControl is not a IDocument");
            }

            _items.Add(new System.Collections.Generic.KeyValuePair<UserControl, IDocument>(userControl, iDocument));
            if (_selectedUserControl != null)
            {
                Children.Remove(_selectedUserControl);
            }
            _selectedUserControl = userControl;
            _tabHeaderControl.SelectedIndex = _items.Count - 1;
            Grid.SetRow(userControl, 0);
            Grid.SetColumn(userControl, 0);
            Grid.SetColumnSpan(userControl, 4);
        }

        public UserControl RemoveAt(int index)
        {
            if ((index < 0) || (index >= Count))
            {
                throw new System.Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": invalid index (" + index + ")");
            }

            UserControl userControl = _items[index].Key;
            _items.RemoveAt(index);
            if (userControl == _selectedUserControl)
            {
                Children.Remove(_selectedUserControl);
                _selectedUserControl = null;

                if (_items.Count > 0)
                {
                    if (index >= _items.Count)
                    {
                        --index;
                    }
                    _selectedUserControl = _items[index].Key;
                    Children.Add(_selectedUserControl);
                }
            }

            return userControl;
        }

        public UserControl GetAt(int index)
        {
            if ((index < 0) || (index >= Count))
            {
                throw new System.Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": invalid index (" + index + ")");
            }

            return _items[index].Key;
        }

        public int Count { get { return _items.Count; } }

        public int SelectedIndex { get { return _tabHeaderControl.SelectedIndex; } }
        public UserControl SelectedItem { get { return SelectedIndex > -1 ? _items[SelectedIndex].Key : null; } }
    }
}
