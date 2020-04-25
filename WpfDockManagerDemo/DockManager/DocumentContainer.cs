using System;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IDocumentContainer
    {
        // Warning warning => pointless? 
        public DocumentContainer()
        {
            _documentTabControl = new DocumentTabControl();
            _documentTabControl.SelectionChanged += TabControl_SelectionChanged;
            _documentTabControl.TabClosed += TabControl_TabClosed;
            Children.Add(_documentTabControl);
            Grid.SetRow(_documentTabControl, 0);
            Grid.SetColumn(_documentTabControl, 0);
        }

        private DocumentTabControl _documentTabControl;

        #region IDocumentContainer

        public event EventHandler SelectionChanged;

        public string Title
        {
            get
            {
                if (_documentTabControl.SelectedItem == null)
                {
                    return null;
                }

                IDocument iDocument = _documentTabControl.SelectedItem.DataContext as IDocument;
                if (iDocument == null)
                {
                    return null;
                }

                return iDocument.Title;
            }
        }

        public void AddUserControl(UserControl userControl)
        {
            _documentTabControl.AddUserControl(userControl);
        }

        private void TabControl_TabClosed(object sender, EventArgs e)
        {
            // WArning warning
        }

        private void TabControl_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public UserControl ExtractUserControl(int index)
        {
            if (index >= _documentTabControl.Count)
            {
                index = _documentTabControl.Count - 1;
            }
            if (index < 0)
            {
                index = 0;
            }

            return _documentTabControl.RemoveAt(index);
        }

        public int GetUserControlCount()
        {
            return _documentTabControl.Count;
        }

        public int GetCurrentTabIndex()
        {
            return _documentTabControl.SelectedIndex;
        }

        public UserControl GetUserControl(int index)
        {
            if (index >= _documentTabControl.Count)
            {
                return null;
            }

            return _documentTabControl.GetAt(index);
        }

        #endregion IDocumentContainer
    }
}
