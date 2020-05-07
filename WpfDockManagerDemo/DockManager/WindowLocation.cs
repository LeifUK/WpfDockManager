using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    [System.Flags]
    public enum WindowLocation
    {
        None = 0x000,
        Middle = 0x001,
        Right = 0x002,
        Left = 0x004,
        Top = 0x008,
        Bottom = 0x010,

        RightEdge = 0x020,
        LeftEdge = 0x040,
        TopEdge = 0x080,
        BottomEdge = 0x100,
    }
}
