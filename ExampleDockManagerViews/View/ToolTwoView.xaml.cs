using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoTwoView.xaml
    /// </summary>
    public partial class ToolTwoView : UserControl, WpfOpenControls.DockManager.IView
    {
        public ToolTwoView()
        {
            InitializeComponent();
        }

        public WpfOpenControls.DockManager.IViewModel IViewModel { get; set; }
    }
}
