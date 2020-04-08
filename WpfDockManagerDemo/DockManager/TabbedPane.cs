using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class TabbedPane : DockPane
    {
        public TabbedPane()
        {
            _tabControl = new System.Windows.Controls.TabControl();
            Children.Add(_tabControl);
            SetRow(_tabControl, 1);
            SetColumn(_tabControl, 0);
            SetColumnSpan(_tabControl, 5);

            _tabControl.Margin = new Thickness(0);
            _tabControl.Padding = new Thickness(-2);
            _tabControl.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
            _tabControl.SelectionChanged += _tabControl_SelectionChanged;
        }

        private void _tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tabControl.SelectedItem == null)
            {
                _titleLabel.Content = null;
                return;
            }

            TabItem tabItem = _tabControl.SelectedItem as TabItem;
            if (tabItem == null)
            {
                _titleLabel.Content = null;
                return;
            }

            UserControl userControl = tabItem.Content as UserControl;
            if (userControl == null)
            {
                throw new Exception("TabbedPane._tabControl_SelectionChanged(): selected item is not a User Control");
            }

            IDocument iDocument = userControl.DataContext as IDocument;
            if (iDocument == null)
            {
                throw new Exception("TabbedPane._tabControl_SelectionChanged(): User Control is not a document");
            }

            _titleLabel.Content = iDocument.Title;
        }

        public event EventHandler Float;
        protected override void FireFloatEvent(object sender, EventArgs e)
        {
            Float?.Invoke(sender, e);
        }

        public event EventHandler FloatSelectedDocument;

        protected override void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Float?.Invoke(this, null); }, delegate { return IsDocked; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Float Selected Tab";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { FloatSelectedDocument?.Invoke(this, null); }, delegate { return IsDocked; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private readonly System.Windows.Controls.TabControl _tabControl;

        public void AddUserControl(UserControl userControl)
        {
            if (userControl == null)
            {
                throw new Exception("TabbedPane.AddUserControl(): User Control is null");
            }

            IDocument iDocument = userControl.DataContext as IDocument;
            if (iDocument == null)
            {
                throw new Exception("TabbedPane.AddUserControl(): User Control is not a document");
            }
            
            _titleLabel.Content = iDocument.Title;

            TabItem tabItem = new TabItem();
            tabItem.Header = iDocument.Title;
            
            tabItem.Content = userControl;
            _tabControl.Items.Add(tabItem);
            _tabControl.SelectedIndex = _tabControl.Items.Count - 1;
        }
    }
}
