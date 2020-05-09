using System.Windows;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for SidePane.xaml
    /// </summary>
    public partial class SideToolPane : Window
    {
        public SideToolPane()
        {
            InitializeComponent();
            _toolPane.ShowAsUnPinned();
        }

        private void _buttonMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        internal WindowLocation WindowLocation
        {
            set
            {
                switch (value)
                {
                    case WindowLocation.TopSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 0, 0, 5);
                        break;
                    case WindowLocation.BottomSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 5, 0, 0);
                        break;
                    case WindowLocation.LeftSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(0, 0, 5, 0);
                        break;
                    case WindowLocation.RightSide:
                        _windowChrome.ResizeBorderThickness = new Thickness(5, 0, 0, 0);
                        break;
                    default:
                        System.Diagnostics.Trace.Assert(false, "Unexpected WindowPosition");
                        break;
                }
            }
        }

        internal ToolPane ToolPane { get { return _toolPane; } }
    }
}
