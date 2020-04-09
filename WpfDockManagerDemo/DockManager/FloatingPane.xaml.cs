using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingPane : Window
    {
        public FloatingPane()
        {
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public event EventHandler Dock;
        public event EventHandler EndDrag;

        private void _buttonMenu_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            menuItem = new MenuItem();
            menuItem.Header = "Dock";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { Dock?.Invoke(this, null); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);
            
            menuItem = new MenuItem();
            menuItem.Header = "Freeze Aspect Ratio";
            menuItem.IsChecked = false;
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        System.Threading.Tasks.Task _dragTask;

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (_dragTask == null)
            {
                // Use a task to detect when the drag ends
                _dragTask = new Task(delegate 
                { 
                    while (Utilities.IsLeftMouseButtonDown())
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    EndDrag?.Invoke(this, null);
                    _dragTask = null;
                });
                _dragTask.Start();
            }
        }

        public void AddView(UserControl userControl)
        {
            DocumentContainer.AddView(userControl);
        }

        public UserControl ExtractView()
        {
            return DocumentContainer.ExtractView();
        }
    }
}

