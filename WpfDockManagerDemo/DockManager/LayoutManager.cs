using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Xml;
using System.Windows.Input;

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
            FloatingTools = new List<FloatingTool>();
            FloatingDocuments = new List<FloatingDocument>();

            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition() { Height = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });

            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(20, System.Windows.GridUnitType.Pixel) });
            CreateSidePanes();

            Background = System.Windows.Media.Brushes.LightBlue;
        }

        ~LayoutManager()
        {
            Shutdown();
        }

        public void Shutdown()
        {
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

                _edgeLocationPane?.Close();
                _edgeLocationPane = null;
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

        internal List<FloatingTool> FloatingTools;
        internal List<FloatingDocument> FloatingDocuments;
        internal ListView _leftPane;
        internal ListView _topPane;
        internal ListView _rightPane;
        internal ListView _bottomPane;
        internal Grid _root;

        private SelectablePane SelectedPane;

        #region dependency properties 

        #region DocumentsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DocumentsSourceProperty = DependencyProperty.Register("DocumentsSource", typeof(System.Collections.Generic.IEnumerable<IDocument>), typeof(LayoutManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentsSourceChanged)));

        public System.Collections.Generic.IEnumerable<IDocument> DocumentsSource
        {
            get
            {
                return (System.Collections.Generic.IEnumerable<IDocument>)GetValue(DocumentsSourceProperty);
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
                DocumentsSource = (System.Collections.Generic.IEnumerable<IDocument>)e.NewValue;
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

        private void CreateSidePanes()
        {
            _leftPane = new ListView();
            _leftPane.Items.Add("L");
            Children.Add(_leftPane);
            Grid.SetRow(_leftPane, 1);
            Grid.SetColumn(_leftPane, 0);
            _leftPane.Height = 50;
            _leftPane.Width = 50;

            _rightPane = new ListView();
            _rightPane.Items.Add("R");
            Children.Add(_rightPane);
            Grid.SetRow(_rightPane, 1);
            Grid.SetColumn(_rightPane, 2);
            _rightPane.Height = 50;
            _rightPane.Width = 50;

            _topPane = new ListView();
            _topPane.Items.Add("T");
            Children.Add(_topPane);
            Grid.SetRow(_topPane, 0);
            Grid.SetColumn(_topPane, 1);
            _topPane.Height = 50;
            _topPane.Width = 50;

            _bottomPane = new ListView();
            _bottomPane.Items.Add("B");
            Children.Add(_bottomPane);
            Grid.SetRow(_bottomPane, 2);
            Grid.SetColumn(_bottomPane, 1);
            _bottomPane.Height = 50;
            _bottomPane.Width = 50;
        }

        public void Clear()
        {
            Children.Clear();
            CreateSidePanes();
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
                                System.Diagnostics.Trace.Assert(iToolView != null, System.Reflection.MethodBase.GetCurrentMethod().Name + ": the UserControl must implement interface IView");
                                iToolView.IDocument = viewModel as IDocument;
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

        private void RegisterDockPane(DockPane dockPane)
        {
            System.Diagnostics.Trace.Assert(dockPane != null);

            dockPane.Close += DockPane_Close;
            dockPane.Float += DockPane_Float;
            dockPane.UngroupCurrent += DockPane_UngroupCurrent;
            dockPane.Ungroup += DockPane_Ungroup;
        }

        private DocumentPane CreateDocumentPane()
        {
            DocumentPane documentPane = new DocumentPane();
            RegisterDockPane(documentPane);
            return documentPane;
        }

        private ToolPane CreateToolPane()
        {
            ToolPane toolPane = new ToolPane();
            RegisterDockPane(toolPane);
            return toolPane;
        }

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

        // Warning warning => as per floating tool!
        private FloatingDocument CreateFloatingDocument()
        {
            FloatingDocument floatingDocument = new FloatingDocument();
            floatingDocument.LocationChanged += FloatingWindow_LocationChanged;
            // Warning warning
            floatingDocument.Closed += FloatingTool_Closed;
            floatingDocument.Ungroup += FloatingPane_Ungroup;
            floatingDocument.UngroupCurrent += FloatingPane_UngroupCurrent;
            floatingDocument.DataContext = new FloatingViewModel();
            (floatingDocument.DataContext as FloatingViewModel).Title = floatingDocument.Title;
            floatingDocument.EndDrag += FloatingPane_EndDrag;
            // Ensure the window remains on top of the main window
            floatingDocument.Owner = App.Current.MainWindow;
            floatingDocument.Show();
            FloatingDocuments.Add(floatingDocument);
            return floatingDocument;
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

                DockManager.DockPane documentPane = CreateDocumentPane();
                documentPane.IViewContainer.AddUserControl(documentViews[0]);

                documentPanel.Children.Add(documentPane);
                list_N.Add(documentPane);
                AddViews(documentViews, list_N, delegate { return CreateDocumentPane(); });
            }

            List<UserControl> toolViews = LoadViewsFromTemplates(ToolTemplates, ToolsSource);
            if ((toolViews != null) && (toolViews.Count > 0))
            {
                List<FrameworkElement> list_N = new List<FrameworkElement>();

                DockManager.DockPane toolPane = CreateToolPane();
                toolPane.IViewContainer.AddUserControl(toolViews[0]);

                (_root as SplitterPane).AddChild(toolPane, false);

                list_N.Add(toolPane);
                AddViews(toolViews, list_N, delegate { return CreateToolPane(); });
            }

            UpdateLayout();
        }

        private XmlElement SaveToolGroupNode(XmlDocument xmlDocument, XmlNode xmlNode, ToolPane toolPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(toolPane != null);

            XmlElement xmlToolGroup = xmlDocument.CreateElement("ToolGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(toolPane).ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(toolPane).ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = toolPane.ActualWidth.ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = toolPane.ActualHeight.ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlToolGroup);
            return xmlToolGroup;
        }

        private XmlElement SaveToolNode(XmlDocument xmlDocument, XmlNode xmlNode, IDocument iDocument, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(iDocument != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Tool");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iDocument.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ID");
            xmlAttribute.Value = iDocument.ID.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private void SaveSplitterPaneNode(XmlDocument xmlDocument, Grid splitterPane, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(splitterPane != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlSplitterPane = xmlDocument.CreateElement("SplitterPane");
            xmlParentNode.AppendChild(xmlSplitterPane);

            GridSplitter gridSplitter = splitterPane.Children.OfType<GridSplitter>().Single();
            System.Diagnostics.Trace.Assert(gridSplitter != null);

            bool isHorizontal = Grid.GetRow(gridSplitter) == 1;
            var xmlAttribute = xmlDocument.CreateAttribute("Orientation");
            xmlAttribute.Value = isHorizontal ? "Horizontal" : "Vertical";
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = splitterPane.ActualWidth.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = splitterPane.ActualHeight.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            List<FrameworkElement> children = splitterPane.Children.OfType<FrameworkElement>().Where(n => !(n is GridSplitter)).OrderBy(n => Grid.GetRow(n) + Grid.GetColumn(n)).ToList();
            foreach (var childNode in children)
            {
                SaveNode(xmlDocument, childNode, xmlSplitterPane);
            }
        }

        private XmlElement SaveDocumentGroupNode(XmlDocument xmlDocument, XmlNode xmlNode, DocumentPane documentPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(documentPane != null);

            XmlElement xmlDocumentGroup = xmlDocument.CreateElement("DocumentGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(documentPane).ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(documentPane).ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = documentPane.ActualWidth.ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = documentPane.ActualHeight.ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlDocumentGroup);
            return xmlDocumentGroup;
        }

        private XmlElement SaveDocumentNode(XmlDocument xmlDocument, XmlNode xmlNode, IDocument iDocument, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(iDocument != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Document");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iDocument.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ID");
            xmlAttribute.Value = iDocument.ID.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private void SaveDocumentPanelNode(XmlDocument xmlDocument, DocumentPanel documentPanel, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(documentPanel != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlDocumentPanel = xmlDocument.CreateElement("DocumentPanel");
            xmlParentNode.AppendChild(xmlDocumentPanel);

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = documentPanel.ActualWidth.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = documentPanel.ActualHeight.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            foreach (var childNode in documentPanel.Children)
            {
                SaveNode(xmlDocument, childNode, xmlDocumentPanel);
            }
        }

        private XmlElement CreateFloatingToolNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingTool floatingTool)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingTool = xmlDocument.CreateElement("FloatingTool");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = floatingTool.ActualWidth.ToString();
            xmlFloatingTool.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = floatingTool.ActualHeight.ToString();
            xmlFloatingTool.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlFloatingTool);
            return xmlFloatingTool;
        }

        private void SaveNode(XmlDocument xmlDocument, Object node, XmlNode xmlParentPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(node != null);
            System.Diagnostics.Trace.Assert(xmlParentPane != null);

            if (node is DocumentPanel)
            {
                SaveDocumentPanelNode(xmlDocument, node as DocumentPanel, xmlParentPane);
            }
            else if (node is DocumentPane)
            {
                DocumentPane documentPane = node as DocumentPane;
                int count = documentPane.IViewContainer.GetUserControlCount();
                if (count < 1)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": no documents");
                }

                XmlNode xmlNodeParent = SaveDocumentGroupNode(xmlDocument, xmlParentPane, documentPane);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = documentPane.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveDocumentNode(xmlDocument, xmlNodeParent, userControl.DataContext as IDocument, userControl.Name);
                }
            }
            else if (node is ToolPane)
            {
                ToolPane toolPane = node as ToolPane;
                int count = toolPane.IViewContainer.GetUserControlCount();
                if (count < 1)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": no tools");
                }

                XmlNode xmlNodeParent = SaveToolGroupNode(xmlDocument, xmlParentPane, toolPane);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = toolPane.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveToolNode(xmlDocument, xmlNodeParent, userControl.DataContext as IDocument, userControl.Name);
                }
            }
            else if (node is Grid)
            {
                SaveSplitterPaneNode(xmlDocument, node as Grid, xmlParentPane);
            }
        }

        public bool SaveLayout(out XmlDocument xmlDocument, string fileNameAndPath)
        {
            xmlDocument = new XmlDocument();

            if (Children.Count == 0)
            {
                return false;
            }

            XmlElement xmlLayoutManager = xmlDocument.CreateElement("LayoutManager");
            xmlDocument.AppendChild(xmlLayoutManager);

            SaveNode(xmlDocument, _root, xmlLayoutManager);
            foreach (FloatingTool floatingTool in FloatingTools)
            {
                int count = floatingTool.IViewContainer.GetUserControlCount();
                if (count < 1)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": no documents");
                }

                XmlElement xmlNodeParent = CreateFloatingToolNode(xmlDocument, xmlLayoutManager, floatingTool);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = floatingTool.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveToolNode(xmlDocument, xmlNodeParent, userControl.DataContext as IDocument, userControl.Name);
                }
            }
            xmlDocument.Save(fileNameAndPath);

            return true;
        }

        private void SetWidthOrHeight(XmlElement xmlElement, FrameworkElement parentFrameworkElement, bool isParentHorizontal, int row, int column)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("width") as XmlAttribute;
            if (xmlAttribute == null)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": unable to load layout: a " + xmlElement.Name + " element must have a width attribute");
            }
            double width = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("height") as XmlAttribute;
            if (xmlAttribute == null)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": unable to load layout: a " + xmlElement.Name + " element must have a height attribute");
            }
            double height = System.Convert.ToDouble(xmlAttribute.Value);

            Grid grid = parentFrameworkElement as Grid;
            if (grid != null)
            {
                if (isParentHorizontal)
                {
                    if (row < grid.RowDefinitions.Count)
                    {
                        grid.RowDefinitions[row].Height = new GridLength(height, GridUnitType.Star);
                    }
                }
                else
                {
                    if (column < grid.ColumnDefinitions.Count)
                    {
                        grid.ColumnDefinitions[column].Width = new GridLength(width, GridUnitType.Star);
                    }
                }
            }
        }

        private void LoadToolGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlToolGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlToolGroupNode in xmlToolGroup.ChildNodes)
            {
                if (xmlToolGroupNode is XmlElement)
                {
                    if ((xmlToolGroupNode as XmlElement).Name == "Tool")
                    {
                        XmlElement xmlToolElement = xmlToolGroupNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;
                        if (xmlAttribute == null)
                        {
                            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": a Tool element must have an ID attribute");
                        }
                        
                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
        }

        private void LoadDocumentGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlDocumentGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlDocumentGroupNode in xmlDocumentGroup.ChildNodes)
            {
                if (xmlDocumentGroupNode is XmlElement)
                {
                    if ((xmlDocumentGroupNode as XmlElement).Name == "Document")
                    {
                        XmlElement xmlToolElement = xmlDocumentGroupNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;
                        if (xmlAttribute == null)
                        {
                            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": a Document element must have an ID attribute");
                        }

                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
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
                    if ((xmlChildNode as XmlElement).Name == "SplitterPane")
                    {
                        XmlElement xmlSplitterPane = xmlChildNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlSplitterPane.Attributes.GetNamedItem("Orientation") as XmlAttribute;
                        if (xmlAttribute == null)
                        {
                            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": a SplitterPane element must have an orientation attribute");
                        }

                        bool isChildHorizontal = xmlAttribute.Value == "Horizontal";

                        SplitterPane newGrid = new SplitterPane(isChildHorizontal);

                        if (parentFrameworkElement == this)
                        {
                            SetRootPane(newGrid);
                            row = Grid.GetRow(_root);
                            column = Grid.GetColumn(_root);
                        }
                        else
                        {
                            System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                            parentElement.AddChild(newGrid);
                            Grid.SetRow(newGrid, row);
                            Grid.SetColumn(newGrid, column);
                        }
                        SetWidthOrHeight(xmlSplitterPane, parentFrameworkElement, isParentHorizontal, row, column);

                        row += rowIncrement;
                        column += columnIncrement;

                        LoadNode(viewsMap, newGrid, xmlSplitterPane, isChildHorizontal);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentPanel")
                    {
                        DocumentPanel documentPanel = new DocumentPanel();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPanel);

                        XmlElement xmlDocumentPanel = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlDocumentPanel, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadNode(viewsMap, documentPanel, xmlDocumentPanel, true);

                        Grid.SetRow(documentPanel, row);
                        Grid.SetColumn(documentPanel, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentGroup")
                    {
                        DocumentPane documentPane = CreateDocumentPane();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPane);

                        XmlElement xmlDocumentGroup = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlDocumentGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadDocumentGroup(viewsMap, xmlDocumentGroup, documentPane.IViewContainer);
                        Grid.SetRow(documentPane, row);
                        Grid.SetColumn(documentPane, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "ToolGroup")
                    {
                        ToolPane toolPane = CreateToolPane();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(toolPane);

                        XmlElement xmlToolGroup = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlToolGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadToolGroup(viewsMap, xmlToolGroup, toolPane.IViewContainer);
                        Grid.SetRow(toolPane, row);
                        Grid.SetColumn(toolPane, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingTool")
                    {
                        FloatingTool floatingTool = CreateFloatingTool();

                        XmlElement xmlfloatingTool = xmlChildNode as XmlElement;
                        LoadToolGroup(viewsMap, xmlfloatingTool, floatingTool.IViewContainer);
                    }
                }
                
                if ((row > 2) || (column > 2))
                {
                    // we can only have two child elements (plus a splitter) in each grid
                    break;
                }
            }
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

            LoadNode(viewsMap, this, xmlDocument.DocumentElement, true);

            return true;
        }

        /*
         * Remove a dock pane from the tree
         */
        private DockPane ExtractDockPane(DockPane dockPane)
        {
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

                parentGrid.Children.Remove(dockPane);

                if (!(parentGrid is DocumentPanel))
                {
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
                    int row = Grid.GetRow(parentGrid);
                    int column = Grid.GetColumn(parentGrid);
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

            DockPane newDockPane = (dockPane is ToolPane) ? (DockPane) CreateToolPane() : CreateDocumentPane();
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

        private FloatingTool CreateFloatingTool()
        {
            FloatingTool floatingTool = new FloatingTool();
            floatingTool.LocationChanged += FloatingWindow_LocationChanged;
            floatingTool.Closed += FloatingTool_Closed;
            floatingTool.Ungroup += FloatingPane_Ungroup;
            floatingTool.UngroupCurrent += FloatingPane_UngroupCurrent;
            floatingTool.DataContext = new FloatingViewModel();
            (floatingTool.DataContext as FloatingViewModel).Title = floatingTool.Title;
            floatingTool.EndDrag += FloatingPane_EndDrag;
            // Ensure the window remains on top of the main window
            floatingTool.Owner = App.Current.MainWindow;
            floatingTool.Show();
            FloatingTools.Add(floatingTool);
            return floatingTool;
        }

        private void DockPane_Float(object sender, FloatEventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is DockPane);

            DockPane dockPane = sender as DockPane;

            Point cursorPositionOnScreen = WpfControlLibrary.Utilities.GetCursorPosition();
            Point cursorPositionInMainWindow = App.Current.MainWindow.PointFromScreen(cursorPositionOnScreen);
            Point cursorPositionInToolPane = dockPane.PointFromScreen(cursorPositionOnScreen);

            Point mainWindowLocation = App.Current.MainWindow.PointToScreen(new Point(0, 0));

            ExtractDockPane(dockPane);

            FloatingPane floatingPane = null;
            if (dockPane is ToolPane)
            {
                floatingPane = CreateFloatingTool();
            }
            else
            {
                floatingPane = CreateFloatingDocument();
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

        // Warning warning => what about the view model? Remove from list???
        private void DockPane_Close(object sender, EventArgs e)
        {
            DockPane dockPane = sender as DockPane;

            if (dockPane == null)
            {
                return;
            }

            ExtractDockPane(dockPane);
        }

        private void FloatingTool_Closed(object sender, EventArgs e)
        {
            if (FloatingTools.Contains(sender as FloatingTool))
            {
                FloatingTools.Remove(sender as FloatingTool);
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
                newFloatingPane = CreateFloatingTool();
            }
            else
            {
                newFloatingPane = CreateFloatingDocument();
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
            _edgeLocationPane?.Close();
            _edgeLocationPane = null;
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
                        //(_windowLocationPane == null) ||
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
                    case WindowLocation.BottomEdge:
                    case WindowLocation.TopEdge:
                    case WindowLocation.LeftEdge:
                    case WindowLocation.RightEdge:

                        if (sender is FloatingTool)
                        {
                            dockPane = CreateToolPane();
                        }
                        else
                        {
                            dockPane = CreateDocumentPane();
                        }
                        ExtractDocuments(floatingPane, dockPane);

                        parentSplitterPane = new SplitterPane((windowLocation == WindowLocation.TopEdge) || (windowLocation == WindowLocation.BottomEdge));
                        bool isFirst = (windowLocation == WindowLocation.TopEdge) || (windowLocation == WindowLocation.LeftEdge);
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
                            dockPane = CreateToolPane();
                        }
                        else
                        {
                            dockPane = CreateDocumentPane();
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
                            DocumentPane documentPane = CreateDocumentPane();
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
        private EdgeLocationPane _edgeLocationPane;
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

                    if (_edgeLocationPane != null)
                    {
                        _edgeLocationPane.Close();
                        _edgeLocationPane = null;
                    }

                    if (sender is FloatingTool)
                    {
                        _edgeLocationPane = new EdgeLocationPane();
                        _edgeLocationPane.AllowsTransparency = true;
                        _edgeLocationPane.Show();
                        Point topLeftPoint = _root.PointToScreen(new Point(0, 0));
                        _edgeLocationPane.Left = topLeftPoint.X;
                        _edgeLocationPane.Top = topLeftPoint.Y;
                        _edgeLocationPane.Width = _root.ActualWidth;
                        _edgeLocationPane.Height = _root.ActualHeight;
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
                if (_edgeLocationPane != null)
                {
                    _edgeLocationPane.Close();
                    _edgeLocationPane = null;
                }
            }

            if ((previousPane != null) && (SelectedPane != previousPane))
            {
                previousPane.IsHighlighted = false;
            }

            WindowLocation windowLocation = WindowLocation.None;

            if (_edgeLocationPane != null)
            {
                windowLocation = _edgeLocationPane.TrySelectIndicator(cursorPositionOnScreen);
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
