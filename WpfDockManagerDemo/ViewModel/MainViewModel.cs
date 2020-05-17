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
            Tools = new System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel>();
            Tools.Add(new ViewModel.ToolOneViewModel());
            Tools.Add(new ViewModel.ToolTwoViewModel());
            Tools.Add(new ViewModel.ToolThreeViewModel());
            Tools.Add(new ViewModel.ToolFourViewModel());
            Tools.Add(new ViewModel.ToolFiveViewModel());

            Documents = new System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel>();
            Documents.Add(new ViewModel.DocumentOneViewModel() { URL = "C:\\File-C1" });
            Documents.Add(new ViewModel.DocumentOneViewModel() { URL = "C:\\File-C2" });
            Documents.Add(new ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D1" });
            Documents.Add(new ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D2" });
            Documents.Add(new ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D3" });
        }

        private System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel> _documents;
        public System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel> Documents
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

        private System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel> _tools;
        public System.Collections.ObjectModel.ObservableCollection<DockManager.IViewModel> Tools
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
