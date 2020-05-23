using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal class DocumentContainer : ViewContainer
    {
        public DocumentContainer()
        {
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(2, System.Windows.GridUnitType.Pixel) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(2, System.Windows.GridUnitType.Pixel) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            CreateTabControl(1, 0);

            _gap = new Border();
            Children.Add(_gap);
            Grid.SetRow(_gap, 2);
            Grid.SetColumn(_gap, 0);
            Grid.SetColumnSpan(_gap, 6);

            System.Windows.ResourceDictionary res = WpfOpenControls.Controls.Utilities.GetResourceDictionary();

            _menuButton = new Button();
            Children.Add(_menuButton);
            Grid.SetRow(_menuButton, 1);
            Grid.SetColumn(_menuButton, 2);
            _menuButton.Click += delegate { if (DisplayGeneralMenu != null) DisplayGeneralMenu(); };
            _menuButton.Style = res["StyleSettingsButton"] as Style;

            _listButton = new Button();
            Children.Add(_listButton);
            Grid.SetRow(_listButton, 1);
            Grid.SetColumn(_listButton, 4);
            _listButton.Click += delegate { Helpers.DisplayItemsMenu(_items, _tabHeaderControl, _selectedUserControl); };
            _listButton.Style = res["StyleViewListButton"] as Style;
        }

        private Button _menuButton;

        public Action DisplayGeneralMenu;

        public override Brush ButtonForeground 
        {
            set
            {
                _listButton.Foreground = value;
                _menuButton.Foreground = value;
            } 
        }

        protected override void SetSelectedUserControlGridPosition()
        {
            Grid.SetRow(_selectedUserControl, 3);
            Grid.SetColumn(_selectedUserControl, 0);
            Grid.SetColumnSpan(_selectedUserControl, 99);
            Grid.SetZIndex(_selectedUserControl, 2);
        }

        protected override void CheckTabCount()
        {
            // No need to do anything ... 
        }
    }
}
