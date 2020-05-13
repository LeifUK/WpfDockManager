using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal interface ILayoutFactory
    {
        DocumentPaneGroup CreateDocumentPaneGroup();
        ToolPaneGroup CreateToolPaneGroup();
        FloatingDocumentPaneGroup CreateFloatingDocumentPaneGroup();
        FloatingToolPaneGroup CreateFloatingToolPaneGroup();

        void SetRootPane(Grid grid, out int row, out int column);
    }
}
