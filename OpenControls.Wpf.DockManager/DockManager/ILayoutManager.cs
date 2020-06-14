using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface ILayoutManager
    {
        Grid RootGrid { get; }
        void RemoveViewModel(IViewModel iViewModel);
    }
}