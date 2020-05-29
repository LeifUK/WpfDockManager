namespace WpfOpenControls.DockManager
{
    public interface IViewModel
    {
        // A user friendly title
        string Title { get; }

        /*
         * Not used by tools.
         * Uniquely identifies a document instance.
         * For example a file path for a text document. 
         */
        string URL { get; }

        bool CanClose { get; }
        bool CanFloat { get; }

        /*
         * Return true if there are edits that need to be saved
         * Not used by Tool view model
         */
        bool HasChanged { get; }
        void Save();
        void Close();
    }
}
