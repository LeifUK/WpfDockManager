namespace WpfOpenControls.DockManager
{
    internal interface IUnpinnedToolParent
    {
        void ViewModelRemoved(IViewModel iViewModel);
        Controls.ToolListBox GetToolListBox(WindowLocation windowLocation);
    }
}
