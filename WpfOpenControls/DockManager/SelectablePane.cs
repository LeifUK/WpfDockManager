using System.Windows.Controls;

namespace WpfOpenControls.DockManager
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
