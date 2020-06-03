using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices.WindowsRuntime;

namespace WpfOpenControls.DockManager
{
    public class ConfigurationViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ConfigurationViewModel(DockManager.LayoutManager layoutManager)
        {
            _layoutManager = layoutManager;

            ToolPaneCornerRadius = _layoutManager.ToolPaneGroupStyle.CornerRadius.ToString();
            ToolPaneBorderColour = (_layoutManager.ToolPaneGroupStyle.BorderBrush as SolidColorBrush).Color;
            ToolPaneBorderThickness = _layoutManager.ToolPaneGroupStyle.BorderThickness.ToString();
            ToolPaneBackgroundColour = (_layoutManager.ToolPaneGroupStyle.Background as SolidColorBrush).Color;
            ToolPaneGapColour = (_layoutManager.ToolPaneGroupStyle.GapBrush as SolidColorBrush).Color;
            ToolPaneGapHeight = (int)_layoutManager.ToolPaneGroupStyle.GapHeight;
            ToolPaneButtonForegroundColour = (_layoutManager.ToolPaneGroupStyle.ButtonForeground as SolidColorBrush).Color;

            ToolPaneHeaderCornerRadius = _layoutManager.ToolPaneGroupStyle.HeaderStyle.CornerRadius.ToString();
            ToolPaneHeaderBorderColour = (_layoutManager.ToolPaneGroupStyle.HeaderStyle.BorderBrush as SolidColorBrush).Color;
            ToolPaneHeaderBorderThickness = _layoutManager.ToolPaneGroupStyle.HeaderStyle.BorderThickness.ToString();
            ToolPaneHeaderBackgroundColour = (_layoutManager.ToolPaneGroupStyle.HeaderStyle.Background as SolidColorBrush).Color;
            ToolPaneHeaderTitlePadding = _layoutManager.ToolPaneGroupStyle.HeaderStyle.TitlePadding.ToString();

            ToolPaneTabCornerRadius = _layoutManager.ToolPaneGroupStyle.TabCornerRadius.ToString();
            ActiveScrollIndicatorColour = (_layoutManager.ToolPaneGroupStyle.ActiveScrollIndicatorBrush as SolidColorBrush).Color;
            InactiveScrollIndicatorColour = (_layoutManager.ToolPaneGroupStyle.InactiveScrollIndicatorBrush as SolidColorBrush).Color;

            ToolPaneSelectedTabBorderColour = (_layoutManager.ToolPaneGroupStyle.SelectedTabStyle.BorderBrush as SolidColorBrush).Color;
            ToolPaneSelectedTabBorderThickness = _layoutManager.ToolPaneGroupStyle.SelectedTabStyle.BorderThickness.ToString();
            ToolPaneSelectedTabBackgroundColour = (_layoutManager.ToolPaneGroupStyle.SelectedTabStyle.Background as SolidColorBrush).Color;
            ToolPaneSelectedTabForegroundColour = (_layoutManager.ToolPaneGroupStyle.SelectedTabStyle.Foreground as SolidColorBrush).Color;
            ToolPaneSelectedTabTitlePadding = _layoutManager.ToolPaneGroupStyle.SelectedTabStyle.TitlePadding.ToString();

            ToolPaneUnselectedTabBorderColour = (_layoutManager.ToolPaneGroupStyle.UnselectedTabStyle.BorderBrush as SolidColorBrush).Color;
            ToolPaneUnselectedTabBorderThickness = _layoutManager.ToolPaneGroupStyle.UnselectedTabStyle.BorderThickness.ToString();
            ToolPaneUnselectedTabBackgroundColour = (_layoutManager.ToolPaneGroupStyle.UnselectedTabStyle.Background as SolidColorBrush).Color;
            ToolPaneUnselectedTabForegroundColour = (_layoutManager.ToolPaneGroupStyle.UnselectedTabStyle.Foreground as SolidColorBrush).Color;
            ToolPaneUnselectedTabTitlePadding = _layoutManager.ToolPaneGroupStyle.UnselectedTabStyle.TitlePadding.ToString();
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
            if (!Parse(_toolPaneCornerRadius, out CornerRadius cornerRadius, "Tool Pane Corner Radius"))
            {
                return;
            }
            toolPaneGroupStyle.CornerRadius = cornerRadius;
            toolPaneGroupStyle.BorderBrush = new SolidColorBrush(ToolPaneBorderColour);
            if (!Parse(ToolPaneBorderThickness, out Thickness thickness, "Tool Pane Border Thickness"))
            {
                return;
            }
            toolPaneGroupStyle.BorderThickness = thickness;
            toolPaneGroupStyle.Background = new SolidColorBrush(ToolPaneBackgroundColour);
            toolPaneGroupStyle.GapBrush = new SolidColorBrush(ToolPaneGapColour);
            // Warning warning
            //toolPaneGroupStyle.GapHeight = thickness;
            toolPaneGroupStyle.ButtonForeground = new SolidColorBrush(ToolPaneButtonForegroundColour);


            if (!Parse(ToolPaneHeaderCornerRadius, out cornerRadius, "Tool Pane Header Corner Radius"))
            {
                return;
            }
            toolPaneGroupStyle.HeaderStyle.CornerRadius = cornerRadius;
            toolPaneGroupStyle.HeaderStyle.BorderBrush = new SolidColorBrush(ToolPaneHeaderBorderColour);
            if (!Parse(ToolPaneHeaderBorderThickness, out thickness, "Tool Pane Header Border Thickness"))
            {
                return;
            }
            toolPaneGroupStyle.HeaderStyle.BorderThickness = thickness;
            toolPaneGroupStyle.HeaderStyle.Background = new SolidColorBrush(ToolPaneHeaderBackgroundColour);
            if (!Parse(ToolPaneHeaderTitlePadding, out thickness, "Tool Pane Header Title Padding"))
            {
                return;
            }
            toolPaneGroupStyle.HeaderStyle.TitlePadding = thickness;


            if (!Parse(ToolPaneTabCornerRadius, out cornerRadius, "Tool Pane Tab Corner Radius"))
            {
                return;
            }
            toolPaneGroupStyle.TabCornerRadius = cornerRadius;
            toolPaneGroupStyle.ActiveScrollIndicatorBrush = new SolidColorBrush(ActiveScrollIndicatorColour);
            toolPaneGroupStyle.InactiveScrollIndicatorBrush = new SolidColorBrush(InactiveScrollIndicatorColour);

            toolPaneGroupStyle.SelectedTabStyle.BorderBrush = new SolidColorBrush(ToolPaneSelectedTabBorderColour);
            if (!Parse(ToolPaneSelectedTabBorderThickness, out thickness, "Tool Pane Selected Tab Border Thickness"))
            {
                return;
            }
            toolPaneGroupStyle.SelectedTabStyle.BorderThickness = thickness;
            toolPaneGroupStyle.SelectedTabStyle.Background = new SolidColorBrush(ToolPaneSelectedTabBackgroundColour);
            toolPaneGroupStyle.SelectedTabStyle.Foreground = new SolidColorBrush(ToolPaneSelectedTabForegroundColour);
            if (!Parse(_toolPaneSelectedTabTitlePadding, out thickness, "Tool Pane Selected Tab Title Padding"))
            {
                return;
            }
            toolPaneGroupStyle.SelectedTabStyle.TitlePadding = thickness;

            toolPaneGroupStyle.UnselectedTabStyle.BorderBrush = new SolidColorBrush(ToolPaneUnselectedTabBorderColour);
            if (!Parse(ToolPaneUnselectedTabBorderThickness, out thickness, "Tool Pane Unselected Tab Border Thickness"))
            {
                return;
            }
            toolPaneGroupStyle.UnselectedTabStyle.BorderThickness = thickness;
            toolPaneGroupStyle.UnselectedTabStyle.Background = new SolidColorBrush(ToolPaneUnselectedTabBackgroundColour);
            toolPaneGroupStyle.UnselectedTabStyle.Foreground = new SolidColorBrush(ToolPaneUnselectedTabForegroundColour);
            if (!Parse(_toolPaneUnselectedTabTitlePadding, out thickness, "Tool Pane Unselected Tab Title Padding"))
            {
                return;
            }
            toolPaneGroupStyle.UnselectedTabStyle.TitlePadding = thickness;


            _layoutManager.ToolPaneGroupStyle = toolPaneGroupStyle;
        }

        private readonly DockManager.LayoutManager _layoutManager;

        public void UpdateView()
        {
            NotifyPropertyChanged(null);
        }

        public string _toolPaneCornerRadius;
        public string ToolPaneCornerRadius
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

        public Color _toolPaneBorderColour;
        public Color ToolPaneBorderColour
        {
            get
            {
                return _toolPaneBorderColour;
            }
            set
            {
                _toolPaneBorderColour = value;
                NotifyPropertyChanged("ToolPaneBorderColour");
            }
        }

        public string _toolPaneBorderThickness;
        public string ToolPaneBorderThickness
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

        public Color _toolPaneBackgroundColour;
        public Color ToolPaneBackgroundColour
        {
            get
            {
                return _toolPaneBackgroundColour;
            }
            set
            {
                _toolPaneBackgroundColour = value;
                NotifyPropertyChanged("ToolPaneBackgroundColour");
            }
        }

        public Color _toolPaneGapColour;
        public Color ToolPaneGapColour
        {
            get
            {
                return _toolPaneGapColour;
            }
            set
            {
                _toolPaneGapColour = value;
                NotifyPropertyChanged("ToolPaneGapColour");
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

        public Color _toolPaneButtonForegroundColour;
        public Color ToolPaneButtonForegroundColour
        {
            get
            {
                return _toolPaneButtonForegroundColour;
            }
            set
            {
                _toolPaneButtonForegroundColour = value;
                NotifyPropertyChanged("ToolPaneButtonForegroundColour");
            }
        }

        public string _toolPaneHeaderCornerRadius;
        public string ToolPaneHeaderCornerRadius
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

        public Color _toolPaneHeaderBorderColour;
        public Color ToolPaneHeaderBorderColour
        {
            get
            {
                return _toolPaneHeaderBorderColour;
            }
            set
            {
                _toolPaneHeaderBorderColour = value;
                NotifyPropertyChanged("ToolPaneHeaderBorderColour");
            }
        }

        public string _toolPaneHeaderBorderThickness;
        public string ToolPaneHeaderBorderThickness
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

        public Color _toolPaneHeaderBackgroundColour;
        public Color ToolPaneHeaderBackgroundColour
        {
            get
            {
                return _toolPaneHeaderBackgroundColour;
            }
            set
            {
                _toolPaneHeaderBackgroundColour = value;
                NotifyPropertyChanged("ToolPaneBackgroundColour");
            }
        }

        public string _toolPaneHeaderTitlePadding;
        public string ToolPaneHeaderTitlePadding
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

        public string _toolPaneTabCornerRadius;
        public string ToolPaneTabCornerRadius
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

        public Color _activeScrollIndicatorColour;
        public Color ActiveScrollIndicatorColour
        {
            get
            {
                return _activeScrollIndicatorColour;
            }
            set
            {
                _activeScrollIndicatorColour = value;
                NotifyPropertyChanged("ActiveScrollIndicatorColour");
            }
        }

        public Color _inactiveScrollIndicatorColour;
        public Color InactiveScrollIndicatorColour
        {
            get
            {
                return _inactiveScrollIndicatorColour;
            }
            set
            {
                _inactiveScrollIndicatorColour = value;
                NotifyPropertyChanged("InactiveScrollIndicatorColour");
            }
        }

        public Color _toolPaneSelectedTabBorderColour;
        public Color ToolPaneSelectedTabBorderColour
        {
            get
            {
                return _toolPaneSelectedTabBorderColour;
            }
            set
            {
                _toolPaneSelectedTabBorderColour = value;
                NotifyPropertyChanged("ToolPaneSelectedTabBorderColour");
            }
        }

        public string _toolPaneSelectedTabBorderThickness;
        public string ToolPaneSelectedTabBorderThickness
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

        public Color _toolPaneSelectedTabBackgroundColour;
        public Color ToolPaneSelectedTabBackgroundColour
        {
            get
            {
                return _toolPaneSelectedTabBackgroundColour;
            }
            set
            {
                _toolPaneSelectedTabBackgroundColour = value;
                NotifyPropertyChanged("ToolPaneSelectedTabBackgroundColour");
            }
        }

        public Color _toolPaneSelectedTabForegroundColour;
        public Color ToolPaneSelectedTabForegroundColour
        {
            get
            {
                return _toolPaneSelectedTabForegroundColour;
            }
            set
            {
                _toolPaneSelectedTabForegroundColour = value;
                NotifyPropertyChanged("ToolPaneSelectedTabForegroundColour");
            }
        }

        public string _toolPaneSelectedTabTitlePadding;
        public string ToolPaneSelectedTabTitlePadding
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

        public Color _toolPaneUnselectedTabBorderColour;
        public Color ToolPaneUnselectedTabBorderColour
        {
            get
            {
                return _toolPaneUnselectedTabBorderColour;
            }
            set
            {
                _toolPaneUnselectedTabBorderColour = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabBorderColour");
            }
        }

        public string _toolPaneUnselectedTabBorderThickness;
        public string ToolPaneUnselectedTabBorderThickness
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

        public Color _toolPaneUnselectedTabBackgroundColour;
        public Color ToolPaneUnselectedTabBackgroundColour
        {
            get
            {
                return _toolPaneUnselectedTabBackgroundColour;
            }
            set
            {
                _toolPaneUnselectedTabBackgroundColour = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabBackgroundColour");
            }
        }

        public Color _toolPaneUnselectedTabForegroundColour;
        public Color ToolPaneUnselectedTabForegroundColour
        {
            get
            {
                return _toolPaneUnselectedTabForegroundColour;
            }
            set
            {
                _toolPaneUnselectedTabForegroundColour = value;
                NotifyPropertyChanged("ToolPaneUnselectedTabForegroundColour");
            }
        }

        public string _toolPaneUnselectedTabTitlePadding;
        public string ToolPaneUnselectedTabTitlePadding
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
