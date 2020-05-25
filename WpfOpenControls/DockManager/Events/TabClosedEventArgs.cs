using System.Windows.Controls;

namespace WpfOpenControls.DockManager.Events
{
    internal class TabClosedEventArgs : System.EventArgs
    {
        public UserControl UserControl { get; set; }
    }
}
