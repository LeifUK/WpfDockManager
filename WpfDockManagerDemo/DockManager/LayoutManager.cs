using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml;

/*
 * Note: I have placed most of the intelligence in this class rather than spreading 
 * it around multiple classes. 
 * 
 * Units: 
 * 
 * The actual width/height and top/left values for a window etc are defined in DIUs: 
 * 
 *      Device independent unit: 1/96"
 *      
 *  Logical unit = Device independent unit
 */

namespace WpfDockManagerDemo.DockManager
{
    public class LayoutManager : System.Windows.Controls.Grid
    {
        public LayoutManager()
        {
            FloatingPanes = new List<FloatingPane>();
        }

        ~LayoutManager()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            if (_windowLocationPane != null)
            {
                try
                {

                    _windowLocationPane.Close();
                    _windowLocationPane = null;
                }
                catch
                {
                    // Ignore
                }
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

        /*
         * Note: 
         * 
         *  The application is responsible for instantiating the view models
         * 
         */

        // The Resources property is in a base class

        /*
         * These can override default appearance
         * They bind to a Title property
         */

        public DataTemplate ToolPaneTitleTemplate { get; set; }
        public DataTemplate ToolPaneHeaderTemplate { get; set; }
        public DataTemplate DocumentPaneTitleTemplate { get; set; }
        public DataTemplate DocumentPaneHeaderTemplate { get; set; }

        public List<FloatingPane> FloatingPanes;

        private DocumentPane SelectedPane;
        private List<UserControl> _views;

        #region dependency properties 

        #region DocumentItems dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentItemsProperty = DependencyProperty.Register("DocumentItems", typeof(System.Collections.Generic.IEnumerable<IDocument>), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentItemsChanged)));

        public System.Collections.Generic.IEnumerable<IDocument> DocumentItems
        {
            get
            {
                return (System.Collections.Generic.IEnumerable<IDocument>)GetValue(DocumentItemsProperty);
            }
            set
            {
                SetValue(DocumentItemsProperty, value);
            }
        }

        private static void OnDocumentItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnDocumentItemsChanged(e);
        }

        protected virtual void OnDocumentItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentItems = (System.Collections.Generic.IEnumerable<IDocument>)e.NewValue;
                // Warning warning
                //CreateControl();
                Create();
            }
        }

        #endregion

        #region ToolItems dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ToolItemsProperty = DependencyProperty.Register("ToolItems", typeof(System.Collections.IEnumerable), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnToolItemsChanged)));

        public System.Collections.IEnumerable ToolItems
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(ToolItemsProperty);
            }
            set
            {
                SetValue(ToolItemsProperty, value);
            }
        }

        private static void OnToolItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutManager)d).OnToolItemsChanged(e);
        }

        protected virtual void OnToolItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ToolItems = (System.Collections.IEnumerable)e.NewValue;
                // Warning warning
                //CreateControl();
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
                return (System.Windows.Controls.ControlTemplate)GetValue(ToolItemsProperty);
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

        public void Clear()
        {
            Children.Clear();
        }

        private void Create()
        {
            _views = new List<UserControl>();

            // First load the views and view models

            foreach (var document in DocumentItems)
            {
                foreach (var item in Resources.Values)
                {
                    if (item is DataTemplate)
                    {
                        DataTemplate dataTemplate = item as DataTemplate;

                        if (document.GetType() == (Type)dataTemplate.DataType)
                        {
                            UserControl view = (dataTemplate.LoadContent() as UserControl);
                            if (view != null)
                            {
                                view.DataContext = document;
                                view.HorizontalAlignment = HorizontalAlignment.Stretch;
                                view.VerticalAlignment = VerticalAlignment.Stretch;

                                _views.Add(view);
                            }
                        }
                    }
                }
            }

            if (_views.Count == 0)
            {
                return;
            }

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

             */

            List<FrameworkElement> list_N = new List<FrameworkElement>();
            List<FrameworkElement> list_N_plus_1 = new List<FrameworkElement>();

            DockManager.DocumentPane documentPane = new DocumentPane();
            documentPane.AddUserControl(_views[0]);

            Children.Add(documentPane);

            list_N.Add(documentPane);

            bool isHorizontal = false;

            int viewIndex = 1;

            while (viewIndex < _views.Count)
            {
                for (int i = 0; (i < list_N.Count) && (viewIndex < _views.Count); ++i)
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

                    node = _views[viewIndex];
                    documentPane = new DocumentPane();
                    documentPane.AddUserControl(node as UserControl);

                    list_N_plus_1.Add(documentPane);

                    splitterPane.AddChild(documentPane, false);

                    ++viewIndex;
                }

                isHorizontal = !isHorizontal;
                list_N = list_N_plus_1;
                list_N_plus_1 = new List<FrameworkElement>();
            }

            UpdateLayout();
        }

        public bool SaveLayout(XmlDocument xmlDocument, Grid grid)
        {
            return true;
        }

        private XmlElement CreateElementNode(XmlDocument xmlDocument, XmlNode xmlNode, IDocument iDocument)
        {
            if ((xmlDocument == null) || (xmlNode == null) || (iDocument == null))
            {
                return null;
            }

            XmlElement xmlElement = xmlDocument.CreateElement("Document");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iDocument.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ID");
            xmlAttribute.Value = iDocument.ID.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private void SaveGrid(XmlDocument xmlDocument, Grid grid, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(grid != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlGridElement = xmlDocument.CreateElement("SplitPanel");
            xmlParentNode.AppendChild(xmlGridElement);

            GridSplitter gridSplitter = grid.Children.OfType<GridSplitter>().Single();
            System.Diagnostics.Trace.Assert(gridSplitter != null);

            bool isHorizontal = Grid.GetRow(gridSplitter) == 1;
            var xmlAttribute = xmlDocument.CreateAttribute("Orientation");
            xmlAttribute.Value = isHorizontal ? "Horizontal" : "Vertical";
            xmlGridElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(grid).ToString();
            xmlGridElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(grid).ToString();
            xmlGridElement.Attributes.Append(xmlAttribute);

            List<FrameworkElement> children = grid.Children.OfType<FrameworkElement>().Where(n => !(n is GridSplitter)).OrderBy(n => Grid.GetRow(n) + Grid.GetColumn(n)).ToList();
            foreach (var childNode in children)
            {
                SaveNode(xmlDocument, childNode, xmlGridElement);
            }
        }

        private void SaveNode(XmlDocument xmlDocument, Object node, XmlNode xmlGridElement)
        {
            if (node is DocumentPane)
            {
                CreateElementNode(xmlDocument, xmlGridElement, (node as DocumentPane).IDocument);
            }
            else if (node is Grid)
            {
                SaveGrid(xmlDocument, node as Grid, xmlGridElement);
            }
        }

        public bool SaveLayout(out XmlDocument xmlDocument, string fileNameAndPath)
        {
            xmlDocument = new XmlDocument();

            if (Children.Count == 0)
            {
                return false;
            }

            SaveNode(xmlDocument, Children[0], xmlDocument);

            // Warning warning
            xmlDocument.Save(fileNameAndPath);

            return true;
        }

        private void LoadNode(Dictionary<string, UserControl> viewsMap, FrameworkElement parentFrameworkElement, XmlNode parentXmlNode, bool isParentHorizontal)
        {
            int row = 0;
            int rowIncrement = isParentHorizontal ? 2 : 0;
            int column = 0;
            int columnIncrement = isParentHorizontal ? 0 : 2;

            foreach (var xmlChildNode in parentXmlNode.ChildNodes)
            {
                if (xmlChildNode is XmlElement)
                {
                    XmlElement xmlElement = xmlChildNode as XmlElement;
                    if (xmlElement.Name == "SplitPanel")
                    {
                        XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("Orientation") as XmlAttribute;
                        if (xmlAttribute == null)
                        {
                            throw new Exception("Unable to load layout: a SplitPanel element must have an orientation attribute");
                        }

                        bool isChildHorizontal = xmlAttribute.Value == "Horizontal";

                        Grid newGrid = new SplitterPane(isChildHorizontal);
                        Grid.SetRow(newGrid, row);
                        Grid.SetColumn(newGrid, column);
                        row += rowIncrement;
                        column += columnIncrement;

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(newGrid);

                        LoadNode(viewsMap, newGrid, xmlElement, isChildHorizontal);

                        if (parentFrameworkElement == this)
                        {
                            // Only one grid at the top level
                            break;
                        }
                    }
                    else if (parentFrameworkElement != this)
                    {
                        if (xmlElement.Name == "Document")
                        {
                            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("ID") as XmlAttribute;
                            if (xmlAttribute == null)
                            {
                                throw new Exception("Unable to load layout: a Document element must have an ID attribute");
                            }

                            if (viewsMap.ContainsKey(xmlAttribute.Value))
                            {
                                DocumentPane documentPane = new DocumentPane();
                                AttachDocumentPane(documentPane);
                                documentPane.AddUserControl(viewsMap[xmlAttribute.Value]);

                                System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                                parentElement.AddChild(documentPane);
                                Grid.SetRow(documentPane, row);
                                Grid.SetColumn(documentPane, column);
                                row += rowIncrement;
                                column += columnIncrement;

                                viewsMap.Remove(xmlAttribute.Value);
                            }
                        }
                    }
                }

                if ((row > 2) || (column > 2))
                {
                    // we can only have two child elements (plus a splitter) in each grid
                    break;
                }
            }
        }

        private DocumentPane ExtractDocumentPane(DocumentPane documentPane)
        {
            if (documentPane == null)
            {
                return null;
            }

            Grid parentGrid = documentPane.Parent as Grid;
            if (parentGrid == null)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": documentPane parent must be a Grid");
            }

            if (parentGrid == this)
            {
                this.Children.Remove(documentPane);
            }
            else
            {
                if (parentGrid.Parent is FloatingPane)
                {
                    return null;
                }

                Grid grandparentGrid = parentGrid.Parent as Grid;
                if (grandparentGrid == null)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Grid parent not a Grid");
                }
                parentGrid.Children.Remove(documentPane);

                FrameworkElement frameworkElement = null;
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
                grandparentGrid.Children.Remove(parentGrid);
                Grid.SetRow(frameworkElement, Grid.GetRow(parentGrid));
                Grid.SetColumn(frameworkElement, Grid.GetColumn(parentGrid));
                grandparentGrid.Children.Add(frameworkElement);
            }

            return documentPane;
        }

        private void AttachDocumentPane(DocumentPane documentPane)
        {
            documentPane.Close += DocumentPane_Close;
            documentPane.Float += DocumentPane_Float;
            documentPane.IsDocked = true;
        }

        private void DocumentPane_Float(object sender, EventArgs e)
        {
            System.Windows.Input.MouseEventArgs mouseEventArgs = e as System.Windows.Input.MouseEventArgs;

            DocumentPane documentPane = sender as DocumentPane;

            if (documentPane == null)
            {
                return;
            }

            Point mousePosition;
            if (mouseEventArgs != null)
            {
                mousePosition = mouseEventArgs.GetPosition(App.Current.MainWindow);
            }
            else
            {
                mousePosition = new Point(App.Current.MainWindow.Left + App.Current.MainWindow.Width/2, App.Current.MainWindow.Top + App.Current.MainWindow.Height / 2);
            }

            Point mainWindowLocation = App.Current.MainWindow.PointToScreen(new Point(0, 0));

            ExtractDocumentPane(documentPane);

            documentPane.IsDocked = false;

            FloatingPane floatingPane = new FloatingPane();
            floatingPane.LocationChanged += FloatingWindow_LocationChanged;

            var child = documentPane.View;
            // Warning warning
            while (true)
            {
                UserControl userControl = documentPane.ExtractUserControl();
                if (userControl == null)
                {
                    break;
                }

                _views.Remove(userControl);
                floatingPane.AddView(userControl);

            }

            floatingPane.DataContext = new FloatingViewModel();
            (floatingPane.DataContext as FloatingViewModel).Title = floatingPane.Title;
            floatingPane.Dock += FloatingWindow_Dock;
            floatingPane.EndDrag += FloatingView_EndDrag;
            // Ensure the window remains on top of the main window
            floatingPane.Owner = App.Current.MainWindow;
            floatingPane.Show();
            floatingPane.Left = mainWindowLocation.X + mousePosition.X;
            floatingPane.Top = mainWindowLocation.Y + mousePosition.Y;

            FloatingPanes.Add(floatingPane);
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
        }

        private DocumentPane AddDocumentPane(SplitterPane splitterPane, UserControl userControl, bool isFirst)
        {
            DocumentPane documentPane = new DocumentPane();
            documentPane.AddUserControl(userControl);

            splitterPane.AddChild(documentPane, isFirst);
            return documentPane;
        }

        // Warning warning
        private UserControl ExtractUserControlFromFloatingPane(FloatingPane floatingPane)
        {
            UserControl userControl = floatingPane.ExtractView();
            FloatingPanes.Remove(floatingPane);
            floatingPane.Close();
            return userControl;
        }

        private void ExtractDocuments(FloatingPane floatingPane, DocumentPane documentPane)
        {
            while (true)
            {
                UserControl userControl = floatingPane.ExtractView();
                if (userControl == null)
                {
                    break;
                }

                documentPane.AddUserControl(userControl);
            }
            floatingPane.Close();
        }

        private void FloatingView_EndDrag(object sender, EventArgs e)
        {
            FloatingPane floatingPane = sender as FloatingPane;

            if (
                    (floatingPane == null) ||
                    (SelectedPane == null) ||
                    !(SelectedPane.Parent is SplitterPane) || 
                    (_windowLocationPane == null) ||
                    (_insertionIndicatorManager == null) ||
                    (_insertionIndicatorManager.WindowLocation == WindowLocation.None)
               )
            {
                return;
            }

            App.Current.Dispatcher.Invoke(delegate
            {
                SplitterPane parentSplitterPane = (SelectedPane.Parent as SplitterPane);
                DocumentPane documentPane = null;

                switch (_insertionIndicatorManager.WindowLocation)
                {
                    case WindowLocation.BottomEdge:
                    case WindowLocation.TopEdge:
                    case WindowLocation.LeftEdge:
                    case WindowLocation.RightEdge:

                        documentPane = new DocumentPane();
                        ExtractDocuments(floatingPane, SelectedPane as DocumentPane);

                        parentSplitterPane = new SplitterPane((_insertionIndicatorManager.WindowLocation == WindowLocation.TopEdge) || (_insertionIndicatorManager.WindowLocation == WindowLocation.BottomEdge));
                        bool isFirst = (_insertionIndicatorManager.WindowLocation == WindowLocation.TopEdge) || (_insertionIndicatorManager.WindowLocation == WindowLocation.LeftEdge);
                        parentSplitterPane.AddChild(documentPane, isFirst);

                        if (Children.Count == 0)
                        {
                            Children.Add(parentSplitterPane);
                        }
                        else
                        {
                            SplitterPane rootSplitterPane = Children[0] as SplitterPane;
                            Children.Remove(rootSplitterPane);
                            Children.Add(parentSplitterPane);
                            parentSplitterPane.AddChild(rootSplitterPane, !isFirst);
                        }
                        break;

                    case WindowLocation.Right:
                    case WindowLocation.Left:
                    case WindowLocation.Top:
                    case WindowLocation.Bottom:

                        documentPane = new DocumentPane();
                        ExtractDocuments(floatingPane, documentPane);

                        parentSplitterPane.Children.Remove(SelectedPane);

                        SplitterPane newGrid = new SplitterPane((_insertionIndicatorManager.WindowLocation == WindowLocation.Top) || (_insertionIndicatorManager.WindowLocation == WindowLocation.Bottom));
                        parentSplitterPane.Children.Add(newGrid);
                        Grid.SetRow(newGrid, Grid.GetRow(SelectedPane));
                        Grid.SetColumn(newGrid, Grid.GetColumn(SelectedPane));

                        bool isTargetFirst = (_insertionIndicatorManager.WindowLocation == WindowLocation.Right) || (_insertionIndicatorManager.WindowLocation == WindowLocation.Bottom);
                        newGrid.AddChild(SelectedPane, isTargetFirst);
                        newGrid.AddChild(documentPane, !isTargetFirst);
                        break;

                    case WindowLocation.Middle:

                        ExtractDocuments(floatingPane, SelectedPane as DocumentPane);
                        CancelSelection();
                        break;
                }

                if (documentPane != null)
                {
                    AttachDocumentPane(documentPane);
                    CancelSelection();
                }
            });
        }

        /*
         * Locates the dock pane at the specified point
         * 
         *      grid: The current grid to search in.
         *      point: A point relative to the top left corner of the main window. 
         *      left: On success this contains the left coordinate of the target DocumentPane relative to the left of the main window.
         *      top: On success this contains the top coordinate of the target DocumentPane relative to the top of the main window.
         */
        private DocumentPane FindView(Grid grid, Point point, ref double leftGrid, ref double topGrid)
        {
            if (grid == null)
            {
                return null;
            }

            // First locate the cell 

            double top = 0.0;
            int row = 0;

            foreach (var rowDefinition in grid.RowDefinitions)
            {
                double newTop = top + rowDefinition.ActualHeight;
                if (newTop >= point.Y)
                {
                    break;
                }
                top = newTop;
                row++;
            }

            double left = 0.0;
            int column = 0;
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                double newLeft = left + columnDefinition.ActualWidth;
                if (newLeft >= point.X)
                {
                    break;
                }
                left = newLeft;
                column++;
            }

            UIElement uiElement = null;
            foreach (var child in grid.Children)
            {
                uiElement = child as UIElement;
                if (uiElement != null)
                {
                    if ((Grid.GetRow(child as UIElement) == row) && (Grid.GetColumn(child as UIElement) == column))
                    {
                        leftGrid += left;
                        topGrid += top;

                        if (uiElement is DocumentPane)
                        {
                            return uiElement as DocumentPane;
                        }

                        // Recursively navigate down the tree

                        if (uiElement is Grid)
                        {
                            Point subPoint = new Point(point.X - left, point.Y - top);
                            return FindView(uiElement as Grid, subPoint, ref leftGrid, ref topGrid);
                        }

                        return null;
                    }
                }
            }
            
            return null;
        }
 
        // The WPF method does not work properly -> call into User32.dll
        Point GetCursorPosition()
        {
            if (User32.GetCursorPos(out User32.POINT point) == false)
            {
                return new Point(0, 0);
            }
            return new Point(point.X, point.Y);
        }

        private WindowLocationPane _windowLocationPane;
        private InsertionIndicatorManager _insertionIndicatorManager;

        private void FloatingWindow_LocationChanged(object sender, EventArgs e)
        {
            Window floatingWindow = sender as Window;
            if (floatingWindow == null)
            {
                throw new Exception("FloatingWindow_LocationChanged(): null floating window");
            }

            DocumentPane previousPane = SelectedPane;

            var source = PresentationSource.FromVisual(this);
            System.Windows.Media.Matrix transformFromDevice = source.CompositionTarget.TransformFromDevice;
            Point cursorPositionOnScreen = GetCursorPosition();

            bool found = false;
            Point pointMouse = transformFromDevice.Transform(cursorPositionOnScreen);
            Rect rectMain = new Rect(App.Current.MainWindow.Left, App.Current.MainWindow.Top, App.Current.MainWindow.ActualWidth, App.Current.MainWindow.ActualHeight);
            if (rectMain.Contains(pointMouse))
            {
                Vector vector = new Vector(1.0, 1.0);
                Point pixelSize = (Point)transformFromDevice.Transform(vector);
                double captionHeightInDIU = SystemParameters.WindowCaptionHeight / pixelSize.Y;

                // The constants are frigs .. 
                Point pointInMainWindow = new Point(pointMouse.X - rectMain.X - 7, pointMouse.Y - rectMain.Y - captionHeightInDIU - 7);

                double topGrid = captionHeightInDIU;
                double leftGrid = 0.0;
                var pane = FindView(this, pointInMainWindow, ref leftGrid, ref topGrid);
                found = pane != null;
                if ((pane != null) && (SelectedPane != pane))
                {
                    Point pointInGrid = new Point(leftGrid, topGrid);

                    pane.IsHighlighted = true;
                    SelectedPane = pane;
                    if (_windowLocationPane != null)
                    {
                        _windowLocationPane.Close();
                    }
                    
                    _windowLocationPane = new WindowLocationPane();
                    _windowLocationPane.AllowsTransparency = true;
                    _windowLocationPane.Show();
                    // The constants are frigs .. 
                    _windowLocationPane.Left = App.Current.MainWindow.Left + pointInGrid.X + SelectedPane.ActualWidth / 2 - _windowLocationPane.ActualWidth / 2 + 7;
                    _windowLocationPane.Top = App.Current.MainWindow.Top + pointInGrid.Y + SelectedPane.ActualHeight / 2 - _windowLocationPane.ActualHeight / 2 + 2;
                    _windowLocationPane.Owner = floatingWindow.Owner;
                }
            }

            if (!found)
            {
                SelectedPane = null;
            }

            if (!found && (_windowLocationPane != null))
            {
                _windowLocationPane.Close();
                _windowLocationPane = null;
            }

            if ((previousPane != null) && (SelectedPane != previousPane))
            {
                previousPane.IsHighlighted = false;
            }

            if (_windowLocationPane != null)
            {
                WindowLocation windowLocation = _windowLocationPane.TrySelectIndicator(cursorPositionOnScreen);
                switch (windowLocation)
                {
                    case WindowLocation.LeftEdge:
                    case WindowLocation.RightEdge:
                    case WindowLocation.TopEdge:
                    case WindowLocation.BottomEdge:
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
                        break;
                    default:
                        if (SelectedPane != null)
                        {
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
                        break;
                }
            }
        }

        private void FloatingWindow_Dock(object sender, EventArgs e)
        {
            // Warning warning
        }

        private void DocumentPane_Close(object sender, EventArgs e)
        {
            DocumentPane documentPane = sender as DocumentPane;

            if (documentPane == null)
            {
                return;
            }

            ExtractDocumentPane(documentPane);
            _views.Remove(documentPane.View);
        }

        public bool LoadLayout(out XmlDocument xmlDocument, string fileNameAndPath)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(fileNameAndPath);

            if (xmlDocument.ChildNodes.Count == 0)
            {
                return false;
            }

            List<UserControl> views = new List<UserControl>();
            Dictionary<string, UserControl> viewsMap = new Dictionary<string, UserControl>();

            // The application defines the views that are supported

            foreach (var document in DocumentItems)
            {
                foreach (var item in Resources.Values)
                {
                    if (item is DataTemplate)
                    {
                        DataTemplate dataTemplate = item as DataTemplate;

                        if (document.GetType() == (Type)dataTemplate.DataType)
                        {
                            UserControl view = (dataTemplate.LoadContent() as UserControl);
                            if (view != null)
                            {
                                view.DataContext = document;
                                view.HorizontalAlignment = HorizontalAlignment.Stretch;
                                view.VerticalAlignment = VerticalAlignment.Stretch;

                                views.Add(view);

                                viewsMap.Add((document as IDocument).ID.ToString(), view);
                            }
                        }
                    }
                }
            }

            // Now load the views into the dock manager => one or more views might not be visible!

            LoadNode(viewsMap, this, xmlDocument, true);

            return true;
        }
    }
}
