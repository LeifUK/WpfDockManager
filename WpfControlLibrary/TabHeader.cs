using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace WpfControlLibrary
{
    public class TabHeader : ListBox
    {
        bool _mouseLeftButtonDown = false;
        int _dragIndex;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            _mouseLeftButtonDown = true;

            // SelectedIndex is yet to be set so we must find it outselves ... 
            Point cursorScreenPosition = WpfControlLibrary.Utilities.GetCursorPosition();
            _dragIndex = GetListBoxItemIndex(cursorScreenPosition);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _mouseLeftButtonDown = false;
        }

        private ListBoxItem GetListBoxItem(int index)
        {
            if (
                (index < 0) ||
                (index >= Items.Count) ||
                (ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated))
            {
                return null;
            }

            return ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        private bool IsMouseOverListBoxItem(Point cursorScreenPosition, int index, out Rect bounds)
        {
            ListBoxItem item = GetListBoxItem(index);
            Point point = item.PointFromScreen(cursorScreenPosition);
            Point itemTopLeft = item.PointToScreen(new Point(0, 0));
            bounds = new Rect(itemTopLeft.X, itemTopLeft.Y, item.ActualWidth, item.ActualHeight);
            return (point.X >= 0) && (point.Y >= 0) && (point.X <= item.ActualWidth) && (point.Y <= item.ActualHeight);
        }

        private int GetListBoxItemIndex(Point cursorScreenPosition)
        {
            for (int index = 0; index < Items.Count; ++index)
            {
                if (IsMouseOverListBoxItem(cursorScreenPosition, index, out Rect bounds))
                {
                    //System.Diagnostics.Debug.WriteLine("_listBox_MouseMove: " + _listBox.SelectedIndex + "," + index);
                    return index;
                }
            }

            return -1;
        }

        private Rect GetListBoxItemBounds(int index)
        {
            ListBoxItem item = GetListBoxItem(index);
            if (item == null)
            {
                return new Rect();
            }
            Point itemTopLeft = item.PointToScreen(new Point(0, 0));
            return new Rect(itemTopLeft.X, itemTopLeft.Y, item.ActualWidth, item.ActualHeight);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (!_mouseLeftButtonDown)
            {
                return;
            }

            Point cursorScreenPosition = WpfControlLibrary.Utilities.GetCursorPosition();
            int selectedIndex = GetListBoxItemIndex(cursorScreenPosition);

            if ((selectedIndex < 0) || (selectedIndex >= Items.Count) || (selectedIndex == _dragIndex))
            {
                return;
            }

            Rect rectSelectedItem = GetListBoxItemBounds(selectedIndex);
            Rect rectDragItem = GetListBoxItemBounds(_dragIndex);

            double selectedWidth = rectSelectedItem.Width;
            double currentWidth = rectDragItem.Width;

            if (_dragIndex > SelectedIndex)
            {
                selectedWidth = -selectedWidth;
            }

            rectDragItem.Offset(selectedWidth, 0);
            if (!rectDragItem.Contains(cursorScreenPosition))
            {
                return;
            }

            // Move the item along to the new index
            var item = Items[_dragIndex];
            Items.Remove(item);
            Items.Insert(selectedIndex, item);
            _dragIndex = selectedIndex;
            SelectedIndex = selectedIndex;
        }
    }
}
