using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfOpenControls.DockManager
{
    internal interface IDockPaneTreeManager
    {
        DockPane ExtractDockPane(DockPane dockPane, out FrameworkElement frameworkElement);

        bool UngroupDockPane(DockPane dockPane, int index, double paneWidth);

        void Float(DockPane dockPane, bool drag, bool selectedTabOnly);

        SelectablePane FindSelectablePane(Grid grid, Point pointOnScreen);

        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);

        SelectablePane FindDocumentPanel(Grid grid);

        void PinToolPane(UnpinnedToolData unpinnedToolData);

        void UnpinToolPane(ToolPaneGroup toolPaneGroup, out bool isHorizontal, out int row, out int column, out Guid siblingGuid);
     }
}
