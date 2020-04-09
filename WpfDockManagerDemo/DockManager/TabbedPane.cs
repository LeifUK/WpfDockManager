using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class TabbedPane : DockPane
    {
        public TabbedPane()
        {
            _documentContainer = new DocumentContainer();

            Children.Add(_documentContainer);
            SetRow(_documentContainer, 1);
            SetColumn(_documentContainer, 0);
            SetColumnSpan(_documentContainer, 5);

            _documentContainer.Margin = new Thickness(0);
        }

        protected DocumentContainer _documentContainer;

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
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": User Control is null");
            }
                        
            IDocument iDocument = userControl.DataContext as IDocument;
            if (iDocument == null)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": User Control is not a document");
            }

            _documentContainer.AddView(userControl);

            _titleLabel.Content = iDocument.Title;
        }
    }
}
