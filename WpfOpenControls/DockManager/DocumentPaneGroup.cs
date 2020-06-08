using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            columnDefinition.Width = new GridLength(Border.BorderThickness.Left, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
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

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(Border.BorderThickness.Right, GridUnitType.Pixel);
            ColumnDefinitions.Add(columnDefinition);

            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[0].Height = new GridLength(Border.BorderThickness.Top, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[2].Height = new GridLength(Border.BorderThickness.Bottom, GridUnitType.Pixel);

            IViewContainer.SelectionChanged += DocumentContainer_SelectionChanged;
            Grid.SetRow(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumn(IViewContainer as System.Windows.UIElement, 1);
            Grid.SetColumnSpan(IViewContainer as System.Windows.UIElement, ColumnDefinitions.Count - 2);

            IsHighlighted = false;

            Border.SetResourceReference(Border.CornerRadiusProperty, "DocumentPaneCornerRadius");
            Border.SetResourceReference(Border.BorderBrushProperty, "DocumentPaneBorderBrush");
            Border.SetResourceReference(Border.BorderThicknessProperty, "DocumentPaneBorderThickness");

            SetResourceReference(Border.BackgroundProperty, "DocumentPaneBackground");

            // Warning warning
            //HighlightBrush.SetValue(Brush.)

        }

        private Brush _background;

        private bool _isHighlighted;
        public override bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                if (value && !IsHighlighted)
                {
                    _background = Background;
                }
                _isHighlighted = value;
                base.Background = IsHighlighted ? HighlightBrush : _background;
            }
        }

        private void DocumentContainer_SelectionChanged(object sender, EventArgs e)
        {
            // Nothing to do!
        }
    }
}
