using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IDocumentContainer
    {
        public DocumentContainer()
        {
            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();
            _tabHeaderControl = new WpfControlLibrary.TabHeaderControl();
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            _tabHeaderControl.SelectionChanged += TabHeaderControl_SelectionChanged;
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

            Border border = new Border();
            Children.Add(border);
            Grid.SetRow(border, 0);
            Grid.SetRowSpan(border, 2);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 4);
            Grid.SetZIndex(border, -1);
            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            border.Background = System.Windows.Media.Brushes.SlateGray;

            _menuButton = new Button();
            Children.Add(_menuButton);
            Grid.SetRow(_menuButton, 1);
            Grid.SetColumn(_menuButton, 2);
            _menuButton.Click += delegate { DisplayGeneralMenu(); };
            // Warning warning warning
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _menuButton.Style = (System.Windows.Style)res["styleDocumentMenuButton"];

            _documentButton = new Button();
            Children.Add(_documentButton);
            Grid.SetRow(_documentButton, 1);
            Grid.SetColumn(_documentButton, 4);
            _documentButton.Click += delegate { DisplayItemsMenu(); };
            _documentButton.Style = FindResource("styleHeaderMenuButton") as Style;
        }

        System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>> _items;

        public void DisplayItemsMenu()
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
                menuItem.Command = new Command(delegate { _tabHeaderControl.SelectedIndex = (int)menuItem.CommandParameter; }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

        private void DisplayGeneralMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            int i = 0;
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Float?.Invoke(this, null); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            int viewCount = _items.Count;
            if (viewCount > 2)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup Current";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { UngroupCurrent?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            if (viewCount > 1)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { Ungroup?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }

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

        private UserControl _selectedUserControl;
        private WpfControlLibrary.TabHeaderControl _tabHeaderControl;
        private Button _documentButton;
        private Button _menuButton;

        #region IDocumentContainer

        public event EventHandler SelectionChanged;
        public event EventHandler Float;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;

        public string Title
        {
            get
            {
                if (_tabHeaderControl.SelectedItem == null)
                {
                    return null;
                }

                IDocument iDocument = _items[_tabHeaderControl.SelectedIndex].Value;
                if (iDocument == null)
                {
                    return null;
                }

                return iDocument.Title;
            }
        }

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
            _tabHeaderControl.SelectedIndex = _items.Count - 1;
        }

        private void TabControl_TabClosed(object sender, EventArgs e)
        {
            // WArning warning
        }

        public UserControl ExtractUserControl(int index)
        {
            if ((index < 0) || (index >= _items.Count))
            {
                return null;
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
                    _tabHeaderControl.SelectedIndex = index;
                }
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
