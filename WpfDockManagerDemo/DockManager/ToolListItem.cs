namespace WpfDockManagerDemo.DockManager
{
    internal class ToolListItem : Controls.IUnpinnedTool
    {
        public int Index { get; set; }
        public IViewContainer IViewContainer { get; set; }
        public string Title
        {
            get
            {
                IViewModel iViewModel = IViewContainer.GetIViewModel(Index);
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
                (IViewContainer == other.IViewContainer) &&
                (Index == other.Index);
        }
    }
}
