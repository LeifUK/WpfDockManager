using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class DemoFourView : UserControl, DockManager.IView
    {
        public DemoFourView()
        {
            InitializeComponent();
        }

        public DockManager.IDocument IDocument { get; set; }
    }
}
