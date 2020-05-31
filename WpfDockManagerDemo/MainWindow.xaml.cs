using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModel.MainViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _layoutManager.Initialise();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_layoutManager != null)
            {
                _layoutManager.Shutdown();
            }
        }

        private void LoadLayout(string path)
        {
            try
            {
                _layoutManager.LoadLayout(path);
                ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
                mainViewModel.LayoutLoaded = true;
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to load layout: " + exception.Message);
            }
        }

        private void LoadLayout()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog == null)
            {
                return;
            }

            dialog.Filter = "Layout Files (*.xml)|*.xml";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                LoadLayout(dialog.FileName);
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to load layout: " + exception.Message);
            }
        }

        private void SaveLayout()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog == null)
            {
                return;
            }

            //dialog.FileName = viewModel.FileName;
            dialog.Filter = "Layout Files (*.xml)|*.xml";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                _layoutManager.SaveLayout(out System.Xml.XmlDocument xmlDocument_saved, dialog.FileName);
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save layout: " + exception.Message);
            }
        }

        private void LoadDefaultLayout()
        {
            try
            {
                LoadLayout(null);
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to load layout: " + exception.Message);
            }
        }

        private void _buttonWindow_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = "Load Default Layout";
            menuItem.IsChecked = false;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { LoadDefaultLayout(); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Load Layout";
            menuItem.IsChecked = false;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { LoadLayout(); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Save Layout";
            menuItem.IsChecked = false;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { SaveLayout(); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void _buttonTools_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
            System.Diagnostics.Trace.Assert(mainViewModel != null);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = "Tool One";
            menuItem.IsChecked = mainViewModel.ToolOneVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.ToolOneVisible = !mainViewModel.ToolOneVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Two";
            menuItem.IsChecked = mainViewModel.ToolTwoVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.ToolTwoVisible = !mainViewModel.ToolTwoVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Three";
            menuItem.IsChecked = mainViewModel.ToolThreeVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.ToolThreeVisible = !mainViewModel.ToolThreeVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Four";
            menuItem.IsChecked = mainViewModel.ToolFourVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.ToolFourVisible = !mainViewModel.ToolFourVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "Tool Five";
            menuItem.IsChecked = mainViewModel.ToolFiveVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.ToolFiveVisible = !mainViewModel.ToolFiveVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void _buttonDocuments_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MainViewModel mainViewModel = DataContext as ViewModel.MainViewModel;
            System.Diagnostics.Trace.Assert(mainViewModel != null);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = null;

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentOne.URL;
            menuItem.IsChecked = mainViewModel.DocumentOneVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.DocumentOneVisible = !mainViewModel.DocumentOneVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentTwo.URL;
            menuItem.IsChecked = mainViewModel.DocumentTwoVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.DocumentTwoVisible = !mainViewModel.DocumentTwoVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentThree.URL;
            menuItem.IsChecked = mainViewModel.DocumentThreeVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.DocumentThreeVisible = !mainViewModel.DocumentThreeVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentFour.URL;
            menuItem.IsChecked = mainViewModel.DocumentFourVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.DocumentFourVisible = !mainViewModel.DocumentFourVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = mainViewModel.DocumentFive.URL;
            menuItem.IsChecked = mainViewModel.DocumentFiveVisible;
            menuItem.Command = new WpfOpenControls.DockManager.Command(delegate { mainViewModel.DocumentFiveVisible = !mainViewModel.DocumentFiveVisible; }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }
    }
}
