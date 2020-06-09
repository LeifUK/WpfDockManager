namespace OpenControls.Wpf.DockManager
{
    internal interface IUnpinnedToolParent
    {
        void ViewModelRemoved(IViewModel iViewModel);
        Controls.IToolListBox GetToolListBox(WindowLocation windowLocation);
    }
}
