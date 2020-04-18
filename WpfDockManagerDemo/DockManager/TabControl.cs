using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    class TabControl : Grid
    {
        public TabControl()
        {
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(10, System.Windows.GridUnitType.Pixel) });

            _tabHeaderControl = new TabHeaderControl();
            Children.Add(_tabHeaderControl);
            Grid.SetRow(_tabHeaderControl, 1);
            Grid.SetColumn(_tabHeaderControl, 0);

            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
        }

        public event System.EventHandler SelectionChanged;

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

        TabHeaderControl _tabHeaderControl;

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
            _selectedUserControl.BorderBrush = System.Windows.Media.Brushes.Gray;
            _selectedUserControl.BorderThickness = new System.Windows.Thickness(0, 0, 0, 1);
            Children.Add(_selectedUserControl);
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
            }

            if (_items.Count > 0)
            {
                if (index >= _items.Count)
                {
                    --index;
                }
                _selectedUserControl = _items[index].Key;
                Children.Add(_selectedUserControl);
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
        public UserControl SelectedItem { get { return _items[_tabHeaderControl.SelectedIndex].Key; } }
    }
}
