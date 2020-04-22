using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoFiveView.xaml
    /// </summary>
    public partial class DemoFiveView : UserControl, DockManager.IView
    {
        public DemoFiveView()
        {
            InitializeComponent();
        }

        public DockManager.IDocument IDocument { get; set; }
    }
}
