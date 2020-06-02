using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DocumentThreeView : UserControl
    {
        public DocumentThreeView()
        {
            InitializeComponent();
        }

        public WpfOpenControls.DockManager.IViewModel IViewModel { get; set; }
    }
}
