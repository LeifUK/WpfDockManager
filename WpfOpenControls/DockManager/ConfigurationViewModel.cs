using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfOpenControls.DockManager
{
    public class ConfigurationViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ConfigurationViewModel(DockManager.LayoutManager layoutManager)
        {
            LayoutManager = layoutManager;

            string text = Newtonsoft.Json.JsonConvert.SerializeObject(LayoutManager.ToolPaneGroupStyle, Newtonsoft.Json.Formatting.Indented);
            ToolPaneGroupStyle = Newtonsoft.Json.JsonConvert.DeserializeObject< ToolPaneGroupStyle>(text);

            ToolPaneCornerRadius = LayoutManager.ToolPaneGroupStyle.CornerRadius;
            ToolPaneBorderBrush = LayoutManager.ToolPaneGroupStyle.BorderBrush;
            ToolPaneBorderThickness = LayoutManager.ToolPaneGroupStyle.BorderThickness;
            AvailableFontSizes = new System.Collections.ObjectModel.ObservableCollection<int>();
            for (int index = 4; index < 42; ++index)
            {
                AvailableFontSizes.Add(index);
            }
            ToolPaneFontSize = (int)LayoutManager.ToolPaneGroupStyle.FontSize;
            ToolPaneBackgroundBrush = LayoutManager.ToolPaneGroupStyle.Background;
            ToolPaneGapBrush = LayoutManager.ToolPaneGroupStyle.GapBrush;
            ToolPaneGapHeight = (int)LayoutManager.ToolPaneGroupStyle.GapHeight;
            ToolPaneButtonForegroundBrush = LayoutManager.ToolPaneGroupStyle.ButtonForeground;

            ToolPaneHeaderCornerRadius = LayoutManager.ToolPaneGroupStyle.HeaderStyle.CornerRadius;
            ToolPaneHeaderBorderBrush = LayoutManager.ToolPaneGroupStyle.HeaderStyle.BorderBrush;
            ToolPaneHeaderBorderThickness = LayoutManager.ToolPaneGroupStyle.HeaderStyle.BorderThickness;
            ToolPaneHeaderBackgroundBrush = LayoutManager.ToolPaneGroupStyle.HeaderStyle.Background;
            ToolPaneHeaderTitlePadding = LayoutManager.ToolPaneGroupStyle.HeaderStyle.TitlePadding;

            ToolPaneTabCornerRadius = LayoutManager.ToolPaneGroupStyle.TabCornerRadius;
            ActiveScrollIndicatorBrush = LayoutManager.ToolPaneGroupStyle.ActiveScrollIndicatorBrush;
            InactiveScrollIndicatorBrush = LayoutManager.ToolPaneGroupStyle.InactiveScrollIndicatorBrush;

            ToolPaneSelectedTabBorderBrush = LayoutManager.ToolPaneGroupStyle.SelectedTabStyle.BorderBrush;
            ToolPaneSelectedTabBorderThickness = LayoutManager.ToolPaneGroupStyle.SelectedTabStyle.BorderThickness;
            ToolPaneSelectedTabBackgroundBrush = LayoutManager.ToolPaneGroupStyle.SelectedTabStyle.Background;
            ToolPaneSelectedTabForegroundBrush = LayoutManager.ToolPaneGroupStyle.SelectedTabStyle.Foreground;
            ToolPaneSelectedTabTitlePadding = LayoutManager.ToolPaneGroupStyle.SelectedTabStyle.TitlePadding;

            ToolPaneUnselectedTabBorderBrush = LayoutManager.ToolPaneGroupStyle.UnselectedTabStyle.BorderBrush;
            ToolPaneUnselectedTabBorderThickness = LayoutManager.ToolPaneGroupStyle.UnselectedTabStyle.BorderThickness;
            ToolPaneUnselectedTabBackgroundBrush = LayoutManager.ToolPaneGroupStyle.UnselectedTabStyle.Background;
            ToolPaneUnselectedTabForegroundBrush = LayoutManager.ToolPaneGroupStyle.UnselectedTabStyle.Foreground;
            ToolPaneUnselectedTabTitlePadding = LayoutManager.ToolPaneGroupStyle.UnselectedTabStyle.TitlePadding;

            NotifyPropertyChanged(null);
        }

        public ToolPaneGroupStyle _toolPaneGroupStyle;
        public ToolPaneGroupStyle ToolPaneGroupStyle
        {
            get
            {
                return _toolPaneGroupStyle;
            }
            set
            {
                _toolPaneGroupStyle = value;
                NotifyPropertyChanged("ToolPaneGroupStyle");
            }
        }


        private bool Parse(string text, out CornerRadius cornerRadius, string fieldName)
        {
            cornerRadius = new CornerRadius();

            Match match = Regex.Match(text, @"(\d),(\d),(\d),(\d)");
            if (!match.Success)
            {
                System.Windows.Forms.MessageBox.Show("Please enter a valid " + fieldName);
                return false;
            }

            cornerRadius = new System.Windows.CornerRadius(
                System.Convert.ToDouble(match.Groups[1].Value),
                    System.Convert.ToDouble(match.Groups[2].Value),
                    System.Convert.ToDouble(match.Groups[3].Value),
                    System.Convert.ToDouble(match.Groups[4].Value)
                    );

            return true;
        }

        private bool Parse(string text, out Thickness thickness, string fieldName)
        {
            thickness = new Thickness();

            Match match = Regex.Match(text, @"(\d),(\d),(\d),(\d)");
            if (!match.Success)
            {
                System.Windows.Forms.MessageBox.Show("Please enter a valid " + fieldName);
                return false;
            }

            thickness = new System.Windows.Thickness(
                System.Convert.ToDouble(match.Groups[1].Value),
                    System.Convert.ToDouble(match.Groups[2].Value),
                    System.Convert.ToDouble(match.Groups[3].Value),
                    System.Convert.ToDouble(match.Groups[4].Value)
                    );

            return true;
        }

        public void Apply()
        {
            ToolPaneGroupStyle toolPaneGroupStyle = new ToolPaneGroupStyle();
            toolPaneGroupStyle.CornerRadius = ToolPaneCornerRadius;
            toolPaneGroupStyle.BorderBrush = ToolPaneBorderBrush;
            toolPaneGroupStyle.BorderThickness = ToolPaneBorderThickness;
            toolPaneGroupStyle.FontSize = (int)ToolPaneFontSize;
            toolPaneGroupStyle.Background = ToolPaneBackgroundBrush;
            toolPaneGroupStyle.GapBrush = ToolPaneGapBrush;
            // Warning warning
            //toolPaneGroupStyle.GapHeight = thickness;
            toolPaneGroupStyle.ButtonForeground = ToolPaneButtonForegroundBrush;


            toolPaneGroupStyle.HeaderStyle.CornerRadius = ToolPaneHeaderCornerRadius;
            toolPaneGroupStyle.HeaderStyle.BorderBrush = ToolPaneHeaderBorderBrush;
            toolPaneGroupStyle.HeaderStyle.BorderThickness = ToolPaneHeaderBorderThickness;
            toolPaneGroupStyle.HeaderStyle.Background = ToolPaneHeaderBackgroundBrush;
            toolPaneGroupStyle.HeaderStyle.TitlePadding = ToolPaneHeaderTitlePadding;


            toolPaneGroupStyle.TabCornerRadius = ToolPaneTabCornerRadius;
            toolPaneGroupStyle.ActiveScrollIndicatorBrush = ActiveScrollIndicatorBrush;
            toolPaneGroupStyle.InactiveScrollIndicatorBrush = InactiveScrollIndicatorBrush;

            toolPaneGroupStyle.SelectedTabStyle.BorderBrush = ToolPaneSelectedTabBorderBrush;
            toolPaneGroupStyle.SelectedTabStyle.BorderThickness = ToolPaneSelectedTabBorderThickness;
            toolPaneGroupStyle.SelectedTabStyle.Background = ToolPaneSelectedTabBackgroundBrush;
            toolPaneGroupStyle.SelectedTabStyle.Foreground = ToolPaneSelectedTabForegroundBrush;
            toolPaneGroupStyle.SelectedTabStyle.TitlePadding = ToolPaneSelectedTabTitlePadding;

            toolPaneGroupStyle.UnselectedTabStyle.BorderBrush = ToolPaneUnselectedTabBorderBrush;
            toolPaneGroupStyle.UnselectedTabStyle.BorderThickness = ToolPaneUnselectedTabBorderThickness;
            toolPaneGroupStyle.UnselectedTabStyle.Background = ToolPaneUnselectedTabBackgroundBrush;
            toolPaneGroupStyle.UnselectedTabStyle.Foreground = ToolPaneUnselectedTabForegroundBrush;
            toolPaneGroupStyle.UnselectedTabStyle.TitlePadding = ToolPaneUnselectedTabTitlePadding;


            LayoutManager.ToolPaneGroupStyle = toolPaneGroupStyle;
        }

        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(LayoutManager.ToolPaneGroupStyle, Newtonsoft.Json.Formatting.Indented);
        }

        public void Deserialize(string data)
        {
            LayoutManager.ToolPaneGroupStyle = Newtonsoft.Json.JsonConvert.DeserializeObject(data) as ToolPaneGroupStyle;
        }

        public readonly DockManager.LayoutManager LayoutManager;

        public void UpdateView()
        {
            NotifyPropertyChanged(null);
        }

        public CornerRadius _toolPaneCornerRadius;
        public CornerRadius ToolPaneCornerRadius
        {
            get
            {
                return _toolPaneCornerRadius;
            }
            set
            {
                _toolPaneCornerRadius = value;
                NotifyPropertyChanged("ToolPaneCornerRadius");
            }
        }

        public Brush _toolPaneBorderBrush;
        public Brush ToolPaneBorderBrush
        {
            get
            {
                return _toolPaneBorderBrush;
            }
            set
            {
                _toolPaneBorderBrush = value;
                NotifyPropertyChanged("ToolPaneBorderBrush");
            }
        }

        public Thickness _toolPaneBorderThickness;
        public Thickness ToolPaneBorderThickness
        {
            get
            {
                return _toolPaneBorderThickness;
            }
            set
            {
                _toolPaneBorderThickness = value;
                NotifyPropertyChanged("ToolPaneBorderThickness");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<int> _availableFontSizes;
        public System.Collections.ObjectModel.ObservableCollection<int> AvailableFontSizes
        {
            get
            {
                return _availableFontSizes;
            }
            set
            {
                _availableFontSizes = value;
                NotifyPropertyChanged("AvailableFontSizes");
            }
        }

        public int _toolPaneFontSize;
        public int ToolPaneFontSize
        {
            get
            {
                return _toolPaneFontSize;
            }
            set
            {
                _toolPaneFontSize = value;
                NotifyPropertyChanged("ToolPaneFontSize");
            }
        }

        public Brush _toolPaneBackgroundBrush;
        public Brush ToolPaneBackgroundBrush
        {
            get
            {
                return _toolPaneBackgroundBrush;
            }
            set
            {
                _toolPaneBackgroundBrush = value;
                NotifyPropertyChanged("ToolPaneBackgroundBrush");
            }
        }

        public Brush _toolPaneGapBrush;
        public Brush ToolPaneGapBrush
        {
            get
            {
                return _toolPaneGapBrush;
            }
            set
            {
                _toolPaneGapBrush = value;
                NotifyPropertyChanged("ToolPaneGapBrush");
            }
        }

        public int _toolPaneGapHeight;
        public int ToolPaneGapHeight
        {
            get
            {
                return _toolPaneGapHeight;
            }
            set
            {
                _toolPaneGapHeight = value;
                NotifyPropertyChanged("ToolPaneGapHeight");
            }
        }

        public Brush _toolPaneButtonForegroundBrush;
        public Brush ToolPaneButtonForegroundBrush
        {
            get
            {
                return _toolPaneButtonForegroundBrush;
            }
            set
            {
                _toolPaneButtonForegroundBrush = value;
                NotifyPropertyChanged("ToolPaneButtonForegroundBrush");
            }
        }

        public CornerRadius _toolPaneHeaderCornerRadius;
        public CornerRadius ToolPaneHeaderCornerRadius
        {
            get
            {
                return _toolPaneHeaderCornerRadius;
            }
            set
            {
                _toolPaneHeaderCornerRadius = value;
                NotifyPropertyChanged("ToolPaneHeaderCornerRadius");
            }
        }

        public Brush _toolPaneHeaderBorderBrush;
        public Brush ToolPaneHeaderBorderBrush
        {
            get
            {
                return _toolPaneHeaderBorderBrush;
            }
            set
            {
                _toolPaneHeaderBorderBrush = value;
                NotifyPropertyChanged("ToolPaneHeaderBorderBrush");
            }
        }

        public Thickness _toolPaneHeaderBorderThickness;
        public Thickness ToolPaneHeaderBorderThickness
        {
            get
            {
                return _toolPaneHeaderBorderThickness;
            }
            set
            {
                _toolPaneHeaderBorderThickness = value;
                NotifyPropertyChanged("ToolPaneHeaderBorderThickness");
            }
        }

        public Brush _toolPaneHeaderBackgroundBrush;
        public Brush ToolPaneHeaderBackgroundBrush
        {
            get
            {
                return _toolPaneHeaderBackgroundBrush;
            }
            set
            {
                _toolPaneHeaderBackgroundBrush = value;
                NotifyPropertyChanged("ToolPaneBackgroundBrush");
            }
        }

        public Thickness _toolPaneHeaderTitlePadding;
        public Thickness ToolPaneHeaderTitlePadding
        {
            get
            {
                return _toolPaneHeaderTitlePadding;
            }
            set
            {
                _toolPaneHeaderTitlePadding = value;
                NotifyPropertyChanged("ToolPaneHeaderTitlePadding");
            }
        }

        public CornerRadius _toolPaneTabCornerRadius;
        public CornerRadius ToolPaneTabCornerRadius
        {
            get
            {
                return _toolPaneTabCornerRadius;
            }
            set
            {
                _toolPaneTabCornerRadius = value;
                NotifyPropertyChanged("ToolPaneTabCornerRadius");
            }
        }

        public Brush _activeScrollIndicatorBrush;
        public Brush ActiveScrollIndicatorBrush
        {
            get
            {
                return _activeScrollIndicatorBrush;
            }
            set
            {
                _activeScrollIndicatorBrush = value;
                NotifyPropertyChanged("ActiveScrollIndicatorBrush");
            }
        }

        public Brush _inactiveScrollIndicatorBrush;
        public Brush InactiveScrollIndicatorBrush
        {
            get
            {
                return _inactiveScrollIndicatorBrush;
            }
            set
            {
                _inactiveScrollIndicatorBrush = value;
                NotifyPropertyChanged("InactiveScrollIndicatorBrush");
            }
        }

        public Brush _toolPaneSelectedTabBorderBrush;
        public Brush ToolPaneSelectedTabBorderBrush
        {
            get
            {
                return _toolPaneSelectedTabBorderBrush;
            }
            set
            {
                _toolPaneSelectedTabBorderBrush = value;
                NotifyPropertyChanged("ToolPaneSelectedTabBorderBrush");
            }
        }

        public Thickness _toolPaneSelectedTabBorderThickness;
        public Thickness ToolPaneSelectedTabBorderThickness
        {
            get
            {
                return _toolPaneSelectedTabBorderThickness;
            }
            set
            {
                _toolPaneSelectedTabBorderThickness = value;
                NotifyPropertyChanged("ToolPaneSelectedTabBorderThickness");
            }
        }

        public Brush _toolPaneSelectedTabBackgroundBrush;
        public Brush ToolPaneSelectedTabBackgroundBrush
        {
            get
            {
                return _toolPaneSelectedTabBackgroundBrush;
            }
            set
            {
                _toolPaneSelectedTabBackgroundBrush = value;
                NotifyPropertyChanged("ToolPaneSelectedTabBackgroundBrush");
            }
        }

        public Brush _toolPaneSelectedTabForegroundBrush;
        public Brush ToolPaneSelectedTabForegroundBrush
        {
            get
            {
                return _toolPaneSelectedTabForegroundBrush;
            }
            set
            {
                _toolPaneSelectedTabForegroundBrush = value;
                NotifyPropertyChanged("ToolPaneSelectedTabForegroundBrush");
            }
        }

        public Thickness _toolPaneSelectedTabTitlePadding;
        public Thickness ToolPaneSelectedTabTitlePadding
        {
            get
            {
                return _toolPaneSelectedTabTitlePadding;
            }
            set
            {
                _toolPaneSelectedTabTitlePadding = value;
                NotifyPropertyChanged("ToolPaneSelectedTabTitlePadding");
            }
        }

        public Brush _toolPaneUnselectedTabBorderBrush;
        public Brush ToolPaneUnselectedTabBorderBrush
        {
            get
            {
                return _toolPaneUnselectedTabBorderBrush;
            }
            set
            {
                _toolPaneUnselectedTabBorderBrush = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabBorderBrush");
            }
        }

        public Thickness _toolPaneUnselectedTabBorderThickness;
        public Thickness ToolPaneUnselectedTabBorderThickness
        {
            get
            {
                return _toolPaneUnselectedTabBorderThickness;
            }
            set
            {
                _toolPaneUnselectedTabBorderThickness = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabBorderThickness");
            }
        }

        public Brush _toolPaneUnselectedTabBackgroundBrush;
        public Brush ToolPaneUnselectedTabBackgroundBrush
        {
            get
            {
                return _toolPaneUnselectedTabBackgroundBrush;
            }
            set
            {
                _toolPaneUnselectedTabBackgroundBrush = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabBackgroundBrush");
            }
        }

        public Brush _toolPaneUnselectedTabForegroundBrush;
        public Brush ToolPaneUnselectedTabForegroundBrush
        {
            get
            {
                return _toolPaneUnselectedTabForegroundBrush;
            }
            set
            {
                _toolPaneUnselectedTabForegroundBrush = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabForegroundBrush");
            }
        }

        public Thickness _toolPaneUnselectedTabTitlePadding;
        public Thickness ToolPaneUnselectedTabTitlePadding
        {
            get
            {
                return _toolPaneUnselectedTabTitlePadding;
            }
            set
            {
                _toolPaneUnselectedTabTitlePadding = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabTitlePadding");
            }
        }

        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
