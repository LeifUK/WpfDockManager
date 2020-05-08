using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DocumentOneView : UserControl, DockManager.IView
    {
        public DocumentOneView()
        {
            InitializeComponent();
        }

        public DockManager.IViewModel IViewModel { get; set; }
    }
}
