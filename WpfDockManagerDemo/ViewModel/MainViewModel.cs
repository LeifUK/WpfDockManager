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
            Documents = new System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument>();
            Documents.Add(new ViewModel.DemoOneViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoTwoViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoOneViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoTwoViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoOneViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoTwoViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoOneViewModel() { ID = id++ });
            Documents.Add(new ViewModel.DemoTwoViewModel() { ID = id++ });
        }

        private System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> _documens;
        public System.Collections.ObjectModel.ObservableCollection<DockManager.IDocument> Documents
        {
            get
            {
                return _documens;
            }
            set
            {
                if (value != Documents)
                {
                    _documens = value;
                    NotifyPropertyChanged("Documents");
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
