using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfOpenControls.DockManager
{
    public interface IView
    {
        DockManager.IViewModel IViewModel { get; set; }
    }
}
