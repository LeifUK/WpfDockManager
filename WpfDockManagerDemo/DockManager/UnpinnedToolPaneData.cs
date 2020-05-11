using System.Windows;

namespace WpfDockManagerDemo.DockManager
{
    /*
     * Stores information for a ToolPane that has been unpinned
     */
    internal class UnpinnedToolPaneData
    {
        public ToolListControl ToolListControl { get; set; }
        public ToolPane ToolPane { get; set; }
        public FrameworkElement Sibling { get; set; }

        // These define the original location relative to the Sibling pane
        public bool IsHorizontal { get; set; }
        public bool IsFirst { get; set; }
    }
}
