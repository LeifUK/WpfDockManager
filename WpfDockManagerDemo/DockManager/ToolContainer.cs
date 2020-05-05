using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class ToolContainer : Grid, IUserViewContainer
    {
        public ToolContainer()
        {
            rowDefinition_UserControl = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) };
            rowDefinition_TabHeader = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) };
            rowDefinition_Spacer = new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) };
            RowDefinitions.Add(rowDefinition_UserControl);
            RowDefinitions.Add(rowDefinition_TabHeader);
            RowDefinitions.Add(rowDefinition_Spacer);

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();

            _tabHeaderControl = new WpfControlLibrary.TabHeaderControl();
            _tabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
            _tabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            Children.Add(_tabHeaderControl);
            Grid.SetRow(_tabHeaderControl, 1);
            Grid.SetColumn(_tabHeaderControl, 0);
            Grid.SetZIndex(_tabHeaderControl, 1);
            _tabHeaderControl.UnselectedTabBackground = System.Windows.Media.Brushes.MidnightBlue;
            _tabHeaderControl.SelectedTabBackground = System.Windows.Media.Brushes.LightSalmon;

            _border = new Border();
            Children.Add(_border);
            Grid.SetRow(_border, 1);
            Grid.SetRowSpan(_border, 2);
            Grid.SetColumn(_border, 0);
            Grid.SetColumnSpan(_border, 4);
            Grid.SetZIndex(_border, -1);
            _border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _border.Background = System.Windows.Media.Brushes.Gray;

            _button = new Button();
            Children.Add(_button);
            Grid.SetRow(_button, 1);
            Grid.SetColumn(_button, 2);
            _button.Click += delegate { Helpers.DisplayItemsMenu(_items, _tabHeaderControl, _selectedUserControl); };
            // Warning warning warning
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _button.Style = (System.Windows.Style)res["MenuButtonStyle"];
        }

        private RowDefinition rowDefinition_UserControl;
        private RowDefinition rowDefinition_TabHeader;
        private RowDefinition rowDefinition_Spacer;

        protected System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>> _items;
        public WpfControlLibrary.TabHeaderControl _tabHeaderControl;
        protected UserControl _selectedUserControl;
        private Button _button;
        private Border _border;

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

        private void _tabHeaderControl_ItemsChanged(object sender, System.EventArgs e)
        {
            var items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();

            foreach (var item in _tabHeaderControl.Items)
            {
                items.Add((System.Collections.Generic.KeyValuePair<UserControl, IDocument>)item);
            }
            int selectedIndex = _tabHeaderControl.SelectedIndex;

            _items = items;
            _tabHeaderControl.SelectedIndex = selectedIndex;

            _tabHeaderControl_SelectionChanged(this, null);
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
                        TabClosed?.Invoke(sender, null);
                    }
                }
            }
        }

        public UserControl RemoveAt(int index)
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
                    _selectedUserControl = _items[index].Key;
                    Children.Add(_selectedUserControl);
                }
            }

            return userControl;
        }

        private void CheckTabCount()
        {
            if (_items.Count == 1)
            {
                rowDefinition_TabHeader.Height = new GridLength(0);
                rowDefinition_Spacer.Height = new GridLength(0);
            }
            else
            {
                rowDefinition_TabHeader.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto);
                rowDefinition_Spacer.Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel);
            }
        }

        private void TabControl_TabClosed(object sender, EventArgs e)
        {
            CheckTabCount();
        }

        private void TabControl_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

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
            if ((_selectedUserControl != null) && Children.Contains(_selectedUserControl))
            {
                Children.Remove(_selectedUserControl);
            }
            _selectedUserControl = userControl;
            Children.Add(userControl);
            Grid.SetRow(userControl, 0);
            Grid.SetColumn(userControl, 0);
            Grid.SetColumnSpan(userControl, 99);
            // Do this AFTER adding the child 
            _tabHeaderControl.SelectedIndex = _items.Count - 1;

            CheckTabCount();
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
            CheckTabCount();

            return userControl;
        }

        public int GetUserControlCount()
        {
            return _items.Count;
        }

        public int GetCurrentTabIndex()
        {
            if (_items.Count == 0)
            {
                return -1;
            }

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
