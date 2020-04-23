using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class ToolTabControl : TabControlBase
    {
        public ToolTabControl() : base(false)
        {
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Pixel) });

            Grid.SetRow(_tabHeaderControl, 1);
            Grid.SetColumn(_tabHeaderControl, 0);
            _tabHeaderControl.UnselectedTabBackground = System.Windows.Media.Brushes.MidnightBlue;
            _tabHeaderControl.SelectedTabBackground = System.Windows.Media.Brushes.LightSalmon;

            Border border = new Border();
            Children.Add(border);
            Grid.SetRow(border, 1);
            Grid.SetRowSpan(border, 2);
            Grid.SetColumn(border, 0);
            Grid.SetColumnSpan(border, 4);
            Grid.SetZIndex(border, -1);
            border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            border.Background = System.Windows.Media.Brushes.Gainsboro;


            _button = new Button();
            Children.Add(_button);
            Grid.SetRow(_button, 1);
            Grid.SetColumn(_button, 2);
            _button.Click += _button_Click; ;
            System.Windows.ResourceDictionary res = (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
            _button.Style = (System.Windows.Style)res["MenuButtonStyle"];
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            DisplayItemsMenu();
        }

        private Button _button;
    }
}
