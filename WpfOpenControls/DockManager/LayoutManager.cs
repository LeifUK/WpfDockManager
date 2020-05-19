using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;
using System.Windows.Media;
using WpfOpenControls.Controls;

namespace WpfOpenControls.DockManager
{
    public class LayoutManager : System.Windows.Controls.Grid, ILayoutFactory
    {
        public LayoutManager()
        {
            Tag = Guid.NewGuid();

            FloatingToolPaneGroups = new List<FloatingToolPaneGroup>();
            FloatingDocumentPaneGroups = new List<FloatingDocumentPaneGroup>();

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            CreateToolListBoxes();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            PreviewMouseDown += LayoutManager_PreviewMouseDown;
            SizeChanged += LayoutManager_SizeChanged;

            _dictUnpinnedToolData = new Dictionary<WindowLocation, List<UnpinnedToolData>>();
            _dictUnpinnedToolData.Add(WindowLocation.LeftSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.TopSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.RightSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.BottomSide, new List<UnpinnedToolData>());
        }

        private void LayoutManager_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point cursorPoint = Utilities.GetCursorPosition();
            Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
            if (
                    (cursorPoint.X > topLeftPoint.X) &&
                    (cursorPoint.Y > topLeftPoint.Y) &&
                    (cursorPoint.X < (topLeftPoint.X + _root.ActualWidth)) &&
                    (cursorPoint.Y < (topLeftPoint.Y + _root.ActualHeight))
                )
            {
                CloseUnpinnedToolPane();
            }
        }

        ~LayoutManager()
        {
            Shutdown();
        }

        ILayoutFactory ILayoutFactory
        {
            get
            {
                return this;
            }
        }

        public void Shutdown()
        {
            CloseUnpinnedToolPane();

            try
            {
                _windowLocationPane?.Close();
                _windowLocationPane = null;
            }
            catch
            {
                // Ignore
            }
            try
            {

                _sideLocationPane?.Close();
                _sideLocationPane = null;
            }
            catch
            {
                // Ignore
            }
        }

        // Call this from the mainwindow when loaded
        public void Initialise()
        {
            //IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
            //if (windowHandle == null)
            //{
            //    throw new Exception("LayoutManager.Initialise(): windowHandle is null");
            //}

            //System.Windows.Interop.HwndSource src = System.Windows.Interop.HwndSource.FromHwnd(windowHandle);
            //if (src == null)
            //{
            //    throw new Exception("LayoutManager.Initialise(): src is null");
            //}

            //src.AddHook(WndProc);
        }

        public DataTemplate ToolPaneTitleTemplate { get; set; }
        public DataTemplate ToolPaneHeaderTemplate { get; set; }
        public DataTemplate DocumentPaneTitleTemplate { get; set; }
        public DataTemplate DocumentPaneHeaderTemplate { get; set; }

        public event EventHandler DocumentClosed;
        public event EventHandler ToolClosed;

        internal List<FloatingToolPaneGroup> FloatingToolPaneGroups;
        internal List<FloatingDocumentPaneGroup> FloatingDocumentPaneGroups;

        internal Controls.ToolListBox _leftToolListBox;
        internal Controls.ToolListBox _topToolListBox;
        internal Controls.ToolListBox _rightToolListBox;
        internal Controls.ToolListBox _bottomToolListBox;

        internal ToolListBoxItem _activeToolListBoxItem;
        internal UnpinnedToolPane _activeUnpinnedToolPane;
        internal UnpinnedToolData _activeUnpinnedToolData;
        internal Controls.ToolListBox _activeToolListBox;

        private Dictionary<WindowLocation, List<UnpinnedToolData>> _dictUnpinnedToolData;

        internal Grid _root;

        private SelectablePane SelectedPane;

        #region dependency properties 

        #region DocumentsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentsSourceProperty = DependencyProperty.Register("DocumentsSource", typeof(System.Collections.Generic.IEnumerable<IViewModel>), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentsSourceChanged)));

        public System.Collections.Generic.IEnumerable<IViewModel> DocumentsSource
        {
            get
            {
                return (System.Collections.Generic.IEnumerable<IViewModel>)GetValue(DocumentsSourceProperty);
            }
            set
            {
                SetValue(DocumentsSourceProperty, value);
            }
        }

        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentsSourceChanged(e);
        }

        protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentsSource = (System.Collections.Generic.IEnumerable<IViewModel>)e.NewValue;
                Create();
            }
        }

        #endregion

        #region ToolsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolsSourceProperty = DependencyProperty.Register("ToolsSource", typeof(System.Collections.IEnumerable), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnToolsSourceChanged)));

        public System.Collections.IEnumerable ToolsSource
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(ToolsSourceProperty);
            }
            set
            {
                SetValue(ToolsSourceProperty, value);
            }
        }

        private static void OnToolsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolsSourceChanged(e);
        }

        protected virtual void OnToolsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ToolsSource = (System.Collections.IEnumerable)e.NewValue;
                Create();
            }
        }

        #endregion

        #region DocumentTemplates dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentTemplatesProperty = DependencyProperty.Register("DocumentTemplates", typeof(List<DataTemplate>), typeof(LayoutManager), new FrameworkPropertyMetadata(new List<DataTemplate>(), new PropertyChangedCallback(OnDocumentTemplatesChanged)));

        public List<DataTemplate> DocumentTemplates
        {
            get
            {
                return (List<DataTemplate>)GetValue(DocumentTemplatesProperty);
            }
            set
            {
                SetValue(DocumentTemplatesProperty, value);
            }
        }

        private static void OnDocumentTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentTemplatesChanged(e);
        }

        protected virtual void OnDocumentTemplatesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentTemplates = (List<DataTemplate>)e.NewValue;
                Create();
            }
        }

        #endregion

        #region ToolTemplates dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolTemplatesProperty = DependencyProperty.Register("ToolTemplates", typeof(System.Collections.Generic.List<DataTemplate>), typeof(LayoutManager), new FrameworkPropertyMetadata(new List<DataTemplate>(), new PropertyChangedCallback(OnToolTemplatesChanged)));

        public System.Collections.Generic.List<DataTemplate> ToolTemplates
        {
            get
            {
                return (System.Collections.Generic.List<DataTemplate>)GetValue(ToolTemplatesProperty);
            }
            set
            {
                SetValue(ToolTemplatesProperty, value);
            }
        }

        private static void OnToolTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolTemplatesChanged(e);
        }

        protected virtual void OnToolTemplatesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ToolTemplates = (System.Collections.Generic.List<DataTemplate>)e.NewValue;
                Create();
            }
        }

        #endregion

        #region CloseControlTemplate dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty CloseControlTemplateProperty = DependencyProperty.Register("CloseControlTemplate", typeof(System.Windows.Controls.ControlTemplate), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCloseControlTemplateChanged)));

        public System.Windows.Controls.ControlTemplate CloseControlTemplate
        {
            get
            {
                return (System.Windows.Controls.ControlTemplate)GetValue(ToolsSourceProperty);
            }
            set
            {
                SetValue(CloseControlTemplateProperty, value);
            }
        }

        private static void OnCloseControlTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnCloseControlTemplateChanged(e);
        }

        protected virtual void OnCloseControlTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                CloseControlTemplate = (System.Windows.Controls.ControlTemplate)e.NewValue;
            }
        }

        #endregion

        private void UpdateProperties(DocumentPaneGroup documentPaneGroup)
        {
            documentPaneGroup.HeaderBackground = DocumentHeaderBackground;
            IViewContainer iViewContainer = documentPaneGroup.IViewContainer;
            iViewContainer.FontSize = DocumentFontSize;
            iViewContainer.FontFamily = DocumentFontFamily;
            iViewContainer.TabCornerRadius = DocumentTabCornerRadius;
            iViewContainer.SelectedTabHeaderBackground = SelectedDocumentTabHeaderBackground;
            iViewContainer.UnselectedTabHeaderBackground = UnselectedDocumentTabHeaderBackground;
            iViewContainer.SelectedTabHeaderForeground = SelectedDocumentTabHeaderForeground;
            iViewContainer.UnselectedTabHeaderForeground = UnselectedDocumentTabHeaderForeground;
            iViewContainer.ActiveScrollIndicatorBrush = ActiveDocumentScrollIndicatorBrush;
            iViewContainer.InactiveScrollIndicatorBrush = InactiveDocumentScrollIndicatorBrush;
        }

        private void UpdateProperties(ToolContainer toolContainer)
        {
            toolContainer.FontSize = ToolFontSize;
            toolContainer.FontFamily = ToolFontFamily;
            toolContainer.TabCornerRadius = ToolTabCornerRadius;
            toolContainer.Background = ToolBackground;
            toolContainer.SelectedTabHeaderBackground = SelectedToolTabHeaderBackground;
            toolContainer.UnselectedTabHeaderBackground = UnselectedToolTabHeaderBackground;
            toolContainer.SelectedTabHeaderForeground = SelectedToolTabHeaderForeground;
            toolContainer.UnselectedTabHeaderForeground = UnselectedToolTabHeaderForeground;
            toolContainer.ActiveScrollIndicatorBrush = ActiveToolScrollIndicatorBrush;
            toolContainer.InactiveScrollIndicatorBrush = InactiveToolScrollIndicatorBrush;
        }

        private void UpdateProperties(ToolPaneGroup toolPaneGroup)
        {
            toolPaneGroup.HeaderBackground = ToolHeaderBackground;
            toolPaneGroup.FontSize = ToolFontSize;
            UpdateProperties(toolPaneGroup.IViewContainer as ToolContainer);
        }

        private void UpdateProperties(FloatingDocumentPaneGroup floatingDocumentPaneGroup)
        {
            floatingDocumentPaneGroup.FontSize = DocumentFontSize;
            floatingDocumentPaneGroup.FontFamily = new FontFamily(DocumentFontFamily);
            floatingDocumentPaneGroup.Background = FloatingDocumentBackground;
            floatingDocumentPaneGroup.HeaderBackground = FloatingDocumentTitleBarBackground;
            IViewContainer iViewContainer = floatingDocumentPaneGroup.IViewContainer;
            iViewContainer.TabCornerRadius = DocumentTabCornerRadius;
            iViewContainer.FontSize = DocumentFontSize;
            iViewContainer.FontFamily = DocumentFontFamily;
            iViewContainer.SelectedTabHeaderBackground = SelectedDocumentTabHeaderBackground;
            iViewContainer.UnselectedTabHeaderBackground = UnselectedDocumentTabHeaderBackground;
            iViewContainer.SelectedTabHeaderForeground = SelectedDocumentTabHeaderForeground;
            iViewContainer.UnselectedTabHeaderForeground = UnselectedDocumentTabHeaderForeground;
            iViewContainer.ActiveScrollIndicatorBrush = ActiveDocumentScrollIndicatorBrush;
            iViewContainer.InactiveScrollIndicatorBrush = InactiveDocumentScrollIndicatorBrush;
        }

        private void UpdateProperties(FloatingToolPaneGroup floatingToolPaneGroup)
        {
            floatingToolPaneGroup.FontSize = ToolFontSize;
            floatingToolPaneGroup.FontFamily = new FontFamily(ToolFontFamily);
            floatingToolPaneGroup.Background = FloatingToolBackground;
            floatingToolPaneGroup.HeaderBackground = FloatingToolTitleBarBackground;
            UpdateProperties(floatingToolPaneGroup.IViewContainer as ToolContainer);
        }

        private void UpdateProperties(Grid grid)
        {
            if (grid == null)
            {
                return;
            }

            foreach (var child in grid.Children)
            {
                if (child is ToolPaneGroup)
                {
                    UpdateProperties(child as ToolPaneGroup);
                }
                else if (child is DocumentPaneGroup)
                {
                    UpdateProperties(child as DocumentPaneGroup);
                }
                else if (child is FloatingToolPaneGroup)
                {
                    UpdateProperties((child as FloatingToolPaneGroup));
                }
                else if (child is FloatingDocumentPaneGroup)
                {
                    UpdateProperties(child as FloatingDocumentPaneGroup);
                }

                if (child is Grid)
                {
                    UpdateProperties(child as Grid);
                }
            }

            if (grid == _root)
            {
                _leftToolListBox.Foreground = SideToolForeground;
                _topToolListBox.Foreground = SideToolForeground;
                _rightToolListBox.Foreground = SideToolForeground;
                _bottomToolListBox.Foreground = SideToolForeground;

                foreach (var floatingDocumentPaneGroup in FloatingDocumentPaneGroups)
                {
                    UpdateProperties(floatingDocumentPaneGroup);
                }

                foreach (var floatingToolPaneGroup in FloatingToolPaneGroups)
                {
                    UpdateProperties(floatingToolPaneGroup);
                }
            }
        }

        #region ToolFontFamily dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolFontFamilyProperty = DependencyProperty.Register("ToolFontFamily", typeof(string), typeof(TabHeaderControl), new FrameworkPropertyMetadata("Arial", new PropertyChangedCallback(OnToolFontFamilyChanged)));

        public string ToolFontFamily
        {
            get
            {
                return (string)GetValue(ToolFontFamilyProperty);
            }
            set
            {
                if (value != ToolFontFamily)
                {
                    SetValue(ToolFontFamilyProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnToolFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolFontFamilyChanged(e);
        }

        protected virtual void OnToolFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue != ToolFontFamily)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ToolFontSize dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolFontSizeProperty = DependencyProperty.Register("ToolFontSize", typeof(double), typeof(TabHeaderControl), new FrameworkPropertyMetadata(12.0, new PropertyChangedCallback(OnToolFontSizeChanged)));

        public double ToolFontSize
        {
            get
            {
                return (double)GetValue(ToolFontSizeProperty);
            }
            set
            {
                if (value != ToolFontSize)
                {
                    SetValue(ToolFontSizeProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnToolFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolFontSizeChanged(e);
        }

        protected virtual void OnToolFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue != ToolFontSize)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ToolTabCornerRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolTabCornerRadiusProperty = DependencyProperty.Register("ToolTabCornerRadius", typeof(CornerRadius), typeof(TabHeaderControl), new FrameworkPropertyMetadata(new CornerRadius(0.0), new PropertyChangedCallback(OnToolTabCornerRadiusChanged)));

        public CornerRadius ToolTabCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ToolTabCornerRadiusProperty);
            }
            set
            {
                if (value != ToolTabCornerRadius)
                {
                    SetValue(ToolTabCornerRadiusProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnToolTabCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolTabCornerRadiusChanged(e);
        }

        protected virtual void OnToolTabCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((CornerRadius)e.NewValue != ToolTabCornerRadius)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ActiveToolScrollIndicatorBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ActiveToolScrollIndicatorBrushProperty = DependencyProperty.Register("ActiveToolScrollIndicatorBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnActiveToolScrollIndicatorBrushChanged)));

        public Brush ActiveToolScrollIndicatorBrush
        {
            get
            {
                return (Brush)GetValue(ActiveToolScrollIndicatorBrushProperty);
            }
            set
            {
                if (value != ActiveToolScrollIndicatorBrush)
                {
                    SetValue(ActiveToolScrollIndicatorBrushProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnActiveToolScrollIndicatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnActiveToolScrollIndicatorBrushChanged(e);
        }

        protected virtual void OnActiveToolScrollIndicatorBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != ActiveToolScrollIndicatorBrush)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region InactiveToolScrollIndicatorBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty InactiveToolScrollIndicatorBrushProperty = DependencyProperty.Register("InactiveToolScrollIndicatorBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnInactiveToolScrollIndicatorBrushChanged)));

        public Brush InactiveToolScrollIndicatorBrush
        {
            get
            {
                return (Brush)GetValue(InactiveToolScrollIndicatorBrushProperty);
            }
            set
            {
                if (value != InactiveToolScrollIndicatorBrush)
                {
                    SetValue(InactiveToolScrollIndicatorBrushProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnInactiveToolScrollIndicatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnInactiveToolScrollIndicatorBrushChanged(e);
        }

        protected virtual void OnInactiveToolScrollIndicatorBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != InactiveToolScrollIndicatorBrush)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ToolBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolBackgroundProperty = DependencyProperty.Register("ToolBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightSteelBlue, new PropertyChangedCallback(OnToolBackgroundChanged)));

        public Brush ToolBackground
        {
            get
            {
                return (Brush)GetValue(ToolBackgroundProperty);
            }
            set
            {
                if (value != ToolBackground)
                {
                    SetValue(ToolBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnToolBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolBackgroundChanged(e);
        }

        protected virtual void OnToolBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != ToolBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ToolHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolHeaderBackgroundProperty = DependencyProperty.Register("ToolHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightSteelBlue, new PropertyChangedCallback(OnToolHeaderBackgroundChanged)));

        public Brush ToolHeaderBackground
        {
            get
            {
                return (Brush)GetValue(ToolHeaderBackgroundProperty);
            }
            set
            {
                if (value != ToolHeaderBackground)
                {
                    SetValue(ToolHeaderBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnToolHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolHeaderBackgroundChanged(e);
        }

        protected virtual void OnToolHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != ToolHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SelectedToolTabHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedToolTabHeaderBackgroundProperty = DependencyProperty.Register("SelectedToolTabHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightSteelBlue, new PropertyChangedCallback(OnSelectedToolTabHeaderBackgroundChanged)));

        public Brush SelectedToolTabHeaderBackground
        {
            get
            {
                return (Brush)GetValue(SelectedToolTabHeaderBackgroundProperty);
            }
            set
            {
                if (value != SelectedToolTabHeaderBackground)
                {
                    SetValue(SelectedToolTabHeaderBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnSelectedToolTabHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSelectedToolTabHeaderBackgroundChanged(e);
        }

        protected virtual void OnSelectedToolTabHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SelectedToolTabHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region UnselectedToolTabHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedToolTabHeaderBackgroundProperty = DependencyProperty.Register("UnselectedToolTabHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnSelectedToolTabHeaderBackgroundChanged)));

        public Brush UnselectedToolTabHeaderBackground
        {
            get
            {
                return (Brush)GetValue(UnselectedToolTabHeaderBackgroundProperty);
            }
            set
            {
                if (value != UnselectedToolTabHeaderBackground)
                {
                    SetValue(UnselectedToolTabHeaderBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnUnselectedToolTabHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnUnselectedToolTabHeaderBackgroundChanged(e);
        }

        protected virtual void OnUnselectedToolTabHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != UnselectedToolTabHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SelectedToolTabHeaderForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedToolTabHeaderForegroundProperty = DependencyProperty.Register("SelectedToolTabHeaderForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Khaki, new PropertyChangedCallback(OnSelectedToolTabHeaderForegroundPropertyChanged)));

        public Brush SelectedToolTabHeaderForeground
        {
            get
            {
                return (Brush)GetValue(SelectedToolTabHeaderForegroundProperty);
            }
            set
            {
                if (value != SelectedToolTabHeaderForeground)
                {
                    SetValue(SelectedToolTabHeaderForegroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnSelectedToolTabHeaderForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSelectedToolTabHeaderForegroundPropertyChanged(e);
        }

        protected virtual void OnSelectedToolTabHeaderForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SelectedToolTabHeaderForeground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region UnselectedToolTabHeaderForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedToolTabHeaderForegroundProperty = DependencyProperty.Register("UnselectedToolTabHeaderForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnUnselectedToolTabHeaderForegroundChanged)));

        public Brush UnselectedToolTabHeaderForeground
        {
            get
            {
                return (Brush)GetValue(UnselectedToolTabHeaderForegroundProperty);
            }
            set
            {
                if (value != UnselectedToolTabHeaderForeground)
                {
                    SetValue(UnselectedToolTabHeaderForegroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnUnselectedToolTabHeaderForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnUnselectedToolTabHeaderForegroundChanged(e);
        }

        protected virtual void OnUnselectedToolTabHeaderForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SelectedToolTabHeaderForeground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region FloatingToolTitleBarBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FloatingToolTitleBarBackgroundProperty = DependencyProperty.Register("FloatingToolTitleBarBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, new PropertyChangedCallback(OnFloatingToolTitleBarBackgroundChanged)));

        public Brush FloatingToolTitleBarBackground
        {
            get
            {
                return (Brush)GetValue(FloatingToolTitleBarBackgroundProperty);
            }
            set
            {
                if (value != FloatingToolTitleBarBackground)
                {
                    SetValue(FloatingToolTitleBarBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnFloatingToolTitleBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnFloatingToolTitleBarBackgroundChanged(e);
        }

        protected virtual void OnFloatingToolTitleBarBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != FloatingToolTitleBarBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region FloatingToolBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FloatingToolBackgroundProperty = DependencyProperty.Register("FloatingToolBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightBlue, new PropertyChangedCallback(OnFloatingToolBackgroundChanged)));

        public Brush FloatingToolBackground
        {
            get
            {
                return (Brush)GetValue(FloatingToolBackgroundProperty);
            }
            set
            {
                if (value != FloatingToolBackground)
                {
                    SetValue(FloatingToolBackgroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnFloatingToolBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnFloatingToolBackgroundChanged(e);
        }

        protected virtual void OnFloatingToolBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != FloatingToolBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region DocumentFontFamily dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentFontFamilyProperty = DependencyProperty.Register("DocumentFontFamily", typeof(string), typeof(TabHeaderControl), new FrameworkPropertyMetadata("Arial", new PropertyChangedCallback(OnDocumentFontFamilyChanged)));

        public string DocumentFontFamily
        {
            get
            {
                return (string)GetValue(DocumentFontFamilyProperty);
            }
            set
            {
                if (value != DocumentFontFamily)
                {
                    SetValue(DocumentFontFamilyProperty, value);
                }
            }
        }

        private static void OnDocumentFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentFontFamilyChanged(e);
        }

        protected virtual void OnDocumentFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((string)e.NewValue != DocumentFontFamily)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region DocumentFontSize dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentFontSizeProperty = DependencyProperty.Register("DocumentFontSize", typeof(double), typeof(TabHeaderControl), new FrameworkPropertyMetadata(12.0, new PropertyChangedCallback(OnDocumentFontSizeChanged)));

        public double DocumentFontSize
        {
            get
            {
                return (double)GetValue(DocumentFontSizeProperty);
            }
            set
            {
                if (value != DocumentFontSize)
                {
                    SetValue(DocumentFontSizeProperty, value);
                }
            }
        }

        private static void OnDocumentFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentFontSizeChanged(e);
        }

        protected virtual void OnDocumentFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue != DocumentFontSize)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region DocumentTabCornerRadius dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentTabCornerRadiusProperty = DependencyProperty.Register("DocumentTabCornerRadius", typeof(CornerRadius), typeof(TabHeaderControl), new FrameworkPropertyMetadata(new CornerRadius(0.0), new PropertyChangedCallback(OnDocumentTabCornerRadiusChanged)));

        public CornerRadius DocumentTabCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(DocumentTabCornerRadiusProperty);
            }
            set
            {
                if (value != DocumentTabCornerRadius)
                {
                    SetValue(DocumentTabCornerRadiusProperty, value);
                }
            }
        }

        private static void OnDocumentTabCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentTabCornerRadiusChanged(e);
        }

        protected virtual void OnDocumentTabCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((CornerRadius)e.NewValue != DocumentTabCornerRadius)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region ActiveDocumentScrollIndicatorBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ActiveDocumentScrollIndicatorBrushProperty = DependencyProperty.Register("ActiveDocumentScrollIndicatorBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnActiveDocumentScrollIndicatorBrushChanged)));

        public Brush ActiveDocumentScrollIndicatorBrush
        {
            get
            {
                return (Brush)GetValue(ActiveDocumentScrollIndicatorBrushProperty);
            }
            set
            {
                if (value != ActiveDocumentScrollIndicatorBrush)
                {
                    SetValue(ActiveDocumentScrollIndicatorBrushProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnActiveDocumentScrollIndicatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnActiveDocumentScrollIndicatorBrushChanged(e);
        }

        protected virtual void OnActiveDocumentScrollIndicatorBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != ActiveDocumentScrollIndicatorBrush)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region InactiveDocumentScrollIndicatorBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty InactiveDocumentScrollIndicatorBrushProperty = DependencyProperty.Register("InactiveDocumentScrollIndicatorBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnInactiveDocumentScrollIndicatorBrushChanged)));

        public Brush InactiveDocumentScrollIndicatorBrush
        {
            get
            {
                return (Brush)GetValue(InactiveDocumentScrollIndicatorBrushProperty);
            }
            set
            {
                if (value != InactiveDocumentScrollIndicatorBrush)
                {
                    SetValue(InactiveDocumentScrollIndicatorBrushProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnInactiveDocumentScrollIndicatorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnInactiveDocumentScrollIndicatorBrushChanged(e);
        }

        protected virtual void OnInactiveDocumentScrollIndicatorBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != InactiveDocumentScrollIndicatorBrush)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region DocumentHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentHeaderBackgroundProperty = DependencyProperty.Register("DocumentHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightBlue, new PropertyChangedCallback(OnDocumentHeaderBackgroundChanged)));

        public Brush DocumentHeaderBackground
        {
            get
            {
                return (Brush)GetValue(DocumentHeaderBackgroundProperty);
            }
            set
            {
                if (value != DocumentHeaderBackground)
                {
                    SetValue(DocumentHeaderBackgroundProperty, value);
                }
            }
        }

        private static void OnDocumentHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentHeaderBackgroundChanged(e);
        }

        protected virtual void OnDocumentHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != DocumentHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SelectedDocumentTabHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedDocumentTabHeaderBackgroundProperty = DependencyProperty.Register("SelectedDocumentTabHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightSteelBlue, new PropertyChangedCallback(OnSelectedDocumentTabHeaderBackgroundChanged)));

        public Brush SelectedDocumentTabHeaderBackground
        {
            get
            {
                return (Brush)GetValue(SelectedDocumentTabHeaderBackgroundProperty);
            }
            set
            {
                if (value != SelectedDocumentTabHeaderBackground)
                {
                    SetValue(SelectedDocumentTabHeaderBackgroundProperty, value);
                }
            }
        }

        private static void OnSelectedDocumentTabHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSelectedDocumentTabHeaderBackgroundChanged(e);
        }

        protected virtual void OnSelectedDocumentTabHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SelectedDocumentTabHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region UnselectedDocumentTabHeaderBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedDocumentTabHeaderBackgroundProperty = DependencyProperty.Register("UnselectedDocumentTabHeaderBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnSelectedDocumentTabHeaderBackgroundChanged)));

        public Brush UnselectedDocumentTabHeaderBackground
        {
            get
            {
                return (Brush)GetValue(UnselectedDocumentTabHeaderBackgroundProperty);
            }
            set
            {
                if (value != UnselectedDocumentTabHeaderBackground)
                {
                    SetValue(UnselectedDocumentTabHeaderBackgroundProperty, value);
                }
            }
        }

        private static void OnUnselectedDocumentTabHeaderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnUnselectedDocumentTabHeaderBackgroundChanged(e);
        }

        protected virtual void OnUnselectedDocumentTabHeaderBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != UnselectedDocumentTabHeaderBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SelectedDocumentTabHeaderForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedDocumentTabHeaderForegroundProperty = DependencyProperty.Register("SelectedDocumentTabHeaderForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Khaki, new PropertyChangedCallback(OnSelectedDocumentTabHeaderForegroundPropertyChanged)));

        public Brush SelectedDocumentTabHeaderForeground
        {
            get
            {
                return (Brush)GetValue(SelectedDocumentTabHeaderForegroundProperty);
            }
            set
            {
                if (value != SelectedDocumentTabHeaderForeground)
                {
                    SetValue(SelectedDocumentTabHeaderForegroundProperty, value);
                }
            }
        }

        private static void OnSelectedDocumentTabHeaderForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSelectedDocumentTabHeaderForegroundPropertyChanged(e);
        }

        protected virtual void OnSelectedDocumentTabHeaderForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SelectedDocumentTabHeaderForeground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region UnselectedDocumentTabHeaderForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedDocumentTabHeaderForegroundProperty = DependencyProperty.Register("UnselectedDocumentTabHeaderForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnUnselectedDocumentTabHeaderForegroundChanged)));

        public Brush UnselectedDocumentTabHeaderForeground
        {
            get
            {
                return (Brush)GetValue(UnselectedDocumentTabHeaderForegroundProperty);
            }
            set
            {
                if (value != UnselectedDocumentTabHeaderForeground)
                {
                    SetValue(UnselectedDocumentTabHeaderForegroundProperty, value);
                }
            }
        }

        private static void OnUnselectedDocumentTabHeaderForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnUnselectedDocumentTabHeaderForegroundChanged(e);
        }

        protected virtual void OnUnselectedDocumentTabHeaderForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != UnselectedDocumentTabHeaderForeground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region FloatingDocumentTitleBarBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FloatingDocumentTitleBarBackgroundProperty = DependencyProperty.Register("FloatingDocumentTitleBarBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.Gainsboro, new PropertyChangedCallback(OnFloatingDocumentTitleBarBackgroundChanged)));

        public Brush FloatingDocumentTitleBarBackground
        {
            get
            {
                return (Brush)GetValue(FloatingDocumentTitleBarBackgroundProperty);
            }
            set
            {
                if (value != FloatingDocumentTitleBarBackground)
                {
                    SetValue(FloatingDocumentTitleBarBackgroundProperty, value);
                }
            }
        }

        private static void OnFloatingDocumentTitleBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnFloatingDocumentTitleBarBackgroundChanged(e);
        }

        protected virtual void OnFloatingDocumentTitleBarBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != FloatingDocumentTitleBarBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region FloatingDocumentBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty FloatingDocumentBackgroundProperty = DependencyProperty.Register("FloatingDocumentBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.LightBlue, new PropertyChangedCallback(OnFloatingDocumentBackgroundChanged)));

        public Brush FloatingDocumentBackground
        {
            get
            {
                return (Brush)GetValue(FloatingDocumentBackgroundProperty);
            }
            set
            {
                if (value != FloatingDocumentBackground)
                {
                    SetValue(FloatingDocumentBackgroundProperty, value);
                }
            }
        }

        private static void OnFloatingDocumentBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnFloatingDocumentBackgroundChanged(e);
        }

        protected virtual void OnFloatingDocumentBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != FloatingDocumentBackground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SideToolForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SideToolForegroundProperty = DependencyProperty.Register("SideToolForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnSideToolForegroundChanged)));

        public Brush SideToolForeground
        {
            get
            {
                return (Brush)GetValue(SideToolForegroundProperty);
            }
            set
            {
                if (value != SideToolForeground)
                {
                    SetValue(SideToolForegroundProperty, value);
                    UpdateProperties(_root);
                }
            }
        }

        private static void OnSideToolForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSideToolForegroundChanged(e);
        }

        protected virtual void OnSideToolForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SideToolForeground)
            {
                UpdateProperties(_root);
            }
        }

        #endregion

        #region SideToolBarBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SideToolBarBrushProperty = DependencyProperty.Register("SideToolBarBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnSideToolBarBrushChanged)));

        public Brush SideToolBarBrush
        {
            get
            {
                return (Brush)GetValue(SideToolBarBrushProperty);
            }
            set
            {
                if (value != SideToolBarBrush)
                {
                    SetValue(SideToolBarBrushProperty, value);
                    _leftToolListBox.BarBrushMouseOver = value;
                    _topToolListBox.BarBrushMouseOver = value;
                    _rightToolListBox.BarBrushMouseOver = value;
                    _bottomToolListBox.BarBrushMouseOver = value;
                }
            }
        }

        private static void OnSideToolBarBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnSideToolBarBrushChanged(e);
        }

        protected virtual void OnSideToolBarBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Brush)e.NewValue != SideToolBarBrush)
            {
                UpdateProperties(_root);
            }
        }


        #endregion

        #region MouseOverSideToolBarBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty MouseOverSideToolBarBrushProperty = DependencyProperty.Register("MouseOverSideToolBarBrush", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata(Brushes.White, null));

        public Brush MouseOverSideToolBarBrush
        {
            get
            {
                return (Brush)GetValue(MouseOverSideToolBarBrushProperty);
            }
            set
            {
                if (value != MouseOverSideToolBarBrush)
                {
                    SetValue(MouseOverSideToolBarBrushProperty, value);
                    _leftToolListBox.BarBrushMouseOver = MouseOverSideToolBarBrush;
                    _topToolListBox.BarBrushMouseOver = MouseOverSideToolBarBrush;
                    _rightToolListBox.BarBrushMouseOver = MouseOverSideToolBarBrush;
                    _bottomToolListBox.BarBrushMouseOver = MouseOverSideToolBarBrush;
                }
            }
        }

        #endregion

        #endregion

        private void ProcessMoveResize()
        {
            if (_activeUnpinnedToolPane != null)
            {
                Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
                double left = topLeftPoint.X;
                double top = topLeftPoint.Y;
                switch (_activeToolListBox.WindowLocation)
                {
                    case WindowLocation.TopSide:
                        _activeUnpinnedToolPane.Width = _root.ActualWidth;
                        break;
                    case WindowLocation.BottomSide:
                        top += _root.ActualHeight - _activeUnpinnedToolPane.ActualHeight;
                        _activeUnpinnedToolPane.Width = _root.ActualWidth;
                        break;
                    case WindowLocation.LeftSide:
                        _activeUnpinnedToolPane.Height = _root.ActualHeight;
                        break;
                    case WindowLocation.RightSide:
                        left += _root.ActualWidth - _activeUnpinnedToolPane.ActualWidth;
                        _activeUnpinnedToolPane.Height = _root.ActualHeight;
                        break;
                }
                _activeUnpinnedToolPane.Left = left;
                _activeUnpinnedToolPane.Top = top;
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            ProcessMoveResize();
        }

        private void LayoutManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProcessMoveResize();
        }

        private void CloseUnpinnedToolPane()
        {
            if (_activeUnpinnedToolPane != null)
            {
                UserControl userControl = _activeUnpinnedToolPane.ToolPane.IViewContainer.ExtractUserControl(0);
                _activeToolListBoxItem.IViewContainer.InsertUserControl(_activeToolListBoxItem.Index, userControl);
                _activeUnpinnedToolPane.Close();
                _activeUnpinnedToolPane = null;
                _activeToolListBoxItem = null;
            }
        }

        private UnpinnedToolPane CreateUnpinnedToolPane(ToolListBoxItem toolListBoxItem, WindowLocation windowLocation)
        {
            UnpinnedToolPane unpinnedToolPane = new UnpinnedToolPane();
            UpdateProperties(unpinnedToolPane.ToolPane);

            UserControl userControl = toolListBoxItem.IViewContainer.ExtractUserControl(toolListBoxItem.Index);
            unpinnedToolPane.ToolPane.IViewContainer.AddUserControl(userControl);
            Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
            unpinnedToolPane.Left = topLeftPoint.X;
            unpinnedToolPane.Top = topLeftPoint.Y;
            if ((windowLocation == WindowLocation.TopSide) || (windowLocation == WindowLocation.BottomSide))
            {
                unpinnedToolPane.Width = _root.ActualWidth;
                double height = toolListBoxItem.Height;
                if (height == 0.0)
                {
                    height = _root.ActualHeight / 3;
                }
                unpinnedToolPane.Height = height;
                if (windowLocation == WindowLocation.BottomSide)
                {
                    unpinnedToolPane.Top += _root.ActualHeight - height;
                }
            }
            else
            {
                unpinnedToolPane.Height = _root.ActualHeight;
                double width = toolListBoxItem.Width;
                if (width == 0.0)
                {
                    width = _root.ActualWidth / 3;
                }
                unpinnedToolPane.Width = width;
                if (windowLocation == WindowLocation.RightSide)
                {
                    unpinnedToolPane.Left += _root.ActualWidth - width;
                }
            }
            unpinnedToolPane.Closed += UnpinnedToolPane_Closed;
            unpinnedToolPane.PinClick += UnpinnedToolPane_PinClick;
            unpinnedToolPane.WindowLocation = windowLocation;
            unpinnedToolPane.Owner = Application.Current.MainWindow;

            unpinnedToolPane.Show();

            return unpinnedToolPane;
        }

        private void ToolListBox_ItemClick(object sender, Controls.ItemClickEventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolListBoxItem);
            System.Diagnostics.Trace.Assert((e != null) && (e.ToolListBox != null));

            ToolListBoxItem toolListBoxItem = _activeToolListBoxItem;
            CloseUnpinnedToolPane();

            if (toolListBoxItem == (sender as ToolListBoxItem))
            {
                return;
            }

            _activeToolListBox = e.ToolListBox;
            _activeToolListBoxItem = sender as ToolListBoxItem;
            _activeUnpinnedToolData = _dictUnpinnedToolData[e.ToolListBox.WindowLocation].Where(n => n.Items.Contains(_activeToolListBoxItem)).First();
            _activeUnpinnedToolPane = CreateUnpinnedToolPane(sender as ToolListBoxItem, e.ToolListBox.WindowLocation);
        }

        private void PinToolPane(UnpinnedToolData unpinnedToolData)
        {
            Grid sibling = null;
            if (unpinnedToolData.Sibling == (Guid)this.Tag)
            {
                sibling = this;
            }
            else
            {
                sibling = FindElement(unpinnedToolData.Sibling, this);
            }

            // This can happen when loading a layout
            if (sibling == null)
            {
                sibling = this;
            }

            SplitterPane newSplitterPane = new SplitterPane(unpinnedToolData.IsHorizontal);

            if (unpinnedToolData.Sibling == (Guid)this.Tag)
            {

                IEnumerable<SplitterPane> enumerableSplitterPanes = Children.OfType<SplitterPane>();
                if (enumerableSplitterPanes.Count() == 1)
                {
                    SplitterPane splitterPane = enumerableSplitterPanes.First();

                    SetRootPane(newSplitterPane);
                    newSplitterPane.AddChild(splitterPane, !unpinnedToolData.IsFirst);
                    newSplitterPane.AddChild(unpinnedToolData.ToolPaneGroup, unpinnedToolData.IsFirst);
                }
                else
                {
                    IEnumerable<DocumentPanel> enumerableDocumentPanels = Children.OfType<DocumentPanel>();
                    System.Diagnostics.Trace.Assert(enumerableDocumentPanels.Count() == 1);

                    DocumentPanel documentPanel = enumerableDocumentPanels.First();

                    SetRootPane(newSplitterPane);
                    newSplitterPane.AddChild(documentPanel, !unpinnedToolData.IsFirst);
                    newSplitterPane.AddChild(unpinnedToolData.ToolPaneGroup, unpinnedToolData.IsFirst);
                }
            }
            else if (sibling.Parent == this)
            {
                SetRootPane(newSplitterPane);
                newSplitterPane.AddChild(sibling, !unpinnedToolData.IsFirst);
                newSplitterPane.AddChild(unpinnedToolData.ToolPaneGroup, unpinnedToolData.IsFirst);
            }
            else
            {
                SplitterPane parentSplitterPane = sibling.Parent as SplitterPane;
                int row = Grid.GetRow(sibling);
                int column = Grid.GetColumn(sibling);
                bool isFirst = (parentSplitterPane.IsHorizontal && (row == 0)) || (!parentSplitterPane.IsHorizontal && (column == 0));
                parentSplitterPane.Children.Remove(sibling);

                parentSplitterPane.AddChild(newSplitterPane, isFirst);

                newSplitterPane.AddChild(sibling, !unpinnedToolData.IsFirst);
                newSplitterPane.AddChild(unpinnedToolData.ToolPaneGroup, unpinnedToolData.IsFirst);
            }
        }

        private void UnpinnedToolPane_PinClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolPane == sender);
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolData != null);

            /*
             * Put the view back into its ToolPane  
             */

            UserControl userControl = _activeUnpinnedToolPane.ToolPane.IViewContainer.ExtractUserControl(0);
            _activeToolListBoxItem.IViewContainer.InsertUserControl(_activeToolListBoxItem.Index, userControl);

            /*
             * Restore the pane in the view tree
             */

            PinToolPane(_activeUnpinnedToolData);

            /*
             * Remove the tool list items from the side bar
             */

            List<ToolListBoxItem> toolListBoxItems = _activeUnpinnedToolData.Items;
            IEnumerable<Controls.IToolListBoxItem> iEnumerable = _activeToolListBox.ItemsSource.Except(toolListBoxItems);
            _activeToolListBox.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem>(iEnumerable);

            _dictUnpinnedToolData[_activeToolListBox.WindowLocation].Remove(_activeUnpinnedToolData);
            _activeUnpinnedToolPane.Close();
            _activeUnpinnedToolPane = null;
            _activeToolListBoxItem = null;
            _activeToolListBox = null;
            _activeUnpinnedToolData = null;
        }

        // Warning warning => this should close the tool pane!
        private void UnpinnedToolPane_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolPane == sender);

            _activeToolListBoxItem.Width = (sender as UnpinnedToolPane).ActualWidth;
            _activeToolListBoxItem.Height = (sender as UnpinnedToolPane).ActualHeight;
        }

        private void AddUnpinnedToolData(UnpinnedToolData unpinnedToolData, WindowLocation windowLocation, Controls.ToolListBox ToolListBox)
        {
            _dictUnpinnedToolData[windowLocation].Add(unpinnedToolData);

            int count = unpinnedToolData.ToolPaneGroup.IViewContainer.GetUserControlCount();
            for (int i = 0; i < count; ++i)
            {
                ToolListBoxItem toolListBoxItem = new ToolListBoxItem()
                {
                    IViewContainer = unpinnedToolData.ToolPaneGroup.IViewContainer,
                    Index = i,
                };
                (ToolListBox.ItemsSource as System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem>).Add(toolListBoxItem);
                unpinnedToolData.Items.Add(toolListBoxItem);
            }
        }

        private SelectablePane FindDocumentPanel(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is DocumentPanel)
                {
                    return child as SelectablePane;
                }
                if (child is SplitterPane)
                {
                    SelectablePane selectablePane = FindDocumentPanel(child as SplitterPane);
                    if (selectablePane != null)
                    {
                        return selectablePane;
                    }
                }
            }

            return null;
        }

        private void ToolPane_UnPinClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolPaneGroup);

            DocumentPanel documentPanel = FindDocumentPanel(_root) as DocumentPanel;
            System.Diagnostics.Trace.Assert(documentPanel != null);

            List<Grid> documentPanelAncestors = new List<Grid>();
            Grid grid = documentPanel;
            while (grid.Parent != this)
            {
                grid = grid.Parent as SplitterPane;
                documentPanelAncestors.Add(grid);
            }

            ToolPaneGroup toolPaneGroup = sender as ToolPaneGroup;

            /*
             * Fidn the first common ancestor for the document panel and the tool pane group
             */

            FrameworkElement frameworkElement = toolPaneGroup;
            while (true)
            {
                if (documentPanelAncestors.Contains(frameworkElement.Parent as Grid))
                {
                    break;
                }

                frameworkElement = frameworkElement.Parent as FrameworkElement;
            }

            SplitterPane splitterPane = frameworkElement.Parent as SplitterPane;
            int row = Grid.GetRow(frameworkElement);
            int column = Grid.GetColumn(frameworkElement);
            bool isHorizontal = splitterPane.IsHorizontal;

            Controls.ToolListBox ToolListBox = null;
            WindowLocation windowLocation = WindowLocation.None;
            if (isHorizontal)
            {
                if (row == 0)
                {
                    ToolListBox = _topToolListBox;
                    windowLocation = WindowLocation.TopSide;
                }
                else
                {
                    ToolListBox = _bottomToolListBox;
                    windowLocation = WindowLocation.BottomSide;
                }
            }
            else
            {
                if (column == 0)
                {
                    ToolListBox = _leftToolListBox;
                    windowLocation = WindowLocation.LeftSide;
                }
                else
                {
                    ToolListBox = _rightToolListBox;
                    windowLocation = WindowLocation.RightSide;
                }
            }

            frameworkElement = toolPaneGroup;
            FrameworkElement parentFrameworkElement = toolPaneGroup.Parent as FrameworkElement;

            System.Diagnostics.Trace.Assert(ToolListBox != null);

            toolPaneGroup = sender as ToolPaneGroup;
            ExtractDockPane(toolPaneGroup, out frameworkElement);

            System.Diagnostics.Trace.Assert(frameworkElement != null);

            UnpinnedToolData unpinnedToolData = new UnpinnedToolData();
            unpinnedToolData.ToolPaneGroup = toolPaneGroup;
            unpinnedToolData.IsHorizontal = isHorizontal;
            unpinnedToolData.IsFirst = (row == 0) && (column == 0);
            unpinnedToolData.Sibling = (Guid)((frameworkElement as Grid).Tag);

            AddUnpinnedToolData(unpinnedToolData, windowLocation, ToolListBox);
        }

        private void FrameworkElementRemoved(FrameworkElement frameworkElement)
        {
            foreach (var keyValuePair in _dictUnpinnedToolData)
            {
                foreach (var item in keyValuePair.Value)
                {
                    if (item.Sibling == (Guid)frameworkElement.Tag)
                    {
                        item.Sibling = (Guid)(frameworkElement.Parent as FrameworkElement).Tag;
                    }
                }
            }
        }

        private void CreateToolListBox(out Controls.ToolListBox ToolListBox, int row, int column, bool isHorizontal, WindowLocation windowLocation)
        {
            System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem> items = new System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem>();
            ToolListBox = new Controls.ToolListBox();
            ToolListBox.WindowLocation = windowLocation;
            ToolListBox.Foreground = SideToolForeground;
            ToolListBox.Background = System.Windows.Media.Brushes.Transparent;
            ToolListBox.ItemsSource = items;
            ToolListBox.IsHorizontal = isHorizontal;
            ToolListBox.DisplayMemberPath = "Title";
            ToolListBox.BarBrush = System.Windows.Media.Brushes.Navy;
            ToolListBox.BarBrushMouseOver = System.Windows.Media.Brushes.Crimson;
            ToolListBox.ItemClick += ToolListBox_ItemClick;
            Children.Add(ToolListBox);
            Grid.SetRow(ToolListBox, row);
            Grid.SetColumn(ToolListBox, column);
        }

        private void CreateToolListBoxes()
        {
            CreateToolListBox(out _leftToolListBox, 1, 0, false, WindowLocation.LeftSide);
            CreateToolListBox(out _rightToolListBox, 1, 2, false, WindowLocation.RightSide);
            CreateToolListBox(out _topToolListBox, 0, 1, true, WindowLocation.TopSide);
            CreateToolListBox(out _bottomToolListBox, 2, 1, true, WindowLocation.BottomSide);
        }

        public void Clear()
        {
            Children.Clear();
            CreateToolListBoxes();
            while (FloatingToolPaneGroups.Count > 0)
            {
                FloatingToolPaneGroups[0].Close();
            }
            FloatingToolPaneGroups.Clear();
            while (FloatingDocumentPaneGroups.Count > 0)
            {
                FloatingDocumentPaneGroups[0].Close();
            }
            FloatingDocumentPaneGroups.Clear();
        }

        public List<UserControl> LoadViewsFromTemplates(List<DataTemplate> dataTemplates, System.Collections.IEnumerable viewModels)
        {
            List<UserControl> views = new List<UserControl>();

            if ((dataTemplates == null) || (dataTemplates.Count == 0) || (viewModels == null))
            {
                return views;
            }

            // First load the views and view models

            foreach (var viewModel in viewModels)
            {
                foreach (var item in dataTemplates)
                {
                    if (item is DataTemplate)
                    {
                        DataTemplate dataTemplate = item as DataTemplate;

                        if (viewModel.GetType() == (Type)dataTemplate.DataType)
                        {
                            UserControl view = (dataTemplate.LoadContent() as UserControl);
                            if (view != null)
                            {
                                IView iToolView = (view as IView);
                                System.Diagnostics.Trace.Assert(iToolView != null, "The UserControl must implement interface IView");
                                iToolView.IViewModel = viewModel as IViewModel;
                                view.DataContext = viewModel;
                                view.HorizontalAlignment = HorizontalAlignment.Stretch;
                                view.VerticalAlignment = VerticalAlignment.Stretch;

                                views.Add(view);
                            }
                        }
                    }
                }
            }

            return views;
        }

        private delegate DockPane DelegateCreateDockPane();

        private void AddViews(List<UserControl> views, List<FrameworkElement> list_N, DelegateCreateDockPane createDockPane)
        {
            List<FrameworkElement> list_N_plus_1 = new List<FrameworkElement>();
            bool isHorizontal = false;
            int viewIndex = 1;

            while (viewIndex < views.Count)
            {
                for (int i = 0; (i < list_N.Count) && (viewIndex < views.Count); ++i)
                {
                    SplitterPane splitterPane = new SplitterPane(isHorizontal);

                    var node = list_N[i];
                    System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)node.Parent;
                    (node.Parent as Grid).Children.Remove(node);

                    parentElement.AddChild(splitterPane);
                    Grid.SetRow(splitterPane, Grid.GetRow(node));
                    Grid.SetColumn(splitterPane, Grid.GetColumn(node));

                    splitterPane.AddChild(node, true);

                    list_N_plus_1.Add(node);

                    node = views[viewIndex];
                    DockManager.DockPane dockPane = createDockPane();
                    dockPane.IViewContainer.AddUserControl(node as UserControl);

                    list_N_plus_1.Add(dockPane);

                    splitterPane.AddChild(dockPane, false);

                    ++viewIndex;
                }

                isHorizontal = !isHorizontal;
                list_N = list_N_plus_1;
                list_N_plus_1 = new List<FrameworkElement>();
            }
        }

         #region ILayoutFactory

        private void RegisterDockPane(DockPane dockPane)
        {
            System.Diagnostics.Trace.Assert(dockPane != null);

            dockPane.Close += DockPane_Close;
            dockPane.Float += DockPane_Float;
            dockPane.FloatTabRequest += DockPane_FloatTabRequest;
            dockPane.UngroupCurrent += DockPane_UngroupCurrent;
            dockPane.Ungroup += DockPane_Ungroup;
        }

        DocumentPaneGroup ILayoutFactory.MakeDocumentPaneGroup()
        {
            DocumentPaneGroup documentPaneGroup = new DocumentPaneGroup();
            UpdateProperties(documentPaneGroup);
            RegisterDockPane(documentPaneGroup);
            return documentPaneGroup;
        }

        ToolPaneGroup ILayoutFactory.MakeToolPaneGroup()
        {
            ToolPaneGroup toolPaneGroup = new ToolPaneGroup();
            UpdateProperties(toolPaneGroup);
            RegisterDockPane(toolPaneGroup);
            toolPaneGroup.UnPinClick += ToolPane_UnPinClick;
            return toolPaneGroup;
        }

        private void RegisterFloatingPane(FloatingPane floatingPane)
        {
            floatingPane.LocationChanged += FloatingWindow_LocationChanged;
            floatingPane.Closed += FloatingPane_Closed;
            floatingPane.Ungroup += FloatingPane_Ungroup;
            floatingPane.UngroupCurrent += FloatingPane_UngroupCurrent;
            floatingPane.DataContext = new FloatingViewModel();
            (floatingPane.DataContext as FloatingViewModel).Title = floatingPane.Title;
            floatingPane.EndDrag += FloatingPane_EndDrag;
            // Ensure the window remains on top of the main window
            floatingPane.Owner = Application.Current.MainWindow;
            floatingPane.Show();
        }

        FloatingDocumentPaneGroup ILayoutFactory.MakeFloatingDocumentPaneGroup()
        {
            FloatingDocumentPaneGroup floatingDocumentPaneGroup = new FloatingDocumentPaneGroup();
            UpdateProperties(floatingDocumentPaneGroup);
            RegisterFloatingPane(floatingDocumentPaneGroup);
            FloatingDocumentPaneGroups.Add(floatingDocumentPaneGroup);
            return floatingDocumentPaneGroup;
        }

        FloatingToolPaneGroup ILayoutFactory.MakeFloatingToolPaneGroup()
        {
            FloatingToolPaneGroup floatingToolPaneGroup = new FloatingToolPaneGroup();
            UpdateProperties(floatingToolPaneGroup);
            RegisterFloatingPane(floatingToolPaneGroup);
            FloatingToolPaneGroups.Add(floatingToolPaneGroup);
            return floatingToolPaneGroup;
        }

        Grid FindElement(Guid guid, Grid parentGrid)
        {
            Grid grid = null;

            foreach (var child in parentGrid.Children)
            {
                grid = child as Grid;
                if (grid != null)
                {
                    if ((grid.Tag != null) && ((Guid)grid.Tag == guid))
                    {
                        return grid;
                    }

                    grid = FindElement(guid, grid);
                    if (grid != null)
                    {
                        return grid;
                    }
                }
            }

            return grid;
        }

        void ILayoutFactory.MakeUnpinnedToolPaneGroup(WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst)
        {
            Controls.ToolListBox toolListBox = null;
            switch (windowLocation)
            {
                case WindowLocation.LeftSide:
                    toolListBox = _leftToolListBox;
                    break;
                case WindowLocation.TopSide:
                    toolListBox = _topToolListBox;
                    break;
                case WindowLocation.RightSide:
                    toolListBox = _rightToolListBox;
                    break;
                case WindowLocation.BottomSide:
                    toolListBox = _bottomToolListBox;
                    break;
            }

            UnpinnedToolData unpinnedToolData = new UnpinnedToolData();
            unpinnedToolData.ToolPaneGroup = toolPaneGroup;
            unpinnedToolData.Sibling = new Guid(siblingGuid);
            unpinnedToolData.IsFirst = isFirst;
            unpinnedToolData.IsHorizontal = isHorizontal;

            AddUnpinnedToolData(unpinnedToolData, windowLocation, toolListBox);
        }

        string ILayoutFactory.MakeDocumentKey(string contentId, string Url)
        {
            return contentId + "," + Url;
        }

        void ILayoutFactory.SetRootPane(Grid grid, out int row, out int column)
        {
            SetRootPane(grid);
            row = Grid.GetRow(grid);
            column = Grid.GetColumn(grid);
        }

        #endregion ILayoutFactory

        private void SetRootPane(Grid grid)
        {
            if ((_root != null) && Children.Contains(_root))
            {
                Children.Remove(_root);
            }
            _root = grid;
            Children.Add(_root);
            Grid.SetRow(_root, 1);
            Grid.SetColumn(_root, 1);
        }

        private void Create()
        {
            Clear();

            /*
             * We descend the tree level by level, adding a new level when the current one is full. 
             * We continue adding nodes until we have exhausted the items in views (created above). 
             * 
                    Parent              Level Index
                       G                    1   
                 /           \              
                 G           G              2 
              /     \     /     \           
              D     D     D     D           3
              
                Assume we are building level N where there are potentially 2 ^ (N-1) nodes (denoted by a star). 
                Maintain two node lists. One for level N and one for level N + 1. 
                List for level N is denoted list(N). 
                Assume level N nodes are complete. 
                Then for each item in list(N) we add two child nodes, and then add these child nodes to list (N+1). 

                First level: 

                       D1

                Add a node -> replace D1 with a grid containing two documents and a splitter: 

                       G
                 /           \              
                 D1          D2              

                Add a node -> replace D1 with a grid containing two documents and a splitter: 

                       G
                 /           \              
                 G           D2
              /     \     
              D1    D3     

                Add a node -> replace D2 with a grid containing two documents and a splitter: 

                       G
                 /           \              
                 G           G
              /     \     /    \
              D1    D3    D2    D4

                and so on ... 

                Document panes are children of a dock panel. At first this is a child of the top level 
                splitter pane, or the layout manager if there are no tool panes

             */

            SetRootPane(new SplitterPane(true));

            DocumentPanel documentPanel = new DocumentPanel();
            (_root as SplitterPane).AddChild(documentPanel, true);

            List<UserControl> documentViews = LoadViewsFromTemplates(DocumentTemplates, DocumentsSource);

            if ((documentViews != null) && (documentViews.Count > 0))
            {
                List<FrameworkElement> list_N = new List<FrameworkElement>();

                DockManager.DockPane documentPane = ILayoutFactory.MakeDocumentPaneGroup();
                documentPane.IViewContainer.AddUserControl(documentViews[0]);

                documentPanel.Children.Add(documentPane);
                list_N.Add(documentPane);
                AddViews(documentViews, list_N, delegate { return ILayoutFactory.MakeDocumentPaneGroup(); });
            }

            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);
            if ((toolViews != null) && (toolViews.Count > 0))
            {
                List<FrameworkElement> list_N = new List<FrameworkElement>();

                DockManager.DockPane toolPaneGroup = ILayoutFactory.MakeToolPaneGroup();
                toolPaneGroup.IViewContainer.AddUserControl(toolViews[0]);

                (_root as SplitterPane).AddChild(toolPaneGroup, false);

                list_N.Add(toolPaneGroup);
                AddViews(toolViews, list_N, delegate { return ILayoutFactory.MakeToolPaneGroup(); });
            }

            UpdateLayout();
        }

        public bool SaveLayout(out XmlDocument xmlDocument, string fileNameAndPath)
        {
            xmlDocument = new XmlDocument();

            if (Children.Count == 0)
            {
                return false;
            }

            Serialisation.LayoutWriter.SaveLayout(
                xmlDocument, 
                _root, 
                FloatingToolPaneGroups, 
                FloatingDocumentPaneGroups, 
                _dictUnpinnedToolData);

            xmlDocument.Save(fileNameAndPath);

            return true;
        }

        public bool LoadLayout(out XmlDocument xmlDocument, string fileNameAndPath)
        {
            Clear();

            xmlDocument = new XmlDocument();
            xmlDocument.Load(fileNameAndPath);

            if (xmlDocument.ChildNodes.Count == 0)
            {
                return false;
            }

            List<UserControl> views = new List<UserControl>();
            Dictionary<string, UserControl> viewsMap = new Dictionary<string, UserControl>();

            List<UserControl> documentViews = LoadViewsFromTemplates(DocumentTemplates, DocumentsSource);
            foreach (var item in documentViews)
            {
                viewsMap.Add(ILayoutFactory.MakeDocumentKey(item.Name, ((item.DataContext) as IViewModel).URL), item);
            }

            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);
            foreach (var item in toolViews)
            {
                viewsMap.Add(item.Name, item);
            }

            // Now load the views into the dock manager => one or more views might not be visible!

            Serialisation.LayoutReader.LoadNode(this, viewsMap, this, this, xmlDocument.DocumentElement, true);

            return true;
        }

        /*
         * Remove a dock pane from the tree
         */
        private DockPane ExtractDockPane(DockPane dockPane, out FrameworkElement frameworkElement)
        {
            frameworkElement = null;

            if (dockPane == null)
            {
                return null;
            }

            Grid parentGrid = dockPane.Parent as Grid;
            System.Diagnostics.Trace.Assert(parentGrid != null, System.Reflection.MethodBase.GetCurrentMethod().Name + ": documentPane parent must be a Grid");

            if (parentGrid == this)
            {
                this.Children.Remove(dockPane);
            }
            else
            {
                Grid grandparentGrid = parentGrid.Parent as Grid;
                System.Diagnostics.Trace.Assert(grandparentGrid != null, System.Reflection.MethodBase.GetCurrentMethod().Name + ": Grid parent not a Grid");

                FrameworkElementRemoved(dockPane);
                parentGrid.Children.Remove(dockPane);

                if (!(parentGrid is DocumentPanel))
                {
                    foreach (var item in parentGrid.Children)
                    {
                        if (!(item is GridSplitter))
                        {
                            frameworkElement = item as FrameworkElement;
                            break;
                        }
                    }

                    System.Diagnostics.Trace.Assert(frameworkElement != null);

                    parentGrid.Children.Remove(frameworkElement);
                    int row = Grid.GetRow(parentGrid);
                    int column = Grid.GetColumn(parentGrid);
                    FrameworkElementRemoved(parentGrid);
                    grandparentGrid.Children.Remove(parentGrid);
                    if (grandparentGrid == this)
                    {
                        SetRootPane(frameworkElement as Grid);
                    }
                    else
                    {
                        grandparentGrid.Children.Add(frameworkElement);
                        Grid.SetRow(frameworkElement, row);
                        Grid.SetColumn(frameworkElement, column);
                    }
                }
            }

            return dockPane;
        }

        private bool UngroupDockPane(DockPane dockPane, int index, double paneWidth)
        {
            System.Diagnostics.Trace.Assert(dockPane != null, System.Reflection.MethodBase.GetCurrentMethod().Name + ": dockPane is null");

            int viewCount = dockPane.IViewContainer.GetUserControlCount();
            if (viewCount < 2)
            {
                // Cannot ungroup one item!
                return false;
            }

            // The parent must be a SplitterPane or the LayoutManager
            Grid parentSplitterPane = dockPane.Parent as Grid;
            System.Diagnostics.Trace.Assert(parentSplitterPane != null, System.Reflection.MethodBase.GetCurrentMethod().Name + ": dockPane.Parent not a Grid");

            UserControl userControl = dockPane.IViewContainer.ExtractUserControl(index);
            if (userControl == null)
            {
                return false;
            }

            DockPane newDockPane = (dockPane is ToolPaneGroup) ? (DockPane)ILayoutFactory.MakeToolPaneGroup() : ILayoutFactory.MakeDocumentPaneGroup();
            newDockPane.IViewContainer.AddUserControl(userControl);

            parentSplitterPane.Children.Remove(dockPane);

            SplitterPane newGrid = new SplitterPane(false);
            parentSplitterPane.Children.Add(newGrid);
            Grid.SetRow(newGrid, Grid.GetRow(dockPane));
            Grid.SetColumn(newGrid, Grid.GetColumn(dockPane));

            newGrid.AddChild(dockPane, true);
            newGrid.AddChild(newDockPane, false);

            return true;
        }

        private void DockPane_Ungroup(object sender, EventArgs e)
        {
            DockPane dockPane = sender as DockPane;
            var parentGrid = dockPane.Parent as Grid;

            int count = 1;
            double paneWidth = dockPane.ActualWidth / dockPane.IViewContainer.GetUserControlCount();
            while (UngroupDockPane(dockPane, 1, paneWidth))
            {
                ++count;
                // Nothing here
            }
        }

        private void DockPane_UngroupCurrent(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is DockPane);

            DockPane dockPane = sender as DockPane;

            double paneWidth = dockPane.ActualWidth / 2;
            int index = dockPane.IViewContainer.GetCurrentTabIndex();
            if (index > -1)
            {
                UngroupDockPane(dockPane, index, paneWidth);
            }
        }

        private void Float(DockPane dockPane, FloatEventArgs e, bool selectedTabOnly)
        {
            if (!selectedTabOnly || (dockPane.IViewContainer.GetUserControlCount() == 1))
            {
                ExtractDockPane(dockPane, out FrameworkElement frameworkElement);
            }

            Point mainWindowLocation = Application.Current.MainWindow.PointToScreen(new Point(0, 0));

            FloatingPane floatingPane = null;
            if (dockPane is ToolPaneGroup)
            {
                floatingPane = ILayoutFactory.MakeFloatingToolPaneGroup();
            }
            else
            {
                floatingPane = ILayoutFactory.MakeFloatingDocumentPaneGroup();
            }

            int index = selectedTabOnly ? dockPane.IViewContainer.GetCurrentTabIndex() : 0;
            while (true)
            {
                UserControl userControl = dockPane.IViewContainer.ExtractUserControl(index);
                if (userControl == null)
                {
                    break;
                }

                floatingPane.IViewContainer.AddUserControl(userControl);

                if (selectedTabOnly)
                {
                    break;
                }
            }

            if (e.Drag)
            {
                IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).EnsureHandle();
                WpfOpenControls.Controls.Utilities.SendLeftMouseButtonUp(hWnd);

                // Ensure the floated window can be dragged by the user
                hWnd = new System.Windows.Interop.WindowInteropHelper(floatingPane).EnsureHandle();
                WpfOpenControls.Controls.Utilities.SendLeftMouseButtonDown(hWnd);
           }
            
            Point cursorPositionOnScreen = WpfOpenControls.Controls.Utilities.GetCursorPosition();
            floatingPane.Left = cursorPositionOnScreen.X - 30;
            floatingPane.Top = cursorPositionOnScreen.Y - 30;
            floatingPane.Width = dockPane.ActualWidth;
            floatingPane.Height = dockPane.ActualHeight;
        }

        private void DockPane_Float(object sender, FloatEventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is DockPane);

            Float(sender as DockPane, e, false);
        }

        private void DockPane_FloatTabRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is DockPane);

            FloatEventArgs floatEventArgs = new FloatEventArgs() { Drag = true };
            Float(sender as DockPane, floatEventArgs, true);
        }

        private void DockPane_Close(object sender, EventArgs e)
        {
            DockPane dockPane = sender as DockPane;

            if (dockPane == null)
            {
                return;
            }

            ExtractDockPane(dockPane, out FrameworkElement frameworkElement);

            if (dockPane is DocumentPaneGroup)
            {
                DocumentClosed?.Invoke(sender, null);
            }
            else if (dockPane is ToolPaneGroup)
            {
                ToolClosed?.Invoke(sender, null);
            }
        }

        private void FloatingPane_Closed(object sender, EventArgs e)
        {
            if ((sender is FloatingToolPaneGroup) && (FloatingToolPaneGroups.Contains(sender as FloatingToolPaneGroup)))
            {
                FloatingToolPaneGroups.Remove(sender as FloatingToolPaneGroup);
            }

            if ((sender is FloatingDocumentPaneGroup) && (FloatingDocumentPaneGroups.Contains(sender as FloatingDocumentPaneGroup)))
            {
                FloatingDocumentPaneGroups.Remove(sender as FloatingDocumentPaneGroup);
            }
        }

        private FloatingPane UnGroupFloatingPane(FloatingPane floatingPane, int index, double left, double top)
        {
            if (floatingPane == null)
            {
                return null;
            }

            UserControl userControl = floatingPane.IViewContainer.ExtractUserControl(index);
            if (userControl == null)
            {
                return null;
            }

            FloatingPane newFloatingPane = null;
            if (floatingPane is FloatingToolPaneGroup)
            {
                newFloatingPane = ILayoutFactory.MakeFloatingToolPaneGroup();
            }
            else
            {
                newFloatingPane = ILayoutFactory.MakeFloatingDocumentPaneGroup();
            }

            newFloatingPane.Left = left;
            newFloatingPane.Top = top;
            newFloatingPane.IViewContainer.AddUserControl(userControl);

            return floatingPane;
        }

        private void FloatingPane_Ungroup(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is FloatingPane);

            FloatingPane floatingPane = sender as FloatingPane;

            int viewCount = floatingPane.IViewContainer.GetUserControlCount();

            double left = floatingPane.Left;
            double top = floatingPane.Top;

            // Warning warning => off screen?
            for (int index = 1; index < viewCount; ++index)
            {
                left += 10;
                top += 10;
                if (UnGroupFloatingPane(floatingPane, 1, left, top) == null)
                {
                    return;
                }
            }
        }

        private void FloatingPane_UngroupCurrent(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is FloatingPane);

            FloatingPane floatingPane = sender as FloatingPane;

            int index = floatingPane.IViewContainer.GetCurrentTabIndex();
            if (index > -1)
            {
                UnGroupFloatingPane(floatingPane, index, floatingPane.Left + 10, floatingPane.Top + 10);
            }
        }

        private void CancelSelection()
        {
            if (SelectedPane != null)
            {
                SelectedPane.IsHighlighted = false;
                SelectedPane = null;
            }
            _insertionIndicatorManager?.HideInsertionIndicator();
            _windowLocationPane?.Close();
            _windowLocationPane = null;
            _sideLocationPane?.Close();
            _sideLocationPane = null;
        }

        private void ExtractDocuments(FloatingPane floatingPane, DockPane dockPane)
        {
            while (true)
            {
                UserControl userControl = floatingPane.IViewContainer.ExtractUserControl(0);
                if (userControl == null)
                {
                    break;
                }

                dockPane.IViewContainer.AddUserControl(userControl);
            }
            floatingPane.Close();
        }

        private void FloatingPane_EndDrag(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is FloatingPane, System.Reflection.MethodBase.GetCurrentMethod().Name + ": sender not a FloatingPane");
            Application.Current.Dispatcher.Invoke(delegate
            {
                FloatingPane floatingPane = sender as FloatingPane;

                if (
                        (floatingPane == null) ||
                        ((SelectedPane != null) && !(SelectedPane.Parent is SplitterPane) && !(SelectedPane.Parent is DocumentPanel) && (SelectedPane.Parent != this)) ||
                        (_insertionIndicatorManager == null) ||
                        (_insertionIndicatorManager.WindowLocation == WindowLocation.None)
                   )
                {
                    return;
                }

                SplitterPane parentSplitterPane = null;
                DockPane dockPane = null;
                SelectablePane selectedPane = SelectedPane;
                WindowLocation windowLocation = _insertionIndicatorManager.WindowLocation;
                CancelSelection();

                switch (windowLocation)
                {
                    case WindowLocation.BottomSide:
                    case WindowLocation.TopSide:
                    case WindowLocation.LeftSide:
                    case WindowLocation.RightSide:

                        if (sender is FloatingToolPaneGroup)
                        {
                            dockPane = ILayoutFactory.MakeToolPaneGroup();
                        }
                        else
                        {
                            dockPane = ILayoutFactory.MakeDocumentPaneGroup();
                        }
                        ExtractDocuments(floatingPane, dockPane);

                        parentSplitterPane = new SplitterPane((windowLocation == WindowLocation.TopSide) || (windowLocation == WindowLocation.BottomSide));
                        bool isFirst = (windowLocation == WindowLocation.TopSide) || (windowLocation == WindowLocation.LeftSide);
                        parentSplitterPane.AddChild(dockPane, isFirst);

                        if (Children.Count == 0)
                        {
                            Children.Add(parentSplitterPane);
                        }
                        else
                        {
                            Grid rootPane = _root;
                            SetRootPane(parentSplitterPane);
                            parentSplitterPane.AddChild(rootPane, !isFirst);
                        }
                        break;

                    case WindowLocation.Right:
                    case WindowLocation.Left:
                    case WindowLocation.Top:
                    case WindowLocation.Bottom:

                        if (sender is FloatingToolPaneGroup)
                        {
                            dockPane = ILayoutFactory.MakeToolPaneGroup();
                        }
                        else
                        {
                            dockPane = ILayoutFactory.MakeDocumentPaneGroup();
                        }
                        ExtractDocuments(floatingPane, dockPane);

                        SplitterPane newGrid = new SplitterPane((windowLocation == WindowLocation.Top) || (windowLocation == WindowLocation.Bottom));

                        if (selectedPane.Parent is DocumentPanel)
                        {
                            DocumentPanel documentPanel = selectedPane.Parent as DocumentPanel;
                            documentPanel.Children.Remove(selectedPane);
                            documentPanel.Children.Add(newGrid);
                        }
                        else
                        {
                            parentSplitterPane = (selectedPane.Parent as SplitterPane);
                            parentSplitterPane.Children.Remove(selectedPane);
                            parentSplitterPane.Children.Add(newGrid);
                            Grid.SetRow(newGrid, Grid.GetRow(selectedPane));
                            Grid.SetColumn(newGrid, Grid.GetColumn(selectedPane));
                        }

                        bool isTargetFirst = (windowLocation == WindowLocation.Right) || (windowLocation == WindowLocation.Bottom);
                        newGrid.AddChild(selectedPane, isTargetFirst);
                        newGrid.AddChild(dockPane, !isTargetFirst);
                        break;

                    case WindowLocation.Middle:

                        if (selectedPane is DockPane)
                        {
                            ExtractDocuments(floatingPane, selectedPane as DockPane);
                        }
                        else if (selectedPane is DocumentPanel)
                        {
                            DocumentPaneGroup documentPaneGroup = ILayoutFactory.MakeDocumentPaneGroup();
                            selectedPane.Children.Add(documentPaneGroup);
                            ExtractDocuments(floatingPane, documentPaneGroup);
                        }
                        break;
                }

                Application.Current.MainWindow.Activate();
            });
        }

        /*
         * Locates the deepest SelectablePane in the tree at the specified on screen point
         */
        private SelectablePane FindSelectablePane(Grid grid, Point pointOnScreen)
        {
            if (grid == null)
            {
                return null;
            }

            foreach (var child in grid.Children)
            {
                if ((child is SelectablePane) || (child is SplitterPane))
                {
                    Grid childGrid = child as Grid;
                    Point pointInToolPane = childGrid.PointFromScreen(pointOnScreen);
                    if (
                            (pointInToolPane.X >= 0) &&
                            (pointInToolPane.X <= childGrid.ActualWidth) &&
                            (pointInToolPane.Y >= 0) &&
                            (pointInToolPane.Y <= childGrid.ActualHeight)
                        )
                    {
                        if (child is DocumentPanel)
                        {
                            if (!(child as DocumentPanel).ContainsDocuments())
                            {
                                return child as DocumentPanel;
                            }
                        }
                        else if (child is DockPane)
                        {
                            return child as DockPane;
                        }

                        return FindSelectablePane(childGrid, pointOnScreen);
                    }
                }
            }

            return null;
        }

        private WindowLocationPane _windowLocationPane;
        private SideLocationPane _sideLocationPane;
        private InsertionIndicatorManager _insertionIndicatorManager;

        private void FloatingWindow_LocationChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is Window);

            Window floatingWindow = sender as Window;
            Point cursorPositionOnScreen = WpfOpenControls.Controls.Utilities.GetCursorPosition();

            bool foundSelectedPane = false;
            Point cursorPositionInMainWindow = PointFromScreen(cursorPositionOnScreen);
            if (
                    (cursorPositionInMainWindow.X >= 0) &&
                    (cursorPositionInMainWindow.X <= this.ActualWidth) && 
                    (cursorPositionInMainWindow.Y >= 0) &&
                    (cursorPositionInMainWindow.Y <= this.ActualHeight) 
                )
            {
                Type paneType = (sender is FloatingDocumentPaneGroup) ? typeof(DocumentPaneGroup) : typeof(ToolPaneGroup);
                var pane = FindSelectablePane(this, cursorPositionOnScreen);
                foundSelectedPane = pane != null;
                if ((pane != null) && (SelectedPane != pane))
                {
                    if (SelectedPane != null)
                    {
                        SelectedPane.IsHighlighted = false;
                    }

                    pane.IsHighlighted = true;
                    SelectedPane = pane;
                    if (_windowLocationPane != null)
                    {
                        _windowLocationPane.Close();
                        _windowLocationPane = null;
                    }

                    if ((paneType == pane.GetType()) || ((pane is DocumentPanel) && (sender is FloatingDocumentPaneGroup)))
                    {
                        _windowLocationPane = new WindowLocationPane();
                        _windowLocationPane.AllowsTransparency = true;
                        if (SelectedPane is DocumentPanel)
                        {
                            _windowLocationPane.ShowIcons(WindowLocation.Middle);
                        }
                        Point topLeftPanePoint = pane.PointToScreen(new Point(0, 0));
                        _windowLocationPane.Left = topLeftPanePoint.X;
                        _windowLocationPane.Top = topLeftPanePoint.Y;
                        _windowLocationPane.Width = SelectedPane.ActualWidth;
                        _windowLocationPane.Height = SelectedPane.ActualHeight;
                        _windowLocationPane.Show();
                    }
                }

                if (sender is FloatingToolPaneGroup)
                {
                    if (_sideLocationPane == null)
                    {
                        _sideLocationPane = new SideLocationPane();
                        _sideLocationPane.AllowsTransparency = true;
                    }

                    Point topLeftRootPoint = _root.PointToScreen(new Point(0, 0));
                    _sideLocationPane.Left = topLeftRootPoint.X;
                    _sideLocationPane.Top = topLeftRootPoint.Y;
                    _sideLocationPane.Width = _root.ActualWidth;
                    _sideLocationPane.Height = _root.ActualHeight;
                    _sideLocationPane.Show();
                }
            }
            else
            {
                if (_sideLocationPane != null)
                {
                    _sideLocationPane.Close();
                    _sideLocationPane = null;
                }
            }

            SelectablePane previousPane = SelectedPane;

            if (!foundSelectedPane)
            {
                SelectedPane = null;
            }

            if (!foundSelectedPane)
            {
                if (_windowLocationPane != null)
                {
                    _windowLocationPane.Close();
                    _windowLocationPane = null;
                }
           }

            if ((previousPane != null) && (SelectedPane != previousPane))
            {
                previousPane.IsHighlighted = false;
            }

            WindowLocation windowLocation = WindowLocation.None;

            if (_sideLocationPane != null)
            {
                windowLocation = _sideLocationPane.TrySelectIndicator(cursorPositionOnScreen);
                switch (windowLocation)
                {
                    case WindowLocation.LeftSide:
                    case WindowLocation.RightSide:
                    case WindowLocation.TopSide:
                    case WindowLocation.BottomSide:
                        if ((_insertionIndicatorManager != null) && (_insertionIndicatorManager.ParentGrid != this))
                        {
                            _insertionIndicatorManager.HideInsertionIndicator();
                            _insertionIndicatorManager = null;
                        }
                        if (_insertionIndicatorManager == null)
                        {
                            _insertionIndicatorManager = new InsertionIndicatorManager(this);
                        }
                        _insertionIndicatorManager.ShowInsertionIndicator(windowLocation);
                        return;
                }
            }

            if ((_windowLocationPane != null) && (SelectedPane != null))
            {
                windowLocation = _windowLocationPane.TrySelectIndicator(cursorPositionOnScreen);
                if ((_insertionIndicatorManager != null) && (_insertionIndicatorManager.ParentGrid != SelectedPane))
                {
                    _insertionIndicatorManager.HideInsertionIndicator();
                    _insertionIndicatorManager = null;
                }
                if (_insertionIndicatorManager == null)
                {
                    _insertionIndicatorManager = new InsertionIndicatorManager(SelectedPane);
                }

                _insertionIndicatorManager.ShowInsertionIndicator(windowLocation);
            }
        }
    }
}
