using System.Windows.Controls;

namespace ExampleDockManagerViews.View
{
    /// <summary>
    /// Interaction logic for DemoOneView.xaml
    /// </summary>
    public partial class DocumentTwoView : UserControl
    {
        public DocumentTwoView()
        {
            InitializeComponent();
        }

        public WpfOpenControls.DockManager.IViewModel IViewModel { get; set; }
    }
}
