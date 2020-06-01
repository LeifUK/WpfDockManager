using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    public class TabStyle
    {
        public Brush BorderBrush { get; set; }
        public Thickness BorderThickness { get; set; }
        public Brush Background { get; set; }
        public Brush Foreground { get; set; }
        public Style CloseTabButtonStyle { get; set; }
        public Thickness TitlePadding { get; set; }
    }
}
