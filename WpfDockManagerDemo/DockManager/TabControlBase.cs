using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class TabControlBase : Grid, ITabControl
    {
        public TabControlBase(int headerRow, int contentRow)
        {
            HeaderRow = headerRow;
            ContentRow = contentRow;
            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();
            _tabHeaderControl = new WpfControlLibrary.TabHeaderControl();
            _tabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
            _tabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            _tabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            Children.Add(_tabHeaderControl);
        }

        private void _tabHeaderControl_ItemsChanged(object sender, System.EventArgs e)
        {
            var items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IDocument>>();

            foreach (var item in _tabHeaderControl.Items)
            {
                items.Add((System.Collections.Generic.KeyValuePair<UserControl,IDocument>)item);
            }
            int selectedIndex = _tabHeaderControl.SelectedIndex;

            _items = items;
            _tabHeaderControl.SelectedIndex = selectedIndex;

            _tabHeaderControl_SelectionChanged(this, null);
        }

        protected readonly int HeaderRow;
        protected readonly int ContentRow;

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

        protected void DisplayItemsMenu()
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

        protected WpfControlLibrary.TabHeaderControl _tabHeaderControl;

        protected UserControl _selectedUserControl;
        protected System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl,IDocument>> _items;

        #region ITabControl

        public event System.EventHandler SelectionChanged;
        public event System.EventHandler TabClosed;

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
            Grid.SetRow(userControl, ContentRow);
            Grid.SetColumn(userControl, 0);
            Grid.SetColumnSpan(userControl, 99);
            Children.Add(userControl);
            // Do this AFTER adding the child 
            _tabHeaderControl.SelectedIndex = _items.Count - 1;
        }

        public UserControl RemoveAt(int index)
        {
            if ((index < 0) || (index >= Count))
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

        public UserControl SelectedItem { get { return ((SelectedIndex > -1) && (SelectedIndex < _items.Count)) ? _items[SelectedIndex].Key : null; } }

        #endregion ITabControl
    }
}
