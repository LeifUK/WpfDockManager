using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WpfOpenControls.DockManager
{
    internal delegate DockPane DelegateCreateDockPane();

    internal interface IDockPaneTreeManager
    {
        void AddViews(List<UserControl> views, List<FrameworkElement> list_N, DelegateCreateDockPane createDockPane);

        DockPane ExtractDockPane(DockPane dockPane, out FrameworkElement frameworkElement);

        bool UngroupDockPane(DockPane dockPane, int index, double paneWidth);

        void Float(DockPane dockPane, bool drag, bool selectedTabOnly);

        SelectablePane FindSelectablePane(Grid grid, Point pointOnScreen);

        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);

        SelectablePane FindDocumentPanel(Grid grid);

        void PinToolPane(UnpinnedToolData unpinnedToolData);

        void UnpinToolPane(ToolPaneGroup toolPaneGroup, out UnpinnedToolData unpinnedToolData);
        
        void CreateDefaultLayout(List<UserControl> documentViews, List<UserControl> toolViews);
    }
}
