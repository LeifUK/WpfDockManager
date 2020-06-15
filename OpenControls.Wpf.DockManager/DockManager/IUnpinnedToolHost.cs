namespace OpenControls.Wpf.DockManager
{
    internal interface IUnpinnedToolHost
    {
        void ViewModelRemoved(IViewModel iViewModel);
        Controls.IToolListBox GetToolListBox(WindowLocation windowLocation);
    }
}
