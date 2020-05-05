using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentPanel : SelectablePane
    {
        public DocumentPanel()
        {
        }

        public override bool IsHighlighted 
        { 
            get ; 
            set ; 
        }

        private bool ContainsDocuments(Grid grid)
        {
            if (grid == null)
            {
                return false;
            }

            foreach (var child in grid.Children)
            {
                if (child is DockPane)
                {
                    return true;
                }
                if (child is Grid)
                {
                    if (ContainsDocuments(child as Grid))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsDocuments()
        {
            return ContainsDocuments(this);
        }
    }
}
