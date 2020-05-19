using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfOpenControls.DockManager
{
    internal class ToolTabControlFactory : ITabControlFactory
    {
        public ITabControl GetTabControl()
        {
            return new ToolTabControl();
        }
    }
}
