using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class ToolFourView : UserControl, DockManager.IView
    {
        public ToolFourView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
