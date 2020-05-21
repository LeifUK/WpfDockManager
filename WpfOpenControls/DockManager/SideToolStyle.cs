using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    public class SideToolStyle
    {
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public Brush Foreground { get; set; }
        public Brush BarBrush { get; set; }
        public Brush MouseOverBarBrush { get; set; }
    }
}
