using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.ViewModel
{
    class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MainViewModel()
        {
            long id = 0;
            Tools = new System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument>();
            Tools.Add(new ViewModel.ToolOneViewModel() { ID = id++ });
            Tools.Add(new ViewModel.ToolTwoViewModel() { ID = id++ });
            Tools.Add(new ViewModel.ToolThreeViewModel() { ID = id++ });
            Tools.Add(new ViewModel.ToolFourViewModel() { ID = id++ });
            Tools.Add(new ViewModel.ToolFiveViewModel() { ID = id++ });

            Documents = new System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument>();
            Documents.Add(new ViewModel.DocumentOneViewModel());
        }

        private System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> _documents;
        public System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> Documents
        {
            get
            {
                return _documents;
            }
            set
            {
                if (value != Documents)
                {
                    _documents = value;
                    NotifyPropertyChanged("Documents");
                }
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> _tools;
        public System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> Tools
        {
            get
            {
                return _tools;
            }
            set
            {
                if (value != Tools)
                {
                    _tools = value;
                    NotifyPropertyChanged("Tools");
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
