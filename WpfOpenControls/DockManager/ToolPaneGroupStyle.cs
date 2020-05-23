using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    public class ToolPaneGroupStyle
    {
        public ToolPaneGroupStyle()
        {
            BorderThickness = new Thickness(0);
            BorderBrush = Brushes.Black;
            CornerRadius = new CornerRadius(0);
            FontSize = 12;
            FontFamily = new FontFamily("Arial");
            Background = Brushes.LightSteelBlue;
            ButtonForeground = Brushes.White;
            HeaderStyle = new ToolHeaderStyle()
            {
                Background = Brushes.SteelBlue,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0),
                CornerRadius = new CornerRadius(0)
            };
            ActiveScrollIndicatorBrush = Brushes.White;
            InactiveScrollIndicatorBrush = Brushes.Transparent;
            TabCornerRadius = new CornerRadius(0);
            SelectedTabStyle = new TabStyle()
            {
                Foreground = Brushes.Black,
                Background = Brushes.AliceBlue,
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Gray
            };
            UnselectedTabStyle = new TabStyle()
            {
                Foreground = Brushes.White,
                Background = Brushes.Navy,
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Gray
            };
        }

        public CornerRadius CornerRadius { get; set; }
        public Brush BorderBrush { get; set; }
        public Thickness BorderThickness { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public Brush Background { get; set; }
        public Brush ButtonForeground { get; set; }
        public ToolHeaderStyle HeaderStyle { get; set; }
        public Brush ActiveScrollIndicatorBrush { get; set; }
        public Brush InactiveScrollIndicatorBrush { get; set; }
        public CornerRadius TabCornerRadius { get; set; }
        public TabStyle SelectedTabStyle { get; set; }
        public TabStyle UnselectedTabStyle { get; set; }

        public Style CommandsButtonStyle { get; set; }
        public Style PinButtonStyle { get; set; }
        public Style CloseButtonStyle { get; set; }
        public Style CloseTabButtonStyle { get; set; }
        public Style ToolListButtonStyle { get; set; }
    }
}
