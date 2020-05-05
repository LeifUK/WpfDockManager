using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal abstract class DockPane : Grid
    {
        public DockPane(IUserViewContainer iUserViewContainer)
        {
            IUserViewContainer = iUserViewContainer;
            IUserViewContainer.TabClosed += IDocumentContainer_TabClosed;
            Children.Add(iUserViewContainer as System.Windows.UIElement);
        }

        private void IDocumentContainer_TabClosed(object sender, EventArgs e)
        {
            if (IUserViewContainer.GetUserControlCount() == 0)
            {
                FireClose();
            }
        }

        public event EventHandler Close;
        public event FloatEventHandler Float;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;
        
        abstract public bool IsHighlighted { get; set; }

        protected void FireClose()
        {
            Close?.Invoke(this, null);
        }

        protected void FireFloat(bool drag)
        {
            FloatEventArgs floatEventArgs = new FloatEventArgs();
            floatEventArgs.Drag = drag;
            Float?.Invoke(this, floatEventArgs);
        }

        public readonly IUserViewContainer IUserViewContainer;

        protected void DisplayGeneralMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { FireFloat(false); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            int viewCount = IUserViewContainer.GetUserControlCount();
            if (viewCount > 2)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup Current";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { UngroupCurrent?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            if (viewCount > 1)
            {
                menuItem = new MenuItem();
                menuItem.Header = "Ungroup";
                menuItem.IsChecked = false;
                menuItem.Command = new Command(delegate { Ungroup?.Invoke(this, null); }, delegate { return true; });
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.IsOpen = true;
        }
    }
}
