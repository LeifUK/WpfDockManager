using System.Collections.Generic;
using System.Windows;

namespace WpfOpenControls.DockManager
{
    internal interface IUnpinnedToolManager
    {
        ToolPaneGroup UnpinnedToolClick(ToolListBoxItem toolListBoxItem, Controls.ToolListBox toolListBox);
        void Unpin(ToolPaneGroup toolPaneGroup);
        void ProcessMoveResize();
        void CloseUnpinnedToolPane();
        void FrameworkElementRemoved(FrameworkElement frameworkElement);
        void MakeUnpinnedToolPaneGroup(Controls.ToolListBox toolListBox, WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst);
        Dictionary<WindowLocation, List<UnpinnedToolData>> GetUnpinnedToolData();
    }
}
