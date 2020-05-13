using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    internal class FloatingDocumentPaneGroup : FloatingPane
    {
        internal FloatingDocumentPaneGroup() : base(new DocumentContainer())
        {

        }
    }
}
