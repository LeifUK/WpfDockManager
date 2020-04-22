using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DemoOneView : UserControl, DockManager.IView
    {
        public DemoOneView()
        {
            InitializeComponent();
        }

        public DockManager.IDocument IDocument { get; set; }
    }
}
