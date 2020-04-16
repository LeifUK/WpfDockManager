﻿using System.Collections.Generic;
using System.ComponentModel;

namespace WpfListboxDemo
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel()
        {
            _listBoxItems = new System.Collections.ObjectModel.ObservableCollection<string>();
            _listBoxItems.Add("Short item");
            _listBoxItems.Add("A longer item");
            _listBoxItems.Add("A quite long item");
            _listBoxItems.Add("A really quite long Item");
        }

        private System.Collections.ObjectModel.ObservableCollection<string> _listBoxItems;
        public System.Collections.ObjectModel.ObservableCollection<string> ListBoxItems
        {
            get
            {
                return _listBoxItems;
            }
            set
            {
                _listBoxItems = value;
                NotifyPropertyChanged("ListBoxItems");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }
}