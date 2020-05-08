using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DocumentTwoView : UserControl, DockManager.IView
    {
        public DocumentTwoView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
