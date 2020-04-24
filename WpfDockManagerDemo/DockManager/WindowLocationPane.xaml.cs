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

            if (_buttonLeft.InputHitTest(_buttonLeft.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Left;
            }

            if (_buttonMiddle.InputHitTest(_buttonMiddle.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Middle;
            }

            if (_buttonRight.InputHitTest(_buttonRight.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Right;
            }

            if (_buttonBottom.InputHitTest(_buttonBottom.PointFromScreen(cursorPositionOnScreen)) != null)
            {
                return WindowLocation.Bottom;
            }

            return WindowLocation.None;
        }
    }
}
