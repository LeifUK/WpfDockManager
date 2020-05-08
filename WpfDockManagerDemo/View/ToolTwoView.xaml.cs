using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoTwoView.xaml
    /// </summary>
    public partial class ToolTwoView : UserControl, DockManager.IView
    {
        public ToolTwoView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
