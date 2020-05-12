using System.Windows;
using System.Collections.Generic;

namespace WpfDockManagerDemo.DockManager
{
    /*
     * Stores information for a ToolPane that has been unpinned
     */
    internal class UnpinnedToolData
    {
        public ToolPaneGroup ToolPane { get; set; }
        public FrameworkElement Sibling { get; set; }

        public List<ToolListBoxItem> Items;

        // These define the original location relative to the Sibling pane
        public bool IsHorizontal { get; set; }
        public bool IsFirst { get; set; }
    }
}
