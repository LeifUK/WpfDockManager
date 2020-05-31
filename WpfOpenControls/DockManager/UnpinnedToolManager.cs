using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace WpfOpenControls.DockManager
{
    internal class UnpinnedToolManager : IUnpinnedToolManager
    {
        public UnpinnedToolManager(IDockPaneTreeManager iDockPaneTreeManager, IDockPaneTree iDockPaneTree, IUnpinnedToolParent iUnpinnedToolPaneOwner)
        {
            IDockPaneTreeManager = iDockPaneTreeManager;
            IDockPaneTree = iDockPaneTree;
            IUnpinnedToolParent = iUnpinnedToolPaneOwner;

            _dictUnpinnedToolData = new Dictionary<WindowLocation, List<UnpinnedToolData>>();
            _dictUnpinnedToolData.Add(WindowLocation.LeftSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.TopSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.RightSide, new List<UnpinnedToolData>());
            _dictUnpinnedToolData.Add(WindowLocation.BottomSide, new List<UnpinnedToolData>());
        }

        private readonly IDockPaneTree IDockPaneTree;
        private readonly IDockPaneTreeManager IDockPaneTreeManager;
        private readonly IUnpinnedToolParent IUnpinnedToolParent;
        private ToolListBoxItem _activeToolListBoxItem;
        private UnpinnedToolPane _activeUnpinnedToolPane;
        private UnpinnedToolData _activeUnpinnedToolData;
        private Controls.IToolListBox _activeToolListBox;
        private Dictionary<WindowLocation, List<UnpinnedToolData>> _dictUnpinnedToolData;

        private UnpinnedToolPane CreateUnpinnedToolPane(ToolListBoxItem toolListBoxItem, WindowLocation windowLocation)
        {
            UnpinnedToolPane unpinnedToolPane = new UnpinnedToolPane();

            UserControl userControl = toolListBoxItem.IViewContainer.ExtractUserControl(toolListBoxItem.Index);
            unpinnedToolPane.ToolPane.IViewContainer.AddUserControl(userControl);
            unpinnedToolPane.ToolPane.HideCommandsButton();
            Point topLeftPoint = IDockPaneTree.RootPane.PointToScreen(new Point(0, 0));
            unpinnedToolPane.Left = topLeftPoint.X;
            unpinnedToolPane.Top = topLeftPoint.Y;
            if ((windowLocation == WindowLocation.TopSide) || (windowLocation == WindowLocation.BottomSide))
            {
                unpinnedToolPane.Width = IDockPaneTree.RootPane.ActualWidth;
                double height = toolListBoxItem.Height;
                if (height == 0.0)
                {
                    height = IDockPaneTree.RootPane.ActualHeight / 3;
                }
                unpinnedToolPane.Height = height;
                if (windowLocation == WindowLocation.BottomSide)
                {
                    unpinnedToolPane.Top += IDockPaneTree.RootPane.ActualHeight - height;
                }
            }
            else
            {
                unpinnedToolPane.Height = IDockPaneTree.RootPane.ActualHeight;
                double width = toolListBoxItem.Width;
                if (width == 0.0)
                {
                    width = IDockPaneTree.RootPane.ActualWidth / 3;
                }
                unpinnedToolPane.Width = width;
                if (windowLocation == WindowLocation.RightSide)
                {
                    unpinnedToolPane.Left += IDockPaneTree.RootPane.ActualWidth - width;
                }
            }
            unpinnedToolPane.CloseRequest += UnpinnedToolPane_CloseRequest;
            unpinnedToolPane.Closed += UnpinnedToolPane_Closed;
            unpinnedToolPane.PinClick += UnpinnedToolPane_PinClick;
            unpinnedToolPane.WindowLocation = windowLocation;
            unpinnedToolPane.Owner = Application.Current.MainWindow;

            unpinnedToolPane.Show();

            return unpinnedToolPane;
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

            IDockPaneTreeManager.PinToolPane(_activeUnpinnedToolData);

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

        private void UnpinnedToolPane_CloseRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(sender is UnpinnedToolPane);
            System.Diagnostics.Trace.Assert(sender == _activeUnpinnedToolPane);

            UserControl userControl = _activeUnpinnedToolPane.ToolPane.IViewContainer.ExtractUserControl(0);
            IViewModel iViewModel = userControl.DataContext as IViewModel;

            System.Diagnostics.Trace.Assert(iViewModel != null);

            /*
             * Remove the tool list items from the side bar
             */

            _activeToolListBox.ItemsSource.Remove(_activeToolListBoxItem);
            if (_activeToolListBox.ItemsSource.Count == 0)
            {
                _dictUnpinnedToolData[_activeToolListBox.WindowLocation].Remove(_activeUnpinnedToolData);
            }
            _activeUnpinnedToolPane.Close();
            _activeUnpinnedToolPane = null;
            _activeToolListBoxItem = null;
            _activeToolListBox = null;
            _activeUnpinnedToolData = null;

            IUnpinnedToolParent.ViewModelRemoved(iViewModel);
        }

        private void UnpinnedToolPane_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(_activeUnpinnedToolPane == sender);

            _activeToolListBoxItem.Width = (sender as UnpinnedToolPane).ActualWidth;
            _activeToolListBoxItem.Height = (sender as UnpinnedToolPane).ActualHeight;
        }

        private void AddUnpinnedToolData(UnpinnedToolData unpinnedToolData, WindowLocation windowLocation, Controls.IToolListBox toolListBox)
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
                (toolListBox.ItemsSource as System.Collections.ObjectModel.ObservableCollection<Controls.IToolListBoxItem>).Add(toolListBoxItem);
                unpinnedToolData.Items.Add(toolListBoxItem);
            }
        }

        #region IUnpinnedToolManager
        
        public void Clear()
        {
            foreach (var keyValuePair in _dictUnpinnedToolData)
            {
                keyValuePair.Value.Clear();
            }
        }

        public Dictionary<WindowLocation, List<UnpinnedToolData>> GetUnpinnedToolData()
        {
            return _dictUnpinnedToolData;
        }

        public void CloseUnpinnedToolPane()
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

        public ToolPaneGroup UnpinnedToolClick(ToolListBoxItem toolListBoxItem, Controls.IToolListBox iToolListBox)
        {
            System.Diagnostics.Trace.Assert(toolListBoxItem != null);
            System.Diagnostics.Trace.Assert(iToolListBox != null);

            ToolListBoxItem activeToolListBoxItem = _activeToolListBoxItem;
            CloseUnpinnedToolPane();

            if (activeToolListBoxItem == toolListBoxItem)
            {
                return null;
            }

            _activeToolListBox = iToolListBox;
            _activeToolListBoxItem = toolListBoxItem;
            _activeUnpinnedToolData = _dictUnpinnedToolData[iToolListBox.WindowLocation].Where(n => n.Items.Contains(_activeToolListBoxItem)).First();
            _activeUnpinnedToolPane = CreateUnpinnedToolPane(toolListBoxItem, iToolListBox.WindowLocation);
            return _activeUnpinnedToolPane.ToolPane;
        }

        public void MakeUnpinnedToolPaneGroup(WindowLocation windowLocation, ToolPaneGroup toolPaneGroup, string siblingGuid, bool isHorizontal, bool isFirst)
        {
            Controls.IToolListBox iToolListBox = IUnpinnedToolParent.GetToolListBox(windowLocation);

            System.Diagnostics.Trace.Assert(iToolListBox != null);
            System.Diagnostics.Trace.Assert(toolPaneGroup != null);

            UnpinnedToolData unpinnedToolData = new UnpinnedToolData();
            unpinnedToolData.ToolPaneGroup = toolPaneGroup;
            unpinnedToolData.SiblingGuid = new Guid(siblingGuid);
            unpinnedToolData.IsFirst = isFirst;
            unpinnedToolData.IsHorizontal = isHorizontal;

            AddUnpinnedToolData(unpinnedToolData, windowLocation, iToolListBox);
        }

        public void Unpin(ToolPaneGroup toolPaneGroup)
        {
            System.Diagnostics.Trace.Assert(toolPaneGroup != null);

            IDockPaneTreeManager.UnpinToolPane(toolPaneGroup, out UnpinnedToolData unpinnedToolData);

            WindowLocation windowLocation = WindowLocation.None;
            if (unpinnedToolData.IsHorizontal)
            {
                if (unpinnedToolData.IsFirst)
                {
                    windowLocation = WindowLocation.TopSide;
                }
                else
                {
                    windowLocation = WindowLocation.BottomSide;
                }
            }
            else
            {
                if (unpinnedToolData.IsFirst)
                {
                    windowLocation = WindowLocation.LeftSide;
                }
                else
                {
                    windowLocation = WindowLocation.RightSide;
                }
            }

            AddUnpinnedToolData(unpinnedToolData, windowLocation, IUnpinnedToolParent.GetToolListBox(windowLocation));
        }

        public void ProcessMoveResize()
        {
            if (_activeUnpinnedToolPane != null)
            {
                Point topLeftPoint = IDockPaneTree.RootPane.PointToScreen(new Point(0, 0));
                double left = topLeftPoint.X;
                double top = topLeftPoint.Y;
                switch (_activeToolListBox.WindowLocation)
                {
                    case WindowLocation.TopSide:
                        _activeUnpinnedToolPane.Width = IDockPaneTree.RootPane.ActualWidth;
                        break;
                    case WindowLocation.BottomSide:
                        top += IDockPaneTree.RootPane.ActualHeight - _activeUnpinnedToolPane.ActualHeight;
                        _activeUnpinnedToolPane.Width = IDockPaneTree.RootPane.ActualWidth;
                        break;
                    case WindowLocation.LeftSide:
                        _activeUnpinnedToolPane.Height = IDockPaneTree.RootPane.ActualHeight;
                        break;
                    case WindowLocation.RightSide:
                        left += IDockPaneTree.RootPane.ActualWidth - _activeUnpinnedToolPane.ActualWidth;
                        _activeUnpinnedToolPane.Height = IDockPaneTree.RootPane.ActualHeight;
                        break;
                }
                _activeUnpinnedToolPane.Left = left;
                _activeUnpinnedToolPane.Top = top;
            }
        }

        public void FrameworkElementRemoved(FrameworkElement frameworkElement)
        {
            foreach (var keyValuePair in _dictUnpinnedToolData)
            {
                foreach (var item in keyValuePair.Value)
                {
                    if (item.SiblingGuid == (Guid)frameworkElement.Tag)
                    {
                        item.SiblingGuid = (Guid)(frameworkElement.Parent as FrameworkElement).Tag;
                    }
                }
            }
        }

        public void Validate(Dictionary<IViewModel, List<string>> viewModelUrlDictionary)
        {
            CloseUnpinnedToolPane();

            foreach (KeyValuePair<WindowLocation,List<UnpinnedToolData>> keyValuePair in _dictUnpinnedToolData)
            {
                Controls.IToolListBox toolListBox = IUnpinnedToolParent.GetToolListBox(keyValuePair.Key);

                foreach (UnpinnedToolData unpinnedToolData in keyValuePair.Value)
                {
                    for(int index = unpinnedToolData.Items.Count - 1; index > -1; --index)
                    {
                        IViewModel iViewModel = unpinnedToolData.Items[index].IViewModel;
                        if (viewModelUrlDictionary.ContainsKey(iViewModel))
                        {
                            viewModelUrlDictionary.Remove(iViewModel);
                        }
                        else
                        {
                            ToolListBoxItem toolListBoxItem = unpinnedToolData.Items[index];
                            toolListBox.ItemsSource.Remove(toolListBoxItem);
                            unpinnedToolData.Items.RemoveAt(index);
                        }
                    }
                }
            }
        }

        #endregion IUnpinnedToolManager
    }
}
