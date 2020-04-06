using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class TabbedPane : Grid
    {
        public TabbedPane()
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
            RowDefinitions[0].Height = new GridLength(20, GridUnitType.Pixel);
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

            /*
             * The tab control fills the entire grid space
             */
            _tabControl = new System.Windows.Controls.TabControl();
            Children.Add(_tabControl);
            SetRow(_tabControl, 0);
            SetRowSpan(_tabControl, 2);

            // Warning warning => temporary just for testing
            Button menuButton = new Button();
            menuButton.Style = FindResource("styleHeaderMenuButton") as Style;
            menuButton.Click += MenuButton_Click;
            Grid.SetRow(menuButton, 0);
            Grid.SetColumn(menuButton, 2);
            Children.Add(menuButton);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private readonly System.Windows.Controls.TabControl _tabControl;

        //public IDocument IDocument { get; private set; }
        //public UserControl View { get; set; }

        public bool AddDocument(UserControl userControl)
        {

            return true;
        }
    }
}
