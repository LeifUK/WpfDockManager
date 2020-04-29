using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfListboxDemo
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public MainWindowModel()
        {
            _listBoxItems = new System.Collections.ObjectModel.ObservableCollection<TabHeaderItem>();
            _listBoxItems.Add(new TabHeaderItem() { Label = "Short item", ID = 0 });
            _listBoxItems.Add(new TabHeaderItem() { Label = "A longer item", ID = 1 });
            _listBoxItems.Add(new TabHeaderItem() { Label = "A quite long item", ID = 2 });
            _listBoxItems.Add(new TabHeaderItem() { Label = "A really quite long Item", ID = 3 });

            SelectedTabBackground = Brushes.Khaki;
            UnselectedTabBackground = Brushes.Navy;
            FontSize = 12;
            FontFamily = "Arial";
            ShowTabBorder = true;
        }

        private System.Collections.ObjectModel.ObservableCollection<TabHeaderItem> _listBoxItems;
        public System.Collections.ObjectModel.ObservableCollection<TabHeaderItem> ListBoxItems
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

        private TabHeaderItem _selectedHeader;
        public TabHeaderItem SelectedHeader
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
                SelectedTabBorderThickness_Bottom = value ? "1,0,1,1" : "0";
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
