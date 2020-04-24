using System;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IDocumentContainer
    {
        // Warning warning => poiwntless? 
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
                IDocument iDocument = Children[0] as IDocument;
                if (iDocument != null)
                {
                    return (iDocument as IDocument).Title;
                }

                if (_documentTabControl.SelectedItem == null)
                {
                    return null;
                }

                iDocument = _documentTabControl.SelectedItem.DataContext as IDocument;
                if (iDocument == null)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": User Control is not a document");
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
