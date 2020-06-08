using System;
using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal class FloatingToolPaneGroup : FloatingPane
    {
        internal FloatingToolPaneGroup() : base(new ToolContainer())
        {
            SetResourceReference(Window.FontSizeProperty, "ToolPaneFontSize");
            SetResourceReference(Window.FontFamilyProperty, "ToolPaneFontFamily");
            SetResourceReference(Window.BackgroundProperty, "ToolPaneBackground");

            TitleBarBackground = FindResource("FloatingToolTitleBarBackground") as Brush;

            IViewContainer.SelectionChanged += IViewContainer_SelectionChanged;
        }

        private void IViewContainer_SelectionChanged(object sender, EventArgs e)
        {
            FloatingViewModel floatingViewModel = DataContext as FloatingViewModel;
            System.Diagnostics.Trace.Assert(floatingViewModel != null);

            floatingViewModel.Title = Application.Current.MainWindow.Title + " - " + IViewContainer.Title;
        }
    }
}
