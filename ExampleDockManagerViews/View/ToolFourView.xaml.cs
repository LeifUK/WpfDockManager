using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class ToolFourView : UserControl
    {
        public ToolFourView()
        {
            InitializeComponent();
        }

        public WpfOpenControls.DockManager.IViewModel IViewModel { get; set; }
    }
}
