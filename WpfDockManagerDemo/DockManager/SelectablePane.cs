using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    abstract class SelectablePane : Grid
    {
        public SelectablePane()
        {
            Tag = System.Guid.NewGuid();
        }

        abstract public bool IsHighlighted { get; set; }
    }
}
