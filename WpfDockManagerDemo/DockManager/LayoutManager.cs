using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;
using WpfControlLibrary;

namespace WpfDockManagerDemo.DockManager
{
    public class LayoutManager : System.Windows.Controls.Grid, ILayoutFactory
    {
        public LayoutManager()
        {
            FloatingTools = new List<FloatingTool>();
            FloatingDocuments = new List<FloatingDocument>();

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Auto) });
            CreateToolLists();

            Background = System.Windows.Media.Brushes.LightBlue;

            App.Current.MainWindow.LocationChanged += MainWindow_LocationChanged;
            PreviewMouseDown += LayoutManager_PreviewMouseDown;

            _dictionaryUnpinnedToolData = new Dictionary<UnpinnedToolData, List<ToolListItem>>();
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
                HideSideToolPane();
            }
        }

        public LayoutManager(Controls.ToolListControl rightPane)
        {
            _rightToolList = rightPane;
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
            HideSideToolPane();

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

        internal List<FloatingTool> FloatingTools;
        internal List<FloatingDocument> FloatingDocuments;

        internal Controls.ToolListControl _leftToolList;
        internal Controls.ToolListControl _topToolList;
        internal Controls.ToolListControl _rightToolList;
        internal Controls.ToolListControl _bottomToolList;

        private Dictionary<UnpinnedToolData, List<ToolListItem>> _dictionaryUnpinnedToolData;

        internal ToolListItem _activeToolListItem;
        internal UnpinnedToolPane _activeUnpinnedToolPane;

        private UnpinnedToolData GetUnpinnedToolData(ToolListItem toolListItem)
        {
            foreach (var keyValuePair in _dictionaryUnpinnedToolData)
            {
                foreach (var item in keyValuePair.Value)
                {
                    if (item == toolListItem)
                    {
                        return keyValuePair.Key;
                    }
                }
            }

            return null;
        }

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
                // Warning warning
                //CreateControl();
            }
        }

        #endregion

        #endregion

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (_activeUnpinnedToolPane != null)
            {
                Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
                double left = topLeftPoint.X;
                double top = topLeftPoint.Y;
                switch (_activeToolListItem.WindowLocation)
                {
                    case WindowLocation.BottomSide:
                        top += _root.ActualHeight - _activeUnpinnedToolPane.ActualHeight;
                        break;
                    case WindowLocation.RightSide:
                        left += _root.ActualWidth - _activeUnpinnedToolPane.ActualWidth;
                        break;
                }
                _activeUnpinnedToolPane.Left = left;
                _activeUnpinnedToolPane.Top = top;
            }
        }

        private void CreateToolList(out Controls.ToolListControl toolListControl, int row, int column, bool isHorizontal)
        {
            System.Collections.ObjectModel.ObservableCollection<Controls.IToolListItem> items = new System.Collections.ObjectModel.ObservableCollection<Controls.IToolListItem>();
            toolListControl = new Controls.ToolListControl();
            toolListControl.Foreground = System.Windows.Media.Brushes.Black;
            toolListControl.Background = System.Windows.Media.Brushes.Transparent;
            toolListControl.ItemsSource = items;
            toolListControl.IsHorizontal = isHorizontal;
            toolListControl.DisplayMemberPath = "Title";
            toolListControl.BarBrush = System.Windows.Media.Brushes.DarkSlateBlue;
            toolListControl.BarBrushMouseOver = System.Windows.Media.Brushes.Orange;
            toolListControl.ItemClick += ToolListControl_ItemClick;
            Children.Add(toolListControl);
            Grid.SetRow(toolListControl, row);
            Grid.SetColumn(toolListControl, column);
        }
        
        private void HideSideToolPane()
        {
            if (_activeUnpinnedToolPane != null)
            {
                UserControl userControl = _activeUnpinnedToolPane.ToolPane.IViewContainer.ExtractUserControl(0);
                _activeToolListItem.IViewContainer.InsertUserControl(_activeToolListItem.Index, userControl);
                _activeUnpinnedToolPane.Close();
                _activeUnpinnedToolPane = null;
                _activeToolListItem = null;
            }
        }

        // Warning warning => move to ToolListItem
        private void ToolListControl_ItemClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolListItem);

            ToolListItem toolListItem = _activeToolListItem;
            HideSideToolPane();

            if (toolListItem == (sender as ToolListItem))
            {
                return;
            }

            _activeToolListItem = sender as ToolListItem;

            _activeUnpinnedToolPane = new UnpinnedToolPane();
            UserControl userControl = _activeToolListItem.IViewContainer.ExtractUserControl(_activeToolListItem.Index);
            _activeUnpinnedToolPane.ToolPane.IViewContainer.AddUserControl(userControl);
            Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
            _activeUnpinnedToolPane.Left = topLeftPoint.X;
            _activeUnpinnedToolPane.Top = topLeftPoint.Y;
            if ((_activeToolListItem.WindowLocation == WindowLocation.TopSide) || (_activeToolListItem.WindowLocation == WindowLocation.BottomSide))
            {
                _activeUnpinnedToolPane.Width = _root.ActualWidth;
                double height = _activeToolListItem.Height;
                if (height == 0.0)
                {
                    height = _root.ActualHeight / 3;
                }
                _activeUnpinnedToolPane.Height = height;
                if (_activeToolListItem.WindowLocation == WindowLocation.BottomSide)
                {
                    _activeUnpinnedToolPane.Top += _root.ActualHeight - height;
                }
            }
            else
            {
                _activeUnpinnedToolPane.Height = _root.ActualHeight;
                double width = _activeToolListItem.Width;
                if (width == 0.0)
                {
                    width = _root.ActualWidth / 3;
                }
                _activeUnpinnedToolPane.Width = width;
                if (_activeToolListItem.WindowLocation == WindowLocation.RightSide)
                {
                    _activeUnpinnedToolPane.Left += _root.ActualWidth - width;
                }
            }
            _activeUnpinnedToolPane.Closed += _displayedSideToolPane_Closed;
            _activeUnpinnedToolPane.PinClick += _displayedSideToolPane_PinClick;
            _activeUnpinnedToolPane.WindowLocation = _activeToolListItem.WindowLocation;
            _activeUnpinnedToolPane.Owner = App.Current.MainWindow;

            _activeUnpinnedToolPane.Show();
        }

        private void PinToolPane(UnpinnedToolData unpinnedToolData)
        {
            if (unpinnedToolData.Sibling == this)
            {
                bool isHorizontal = (_activeToolListItem.WindowLocation == WindowLocation.TopSide) || (_activeToolListItem.WindowLocation == WindowLocation.BottomSide);

                SplitterPane newSplitterPane = new SplitterPane(isHorizontal);
                // Could be a splitter and a document panel ... 
                IEnumerable<SplitterPane> enumerableSplitterPanes = Children.OfType<SplitterPane>();
                if (enumerableSplitterPanes.Count() == 1)
                {
                    SplitterPane parentSplitterPane = enumerableSplitterPanes.First();

                    IEnumerable<ToolPane> enumerableToolPanes = parentSplitterPane.Children.OfType<ToolPane>();

                    ToolPane toolPane = enumerableToolPanes.First();
                    parentSplitterPane.Children.Remove(toolPane);

                    bool isFirst = (_activeToolListItem.WindowLocation == WindowLocation.TopSide) || (_activeToolListItem.WindowLocation == WindowLocation.LeftSide);
                    newSplitterPane.AddChild(toolPane, !isFirst);
                    newSplitterPane.AddChild(unpinnedToolData.ToolPane, isFirst);

                    parentSplitterPane.AddChild(newSplitterPane, isFirst);
                }
                else
                {
                    IEnumerable<DocumentPanel> enumerableDocumentPanels = Children.OfType<DocumentPanel>();
                    System.Diagnostics.Trace.Assert(enumerableDocumentPanels.Count() == 1);

                    DocumentPanel documentPanel = enumerableDocumentPanels.First();

                    SetRootPane(newSplitterPane);
                    newSplitterPane.AddChild(documentPanel, !unpinnedToolData.IsFirst);
                    newSplitterPane.AddChild(unpinnedToolData.ToolPane, unpinnedToolData.IsFirst);
                }
            }
            else if (unpinnedToolData.Sibling.Parent == this)
            {
                bool isHorizontal = (_activeToolListItem.WindowLocation == WindowLocation.TopSide) || (_activeToolListItem.WindowLocation == WindowLocation.BottomSide);
                bool isFirst = (_activeToolListItem.WindowLocation == WindowLocation.TopSide) || (_activeToolListItem.WindowLocation == WindowLocation.LeftSide);

                SplitterPane newSplitterPane = new SplitterPane(isHorizontal);
                SetRootPane(newSplitterPane);
                newSplitterPane.AddChild(unpinnedToolData.Sibling, !isFirst);
                newSplitterPane.AddChild(unpinnedToolData.ToolPane, isFirst);
            }
            else
            {
                SplitterPane newSplitterPane = new SplitterPane(unpinnedToolData.IsHorizontal);
                SplitterPane parentSplitterPane = unpinnedToolData.Sibling.Parent as SplitterPane;
                int row = Grid.GetRow(unpinnedToolData.Sibling);
                int column = Grid.GetColumn(unpinnedToolData.Sibling);
                bool isFirst = (parentSplitterPane.IsHorizontal && (row == 0)) || (!parentSplitterPane.IsHorizontal && (column == 0));
                parentSplitterPane.Children.Remove(unpinnedToolData.Sibling);

                parentSplitterPane.AddChild(newSplitterPane, isFirst);

                newSplitterPane.AddChild(unpinnedToolData.Sibling, !unpinnedToolData.IsFirst);
                newSplitterPane.AddChild(unpinnedToolData.ToolPane, unpinnedToolData.IsFirst);
            }
        }

        private void _displayedSideToolPane_PinClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolPane == sender);

            /*
             * Put the view back into its ToolPane  
             */

            UnpinnedToolData unpinnedToolData = GetUnpinnedToolData(_activeToolListItem);

            System.Diagnostics.Trace.Assert(unpinnedToolData != null);

            UserControl userControl = _activeUnpinnedToolPane.ToolPane.IViewContainer.ExtractUserControl(0);
            _activeToolListItem.IViewContainer.InsertUserControl(_activeToolListItem.Index, userControl);

            /*
             * Restore the pane in the view tree
             */

            PinToolPane(unpinnedToolData);

            System.Diagnostics.Trace.Assert(_dictionaryUnpinnedToolData.ContainsKey(unpinnedToolData));

            /*
             * Remove the tool list items from the side bar
             */

            List<ToolListItem> toolListItems = _dictionaryUnpinnedToolData[unpinnedToolData];
            IEnumerable<Controls.IToolListItem> iEnumerable = unpinnedToolData.ToolListControl.ItemsSource.Except(toolListItems);
            unpinnedToolData.ToolListControl.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<Controls.IToolListItem>(iEnumerable);

            _activeUnpinnedToolPane.Close();
            _activeUnpinnedToolPane = null;
            _activeToolListItem = null;
        }

        // Warning warning => this should close the tool pane!
        private void _displayedSideToolPane_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolPane == sender);

            _activeToolListItem.Width = (sender as UnpinnedToolPane).ActualWidth;
            _activeToolListItem.Height = (sender as UnpinnedToolPane).ActualHeight;
        }

        private void ToolPane_UnPinClick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is ToolPane);

            Controls.ToolListControl toolListControl = null;
            ToolPane toolPane = sender as ToolPane;
            FrameworkElement frameworkElement = toolPane;
            FrameworkElement parentFrameworkElement = toolPane.Parent as FrameworkElement;
            SplitterPane parentSplitterPane = toolPane.Parent as SplitterPane;
            WindowLocation windowLocation = WindowLocation.None;
            while (true)
            {
                System.Diagnostics.Trace.Assert(parentFrameworkElement is SplitterPane);

                if (parentFrameworkElement.Parent == this)
                {
                    SplitterPane splitterPane = parentFrameworkElement as SplitterPane;
                    if (splitterPane.IsHorizontal)
                    {
                        if (Grid.GetRow(frameworkElement) == 0)
                        {
                            toolListControl = _topToolList;
                            windowLocation = WindowLocation.TopSide;
                        }
                        else
                        {
                            toolListControl = _bottomToolList;
                            windowLocation = WindowLocation.BottomSide;
                        }
                    }
                    else
                    {
                        if (Grid.GetColumn(frameworkElement) == 0)
                        {
                            toolListControl = _leftToolList;
                            windowLocation = WindowLocation.LeftSide;
                        }
                        else
                        {
                            toolListControl = _rightToolList;
                            windowLocation = WindowLocation.RightSide;
                        }
                    }
                    break;
                }

                frameworkElement = parentFrameworkElement;
                parentFrameworkElement = parentFrameworkElement.Parent as FrameworkElement;
            }

            System.Diagnostics.Trace.Assert(toolListControl != null);

            toolPane = sender as ToolPane;
            ExtractDockPane(toolPane, out frameworkElement);

            System.Diagnostics.Trace.Assert(frameworkElement != null);

            UnpinnedToolData unpinnedToolData = new UnpinnedToolData();
            unpinnedToolData.ToolListControl = toolListControl;
            unpinnedToolData.ToolPane = toolPane;
            unpinnedToolData.IsHorizontal = parentSplitterPane.IsHorizontal;
            unpinnedToolData.IsFirst =
                (parentSplitterPane.IsHorizontal && (Grid.GetRow(toolPane) == 0)) ||
                (!parentSplitterPane.IsHorizontal && (Grid.GetColumn(toolPane) == 0));
            unpinnedToolData.Sibling = frameworkElement;

            List<ToolListItem> listTooListItem = new List<ToolListItem>();
            _dictionaryUnpinnedToolData.Add(unpinnedToolData, listTooListItem);

            int count = toolPane.IViewContainer.GetUserControlCount();
            for (int i = 0; i < count; ++i)
            {
                ToolListItem toolListItem = new ToolListItem()
                {
                    IViewContainer = unpinnedToolData.ToolPane.IViewContainer,
                    Index = i,
                    WindowLocation = windowLocation
                };
                (toolListControl.ItemsSource as System.Collections.ObjectModel.ObservableCollection<Controls.IToolListItem>).Add(toolListItem);
                listTooListItem.Add(toolListItem);
            }
        }

        private void FrameworkElementRemoved(FrameworkElement frameworkElement)
        {
            foreach (var keyValuePair in _dictionaryUnpinnedToolData)
            {
                if (keyValuePair.Key.Sibling == frameworkElement)
                {
                    keyValuePair.Key.Sibling = frameworkElement.Parent as FrameworkElement;
                }
            }
        }

        private void CreateToolLists()
        {
            CreateToolList(out _leftToolList, 1, 0, false);
            CreateToolList(out _rightToolList, 1, 2, false);
            CreateToolList(out _topToolList, 0, 1, true);
            CreateToolList(out _bottomToolList, 2, 1, true);
        }

        public void Clear()
        {
            Children.Clear();
            CreateToolLists();
            while (FloatingTools.Count > 0)
            {
                FloatingTools[0].Close();
            }
            FloatingTools.Clear();
            while (FloatingDocuments.Count > 0)
            {
                FloatingDocuments[0].Close();
            }
            FloatingDocuments.Clear();
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
            dockPane.UngroupCurrent += DockPane_UngroupCurrent;
            dockPane.Ungroup += DockPane_Ungroup;
        }

        DocumentPane ILayoutFactory.CreateDocumentPane()
        {
            DocumentPane documentPane = new DocumentPane();
            RegisterDockPane(documentPane);
            return documentPane;
        }

        ToolPane ILayoutFactory.CreateToolPane()
        {
            ToolPane toolPane = new ToolPane();
            RegisterDockPane(toolPane);
            toolPane.UnPinClick += ToolPane_UnPinClick;
            return toolPane;
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
            floatingPane.Owner = App.Current.MainWindow;
            floatingPane.Show();
        }

        FloatingDocument ILayoutFactory.CreateFloatingDocument()
        {
            FloatingDocument floatingDocument = new FloatingDocument();
            RegisterFloatingPane(floatingDocument);
            FloatingDocuments.Add(floatingDocument);
            return floatingDocument;
        }

        FloatingTool ILayoutFactory.CreateFloatingTool()
        {
            FloatingTool floatingTool = new FloatingTool();
            RegisterFloatingPane(floatingTool);
            FloatingTools.Add(floatingTool);
            return floatingTool;
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

                DockManager.DockPane documentPane = ILayoutFactory.CreateDocumentPane();
                documentPane.IViewContainer.AddUserControl(documentViews[0]);

                documentPanel.Children.Add(documentPane);
                list_N.Add(documentPane);
                AddViews(documentViews, list_N, delegate { return ILayoutFactory.CreateDocumentPane(); });
            }

            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);
            if ((toolViews != null) && (toolViews.Count > 0))
            {
                List<FrameworkElement> list_N = new List<FrameworkElement>();

                DockManager.DockPane toolPane = ILayoutFactory.CreateToolPane();
                toolPane.IViewContainer.AddUserControl(toolViews[0]);

                (_root as SplitterPane).AddChild(toolPane, false);

                list_N.Add(toolPane);
                AddViews(toolViews, list_N, delegate { return ILayoutFactory.CreateToolPane(); });
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

            Serialisation.LayoutWriter.SaveLayout(xmlDocument, _root, FloatingTools);

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
                viewsMap.Add(item.Name, item);
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

            DockPane newDockPane = (dockPane is ToolPane) ? (DockPane)ILayoutFactory.CreateToolPane() : ILayoutFactory.CreateDocumentPane();
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

        private void DockPane_Float(object sender, FloatEventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is DockPane);

            DockPane dockPane = sender as DockPane;

            Point cursorPositionOnScreen = WpfControlLibrary.Utilities.GetCursorPosition();
            Point cursorPositionInMainWindow = App.Current.MainWindow.PointFromScreen(cursorPositionOnScreen);
            Point cursorPositionInToolPane = dockPane.PointFromScreen(cursorPositionOnScreen);

            Point mainWindowLocation = App.Current.MainWindow.PointToScreen(new Point(0, 0));

            ExtractDockPane(dockPane, out FrameworkElement frameworkElement);

            FloatingPane floatingPane = null;
            if (dockPane is ToolPane)
            {
                floatingPane = ILayoutFactory.CreateFloatingTool();
            }
            else
            {
                floatingPane = ILayoutFactory.CreateFloatingDocument();
            }

            while (true)
            {
                UserControl userControl = dockPane.IViewContainer.ExtractUserControl(0);
                if (userControl == null)
                {
                    break;
                }

                floatingPane.IViewContainer.AddUserControl(userControl);
            }

            floatingPane.Left = mainWindowLocation.X + cursorPositionInMainWindow.X - cursorPositionInToolPane.X;
            floatingPane.Top = mainWindowLocation.Y + cursorPositionInMainWindow.Y - cursorPositionInToolPane.Y;
            floatingPane.Width = dockPane.ActualWidth;
            floatingPane.Height = dockPane.ActualHeight;

            if (e.Drag)
            {
                // Ensure the floated window can be dragged by the user
                IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(floatingPane).EnsureHandle();
                WpfControlLibrary.Utilities.SendLeftMouseButtonDown(hWnd);
            }
        }

        private void DockPane_Close(object sender, EventArgs e)
        {
            DockPane dockPane = sender as DockPane;

            if (dockPane == null)
            {
                return;
            }

            ExtractDockPane(dockPane, out FrameworkElement frameworkElement);

            if (dockPane is DocumentPane)
            {
                DocumentClosed?.Invoke(sender, null);
            }
            else if (dockPane is ToolPane)
            {
                ToolClosed?.Invoke(sender, null);
            }
        }

        private void FloatingPane_Closed(object sender, EventArgs e)
        {
            if ((sender is FloatingTool) && (FloatingTools.Contains(sender as FloatingTool)))
            {
                FloatingTools.Remove(sender as FloatingTool);
            }

            if ((sender is FloatingDocument) && (FloatingDocuments.Contains(sender as FloatingDocument)))
            {
                FloatingDocuments.Remove(sender as FloatingDocument);
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
            if (floatingPane is FloatingTool)
            {
                newFloatingPane = ILayoutFactory.CreateFloatingTool();
            }
            else
            {
                newFloatingPane = ILayoutFactory.CreateFloatingDocument();
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

        private void ExtractDocuments(FloatingPane floatingPane, DockPane toolPane)
        {
            while (true)
            {
                UserControl userControl = floatingPane.IViewContainer.ExtractUserControl(0);
                if (userControl == null)
                {
                    break;
                }

                toolPane.IViewContainer.AddUserControl(userControl);
            }
            floatingPane.Close();
        }

        private void FloatingPane_EndDrag(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is FloatingPane, System.Reflection.MethodBase.GetCurrentMethod().Name + ": sender not a FloatingPane");

            App.Current.Dispatcher.Invoke(delegate
            {
                FloatingPane floatingPane = sender as FloatingPane;

                if (SelectedPane == null)
                {
                    return;
                }

                if (
                        (floatingPane == null) ||
                        (SelectedPane == null) ||
                        (!(SelectedPane.Parent is SplitterPane) && !(SelectedPane.Parent is DocumentPanel) && (SelectedPane.Parent != this)) ||
                        (_insertionIndicatorManager == null) ||
                        (_insertionIndicatorManager.WindowLocation == WindowLocation.None)
                   )
                {
                    return;
                }

                SplitterPane parentSplitterPane = (SelectedPane.Parent as SplitterPane);
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

                        if (sender is FloatingTool)
                        {
                            dockPane = ILayoutFactory.CreateToolPane();
                        }
                        else
                        {
                            dockPane = ILayoutFactory.CreateDocumentPane();
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

                        if (sender is FloatingTool)
                        {
                            dockPane = ILayoutFactory.CreateToolPane();
                        }
                        else
                        {
                            dockPane = ILayoutFactory.CreateDocumentPane();
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
                            DocumentPane documentPane = ILayoutFactory.CreateDocumentPane();
                            selectedPane.Children.Add(documentPane);
                            ExtractDocuments(floatingPane, documentPane);
                        }
                        break;
                }

                App.Current.MainWindow.Activate();
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
            Point cursorPositionOnScreen = WpfControlLibrary.Utilities.GetCursorPosition();

            bool found = false;
            Point cursorPositionInMainWindow = PointFromScreen(cursorPositionOnScreen);
            if (
                    (cursorPositionInMainWindow.X >= 0) &&
                    (cursorPositionInMainWindow.X <= this.ActualWidth) && 
                    (cursorPositionInMainWindow.Y >= 0) &&
                    (cursorPositionInMainWindow.Y <= this.ActualHeight) 
                )
            {
                Type paneType = (sender is FloatingDocument) ? typeof(DocumentPane) : typeof(ToolPane);
                var pane = FindSelectablePane(this, cursorPositionOnScreen);
                found = pane != null;
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

                    if ((paneType == pane.GetType()) || ((pane is DocumentPanel) && (sender is FloatingDocument)))
                    {
                        _windowLocationPane = new WindowLocationPane();
                        _windowLocationPane.AllowsTransparency = true;
                        _windowLocationPane.Show();
                        Point topLeftPoint = pane.PointToScreen(new Point(0, 0));
                        _windowLocationPane.Left = topLeftPoint.X;
                        _windowLocationPane.Top = topLeftPoint.Y;
                        _windowLocationPane.Width = SelectedPane.ActualWidth;
                        _windowLocationPane.Height = SelectedPane.ActualHeight;
                    }

                    if (_sideLocationPane != null)
                    {
                        _sideLocationPane.Close();
                        _sideLocationPane = null;
                    }

                    if (sender is FloatingTool)
                    {
                        _sideLocationPane = new SideLocationPane();
                        _sideLocationPane.AllowsTransparency = true;
                        _sideLocationPane.Show();
                        Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
                        _sideLocationPane.Left = topLeftPoint.X;
                        _sideLocationPane.Top = topLeftPoint.Y;
                        _sideLocationPane.Width = _root.ActualWidth;
                        _sideLocationPane.Height = _root.ActualHeight;
                    }
                }
            }

            SelectablePane previousPane = SelectedPane;

            if (!found)
            {
                SelectedPane = null;
            }

            if (!found)
            {
                if (_windowLocationPane != null)
                {
                    _windowLocationPane.Close();
                    _windowLocationPane = null;
                }
                if (_sideLocationPane != null)
                {
                    _sideLocationPane.Close();
                    _sideLocationPane = null;
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
                if (SelectedPane is DocumentPanel)
                {
                    _windowLocationPane.ShowIcons(WindowLocation.Middle);
                }
            }
        }
    }
}
