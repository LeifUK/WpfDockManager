using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class SplitterPane : Grid
    {
        public SplitterPane(bool isHorizontal)
        {
            IsHorizontal = isHorizontal;

            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            var gridSplitter = new GridSplitter();
            gridSplitter.Background = System.Windows.Media.Brushes.Gainsboro;
            gridSplitter.BorderThickness = new Thickness(1);
            Children.Add(gridSplitter);

            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());

            if (IsHorizontal)
            {

                RowDefinition rowDefinition = new RowDefinition();
                RowDefinitions.Add(rowDefinition);
                rowDefinition.Height = new GridLength(1, GridUnitType.Auto);

                RowDefinitions.Add(new RowDefinition());

                ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);

                gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                gridSplitter.Height = 4;
                Grid.SetRow(gridSplitter, 1);
                Grid.SetColumn(gridSplitter, 0);
            }
            else
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Auto);
                ColumnDefinitions.Add(columnDefinition);

                ColumnDefinitions.Add(new ColumnDefinition());

                RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);

                gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                gridSplitter.Width = 4;
                Grid.SetRow(gridSplitter, 0);
                Grid.SetColumn(gridSplitter, 1);
            }
        }

        public readonly bool IsHorizontal;

        public void AddChild(FrameworkElement frameworkElement, bool isFirst)
        {
            Children.Add(frameworkElement);
            int row = 0;
            int column = 0;
            if (!isFirst)
            {
                if (IsHorizontal)
                {
                    row = 2;
                }
                else
                {
                    column = 2;
                }
            }
            Grid.SetRow(frameworkElement, row);
            Grid.SetColumn(frameworkElement, column);
        }

        public OldTabbedPane AddTab(DocumentPane targetPane, UserControl userControl, WindowLocation windowLocation)
        {
            OldTabbedPane tabbedPane = null;

            return tabbedPane;
        }
    }
}
