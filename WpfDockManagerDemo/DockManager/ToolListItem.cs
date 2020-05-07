using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    internal class ToolListItem
    {
        public ToolPane ToolPane { get; set; }
        public string Title
        {
            get
            {
                return ToolPane == null ? "" : ToolPane.IViewContainer.Title;
            }
        }
    }
}
