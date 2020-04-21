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
            Tools.Add(new ViewModel.DemoOneViewModel() { ID = id++ });
            Tools.Add(new ViewModel.DemoTwoViewModel() { ID = id++ });
            Tools.Add(new ViewModel.DemoThreeViewModel() { ID = id++ });
            Tools.Add(new ViewModel.DemoFourViewModel() { ID = id++ });
            Tools.Add(new ViewModel.DemoFiveViewModel() { ID = id++ });
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
