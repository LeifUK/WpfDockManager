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
        public IViewContainer IViewContainer { get; set; }
        public int Index { get; set; }
        public string Title
        {
            get
            {
                if (IViewContainer != null)
                {
                    IViewModel iViewModel = IViewContainer.GetIViewModel(Index);
                    if (iViewModel != null)
                    {
                        return iViewModel.Title;
                    }
                }

                return "";
            }
        }
    }
}
