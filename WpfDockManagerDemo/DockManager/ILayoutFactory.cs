using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal interface ILayoutFactory
    {
        DocumentPaneGroup CreateDocumentPaneGroup();
        ToolPaneGroup CreateToolPaneGroup();
        FloatingDocumentPaneGroup CreateFloatingDocumentPaneGroup();
        FloatingToolPaneGroup CreateFloatingToolPaneGroup();
        void CreateUnpinnedToolPaneGroup(WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst);

        void SetRootPane(Grid grid, out int row, out int column);
    }
}
