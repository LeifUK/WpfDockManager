using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentPane : DockPane
    {
        public DocumentPane() : base(new DocumentContainer())
        {
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
            RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);

            Border = new Border();
            Border.VerticalAlignment = VerticalAlignment.Stretch;
            Border.HorizontalAlignment = HorizontalAlignment.Stretch;
            Border.Background = System.Windows.Media.Brushes.Gray;
            Border.BorderBrush = System.Windows.Media.Brushes.Black;
            Border.BorderThickness = new Thickness(1);
            Grid.SetRow(Border, 0);
            Grid.SetColumn(Border, 0);
            Grid.SetColumnSpan(Border, 5);
            Grid.SetZIndex(Border, -1);
            Children.Add(Border);

            IDocumentContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IDocumentContainer as System.Windows.UIElement, 0);
            Grid.SetColumn(IDocumentContainer as System.Windows.UIElement, 0);
            Grid.SetColumnSpan(IDocumentContainer as System.Windows.UIElement, ColumnDefinitions.Count);
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            // Warning warning
            //_titleLabel.Content = IDocumentContainer.Title;
        }

        protected Label _titleLabel;

        public Border Border { get; private set; }

        private bool _isHighlighted;
        // Warning warning => move to DockPane?
        public override bool IsHighlighted
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

        //Point _mouseDownPosition;

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    _mouseDownPosition = e.GetPosition(this);
        //    base.OnMouseLeftButtonDown(e);
        //    System.Windows.Input.Mouse.Capture(this);
        //}

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonUp(e);
        //    System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if (System.Windows.Input.Mouse.Captured == this)
        //    {
        //        Point mousePosition = e.GetPosition(this);
        //        double xdiff = mousePosition.X - _mouseDownPosition.X;
        //        double ydiff = mousePosition.Y - _mouseDownPosition.Y;
        //        if ((xdiff * xdiff + ydiff * ydiff) > 100)
        //        {

        //            FireFloat();
        //            System.Windows.Input.Mouse.Capture(this, CaptureMode.None);
        //        }
        //    }
        //}
    }
}
