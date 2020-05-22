using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal interface IFloatingPane
    {
        IViewContainer IViewContainer { get; }
        void Close();
        double FontSize { set; }
        FontFamily FontFamily { set; }
        Brush Background { set; }
        Brush TitleBarBackground { set; }
    }
}
