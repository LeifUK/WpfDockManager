using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal abstract class DockPane : Grid
    {
        public DockPane()
        {
            IsDocked = true;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[0].Height = new GridLength(20, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

            Border = new Border();
            Border.VerticalAlignment = VerticalAlignment.Stretch;
            Border.HorizontalAlignment = HorizontalAlignment.Stretch;
            Border.Background = System.Windows.Media.Brushes.SteelBlue;
            Border.BorderBrush = System.Windows.Media.Brushes.Black;
            Border.BorderThickness = new Thickness(1);
            Grid.SetRow(Border, 0);
            Grid.SetColumn(Border, 0);
            Grid.SetColumnSpan(Border, 5);
            Children.Add(Border);

            _titleLabel = new Label();
            _titleLabel.FontSize = 12;
            _titleLabel.Padding = new Thickness(4, 0, 0, 0);
            _titleLabel.VerticalAlignment = VerticalAlignment.Center;
            _titleLabel.Background = System.Windows.Media.Brushes.Transparent;
            _titleLabel.Foreground = System.Windows.Media.Brushes.White;
            Grid.SetRow(_titleLabel, 0);
            Grid.SetColumn(_titleLabel, 0);
            Children.Add(_titleLabel);

            Button menuButton = new Button();
            menuButton.Style = FindResource("styleHeaderMenuButton") as Style;
            menuButton.Click += MenuButton_Click;
            Grid.SetRow(menuButton, 0);
            Grid.SetColumn(menuButton, 2);
            Children.Add(menuButton);

            Button pinButton = new Button();
            pinButton.Style = FindResource("styleHeaderPinButton") as Style;
            Grid.SetRow(pinButton, 0);
            Grid.SetColumn(pinButton, 3);
            Children.Add(pinButton);

        }
        protected abstract void MenuButton_Click(object sender, RoutedEventArgs e);

        protected Label _titleLabel;

        public event EventHandler Dock;
        public Border Border { get; private set; }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                _isHighlighted = value;
                Border.Background = IsHighlighted ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.SteelBlue;
            }
        }

        public bool IsDocked { get; set; }

        Point _mouseDownPosition;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _mouseDownPosition = e.GetPosition(this);
            base.OnMouseLeftButtonDown(e);
            System.Windows.Input.Mouse.Capture(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
        }

        protected abstract void FireFloatEvent(object sender, EventArgs e);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (System.Windows.Input.Mouse.Captured == this)
            {
                Point mousePosition = e.GetPosition(this);
                double xdiff = mousePosition.X - _mouseDownPosition.X;
                double ydiff = mousePosition.Y - _mouseDownPosition.Y;
                if ((xdiff * xdiff + ydiff * ydiff) > 100)
                {

                    FireFloatEvent(this, e);
                    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
                }
            }
        }
    }
}
