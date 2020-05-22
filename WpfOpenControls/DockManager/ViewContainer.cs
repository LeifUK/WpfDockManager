using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal abstract class ViewContainer : Grid, IViewContainer
    {
        protected System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>> _items;
        public WpfOpenControls.Controls.TabHeaderControl _tabHeaderControl;
        protected UserControl _selectedUserControl;
        protected Border _gap;

        protected void CreateTabControl(int row, int column)
        {
            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>>();

            _tabHeaderControl = new WpfOpenControls.Controls.TabHeaderControl();
            _tabHeaderControl.SelectionChanged += _tabHeaderControl_SelectionChanged;
            _tabHeaderControl.CloseTabRequest += _tabHeaderControl_CloseTabRequest;
            _tabHeaderControl.FloatTabRequest += _tabHeaderControl_FloatTabRequest;
            _tabHeaderControl.ItemsChanged += _tabHeaderControl_ItemsChanged;
            _tabHeaderControl.ItemsSource = _items;
            _tabHeaderControl.DisplayMemberPath = "Value.Title";
            Children.Add(_tabHeaderControl);
            Grid.SetRow(_tabHeaderControl, row);
            Grid.SetColumn(_tabHeaderControl, column);
        }

        protected abstract void SetSelectedUserControlGridPosition();

        protected void _tabHeaderControl_SelectionChanged(object sender, System.EventArgs e)
        {
            if ((_selectedUserControl != null) && (Children.Contains(_selectedUserControl)))
            {
                Children.Remove(_selectedUserControl);
            }
            _selectedUserControl = null;

            if ((_tabHeaderControl.SelectedIndex > -1) && (_tabHeaderControl.SelectedIndex < _items.Count))
            {
                _selectedUserControl = _items[_tabHeaderControl.SelectedIndex].Key;
                Children.Add(_selectedUserControl);
                SetSelectedUserControlGridPosition();
            }
            CheckTabCount();

            SelectionChanged?.Invoke(sender, e);
        }

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

        protected void _tabHeaderControl_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, e);
        }

        #region IViewContainer

        public double FontSize
        {
            set
            {
                _tabHeaderControl.FontSize = value;
            }
        }

        public FontFamily FontFamily
        {
            set
            {
                _tabHeaderControl.FontFamily = value;
            }
        }

        public CornerRadius TabCornerRadius
        {
            set
            {
                _tabHeaderControl.TabCornerRadius = value;
            }
        }

        public abstract Brush ButtonForeground { set; }

        public TabStyle SelectedTabStyle
        {
            set
            {
                _tabHeaderControl.SelectedTabBorderThickness = value.BorderThickness;
                _tabHeaderControl.SelectedTabBorderBrush = value.BorderBrush;
                _tabHeaderControl.SelectedTabBackground = value.Background;
                _tabHeaderControl.SelectedTabForeground = value.Foreground;
                _gap.Background = value.Background;
            }
        }

        public TabStyle UnselectedTabStyle
        {
            set
            {
                _tabHeaderControl.UnselectedTabBorderThickness = value.BorderThickness;
                _tabHeaderControl.UnselectedTabBorderBrush = value.BorderBrush;
                _tabHeaderControl.UnselectedTabBackground = value.Background;
                _tabHeaderControl.UnselectedTabForeground = value.Foreground;
            }
        }

        public Brush ActiveScrollIndicatorBrush
        {
            set
            {
                _tabHeaderControl.ActiveArrowBrush = value;
            }
        }

        public Brush InactiveScrollIndicatorBrush
        {
            set
            {
                _tabHeaderControl.InactiveArrowBrush = value;
            }
        }

        public Style TabItemStyle 
        { 
            set
            {
                _tabHeaderControl.ItemContainerStyle = value;
            }
        }

        public event EventHandler SelectionChanged;
        public event EventHandler TabClosed;
        public event EventHandler FloatTabRequest;

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

        public void AddUserControl(UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IViewModel);

            _items.Add(new System.Collections.Generic.KeyValuePair<UserControl, IViewModel>(userControl, userControl.DataContext as IViewModel));
            _tabHeaderControl.SelectedItem = _items[_items.Count - 1];
        }

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
            _tabHeaderControl.ItemsSource = _items;
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
