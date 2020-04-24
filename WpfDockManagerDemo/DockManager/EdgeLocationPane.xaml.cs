using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for EdgeLocationPane.xaml
    /// </summary>
    public partial class EdgeLocationPane : Window
    {
        public EdgeLocationPane()
        {
            InitializeComponent();
        }

        public WindowLocation TrySelectIndicator(Point cursorPositionOnScreen)
        {
            if (_buttonTopEdge.InputHitTest(_buttonTopEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.TopEdge;
            }

            if (_buttonLeftEdge.InputHitTest(_buttonLeftEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.LeftEdge;
            }

            if (_buttonRightEdge.InputHitTest(_buttonRightEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.RightEdge;
            }

            if (_buttonBottomEdge.InputHitTest(_buttonBottomEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.BottomEdge;
            }

            return WindowLocation.None;
        }
    }
}
