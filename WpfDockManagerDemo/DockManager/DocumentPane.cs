using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentPane : DockPane
    {
        public DocumentPane()
        {

            Button closeButton = new Button();
            closeButton.Style = FindResource("styleHeaderCloseButton") as Style;
            Grid.SetRow(closeButton, 0);
            Grid.SetColumn(closeButton, 4);
            Panel.SetZIndex(closeButton, 99);
            Children.Add(closeButton);
            closeButton.Click += delegate
            {
                if (IDocument != null)
                {
                    IDocument.Closing();
                }
                Close?.Invoke(this, null);
            };
        }

        public event EventHandler Float;
        protected override void FireFloatEvent(object sender, EventArgs e)
        {
            Float?.Invoke(sender, e);
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

        protected override void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Float?.Invoke(this, null); }, delegate { return IsDocked; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        public event EventHandler Close;

        public IDocument IDocument { get; private set; }
        public UserControl View { get; set; }

        public void AddUserControl(UserControl userControl)
        {
            if (userControl == null)
            {
                throw new Exception("DocumentPane.AddUserControl(): User Control is null");
            }
            View = userControl;

            IDocument iDocument = userControl.DataContext as IDocument;
            if (iDocument == null)
            {
                throw new Exception("DocumentPane.AddUserControl(): User Control is not a document");
            }
            IDocument = iDocument;

            _titleLabel.Content = IDocument.Title;

            Children.Add(userControl);
            Grid.SetRow(userControl, 1);
            Grid.SetColumn(userControl, 0);
            Grid.SetColumnSpan(userControl, ColumnDefinitions.Count);
        }

        public UserControl ExtractUserControl()
        {
            if (View == null)
            {
                throw new Exception("DocumentPane.ExtractDocument(): View null");
            }

            Children.Remove(View);
            UserControl userControl = View;
            View = null;
            IDocument = null;
            return userControl;
        }
    }
}
