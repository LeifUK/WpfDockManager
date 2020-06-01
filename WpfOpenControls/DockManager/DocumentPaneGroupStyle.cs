using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    public class DocumentPaneGroupStyle
    {
        public DocumentPaneGroupStyle()
        {
            BorderThickness = new Thickness(0);
            BorderBrush = Brushes.Black;
            CornerRadius = new CornerRadius(0);
            FontSize = 12;
            FontFamily = new FontFamily("Arial");
            ButtonForeground = Brushes.White;
            Background = Brushes.SteelBlue;
            GapBrush = Brushes.AliceBlue;
            GapHeight = 2;
            ActiveScrollIndicatorBrush = Brushes.White;
            InactiveScrollIndicatorBrush = Brushes.Transparent;
            TabCornerRadius = new CornerRadius(0);
            SelectedTabStyle = new TabStyle() 
            { 
                Foreground = Brushes.Black, 
                Background = Brushes.AliceBlue, 
                BorderThickness = new Thickness(0), 
                BorderBrush = Brushes.Gray,
                TitlePadding = new Thickness(4,0,0,0)
            };
            UnselectedTabStyle = new TabStyle() 
            { 
                Foreground = Brushes.White, 
                Background = Brushes.Navy, 
                BorderThickness = new Thickness(0), 
                BorderBrush = Brushes.Gray,
                TitlePadding = new Thickness(4,0,0,0)
            };
        }

        public CornerRadius CornerRadius { get; set; }
        public Brush BorderBrush { get; set; }
        public Thickness BorderThickness { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public Brush Background { get; set; }
        public Brush GapBrush { get; set; }
        public double GapHeight { get; set; }
        public Brush ButtonForeground { get; set; }
        public Brush ActiveScrollIndicatorBrush { get; set; }
        public Brush InactiveScrollIndicatorBrush { get; set; }
        public CornerRadius TabCornerRadius { get; set; }
        public TabStyle SelectedTabStyle { get; set; }
        public TabStyle UnselectedTabStyle { get; set; }
        public Style CommandsButtonStyle { get; set; }
        public Style DocumentListButtonStyle { get; set; }
    }
}
