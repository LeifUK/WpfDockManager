using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal abstract class DockPane : SelectablePane
    {
        public DockPane(IViewContainer iViewContainer)
        {
            IViewContainer = iViewContainer;
            IViewContainer.TabClosed += IViewContainer_TabClosed;
            IViewContainer.FloatTabRequest += IViewContainer_FloatTabRequest;
            Children.Add(iViewContainer as System.Windows.UIElement);
        }

        private void IViewContainer_FloatTabRequest(object sender, EventArgs e)
        {
            FloatTabRequest?.Invoke(this, e);
        }

        private void IViewContainer_TabClosed(object sender, EventArgs e)
        {
            if (IViewContainer.GetUserControlCount() == 0)
            {
                FireClose();
            }
        }

        public event EventHandler Close;
        public event FloatEventHandler Float;
        public event EventHandler FloatTabRequest;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;

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

        public readonly IViewContainer IViewContainer;

        protected void DisplayGeneralMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Float";
            menuItem.IsChecked = false;
            menuItem.Command = new Command(delegate { FireFloat(false); }, delegate { return true; });
            contextMenu.Items.Add(menuItem);

            int viewCount = IViewContainer.GetUserControlCount();
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
