using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class ToolContainer : ViewContainer
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

            _items = new System.Collections.ObjectModel.ObservableCollection<System.Collections.Generic.KeyValuePair<UserControl, IViewModel>>();

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

        private Button _button;
        private Border _border;

        protected override void _tabHeaderControl_SelectionChanged(object sender, System.EventArgs e)
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

        protected override void CheckTabCount()
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

        private void TabControl_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public override event EventHandler SelectionChanged;

        public override void AddUserControl(UserControl userControl)
        {
            System.Diagnostics.Trace.Assert(userControl != null);
            System.Diagnostics.Trace.Assert(userControl.DataContext is IViewModel);

            _items.Add(new System.Collections.Generic.KeyValuePair<UserControl, IViewModel>(userControl, userControl.DataContext as IViewModel));
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
    }
}
