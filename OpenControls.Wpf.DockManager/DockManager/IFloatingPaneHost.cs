using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal interface IFloatingPaneHost
    {
        Grid RootGrid { get; }
        void RemoveViewModel(IViewModel iViewModel);
        SelectablePane FindSelectablePane(Grid grid, Point pointOnScreen);
        void Unfloat(FloatingPane floatingPane, SelectablePane selectedPane, WindowLocation windowLocation);
    }
}