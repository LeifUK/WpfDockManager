using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class ToolOneView : UserControl, DockManager.IView
    {
        public ToolOneView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
