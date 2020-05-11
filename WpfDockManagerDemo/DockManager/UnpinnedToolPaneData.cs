using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDockManagerDemo.DockManager
{
    internal class UnpinnedToolPaneData
    {
        public ToolListControl ToolListControl { get; set; }
        // These are the tool list control items ... warning warning
        public System.Collections.ObjectModel.ObservableCollection<ToolListItem> ToolListItems { get; set; }
        public ToolPane ToolPane { get; set; }
        public FrameworkElement Sibling { get; set; }

        // These define the original location relative to the Sibling pane
        public bool IsHorizontal { get; set; }
        public bool IsFirst { get; set; }
    }
}
