using System.Windows;
using System.Windows.Controls;

namespace WpfOpenControls.DockManager
{
    internal interface IDockPaneTree
    {
        void FrameworkElementRemoved(FrameworkElement frameworkElement);
        Grid RootPane { get; set; }
        UIElementCollection Children { get; }
    }
}
