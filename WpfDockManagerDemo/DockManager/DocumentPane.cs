using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentPane : Grid
    {
        public DocumentPane()
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

            Label label = new Label();
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Background = System.Windows.Media.Brushes.Transparent;
            label.Foreground = System.Windows.Media.Brushes.White;
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);
            Children.Add(label);

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

            Button closeButton = new Button();
            closeButton.Style = FindResource("styleHeaderCloseButton") as Style;
            closeButton.Click += delegate 
            {
                if (IDocument != null)
                {
                    IDocument.Closing();
                }
                Close?.Invoke(this, null); 
            };
            Grid.SetRow(closeButton, 0);
            Grid.SetColumn(closeButton, 4);
            Panel.SetZIndex(closeButton, 99);
            Children.Add(closeButton);
        }

        /*
         * Delegate management to the parent
         */

        public Border Border { get; private set; }

        public event EventHandler Close;
        public event EventHandler Float;
        public event EventHandler Dock;

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                Border.Background = value ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.SteelBlue;
                _isHighlighted = value;
            }
        }

        public bool IsDocked { get; set; }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new Command(call => Close?.Invoke(this, null));
                }
                return _closeCommand;
            }
        }

        private ICommand _floatCommand;
        public ICommand FloatCommand
        {
            get
            {
                if (_floatCommand == null)
                {
                    _floatCommand = new Command(call => Float?.Invoke(this, null));
                }
                return _floatCommand;
            }
        }

        private ICommand _dockCommand;
        public ICommand DockCommand
        {
            get
            {
                if (_dockCommand == null)
                {
                    _dockCommand = new Command(call => Dock?.Invoke(this, null));
                }
                return _dockCommand;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Float?.Invoke(this, null); }, delegate { return IsDocked; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        public IDocument IDocument { get; private set; }
        public UserControl View { get; set; }

        public void AddDocument(UserControl userControl)
        {
            View = userControl;
            IDocument = userControl.DataContext as IDocument;

            Children.Add(userControl);
            Grid.SetRow(userControl, 1);
            Grid.SetColumn(userControl, 0);
            Grid.SetColumnSpan(userControl, ColumnDefinitions.Count);
        }

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

                    Float?.Invoke(this, e);
                    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
                }
            }
        }
    }
}
