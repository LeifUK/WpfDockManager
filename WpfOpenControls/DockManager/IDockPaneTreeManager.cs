using System.Windows.Controls;
using System.Windows;

namespace WpfOpenControls.DockManager
{
    internal interface IDockPaneTreeManager
    {
        DockPane ExtractDockPane(DockPane dockPane, out FrameworkElement frameworkElement);

        bool UngroupDockPane(DockPane dockPane, int index, double paneWidth);

        void Float(DockPane dockPane, FloatEventArgs e, bool selectedTabOnly);

        SelectablePane FindSelectablePane(Grid grid, Point pointOnScreen);

        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);
    }
}
