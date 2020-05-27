using System;
using System.ComponentModel;
using System.Linq;
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

        public bool IsToolVisible(Type type)
        {
            return (Tools.Where(n => n.GetType() == type).Count() > 0);
        }

        public void ShowTool(bool show, Type type)
        {
            bool isVisible = IsToolVisible(type);
            if (isVisible == show)
            {
                return;
            }

            if (show == false)
            {
                var enumerator = Tools.Where(n => n.GetType() == type);
                Tools.Remove(enumerator.First());
            }
            else
            {
                IViewModel iViewModel = (IViewModel)Activator.CreateInstance(type);
                System.Diagnostics.Trace.Assert(iViewModel != null);
                Tools.Add(iViewModel);
            }
        }

        public bool ToolOneVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolOneViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolOneViewModel));
                NotifyPropertyChanged("ToolOneVisible");
            }
        }

        public bool ToolTwoVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolTwoViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolTwoViewModel));
                NotifyPropertyChanged("ToolTwoVisible");
            }
        }

        public bool ToolThreeVisible
        {
            get
            {
                return (Tools.Where(n => n.GetType() == typeof(ExampleDockManagerViews.ViewModel.ToolThreeViewModel)).Count() > 0);
            }
            set
            {
                ShowTool(value, typeof(ExampleDockManagerViews.ViewModel.ToolThreeViewModel));
                NotifyPropertyChanged("ToolThreeVisible");
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
