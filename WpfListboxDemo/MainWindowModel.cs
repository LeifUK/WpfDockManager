using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

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

            SelectedTabBackground = Brushes.Khaki;
            UnselectedTabBackground = Brushes.Navy;
            FontSize = 12;
            FontFamily = "Arial";
            ShowTabBorder = true;
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

        private Brush _selectedTabBackground;
        public Brush SelectedTabBackground
        {
            get
            {
                return _selectedTabBackground;
            }
            set
            {
                _selectedTabBackground = value;
                NotifyPropertyChanged("SelectedTabBackground");
            }
        }

        private Brush _unselectedTabBackground;
        public Brush UnselectedTabBackground
        {
            get
            {
                return _unselectedTabBackground;
            }
            set
            {
                _unselectedTabBackground = value;
                NotifyPropertyChanged("UnselectedTabBackground");
            }
        }

        private string _selectedHeader;
        public string SelectedHeader
        {
            get
            {
                return _selectedHeader;
            }
            set
            {
                _selectedHeader = value;
                NotifyPropertyChanged("SelectedHeader");
            }
        }

        private string _fontFamily;
        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                _fontFamily = value;
                NotifyPropertyChanged("FontFamily");
            }
        }

        private double _fontSize;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                NotifyPropertyChanged("FontSize");
            }
        }

        private bool _showTabBorder;
        public bool ShowTabBorder
        {
            get
            {
                return _showTabBorder;
            }
            set
            {
                _showTabBorder = value;
                SelectedTabBorderThickness_Top = value ? "1,1,1,0" : "0";
                SelectedTabBorderThickness_Bottom = SelectedTabBorderThickness_Top;
                NotifyPropertyChanged("ShowTabBorder");
            }
        }

        private string _selectedTabBorderThickness_Top;
        public string SelectedTabBorderThickness_Top
        {
            get
            {
                return _selectedTabBorderThickness_Top;
            }
            set
            {
                _selectedTabBorderThickness_Top = value;
                NotifyPropertyChanged("SelectedTabBorderThickness_Top");
            }
        }

        private string _selectedTabBorderThickness_Bottom;
        public string SelectedTabBorderThickness_Bottom
        {
            get
            {
                return _selectedTabBorderThickness_Bottom;
            }
            set
            {
                _selectedTabBorderThickness_Bottom = value;
                NotifyPropertyChanged("SelectedTabBorderThickness_Bottom");
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
