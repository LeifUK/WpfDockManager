using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IUserViewContainer
    {
        public DocumentContainer()
        {
            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();
            _tabHeaderControl = new WpfControlLibrary.TabHeaderControl();
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            _tabHeaderControl.SelectionChanged += TabHeaderControl_SelectionChanged;
            _tabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;
            Children.Add(_tabHeaderControl);

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            Grid.SetRow(_tabHeaderControl, 1);
            Grid.SetRowSpan(_tabHeaderControl, 1);
            Grid.SetColumn(_tabHeaderControl, 0);
            Grid.SetColumnSpan(_tabHeaderControl, 1);
            _tabHeaderControl.UnselectedTabBackground = System.Windows.Media.Brushes.MidnightBlue;
            _tabHeaderControl.SelectedTabBackground = System.Windows.Media.Brushes.LightSalmon;

            _menuButton = new Button();
            Children.Add(_menuButton);
            Grid.SetRow(_menuButton, 1);
            Grid.SetColumn(_menuButton, 2);
            _menuButton.Click += delegate { if (DisplayGeneralMenu != null) DisplayGeneralMenu(); };
            // Warning warning warning
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _menuButton.Style = (System.Windows.Style)res["styleDocumentMenuButton"];

            _documentButton = new Button();
            Children.Add(_documentButton);
            Grid.SetRow(_documentButton, 1);
            Grid.SetColumn(_documentButton, 4);
            _documentButton.Click += delegate { Helpers.DisplayItemsMenu(_items, _tabHeaderControl, _selectedUserControl); };
            _documentButton.Style = FindResource("styleHeaderMenuButton") as Style;
        }

        private System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>> _items;
        public Action DisplayGeneralMenu;

        private void _tabHeaderControl_ItemsChanged(object sender, EventArgs e)
        {
            var items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();

            foreach (var item in _tabHeaderControl.Items)
            {
                items.Add((System.Collections.Generic.KeyValuePair<UserControl, IDocument>)item);
            }
            int selectedIndex = (_tabHeaderControl.SelectedIndex == -1) ? 0 : _tabHeaderControl.SelectedIndex;

            _items = items;

            _tabHeaderControl.SelectedIndex = selectedIndex;
        }

        private void TabHeaderControl_SelectionChanged(object sender, EventArgs e)
        {
            if ((_selectedUserControl != null) && (Children.Contains(_selectedUserControl)))
            {
                Children.Remove(_selectedUserControl);
            }

            _selectedUserControl = null;
            if (_tabHeaderControl.SelectedIndex != -1)
            {
                _selectedUserControl = _items[_tabHeaderControl.SelectedIndex].Key;
                Children.Add(_selectedUserControl);
                Grid.SetRow(_selectedUserControl, 2);
                Grid.SetColumn(_selectedUserControl, 0);
                Grid.SetColumnSpan(_selectedUserControl, 99);
                Grid.SetZIndex(_selectedUserControl, 2);
            }

            SelectionChanged?.Invoke(sender, e);
        }

        private void _tabHeaderControl_CloseTabRequest(object sender, EventArgs e)
        {
            for (int index = 0; index < _items.Count; ++index)
            {
                System.Collections.Generic.KeyValuePair<UserControl, IDocument> item = (System.Collections.Generic.KeyValuePair<UserControl, IDocument>)sender;
                if (_items[index].Key == item.Key)
                {
                    _items.RemoveAt(index);
                    break;
                }
            }
            TabClosed?.Invoke(sender, e);
        }

        private UserControl _selectedUserControl;
        private WpfControlLibrary.TabHeaderControl _tabHeaderControl;
        private Button _documentButton;
        private Button _menuButton;

        #region IDocumentContainer

        public event EventHandler SelectionChanged;
        public event EventHandler TabClosed;

        public string Title
        {
            get
            {
                if ((_items.Count == 0) || (_tabHeaderControl.SelectedIndex == -1))
                {
                    return null;
                }

                return _items[_tabHeaderControl.SelectedIndex].Value.Title;
            }
        }

        public void AddUserControl(UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IDocument);

            _items.Add(new System.Collections.Generic.KeyValuePair<UserControl, IDocument>(userControl, userControl.DataContext as IDocument));
            _tabHeaderControl.SelectedItem = _items[_items.Count - 1];
        }

        public UserControl ExtractUserControl(int index)
        {
            if ((index < 0) || (index >= _items.Count))
            {
                return null;
            }

            UserControl userControl = _items[index].Key;
            _items.RemoveAt(index);
            if (Children.Contains(userControl))
            {
                Children.Remove(userControl);
            }

            return userControl;
        }

        public int GetUserControlCount()
        {
            return _tabHeaderControl.Items.Count;
        }

        public int GetCurrentTabIndex()
        {
            return _tabHeaderControl.SelectedIndex;
        }

        public UserControl GetUserControl(int index)
        {
            if ((index < 0) || (index >= _items.Count))
            {
                return null;
            }

            return _items[index].Key;
        }

        #endregion IDocumentContainer
    }
}
