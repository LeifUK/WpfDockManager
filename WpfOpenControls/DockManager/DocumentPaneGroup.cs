using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfOpenControls.DockManager
{
    internal class DocumentPaneGroup : DockPane
    {
        public DocumentPaneGroup() : base(new DocumentContainer())
        {
            (IViewContainer as DocumentContainer).DisplayGeneralMenu = DisplayGeneralMenu;

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

            HeaderBorder = new Border();
            HeaderBorder.VerticalAlignment = VerticalAlignment.Stretch;
            HeaderBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            HeaderBorder.BorderThickness = new Thickness(0);
            Grid.SetRow(HeaderBorder, 0);
            Grid.SetColumn(HeaderBorder, 0);
            Grid.SetColumnSpan(HeaderBorder, 5);
            Grid.SetZIndex(HeaderBorder, -1);
            Children.Add(HeaderBorder);

            IViewContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IViewContainer as System.Windows.UIElement, 0);
            Grid.SetColumn(IViewContainer as System.Windows.UIElement, 0);
            Grid.SetColumnSpan(IViewContainer as System.Windows.UIElement, ColumnDefinitions.Count);

            IsHighlighted = false;
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            // Nothing to do!
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
