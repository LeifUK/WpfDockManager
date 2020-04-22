using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class DemoThreeView : UserControl, DockManager.IView
    {
        public DemoThreeView()
        {
            InitializeComponent();
        }

        public DockManager.IDocument IDocument { get; set; }
    }
}
