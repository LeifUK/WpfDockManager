using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal abstract class ViewContainer : Grid, IViewContainer
    {
        protected System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>> _items;
        public WpfControlLibrary.TabHeaderControl _tabHeaderControl;
        protected UserControl _selectedUserControl;

        protected abstract void _tabHeaderControl_SelectionChanged(object sender, System.EventArgs e);

        protected void _tabHeaderControl_ItemsChanged(object sender, System.EventArgs e)
        {
            var items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>>();

            foreach (var item in _tabHeaderControl.Items)
            {
                items.Add((System.Collections.Generic.KeyValuePair<UserControl, IViewModel>)item);
            }
            int selectedIndex = (_tabHeaderControl.SelectedIndex == -1) ? 0 : _tabHeaderControl.SelectedIndex;

            _items = items;
            _tabHeaderControl.SelectedIndex = selectedIndex;

            _tabHeaderControl_SelectionChanged(this, null);
        }

        protected void _tabHeaderControl_CloseTabRequest(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            System.Collections.Generic.KeyValuePair<UserControl, IViewModel> item = (System.Collections.Generic.KeyValuePair<UserControl, IViewModel>)sender;
            if (item.Value.CanClose)
            {
                if (item.Value.HasChanged)
                {
                    System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("There are unsaved changes in the document. Do you wish to save the changes before closing?", "Close " + item.Value.Title, System.Windows.Forms.MessageBoxButtons.YesNoCancel);

                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        item.Value.Save();
                    }

                    if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                int index = _items.IndexOf(item);

                _items.RemoveAt(index);
                _tabHeaderControl.ItemsSource = _items;

                if (item.Key == _selectedUserControl)
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

                CheckTabCount();

                TabClosed?.Invoke(sender, null);
            }
        }

        #region IViewContainer

        public abstract event EventHandler SelectionChanged;
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

        protected abstract void CheckTabCount();

        public abstract void AddUserControl(UserControl userControl);

        public void InsertUserControl(int index, UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(index > -1);
            System.Diagnostics.Trace.Assert(index <= _items.Count);
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IViewModel);

            _items.Insert(index, new System.Collections.Generic.KeyValuePair<UserControl, IViewModel>(userControl, userControl.DataContext as IViewModel));
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

        public IViewModel GetIViewModel(int index)
        {
            UserControl userControl = GetUserControl(index);
            if (userControl == null)
            {
                return null;
            }

            return userControl.DataContext as IViewModel;
        }

        #endregion IViewContainer
    }
}
