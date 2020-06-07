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

namespace WpfOpenControls.DockManager
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : Window
    {
        public ConfigurationView(DockManager.LayoutManager layoutManager)
        {
            InitializeComponent();

            DataContext = new ConfigurationViewModel(layoutManager);
        }

        private void _buttonApply_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);

            (DataContext as ConfigurationViewModel).Apply();
        }

        private void _buttonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);

            (DataContext as ConfigurationViewModel).UpdateView();
        }

        private void _buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);
            (DataContext as ConfigurationViewModel).LoadSelectedTheme();
        }

        private void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);
            LayoutManager layoutManager = (DataContext as ConfigurationViewModel).LayoutManager;

            LayoutManagerStyle layoutManagerStyle = new LayoutManagerStyle();
            layoutManagerStyle.ToolPaneGroupStyle = layoutManager.ToolPaneGroupStyle;
            layoutManagerStyle.DocumentPaneGroupStyle = layoutManager.DocumentPaneGroupStyle;

            string text = Newtonsoft.Json.JsonConvert.SerializeObject(layoutManagerStyle, Newtonsoft.Json.Formatting.Indented);

            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog == null)
            {
                return;
            }

            dialog.Filter = "Theme Files (*.thm)|*.thm";
            dialog.CheckFileExists = false;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            try
            {
                System.IO.File.WriteAllText(dialog.FileName, text);
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save theme: " + exception.Message);
            }
        }

        private void _buttonNew_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);
            ConfigurationViewModel configurationViewModel = DataContext as ConfigurationViewModel;

            NewThemeView newThemeView = new NewThemeView();
            newThemeView.Owner = this;
            newThemeView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            NewThemeViewModel newThemeViewModel = new NewThemeViewModel();
            newThemeView.DataContext = newThemeViewModel;
            if (newThemeView.ShowDialog() == true)
            {
                configurationViewModel.SaveTheme(newThemeViewModel.Text);
            }
        }

        private void _buttonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
