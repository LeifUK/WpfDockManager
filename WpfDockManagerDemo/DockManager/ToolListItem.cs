namespace WpfDockManagerDemo.DockManager
{
    internal class ToolListItem
    {
        public UnpinnedToolPaneData UnpinnedToolPaneData;
        public int Index { get; set; }
        public IViewContainer IViewContainer
        {
            get
            {
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData != null);
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData.ToolPane != null);
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData.ToolPane.IViewContainer != null);

                return UnpinnedToolPaneData.ToolPane.IViewContainer;
            }
        }
        public string Title
        {
            get
            {
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData != null);
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData.ToolPane != null);
                System.Diagnostics.Trace.Assert(UnpinnedToolPaneData.ToolPane.IViewContainer != null);

                IViewModel iViewModel = UnpinnedToolPaneData.ToolPane.IViewContainer.GetIViewModel(Index);
                if (iViewModel != null)
                {
                    return iViewModel.Title;
                }

                return "";
            }
        }
        public WindowLocation WindowLocation { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public bool Equals(ToolListItem other)
        {
            return
                (UnpinnedToolPaneData.ToolPane == other.UnpinnedToolPaneData.ToolPane) &&
                (Index == other.Index);
        }
    }
}
