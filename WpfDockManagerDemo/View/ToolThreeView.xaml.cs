using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class ToolThreeView : UserControl, DockManager.IView
    {
        public ToolThreeView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
