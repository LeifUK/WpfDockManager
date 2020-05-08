using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DocumentThreeView : UserControl, DockManager.IView
    {
        public DocumentThreeView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
