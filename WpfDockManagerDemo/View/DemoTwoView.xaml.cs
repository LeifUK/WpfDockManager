using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoTwoView.xaml
    /// </summary>
    public partial class DemoTwoView : UserControl, DockManager.IView
    {
        public DemoTwoView()
        {
            InitializeComponent();
        }

        public DockManager.IDocument IDocument { get; set; }
    }
}
