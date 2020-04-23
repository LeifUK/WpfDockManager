using System;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal class DocumentContainer : Grid, IDocumentContainer
    {
        public DocumentContainer(ITabControlFactory iTabControlFactory)
        {
            _iTabControlFactory = iTabControlFactory;
        }

        private readonly ITabControlFactory _iTabControlFactory;

        public event EventHandler SelectionChanged;

        public string Title
        {
            get
            {
                if (Children[0] == null)
                {
                    return null;
                }

                IDocument iDocument = Children[0] as IDocument;
                if (iDocument != null)
                {
                    return (iDocument as IDocument).Title;
                }

                ToolTabControl tabControl = Children[0] as ToolTabControl;

                if (tabControl == null)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not a valid type -> " + Children[0].GetType().FullName);
                }

                if (tabControl.SelectedItem == null)
                {
                    return null;
                }

                iDocument = tabControl.SelectedItem.DataContext as IDocument;
                if (iDocument == null)
                {
                    throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": User Control is not a document");
                }

                return iDocument.Title;
            }
        }

        public void AddUserControl(UserControl userControl)
        {
            if (Children.Count == 0)
            {
                Children.Add(userControl);
            }
            else if (Children[0] is UserControl)
            {
                UserControl userControl2 = Children[0] as UserControl;
                Children.RemoveAt(0);

                ITabControl tabControl = _iTabControlFactory.GetTabControl();
                tabControl.SelectionChanged += TabControl_SelectionChanged;
                tabControl.TabClosed += TabControl_TabClosed;

                Children.Add(tabControl as System.Windows.UIElement);
                Grid.SetRow(tabControl as System.Windows.UIElement, 0);
                Grid.SetColumn(tabControl as System.Windows.UIElement, 0);

                tabControl.AddUserControl(userControl2);
                tabControl.AddUserControl(userControl);
            }
            else if (Children[0] is Grid)
            {
                ITabControl tabControl = Children[0] as ITabControl;
                tabControl.AddUserControl(userControl);
            }
            else
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not an expected type -> " + Children[0].GetType().FullName);
            }
        }

        private void CheckTabCount()
        {
            ITabControl tabControl = Children[0] as ITabControl;
            if (tabControl == null)
            {
                return;
            }

            if (tabControl.Count == 1)
            {
                UserControl userControl = tabControl.RemoveAt(0);
                Children.Remove((System.Windows.UIElement)tabControl);
                Children.Add(userControl);
            }
        }

        private void TabControl_TabClosed(object sender, EventArgs e)
        {
            CheckTabCount();
        }

        private void TabControl_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public UserControl ExtractUserControl(int index)
        {
            if (Children.Count == 0)
            {
                return null;
            }

            UserControl userControl = null;
            if (Children[0] is UserControl)
            {
                userControl = Children[0] as UserControl;
                Children.RemoveAt(0);
            }
            else if (Children[0] is ITabControl)
            {
                ITabControl tabControl = Children[0] as ITabControl;
                if (index >= tabControl.Count)
                {
                    index = tabControl.Count - 1;
                }
                if (index < 0)
                {
                    index = 0;
                }

                userControl = tabControl.RemoveAt(index);
                CheckTabCount();
            }
            else
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not an expected type -> " + Children[0].GetType().FullName);
            }

            return userControl;
        }

        public int GetUserControlCount()
        {
            if (Children.Count <= 0)
            {
                return 0;
            }
            else if (Children[0] is UserControl)
            {
                return 1;
            }
            else if (Children[0] is ITabControl)
            {
                return (Children[0] as ITabControl).Count;
            }

            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not an expected type -> " + Children[0].GetType().FullName);
        }

        public int GetCurrentTabIndex()
        {
            if (Children.Count <= 0)
            {
                return -1;
            }
            else if (Children[0] is UserControl)
            {
                return -1;
            }
            else if (Children[0] is TabControl)
            {
                return (Children[0] as TabControl).SelectedIndex;
            }

            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not an expected type -> " + Children[0].GetType().FullName);
        }

        public UserControl GetUserControl(int index)
        {
            if (Children.Count <= 0)
            {
                return null;
            }

            if (Children[0] is UserControl)
            {
                return (index == 0) ? Children[0] as UserControl : null;
            }
            else if (Children[0] is ITabControl)
            {
                ITabControl tabControl = Children[0] as ITabControl;

                if (index >= tabControl.Count)
                {
                    return null;
                }

                return tabControl.GetAt(index);
            }

            throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name + ": Children[0] is not an expected type -> " + Children[0].GetType().FullName);
        }
    }
}
