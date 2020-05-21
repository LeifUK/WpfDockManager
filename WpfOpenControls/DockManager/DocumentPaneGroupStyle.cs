using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    public class DocumentPaneGroupStyle
    {
        public CornerRadius TabCornerRadius { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        //public Brush Background { get; set; }
        public Brush ButtonForeground { get; set; }
        public Brush HeaderBackground { get; set; }
        public Brush ActiveScrollIndicatorBrush { get; set; }
        public Brush InactiveScrollIndicatorBrush { get; set; }
        public TabStyle SelectedTabStyle { get; set; }
        public TabStyle UnselectedTabStyle { get; set; }
    }
}
