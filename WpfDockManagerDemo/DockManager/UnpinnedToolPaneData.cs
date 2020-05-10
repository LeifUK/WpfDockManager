using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    internal class UnpinnedToolPaneData
    {
        public ToolPane ToolPane { get; set; }
        public SelectablePane Sibling { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
