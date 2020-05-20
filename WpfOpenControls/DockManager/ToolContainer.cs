using System.Windows;
using System.Windows.Controls;

namespace WpfOpenControls.DockManager
{
    internal class ToolContainer : ViewContainer
    {
        public ToolContainer()
        {
            rowDefinition_UserControl = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) };
            rowDefinition_Gap = new RowDefinition() { Height = new System.Windows.GridLength(2, System.Windows.GridUnitType.Pixel) };
            rowDefinition_TabHeader = new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) };
            rowDefinition_Spacer = new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) };
            RowDefinitions.Add(rowDefinition_UserControl);
            RowDefinitions.Add(rowDefinition_Gap);
            RowDefinitions.Add(rowDefinition_TabHeader);
            RowDefinitions.Add(rowDefinition_Spacer);

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            CreateTabControl(2, 0);
            Grid.SetZIndex(_tabHeaderControl, 1);
            
            _gap = new Border();
            Children.Add(_gap);
            Grid.SetRow(_gap, 1);
            Grid.SetColumn(_gap, 0);
            Grid.SetColumnSpan(_gap, 4);

            _border = new Border();
            Children.Add(_border);
            Grid.SetRow(_border, 2);
            Grid.SetRowSpan(_border, 2);
            Grid.SetColumn(_border, 0);
            Grid.SetColumnSpan(_border, 4);
            Grid.SetZIndex(_border, -1);
            _border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            _border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            _border.Background = System.Windows.Media.Brushes.Transparent;

            _button = new Button();
            _button.VerticalAlignment = VerticalAlignment.Center;
            Children.Add(_button);
            Grid.SetRow(_button, 1);
            Grid.SetColumn(_button, 2);
            _button.Click += delegate { Helpers.DisplayItemsMenu(_items, _tabHeaderControl, _selectedUserControl); };
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)Application.LoadComponent(new System.Uri("/WpfOpenControls;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _button.Style = WpfOpenControls.Controls.Utilities.GetResourceDictionary()["StyleViewListButton"] as Style;
        }

        private RowDefinition rowDefinition_UserControl;
        private RowDefinition rowDefinition_Gap;
        private RowDefinition rowDefinition_TabHeader;
        private RowDefinition rowDefinition_Spacer;

        private Button _button;
        private Border _border;

        protected override void SetSelectedUserControlGridPosition()
        {
            Grid.SetRow(_selectedUserControl, 0);
            Grid.SetColumn(_selectedUserControl, 0);
            Grid.SetColumnSpan(_selectedUserControl, 99);
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
    }
}
