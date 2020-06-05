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

            /*
             * Document pane group
             */

            ToolPaneCornerRadius = LayoutManager.ToolPaneGroupStyle.CornerRadius;
            ToolPaneBorderBrush = LayoutManager.ToolPaneGroupStyle.BorderBrush;
            ToolPaneBorderThickness = LayoutManager.ToolPaneGroupStyle.BorderThickness;
            AvailableFontSizes = new System.Collections.ObjectModel.ObservableCollection<int>();
            for (int index = 4; index < 42; ++index)
            {
                AvailableFontSizes.Add(index);
            }
            ToolPaneFontSize = (int)LayoutManager.ToolPaneGroupStyle.FontSize;
            ToolPaneFontFamily = LayoutManager.ToolPaneGroupStyle.FontFamily;
            ToolPaneBackgroundBrush = LayoutManager.ToolPaneGroupStyle.Background;
            ToolPaneGapBrush = LayoutManager.ToolPaneGroupStyle.GapBrush;
            AvailableGapHeights = new System.Collections.ObjectModel.ObservableCollection<double>();
            for (double index = 0; index < 12; ++index)
            {
                AvailableGapHeights.Add(index);
            }
            ToolPaneGapHeight = LayoutManager.ToolPaneGroupStyle.GapHeight;
            ToolPaneButtonForegroundBrush = LayoutManager.ToolPaneGroupStyle.ButtonForeground;

            ToolPaneHeaderCornerRadius = LayoutManager.ToolPaneGroupStyle.HeaderStyle.CornerRadius;
            ToolPaneHeaderBorderBrush = LayoutManager.ToolPaneGroupStyle.HeaderStyle.BorderBrush;
            ToolPaneHeaderBorderThickness = LayoutManager.ToolPaneGroupStyle.HeaderStyle.BorderThickness;
            ToolPaneHeaderBackgroundBrush = LayoutManager.ToolPaneGroupStyle.HeaderStyle.Background;
            ToolPaneHeaderTitlePadding = LayoutManager.ToolPaneGroupStyle.HeaderStyle.TitlePadding;

            ToolPaneTabCornerRadius = LayoutManager.ToolPaneGroupStyle.TabCornerRadius;
            ToolPaneActiveScrollIndicatorBrush = LayoutManager.ToolPaneGroupStyle.ActiveScrollIndicatorBrush;
            ToolPaneInactiveScrollIndicatorBrush = LayoutManager.ToolPaneGroupStyle.InactiveScrollIndicatorBrush;

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

            /*
             * Document pane group
             */

            DocumentPaneCornerRadius = LayoutManager.DocumentPaneGroupStyle.CornerRadius;
            DocumentPaneBorderBrush = LayoutManager.DocumentPaneGroupStyle.BorderBrush;
            DocumentPaneBorderThickness = LayoutManager.DocumentPaneGroupStyle.BorderThickness;
            AvailableFontSizes = new System.Collections.ObjectModel.ObservableCollection<int>();
            for (int index = 4; index < 42; ++index)
            {
                AvailableFontSizes.Add(index);
            }
            DocumentPaneFontSize = (int)LayoutManager.DocumentPaneGroupStyle.FontSize;
            DocumentPaneFontFamily = LayoutManager.DocumentPaneGroupStyle.FontFamily;
            DocumentPaneBackgroundBrush = LayoutManager.DocumentPaneGroupStyle.Background;
            DocumentPaneGapBrush = LayoutManager.DocumentPaneGroupStyle.GapBrush;
            DocumentPaneGapHeight = LayoutManager.DocumentPaneGroupStyle.GapHeight;
            DocumentPaneButtonForegroundBrush = LayoutManager.DocumentPaneGroupStyle.ButtonForeground;

            DocumentPaneTabCornerRadius = LayoutManager.DocumentPaneGroupStyle.TabCornerRadius;
            DocumentPaneActiveScrollIndicatorBrush = LayoutManager.DocumentPaneGroupStyle.ActiveScrollIndicatorBrush;
            DocumentPaneInactiveScrollIndicatorBrush = LayoutManager.DocumentPaneGroupStyle.InactiveScrollIndicatorBrush;

            DocumentPaneSelectedTabBorderBrush = LayoutManager.DocumentPaneGroupStyle.SelectedTabStyle.BorderBrush;
            DocumentPaneSelectedTabBorderThickness = LayoutManager.DocumentPaneGroupStyle.SelectedTabStyle.BorderThickness;
            DocumentPaneSelectedTabBackgroundBrush = LayoutManager.DocumentPaneGroupStyle.SelectedTabStyle.Background;
            DocumentPaneSelectedTabForegroundBrush = LayoutManager.DocumentPaneGroupStyle.SelectedTabStyle.Foreground;
            DocumentPaneSelectedTabTitlePadding = LayoutManager.DocumentPaneGroupStyle.SelectedTabStyle.TitlePadding;

            DocumentPaneUnselectedTabBorderBrush = LayoutManager.DocumentPaneGroupStyle.UnselectedTabStyle.BorderBrush;
            DocumentPaneUnselectedTabBorderThickness = LayoutManager.DocumentPaneGroupStyle.UnselectedTabStyle.BorderThickness;
            DocumentPaneUnselectedTabBackgroundBrush = LayoutManager.DocumentPaneGroupStyle.UnselectedTabStyle.Background;
            DocumentPaneUnselectedTabForegroundBrush = LayoutManager.DocumentPaneGroupStyle.UnselectedTabStyle.Foreground;
            DocumentPaneUnselectedTabTitlePadding = LayoutManager.DocumentPaneGroupStyle.UnselectedTabStyle.TitlePadding;

            NotifyPropertyChanged(null);
        }
        
        public void Apply()
        {
            ToolPaneGroupStyle toolPaneGroupStyle = new ToolPaneGroupStyle();
            toolPaneGroupStyle.CornerRadius = ToolPaneCornerRadius;
            toolPaneGroupStyle.BorderBrush = ToolPaneBorderBrush;
            toolPaneGroupStyle.BorderThickness = ToolPaneBorderThickness;
            toolPaneGroupStyle.FontSize = (int)ToolPaneFontSize;
            toolPaneGroupStyle.FontFamily = ToolPaneFontFamily;
            toolPaneGroupStyle.Background = ToolPaneBackgroundBrush;
            toolPaneGroupStyle.GapBrush = ToolPaneGapBrush;
            toolPaneGroupStyle.GapHeight = ToolPaneGapHeight;
            toolPaneGroupStyle.ButtonForeground = ToolPaneButtonForegroundBrush;


            toolPaneGroupStyle.HeaderStyle.CornerRadius = ToolPaneHeaderCornerRadius;
            toolPaneGroupStyle.HeaderStyle.BorderBrush = ToolPaneHeaderBorderBrush;
            toolPaneGroupStyle.HeaderStyle.BorderThickness = ToolPaneHeaderBorderThickness;
            toolPaneGroupStyle.HeaderStyle.Background = ToolPaneHeaderBackgroundBrush;
            toolPaneGroupStyle.HeaderStyle.TitlePadding = ToolPaneHeaderTitlePadding;


            toolPaneGroupStyle.TabCornerRadius = ToolPaneTabCornerRadius;
            toolPaneGroupStyle.ActiveScrollIndicatorBrush = ToolPaneActiveScrollIndicatorBrush;
            toolPaneGroupStyle.InactiveScrollIndicatorBrush = ToolPaneInactiveScrollIndicatorBrush;

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



            DocumentPaneGroupStyle documentPaneGroupStyle = new DocumentPaneGroupStyle();
            documentPaneGroupStyle.CornerRadius = DocumentPaneCornerRadius;
            documentPaneGroupStyle.BorderBrush = DocumentPaneBorderBrush;
            documentPaneGroupStyle.BorderThickness = DocumentPaneBorderThickness;
            documentPaneGroupStyle.FontSize = (int)DocumentPaneFontSize;
            documentPaneGroupStyle.FontFamily = DocumentPaneFontFamily;
            documentPaneGroupStyle.Background = DocumentPaneBackgroundBrush;
            documentPaneGroupStyle.GapBrush = DocumentPaneGapBrush;
            documentPaneGroupStyle.GapHeight = DocumentPaneGapHeight;
            documentPaneGroupStyle.ButtonForeground = DocumentPaneButtonForegroundBrush;

            documentPaneGroupStyle.TabCornerRadius = DocumentPaneTabCornerRadius;
            documentPaneGroupStyle.ActiveScrollIndicatorBrush = DocumentPaneActiveScrollIndicatorBrush;
            documentPaneGroupStyle.InactiveScrollIndicatorBrush = DocumentPaneInactiveScrollIndicatorBrush;

            documentPaneGroupStyle.SelectedTabStyle.BorderBrush = DocumentPaneSelectedTabBorderBrush;
            documentPaneGroupStyle.SelectedTabStyle.BorderThickness = DocumentPaneSelectedTabBorderThickness;
            documentPaneGroupStyle.SelectedTabStyle.Background = DocumentPaneSelectedTabBackgroundBrush;
            documentPaneGroupStyle.SelectedTabStyle.Foreground = DocumentPaneSelectedTabForegroundBrush;
            documentPaneGroupStyle.SelectedTabStyle.TitlePadding = DocumentPaneSelectedTabTitlePadding;

            documentPaneGroupStyle.UnselectedTabStyle.BorderBrush = DocumentPaneUnselectedTabBorderBrush;
            documentPaneGroupStyle.UnselectedTabStyle.BorderThickness = DocumentPaneUnselectedTabBorderThickness;
            documentPaneGroupStyle.UnselectedTabStyle.Background = DocumentPaneUnselectedTabBackgroundBrush;
            documentPaneGroupStyle.UnselectedTabStyle.Foreground = DocumentPaneUnselectedTabForegroundBrush;
            documentPaneGroupStyle.UnselectedTabStyle.TitlePadding = DocumentPaneUnselectedTabTitlePadding;


            LayoutManager.DocumentPaneGroupStyle = documentPaneGroupStyle;
        }

        public readonly DockManager.LayoutManager LayoutManager;

        public void UpdateView()
        {
            NotifyPropertyChanged(null);
        }

        // Tool pane group

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

        private FontFamily _toolPaneFontFamily;
        public FontFamily ToolPaneFontFamily 
        {
            get
            {
                return _toolPaneFontFamily;
            }
            set
            {
                _toolPaneFontFamily = value;
                NotifyPropertyChanged("ToolPaneFontFamily");
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

        private System.Collections.ObjectModel.ObservableCollection<double> _availableGapHeights;
        public System.Collections.ObjectModel.ObservableCollection<double> AvailableGapHeights
        {
            get
            {
                return _availableGapHeights;
            }
            set
            {
                _availableGapHeights = value;
                NotifyPropertyChanged("AvailableGapHeights");
            }
        }

        public double _toolPaneGapHeight;
        public double ToolPaneGapHeight
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

        public Brush _toolPaneActiveScrollIndicatorBrush;
        public Brush ToolPaneActiveScrollIndicatorBrush
        {
            get
            {
                return _toolPaneActiveScrollIndicatorBrush;
            }
            set
            {
                _toolPaneActiveScrollIndicatorBrush = value;
                NotifyPropertyChanged("ToolPaneActiveScrollIndicatorBrush");
            }
        }

        public Brush __toolPaneInactiveScrollIndicatorBrush;
        public Brush ToolPaneInactiveScrollIndicatorBrush
        {
            get
            {
                return __toolPaneInactiveScrollIndicatorBrush;
            }
            set
            {
                __toolPaneInactiveScrollIndicatorBrush = value;
                NotifyPropertyChanged("ToolPaneInactiveScrollIndicatorBrush");
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

        // Document pane group 

        public CornerRadius _documentPaneCornerRadius;
        public CornerRadius DocumentPaneCornerRadius
        {
            get
            {
                return _documentPaneCornerRadius;
            }
            set
            {
                _documentPaneCornerRadius = value;
                NotifyPropertyChanged("DocumentPaneCornerRadius");
            }
        }

        public Brush _documentPaneBorderBrush;
        public Brush DocumentPaneBorderBrush
        {
            get
            {
                return _documentPaneBorderBrush;
            }
            set
            {
                _documentPaneBorderBrush = value;
                NotifyPropertyChanged("DocumentPaneBorderBrush");
            }
        }

        public Thickness _documentPaneBorderThickness;
        public Thickness DocumentPaneBorderThickness
        {
            get
            {
                return _documentPaneBorderThickness;
            }
            set
            {
                _documentPaneBorderThickness = value;
                NotifyPropertyChanged("DocumentPaneBorderThickness");
            }
        }

        public int _documentPaneFontSize;
        public int DocumentPaneFontSize
        {
            get
            {
                return _documentPaneFontSize;
            }
            set
            {
                _documentPaneFontSize = value;
                NotifyPropertyChanged("DocumentPaneFontSize");
            }
        }

        private FontFamily _documentPaneFontFamily;
        public FontFamily DocumentPaneFontFamily
        {
            get
            {
                return _documentPaneFontFamily;
            }
            set
            {
                _documentPaneFontFamily = value;
                NotifyPropertyChanged("DocumentPaneFontFamily");
            }
        }

        public Brush _documentPaneBackgroundBrush;
        public Brush DocumentPaneBackgroundBrush
        {
            get
            {
                return _documentPaneBackgroundBrush;
            }
            set
            {
                _documentPaneBackgroundBrush = value;
                NotifyPropertyChanged("DocumentPaneBackgroundBrush");
            }
        }

        public Brush _documentPaneGapBrush;
        public Brush DocumentPaneGapBrush
        {
            get
            {
                return _documentPaneGapBrush;
            }
            set
            {
                _documentPaneGapBrush = value;
                NotifyPropertyChanged("DocumentPaneGapBrush");
            }
        }

        public double _documentPaneGapHeight;
        public double DocumentPaneGapHeight
        {
            get
            {
                return _documentPaneGapHeight;
            }
            set
            {
                _documentPaneGapHeight = value;
                NotifyPropertyChanged("DocumentPaneGapHeight");
            }
        }

        public Brush _documentPaneButtonForegroundBrush;
        public Brush DocumentPaneButtonForegroundBrush
        {
            get
            {
                return _documentPaneButtonForegroundBrush;
            }
            set
            {
                _documentPaneButtonForegroundBrush = value;
                NotifyPropertyChanged("DocumentPaneButtonForegroundBrush");
            }
        }

        public CornerRadius _documentPaneTabCornerRadius;
        public CornerRadius DocumentPaneTabCornerRadius
        {
            get
            {
                return _documentPaneTabCornerRadius;
            }
            set
            {
                _documentPaneTabCornerRadius = value;
                NotifyPropertyChanged("DocumentPaneTabCornerRadius");
            }
        }

        public Brush _documentPaneActiveScrollIndicatorBrush;
        public Brush DocumentPaneActiveScrollIndicatorBrush
        {
            get
            {
                return _documentPaneActiveScrollIndicatorBrush;
            }
            set
            {
                _documentPaneActiveScrollIndicatorBrush = value;
                NotifyPropertyChanged("DocumentPaneActiveScrollIndicatorBrush");
            }
        }

        public Brush _documentPaneInactiveScrollIndicatorBrush;
        public Brush DocumentPaneInactiveScrollIndicatorBrush
        {
            get
            {
                return _documentPaneInactiveScrollIndicatorBrush;
            }
            set
            {
                _documentPaneInactiveScrollIndicatorBrush = value;
                NotifyPropertyChanged("DocumentPaneInactiveScrollIndicatorBrush");
            }
        }

        public Brush _documentPaneSelectedTabBorderBrush;
        public Brush DocumentPaneSelectedTabBorderBrush
        {
            get
            {
                return _documentPaneSelectedTabBorderBrush;
            }
            set
            {
                _documentPaneSelectedTabBorderBrush = value;
                NotifyPropertyChanged("DocumentPaneSelectedTabBorderBrush");
            }
        }

        public Thickness _documentPaneSelectedTabBorderThickness;
        public Thickness DocumentPaneSelectedTabBorderThickness
        {
            get
            {
                return _documentPaneSelectedTabBorderThickness;
            }
            set
            {
                _documentPaneSelectedTabBorderThickness = value;
                NotifyPropertyChanged("DocumentPaneSelectedTabBorderThickness");
            }
        }

        public Brush _documentPaneSelectedTabBackgroundBrush;
        public Brush DocumentPaneSelectedTabBackgroundBrush
        {
            get
            {
                return _documentPaneSelectedTabBackgroundBrush;
            }
            set
            {
                _documentPaneSelectedTabBackgroundBrush = value;
                NotifyPropertyChanged("DocumentPaneSelectedTabBackgroundBrush");
            }
        }

        public Brush _documentPaneSelectedTabForegroundBrush;
        public Brush DocumentPaneSelectedTabForegroundBrush
        {
            get
            {
                return _documentPaneSelectedTabForegroundBrush;
            }
            set
            {
                _documentPaneSelectedTabForegroundBrush = value;
                NotifyPropertyChanged("DocumentPaneSelectedTabForegroundBrush");
            }
        }

        public Thickness _documentPaneSelectedTabTitlePadding;
        public Thickness DocumentPaneSelectedTabTitlePadding
        {
            get
            {
                return _documentPaneSelectedTabTitlePadding;
            }
            set
            {
                _documentPaneSelectedTabTitlePadding = value;
                NotifyPropertyChanged("DocumentPaneSelectedTabTitlePadding");
            }
        }

        public Brush _documentPaneUnselectedTabBorderBrush;
        public Brush DocumentPaneUnselectedTabBorderBrush
        {
            get
            {
                return _documentPaneUnselectedTabBorderBrush;
            }
            set
            {
                _documentPaneUnselectedTabBorderBrush = value;
                NotifyPropertyChanged("DocumentPaneUnselectedTabBorderBrush");
            }
        }

        public Thickness _documentPaneUnselectedTabBorderThickness;
        public Thickness DocumentPaneUnselectedTabBorderThickness
        {
            get
            {
                return _documentPaneUnselectedTabBorderThickness;
            }
            set
            {
                _documentPaneUnselectedTabBorderThickness = value;
                NotifyPropertyChanged("DocumentPaneUnselectedTabBorderThickness");
            }
        }

        public Brush _documentPaneUnselectedTabBackgroundBrush;
        public Brush DocumentPaneUnselectedTabBackgroundBrush
        {
            get
            {
                return _documentPaneUnselectedTabBackgroundBrush;
            }
            set
            {
                _documentPaneUnselectedTabBackgroundBrush = value;
                NotifyPropertyChanged("DocumentPaneUnselectedTabBackgroundBrush");
            }
        }

        public Brush _documentPaneUnselectedTabForegroundBrush;
        public Brush DocumentPaneUnselectedTabForegroundBrush
        {
            get
            {
                return _documentPaneUnselectedTabForegroundBrush;
            }
            set
            {
                _documentPaneUnselectedTabForegroundBrush = value;
                NotifyPropertyChanged("DocumentPaneUnselectedTabForegroundBrush");
            }
        }

        public Thickness _documentPaneUnselectedTabTitlePadding;
        public Thickness DocumentPaneUnselectedTabTitlePadding
        {
            get
            {
                return _documentPaneUnselectedTabTitlePadding;
            }
            set
            {
                _documentPaneUnselectedTabTitlePadding = value;
                NotifyPropertyChanged("DocumentPaneUnselectedTabTitlePadding");
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
