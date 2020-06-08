using System;
using System.Windows;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal class FloatingDocumentPaneGroup : FloatingPane
    {
        internal FloatingDocumentPaneGroup() : base(new DocumentContainer())
        {
            SetResourceReference(Window.FontSizeProperty, "DocumentPaneFontSize");
            SetResourceReference(Window.FontFamilyProperty, "DocumentPaneFontFamily");
            SetResourceReference(Window.BackgroundProperty, "DocumentPaneBackground");

            TitleBarBackground = FindResource("FloatingDocumentTitleBarBackground") as Brush;

            IViewContainer.SelectionChanged += IViewContainer_SelectionChanged;
            (IViewContainer as DocumentContainer).HideCommandsButton();
        }

        private void IViewContainer_SelectionChanged(object sender, EventArgs e)
        {
            FloatingViewModel floatingViewModel = DataContext as FloatingViewModel;
            System.Diagnostics.Trace.Assert(floatingViewModel != null);

            floatingViewModel.Title = Application.Current.MainWindow.Title + " - " + IViewContainer.URL;
        }
    }
}
