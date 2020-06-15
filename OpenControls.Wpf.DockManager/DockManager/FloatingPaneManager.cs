﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenControls.Wpf.DockManager
{
    internal class FloatingPaneManager : IFloatingPaneManager
    {
        public FloatingPaneManager(
            IFloatingPaneHost iFloatingPaneManagerOwner, 
            ILayoutFactory iLayoutFactory)
        {
            IFloatingPaneHost = iFloatingPaneManagerOwner;
            ILayoutFactory = iLayoutFactory;
            FloatingToolPaneGroups = new List<IFloatingPane>();
            FloatingDocumentPaneGroups = new List<IFloatingPane>();
        }

        private WindowLocationPane _windowLocationPane;
        private SideLocationPane _sideLocationPane;
        private InsertionIndicatorManager _insertionIndicatorManager;

        private SelectablePane SelectedPane;

        private readonly IFloatingPaneHost IFloatingPaneHost;
        private readonly ILayoutFactory ILayoutFactory;

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

                _sideLocationPane?.Close();
                _sideLocationPane = null;
            }
            catch
            {
                // Ignore
            }
        }

        private void FloatingPane_Closed(object sender, EventArgs e)
        {
            FloatingPane floatingPane = sender as FloatingPane;

            int count = floatingPane.IViewContainer.GetUserControlCount();
            for (int index = count - 1; index > -1; --index)
            {
                UserControl userControl = floatingPane.IViewContainer.GetUserControl(index);
                IFloatingPaneHost.RemoveViewModel(userControl.DataContext as IViewModel);
            }

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

        private void FloatingWindow_LocationChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is Window);

            Window floatingWindow = sender as Window;
            Point cursorPositionOnScreen = OpenControls.Wpf.DockManager.Controls.Utilities.GetCursorPosition();
            SelectablePane previousPane = SelectedPane;
            bool foundSelectedPane = false;
            Point cursorPositionInMainWindow = IFloatingPaneHost.RootGrid.PointFromScreen(cursorPositionOnScreen);
            if (
                    (cursorPositionInMainWindow.X >= 0) &&
                    (cursorPositionInMainWindow.X <= IFloatingPaneHost.RootGrid.ActualWidth) &&
                    (cursorPositionInMainWindow.Y >= 0) &&
                    (cursorPositionInMainWindow.Y <= IFloatingPaneHost.RootGrid.ActualHeight)
                )
            {
                Type paneType = (sender is FloatingDocumentPaneGroup) ? typeof(DocumentPaneGroup) : typeof(ToolPaneGroup);
                var pane = IFloatingPaneHost.FindSelectablePane(cursorPositionOnScreen);
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

                    Point topLeftRootPoint = IFloatingPaneHost.RootPane.PointToScreen(new Point(0, 0));
                    _sideLocationPane.Left = topLeftRootPoint.X;
                    _sideLocationPane.Top = topLeftRootPoint.Y;
                    _sideLocationPane.Width = IFloatingPaneHost.RootPane.ActualWidth;
                    _sideLocationPane.Height = IFloatingPaneHost.RootPane.ActualHeight;
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

            if (!foundSelectedPane)
            {
                SelectedPane = null;
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
                        if ((_insertionIndicatorManager != null) && (_insertionIndicatorManager.ParentGrid != IFloatingPaneHost))
                        {
                            _insertionIndicatorManager.HideInsertionIndicator();
                            _insertionIndicatorManager = null;
                        }
                        if (_insertionIndicatorManager == null)
                        {
                            _insertionIndicatorManager = new InsertionIndicatorManager(IFloatingPaneHost.RootGrid);
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
            else if (_insertionIndicatorManager != null)
            {
                _insertionIndicatorManager.HideInsertionIndicator();
                _insertionIndicatorManager = null;
            }
        }

        private void FloatingPane_EndDrag(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is FloatingPane, System.Reflection.MethodBase.GetCurrentMethod().Name + ": sender not a FloatingPane");
            Application.Current.Dispatcher.Invoke(delegate
            {
                FloatingPane floatingPane = sender as FloatingPane;

                if (
                        (floatingPane == null) ||
                        ((SelectedPane != null) && !(SelectedPane.Parent is SplitterPane) && !(SelectedPane.Parent is DocumentPanel) && (SelectedPane.Parent != IFloatingPaneHost.RootGrid)) ||
                        (_insertionIndicatorManager == null) ||
                        (_insertionIndicatorManager.WindowLocation == WindowLocation.None)
                   )
                {
                    return;
                }

                SelectablePane selectedPane = SelectedPane;
                WindowLocation windowLocation = _insertionIndicatorManager.WindowLocation;
                CancelSelection();

                IFloatingPaneHost.Unfloat(floatingPane, selectedPane, windowLocation);

                Application.Current.MainWindow.Activate();
            });
        }

        private void ValidateFloatingPanes(Dictionary<IViewModel, List<string>> viewModels, List<IFloatingPane> floatingPanes)
        {
            int count = floatingPanes.Count;
            for (int paneIndex = count - 1; paneIndex > -1; --paneIndex)
            {
                IViewContainer iViewContainer = floatingPanes[paneIndex].IViewContainer;
                int tabCount = iViewContainer.GetUserControlCount();

                for (int index = tabCount - 1; index > -1; --index)
                {
                    IViewModel iViewModel = iViewContainer.GetIViewModel(index);
                    if (viewModels.ContainsKey(iViewModel) && (viewModels[iViewModel].Contains(iViewModel.URL)))
                    {
                        viewModels[iViewModel].Remove(iViewModel.URL);
                        if (viewModels[iViewModel].Count == 0)
                        {
                            viewModels.Remove(iViewModel);
                        }
                    }
                    else
                    {
                        iViewContainer.ExtractUserControl(index);
                    }
                }

                if (iViewContainer.GetUserControlCount() == 0)
                {
                    floatingPanes[paneIndex].Close();
                }
            }
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

        #region IFloatingPaneManager

        public List<IFloatingPane> FloatingToolPaneGroups { get; }

        public List<IFloatingPane> FloatingDocumentPaneGroups { get; }

        public void Clear()
        {
            while (FloatingToolPaneGroups.Count > 0)
            {
                (FloatingToolPaneGroups[0] as FloatingPane).Closed -= FloatingPane_Closed;
                FloatingToolPaneGroups[0].Close();
                FloatingToolPaneGroups.RemoveAt(0);
            }
            FloatingToolPaneGroups.Clear();
            while (FloatingDocumentPaneGroups.Count > 0)
            {
                (FloatingDocumentPaneGroups[0] as FloatingPane).Closed -= FloatingPane_Closed;
                FloatingDocumentPaneGroups[0].Close();
                FloatingDocumentPaneGroups.RemoveAt(0);
            }
            FloatingDocumentPaneGroups.Clear();
        }

        public void CancelSelection()
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

        public void Show(FloatingDocumentPaneGroup floatingDocumentPaneGroup)
        {
            FloatingDocumentPaneGroups.Add(floatingDocumentPaneGroup);
            RegisterFloatingPane(floatingDocumentPaneGroup);
        }

        public void Show(FloatingToolPaneGroup floatingToolPaneGroup)
        {
            FloatingToolPaneGroups.Add(floatingToolPaneGroup);
            RegisterFloatingPane(floatingToolPaneGroup);
        }

        public void ValidateFloatingToolPanes(Dictionary<IViewModel, List<string>> viewModelUrlDictionary)
        {
            ValidateFloatingPanes(viewModelUrlDictionary, FloatingToolPaneGroups);
        }

        public void ValidateFloatingDocumentPanes(Dictionary<IViewModel, List<string>> viewModelUrlDictionary)
        {
            ValidateFloatingPanes(viewModelUrlDictionary, FloatingDocumentPaneGroups);
        }
        
        #endregion IFloatingPaneManager
    }
}
