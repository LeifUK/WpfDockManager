using System.ComponentModel;
using WpfOpenControls.DockManager;

namespace WpfDockManagerDemo_2.ViewModel
{
    class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Tools = new System.Collections.ObjectModel.ObservableCollection<IViewModel>();
            Tools.Add(new ExampleDockManagerViews.ViewModel.ToolOneViewModel());
            Tools.Add(new ExampleDockManagerViews.ViewModel.ToolTwoViewModel());
            Tools.Add(new ExampleDockManagerViews.ViewModel.ToolThreeViewModel());
            Tools.Add(new ExampleDockManagerViews.ViewModel.ToolFourViewModel());
            Tools.Add(new ExampleDockManagerViews.ViewModel.ToolFiveViewModel());

            Documents = new System.Collections.ObjectModel.ObservableCollection<IViewModel>();
            Documents.Add(new ExampleDockManagerViews.ViewModel.DocumentOneViewModel() { URL = "C:\\File-C1" });
            Documents.Add(new ExampleDockManagerViews.ViewModel.DocumentOneViewModel() { URL = "C:\\File-C2" });
            Documents.Add(new ExampleDockManagerViews.ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D1" });
            Documents.Add(new ExampleDockManagerViews.ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D2" });
            Documents.Add(new ExampleDockManagerViews.ViewModel.DocumentTwoViewModel() { URL = "D:\\File-D3" });
        }

        private System.Collections.ObjectModel.ObservableCollection<IViewModel> _documents;
        public System.Collections.ObjectModel.ObservableCollection<IViewModel> Documents
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

        private System.Collections.ObjectModel.ObservableCollection<IViewModel> _tools;
        public System.Collections.ObjectModel.ObservableCollection<IViewModel> Tools
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
