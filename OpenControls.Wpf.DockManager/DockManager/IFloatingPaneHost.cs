using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPaneHost
    {
        Grid RootGrid { get; }
        void RemoveViewModel(IViewModel iViewModel);
    }
}