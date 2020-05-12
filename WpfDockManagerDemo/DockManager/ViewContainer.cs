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

        #region IViewContainer

        public abstract event EventHandler SelectionChanged;
        public abstract event EventHandler TabClosed;

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
