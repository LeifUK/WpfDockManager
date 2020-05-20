using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    abstract class SelectablePane : Grid
    {
        public SelectablePane()
        {
            Tag = System.Guid.NewGuid();
        }

        abstract public bool IsHighlighted { get; set; }

        public Brush HighlightBrush { get; set; }
    }
}
