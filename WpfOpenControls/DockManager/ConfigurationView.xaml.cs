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

        private System.Drawing.Color Convert(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);

            (DataContext as ConfigurationViewModel).UpdateView();
        }



        //public void Deserialize(string data)
        //{
        //    _layoutManager.ToolPaneGroupStyle = Newtonsoft.Json.JsonConvert.DeserializeObject(data) as ToolPaneGroupStyle;
        //}

        private void _buttonLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        class LayoutManagerStyle
        {
            public ToolPaneGroupStyle ToolPaneGroupStyle;
            public DocumentPaneGroupStyle DocumentPaneGroupStyle;
        }

        private void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Trace.Assert(DataContext is ConfigurationViewModel);
            LayoutManager layoutManager = (DataContext as ConfigurationViewModel).LayoutManager;

            LayoutManagerStyle layoutManagerStyle = new LayoutManagerStyle();
            layoutManagerStyle.ToolPaneGroupStyle = layoutManager.ToolPaneGroupStyle;
            layoutManagerStyle.DocumentPaneGroupStyle = layoutManager.DocumentPaneGroupStyle;

            string text = Newtonsoft.Json.JsonConvert.SerializeObject(layoutManagerStyle, Newtonsoft.Json.Formatting.Indented);

            System.IO.File.WriteAllText("c:\\Temp\\test.txt", text);
        }
    }
}
