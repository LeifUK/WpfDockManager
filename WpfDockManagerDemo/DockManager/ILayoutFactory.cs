using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal interface ILayoutFactory
    {
        DocumentPane CreateDocumentPane();
        ToolPane CreateToolPane();
        FloatingDocument CreateFloatingDocument();
        FloatingTool CreateFloatingTool();

        void SetRootPane(Grid grid, out int row, out int column);
    }
}
