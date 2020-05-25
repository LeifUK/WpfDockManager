using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal class ToolPaneGroup : DockPane
    {
        public ToolPaneGroup() : base(new ToolContainer())
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Left, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinitions.Add(columnDefinition);
            
            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Right, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[0].Height = new GridLength(Border.BorderThickness.Top, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[3].Height = new GridLength(Border.BorderThickness.Bottom, GridUnitType.Pixel);

            HeaderBorder = new Border();
            HeaderBorder.VerticalAlignment = VerticalAlignment.Stretch;
            HeaderBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            HeaderBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            HeaderBackground = System.Windows.Media.Brushes.MidnightBlue;
            HeaderBorder.BorderThickness = new Thickness(0);
            Grid.SetRow(HeaderBorder, 1);
            Grid.SetColumn(HeaderBorder, 1);
            Grid.SetColumnSpan(HeaderBorder, ColumnDefinitions.Count);
            Children.Add(HeaderBorder);

            _titleLabel = new Label();
            _titleLabel.FontSize = 12;
            _titleLabel.Padding = new Thickness(4, 0, 0, 0);
            _titleLabel.VerticalAlignment = VerticalAlignment.Center;
            _titleLabel.Background = System.Windows.Media.Brushes.Transparent;
            _titleLabel.Foreground = System.Windows.Media.Brushes.White;
            Grid.SetRow(_titleLabel, 1);
            Grid.SetColumn(_titleLabel, 1);
            Children.Add(_titleLabel);

            System.Windows.ResourceDictionary res = WpfOpenControls.Controls.Utilities.GetResourceDictionary();

            _commandsButton = new Button();
            _commandsButton.VerticalAlignment = VerticalAlignment.Center;
            _commandsButton.Style = res["StyleViewListButton"] as Style;
            _commandsButton.Click += delegate { DisplayGeneralMenu(); };
            Grid.SetRow(_commandsButton, 1);
            Grid.SetColumn(_commandsButton, 4);
            Children.Add(_commandsButton);

            _pinButton = new Button();
            _pinButton.VerticalAlignment = VerticalAlignment.Center;
            _pinButton.LayoutTransform = new System.Windows.Media.RotateTransform();
            _pinButton.Style = res["StylePinButton"] as Style;
            _pinButton.Click += PinButton_Click;
            Grid.SetRow(_pinButton, 1);
            Grid.SetColumn(_pinButton, 6);
            Children.Add(_pinButton);

            _closeButton = new Button();
            _closeButton.VerticalAlignment = VerticalAlignment.Center;
            _closeButton.Style = res["StyleCloseButton"] as Style;
            Grid.SetRow(_closeButton, 1);
            Grid.SetColumn(_closeButton, 8);
            Panel.SetZIndex(_closeButton, 99);
            Children.Add(_closeButton);
            _closeButton.Click += delegate { FireCloseRequest(); };

            IViewContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IViewContainer as System.Windows.UIElement, 2);
            Grid.SetColumn(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumnSpan(IViewContainer as System.Windows.UIElement, ColumnDefinitions.Count - 2);
        }

        public Style CloseButtonStyle
        {
            set
            {
                if (value != null)
                {
                    _closeButton.Style = value;
                }
            }
        }

        public Style PinButtonStyle
        {
            set
            {
                if (value != null)
                {
                    _pinButton.Style = value;
                }
            }
        }

        public Style CommandsButtonStyle
        {
            set
            {
                if (value != null)
                {
                    _commandsButton.Style = value;
                }
            }
        }

        public Border HeaderBorder;

        protected Brush _headerBackground;
        public Brush HeaderBackground
        {
            set
            {
                _headerBackground = value;
                if (HeaderBorder != null)
                {
                    HeaderBorder.Background = value;
                }
            }
        }

        private bool _isHighlighted;
        public override bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                _isHighlighted = value;
                if (HeaderBorder != null)
                {
                    HeaderBorder.Background = IsHighlighted ? HighlightBrush : _headerBackground;
                }
            }
        }

        public Brush ButtonForeground
        {
            set
            {
                _pinButton.Foreground = value;
                _closeButton.Foreground = value;
                _commandsButton.Foreground = value;
            }
        }

        public double FontSize
        {
            set
            {
                if (_titleLabel != null)
                {
                    _titleLabel.FontSize = value;
                }
            }
        }

        public FontFamily FontFamily
        {
            set
            {
                if (_titleLabel != null)
                {
                    _titleLabel.FontFamily = value;
                }
            }
        }

        public void ShowAsUnPinned()
        {
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).Angle = 90.0;
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).CenterX = 0.5;
            (_pinButton.LayoutTransform as System.Windows.Media.RotateTransform).CenterY = 0.5;
        }

        public event EventHandler UnPinClick;

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            UnPinClick?.Invoke(this, null);
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            _titleLabel.Content = IViewContainer.Title;
        }

        protected Label _titleLabel;

        public string Title { get { return IViewContainer.Title; } }

        private Button _pinButton;
        private Button _closeButton;
        private Button _commandsButton;
        private Point _mouseDownPosition;

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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (System.Windows.Input.Mouse.Captured == this)
            {
                Point mousePosition = e.GetPosition(this);
                double xdiff = mousePosition.X - _mouseDownPosition.X;
                double ydiff = mousePosition.Y - _mouseDownPosition.Y;
                if ((xdiff * xdiff + ydiff * ydiff) > 200)
                {

                    FireFloat(true);
                    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
                }
            }
        }
    }
}
