using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    abstract class SelectablePane : Grid
    {
        abstract public bool IsHighlighted { get; set; }
    }
}
