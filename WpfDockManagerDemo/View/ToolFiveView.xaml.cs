using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoFiveView.xaml
    /// </summary>
    public partial class ToolFiveView : UserControl, DockManager.IView
    {
        public ToolFiveView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
