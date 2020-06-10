using System.Windows.Media;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPane
    {
        IViewContainer IViewContainer { get; }
        void Close();
    }
}
