using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IDocumentContainer
    {
        // Warning warning => pointless? 
        public DocumentContainer()
        {
            _tabControlBase = new TabControlBase(1, 2);
            _tabControlBase.SelectionChanged += TabControl_SelectionChanged;
            Children.Add(_tabControlBase);

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            Grid.SetRow(_tabControlBase, 1);
            Grid.SetRowSpan(_tabControlBase, 1);
            Grid.SetColumn(_tabControlBase, 0);
            Grid.SetColumnSpan(_tabControlBase, 1);
            // Warning warning
            _tabControlBase._tabHeaderControl.UnselectedTabBackground = System.Windows.Media.Brushes.MidnightBlue;
            _tabControlBase._tabHeaderControl.SelectedTabBackground = System.Windows.Media.Brushes.LightSalmon;

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
            _documentButton.Click += delegate { _tabControlBase.DisplayItemsMenu(); };
            _documentButton.Style = FindResource("styleHeaderMenuButton") as Style;
        }

        private TabControlBase _tabControlBase;
        private Button _documentButton;
        private Button _menuButton;

        private void DisplayGeneralMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            int i = 0;
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Float?.Invoke(this, null); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            int viewCount = _tabControlBase.Count;
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

        private void _documentTabControl_Float(object sender, EventArgs e)
        {
            Float?.Invoke(sender, e);
        }

        //private DocumentTabControl _documentTabControl;

        #region IDocumentContainer

        public event EventHandler SelectionChanged;
        public event EventHandler Float;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;

        public string Title
        {
            get
            {
                if (_tabControlBase.SelectedItem == null)
                {
                    return null;
                }

                IDocument iDocument = _tabControlBase.SelectedItem.DataContext as IDocument;
                if (iDocument == null)
                {
                    return null;
                }

                return iDocument.Title;
            }
        }

        public void AddUserControl(UserControl userControl)
        {
            _tabControlBase.AddUserControl(userControl);
        }

        private void TabControl_TabClosed(object sender, EventArgs e)
        {
            // WArning warning
        }

        private void DisplaySelectedUserControl()
        {
            Children.Add(_selectedUserControl);
            Grid.SetRow(_selectedUserControl, 2);
            Grid.SetColumn(_selectedUserControl, 0);
            Grid.SetColumnSpan(_selectedUserControl, 99);
            Grid.SetZIndex(_selectedUserControl, 2);
        }

        UserControl _selectedUserControl;
        private void TabControl_SelectionChanged(object sender, EventArgs e)
        {
            if ((_selectedUserControl != null) && (Children.Contains(_selectedUserControl)))
            {
                Children.Remove(_selectedUserControl);
            }
            _selectedUserControl = null;

            _selectedUserControl = _tabControlBase.SelectedItem;
            if (_selectedUserControl != null)
            {
                DisplaySelectedUserControl();
            }
            SelectionChanged?.Invoke(sender, e);
        }

        public UserControl ExtractUserControl(int index)
        {
            UserControl userControl = _tabControlBase.GetAt(index);
            if (Children.Contains(userControl))
            {
                Children.Remove(userControl);
            }
            return _tabControlBase.RemoveAt(index);
        }

        public int GetUserControlCount()
        {
            return _tabControlBase.Count;
        }

        public int GetCurrentTabIndex()
        {
            return _tabControlBase.SelectedIndex;
        }

        public UserControl GetUserControl(int index)
        {
            if (index >= _tabControlBase.Count)
            {
                return null;
            }

            return _tabControlBase.GetAt(index);
        }

        #endregion IDocumentContainer
    }
}
