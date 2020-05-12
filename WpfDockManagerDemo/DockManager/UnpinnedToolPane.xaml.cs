using System;
using System.Windows;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for SidePane.xaml
    /// </summary>
    public partial class UnpinnedToolPane : Window
    {
        public UnpinnedToolPane()
        {
            InitializeComponent();
            _windowChrome.GlassFrameThickness = new Thickness(1);
            _toolPane.ShowAsUnPinned();
            _toolPane.UnPinClick += _toolPane_UnPinClick;
        }

        public event EventHandler PinClick;

        private void _toolPane_UnPinClick(object sender, System.EventArgs e)
        {
            PinClick?.Invoke(this, null);
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
