using System.Windows;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for IndicatorPane.xaml
    /// </summary>
    public partial class WindowLocationPane : Window
    {
        public WindowLocationPane()
        {
            InitializeComponent();
        }

        public WindowLocation TrySelectIndicator(Point cursorPositionOnScreen)
        {
            if (_buttonTop.InputHitTest(_buttonTop.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Top;
            }

            if (_buttonTopEdge.InputHitTest(_buttonTopEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.TopEdge;
            }

            if (_buttonLeft.InputHitTest(_buttonLeft.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Left;
            }

            if (_buttonLeftEdge.InputHitTest(_buttonLeftEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.LeftEdge;
            }

            if (_buttonMiddle.InputHitTest(_buttonMiddle.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Middle;
            }

            if (_buttonRight.InputHitTest(_buttonRight.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Right;
            }

            if (_buttonRightEdge.InputHitTest(_buttonRightEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.RightEdge;
            }

            if (_buttonBottom.InputHitTest(_buttonBottom.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Bottom;
            }

            if (_buttonBottomEdge.InputHitTest(_buttonBottomEdge.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.BottomEdge;
            }

            return WindowLocation.None;
        }
    }
}
