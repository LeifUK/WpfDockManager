using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using BetterWpfControls.Components;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace BetterWpfControls.Panels
{
    public class ScrollablePanel : Panel
    {
        #region .ctors

        public ScrollablePanel()
        {
            AddVisualChild(_btnScrollLeft = new ContentPresenter() { });
            AddVisualChild(_btnScrollRight = new ContentPresenter() { });

            ScrollToLeftCommand = new DelegateCommand(ScrollToLeft, () => HorizontalOffsetToSelectedItem != 0.0);
            ScrollToRightCommand = new DelegateCommand(ScrollToRight, () =>
            {
                var viewportLeft = GetViewportLeft();
                var viewportRight = GetViewportRight();
                var x = GetViewportLeft() + InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e)).Sum(e => e.DesiredSize.Width);
                return x + HorizontalOffsetToSelectedItem > viewportRight && Math.Abs(x + HorizontalOffsetToSelectedItem - viewportRight) > 0.0001;
            });

            // set default scroll buttons
            _btnScrollLeft.Content = new ImageButton() { Source = new BitmapImage(new Uri("/BetterWpfControls;component/Resources/scrollLeft.png", UriKind.RelativeOrAbsolute)), Command = ScrollToLeftCommand, Width = 16, Margin = new Thickness(0, 0, 2, 0) };
            _btnScrollRight.Content = new ImageButton() { Source = new BitmapImage(new Uri("/BetterWpfControls;component/Resources/scrollLeft.png", UriKind.RelativeOrAbsolute)), Command = ScrollToRightCommand, Width = 16, Margin = new Thickness(2, 0, 0, 0), RenderTransformOrigin = new Point(0.5, 0.5), RenderTransform = new RotateTransform() { Angle = 180 } };
        }

        #endregion .ctors

        #region Fields

        private ContentPresenter _btnScrollLeft;
        private ContentPresenter _btnScrollRight;
        private ItemsControl _owner;
        private bool _keepSelectedItemInView;

        #endregion Fields

        #region Properties

        #region IsLocked

        public static bool GetIsLocked(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLockedProperty);
        }

        public static void SetIsLocked(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLockedProperty, value);

            var panel = Extensions.GetParent<ScrollablePanel>(obj);
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        // Using a DependencyProperty as the backing store for IsLocked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(ScrollablePanel), new UIPropertyMetadata(false));

        #endregion IsLocked

        #region MaxItemWidth

        public double MaxItemWidth
        {
            get { return (double)GetValue(MaxItemWidthProperty); }
            set { SetValue(MaxItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxItemWidthProperty =
            DependencyProperty.Register("MaxItemWidth", typeof(double), typeof(ScrollablePanel), new UIPropertyMetadata(250.0));

        #endregion MaxItemWidth

        #region MinItemWidth

        public double MinItemWidth
        {
            get { return (double)GetValue(MinItemWidthProperty); }
            set { SetValue(MinItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinItemWidthProperty =
            DependencyProperty.Register("MinItemWidth", typeof(double), typeof(ScrollablePanel), new UIPropertyMetadata(100.0));

        #endregion MinItemWidth

        #region SelectedItem

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ScrollablePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, (s, e) =>
            {
                ((ScrollablePanel)s)._keepSelectedItemInView = true;
                //((ScrollablePanel)s).OnSelectedItemChanged();
            }));

        private void OnSelectedItemChanged()
        {
            //UpdateHorizontalOffset(GetViewportLeft(), GetViewportRight(), InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e)), GetContainer(SelectedItem));
        }

        #endregion SelectedItem

        #region HorizontalOffsetToSelectedItem

        public double HorizontalOffsetToSelectedItem
        {
            get { return (double)GetValue(HorizontalOffsetToSelectedItemProperty); }
            set { SetValue(HorizontalOffsetToSelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffsetToSelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetToSelectedItemProperty =
            DependencyProperty.Register("HorizontalOffsetToSelectedItem", typeof(double), typeof(ScrollablePanel), new UIPropertyMetadata(0.0, (s, e) =>
            {
                var p = (ScrollablePanel)s;
                if (p.ScrollToLeftCommand is DelegateCommand)
                {
                    (p.ScrollToLeftCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
                if (p.ScrollToRightCommand is DelegateCommand)
                {
                    (p.ScrollToRightCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            }));

        #endregion HorizontalOffsetToSelectedItem

        #region IsTruncatingItems

        public bool IsTruncatingItems
        {
            get { return (bool)GetValue(IsTruncatingItemsProperty); }
            set { SetValue(IsTruncatingItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTruncatingItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTruncatingItemsProperty =
            DependencyProperty.Register("IsTruncatingItems", typeof(bool), typeof(ScrollablePanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion IsTruncatingItems

        #region VisualChildrenCount

        protected override int VisualChildrenCount
        {
            get
            {
                return base.VisualChildrenCount + 2;
            }
        }

        #endregion VisualChildrenCount

        #region IsScroller

        public static bool GetIsScroller(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsScrollerProperty);
        }

        public static void SetIsScroller(DependencyObject obj, bool value)
        {
            obj.SetValue(IsScrollerProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsScroller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsScrollerProperty =
            DependencyProperty.RegisterAttached("IsScroller", typeof(bool), typeof(ScrollablePanel), new UIPropertyMetadata(false));

        #endregion IsScroller

        #region ScrollToLeftContent

        public object ScrollToLeftContent
        {
            get { return (object)GetValue(ScrollToLeftContentProperty); }
            set { SetValue(ScrollToLeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftContentProperty =
            DependencyProperty.Register("ScrollToLeftContent", typeof(object), typeof(ScrollablePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (s, e) =>
            {
                ((ScrollablePanel)s)._btnScrollLeft.Content = e.NewValue;
                if (e.NewValue is FrameworkElement)
                {
                    (e.NewValue as FrameworkElement).DoWhenLoaded(() =>
                    {
                        AttachScrollHandlers((ScrollablePanel)s, e.NewValue as FrameworkElement, ScrollToLeftCommandProperty);
                    });
                }
            }));

        #endregion ScrollToLeftContent

        #region ScrollToLeftCommand

        public ICommand ScrollToLeftCommand
        {
            get { return (ICommand)GetValue(ScrollToLeftCommandProperty); }
            set { SetValue(ScrollToLeftCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftCommandProperty =
            DependencyProperty.Register("ScrollToLeftCommand", typeof(ICommand), typeof(ScrollablePanel), new UIPropertyMetadata(null));

        #endregion ScrollToLeftCommand

        #region ScrollToRightContent

        public object ScrollToRightContent
        {
            get { return (object)GetValue(ScrollToRightContentProperty); }
            set { SetValue(ScrollToRightContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightContentProperty =
            DependencyProperty.Register("ScrollToRightContent", typeof(object), typeof(ScrollablePanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (s, e) =>
            {
                ((ScrollablePanel)s)._btnScrollRight.Content = e.NewValue;
                if (e.NewValue is FrameworkElement)
                {
                    (e.NewValue as FrameworkElement).DoWhenLoaded(() =>
                    {
                        AttachScrollHandlers((ScrollablePanel)s, e.NewValue as FrameworkElement, ScrollToRightCommandProperty);
                    });
                }
            }));

        private static void AttachScrollHandlers(ScrollablePanel p, FrameworkElement fe, DependencyProperty property)
        {
            Extensions.TraverseVisualTree(fe, e =>
            {
                if (GetIsScroller(e))
                {
                    if (e is ButtonBase)
                    {
                        (e as ButtonBase).SetBinding(ButtonBase.CommandProperty, new Binding(property.Name) { Source = p });
                    }
                }
                return Extensions.TraverseResult.Continue;
            });
        }

        #endregion ScrollToRightContent

        #region ScrollToRightCommand

        public ICommand ScrollToRightCommand
        {
            get { return (ICommand)GetValue(ScrollToRightCommandProperty); }
            set { SetValue(ScrollToRightCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightCommandProperty =
            DependencyProperty.Register("ScrollToRightCommand", typeof(ICommand), typeof(ScrollablePanel), new UIPropertyMetadata(null));

        #endregion ScrollToRightCommand

        #region SelectItemOnScroll

        public bool SelectItemOnScroll
        {
            get { return (bool)GetValue(SelectItemOnScrollProperty); }
            set { SetValue(SelectItemOnScrollProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItemOnScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectItemOnScrollProperty =
            DependencyProperty.Register("SelectItemOnScroll", typeof(bool), typeof(ScrollablePanel), new UIPropertyMetadata(true));

        #endregion SelectItemOnScroll

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == base.VisualChildrenCount)
                return _btnScrollLeft;
            if (index == base.VisualChildrenCount + 1)
                return _btnScrollRight;
            return base.GetVisualChild(index);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_btnScrollLeft != null)
            {
                _btnScrollLeft.Measure(availableSize);
            }
            if (_btnScrollRight != null)
            {
                _btnScrollRight.Measure(availableSize);
            }

            foreach (var item in InternalChildren.OfType<UIElement>())
            {
                item.Measure(availableSize);
            }

            var allItems = InternalChildren.OfType<UIElement>();
            var totalWidth = allItems.Count() > 0 ? InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width) : 0;
            var isTruncatingItems = totalWidth > availableSize.Width;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                IsTruncatingItems = isTruncatingItems;
                if (ScrollToLeftCommand is DelegateCommand)
                {
                    ((DelegateCommand)ScrollToLeftCommand).RaiseCanExecuteChanged();
                }
                if (ScrollToRightCommand is DelegateCommand)
                {
                    ((DelegateCommand)ScrollToRightCommand).RaiseCanExecuteChanged();
                }
            }));

            if (allItems.Count() > 0)
            {
                return new Size(
                    Math.Min(availableSize.Width, totalWidth),
                    Math.Min(availableSize.Height, InternalChildren.OfType<UIElement>().Max(e => e.DesiredSize.Height)));
            }
            return Size.Empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var x = 0.0;

            foreach (var item in InternalChildren.OfType<UIElement>().Where(e => GetIsLocked(e)))
            {
                Panel.SetZIndex(item, int.MaxValue);
                item.Arrange(new Rect(new Point(x, 0), new Size(item.DesiredSize.Width, finalSize.Height)));
                x += item.DesiredSize.Width;
            }

            if (IsTruncatingItems)
            {
                if (x > 0)
                    x += 2; /*add margin between scroller and fixed content*/
                x += _btnScrollLeft.DesiredSize.Width;
            }

            var viewportLeft = x;
            var viewportRight = finalSize.Width - (IsTruncatingItems ? _btnScrollRight.DesiredSize.Width : 0);

            var floatingItems = InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e));
            if (floatingItems.Count() > 0)
            {
                UpdateHorizontalOffset(viewportLeft, viewportRight, floatingItems, _keepSelectedItemInView || SelectItemOnScroll ? GetContainer(SelectedItem) : null);

                EnumerateFloatingItems(floatingItems, viewportLeft, (xAdj, item) =>
                {
                    ApplyMask(viewportLeft, viewportRight, item, xAdj);
                    item.Arrange(new Rect(new Point(xAdj, 0), new Size(item.DesiredSize.Width, finalSize.Height)));
                });
            }

            ArrangeScroller(_btnScrollLeft, viewportLeft - _btnScrollLeft.DesiredSize.Width, finalSize.Height);
            ArrangeScroller(_btnScrollRight, finalSize.Width - _btnScrollRight.DesiredSize.Width, finalSize.Height);

            return base.ArrangeOverride(finalSize);
        }

        private void ArrangeScroller(UIElement el, double xAt, double height)
        {
            if (el != null)
            {
                if (IsTruncatingItems)
                {
                    Panel.SetZIndex(el, int.MaxValue);
                    el.Visibility = Visibility.Visible;
                    el.Arrange(new Rect(new Point(xAt, 0), new Size(el.DesiredSize.Width, height)));
                }
                else
                {
                    el.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static void ApplyMask(double viewportLeft, double viewportRight, UIElement item, double xLeft)
        {
            var xRight = xLeft + item.DesiredSize.Width;

            if (xLeft < viewportLeft && Math.Abs(xLeft - viewportLeft) > 0.0001)
            {
                if (xRight > viewportLeft)
                {
                    item.Opacity = 1;
                    item.IsHitTestVisible = true;
                    var brush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = 0 });
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = (viewportLeft - xLeft) / item.DesiredSize.Width });
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = Math.Min(1, (viewportLeft - xLeft) / item.DesiredSize.Width + 0.05) });
                    item.OpacityMask = brush;
                }
                else
                {
                    item.Opacity = 0;
                    item.IsHitTestVisible = false;
                }
            }
            else
            {
                if (xLeft >= viewportRight)
                {
                    item.Opacity = 0;
                    item.IsHitTestVisible = false;
                }
                else
                {
                    item.OpacityMask = null;
                    item.IsHitTestVisible = true;
                    item.Opacity = 1;

                    if (xRight > viewportRight)
                    {
                        var brush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = Math.Max(0, (viewportRight - xLeft) / item.DesiredSize.Width - 0.05) });
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = (viewportRight - xLeft) / item.DesiredSize.Width });
                        item.OpacityMask = brush;
                    }
                }
            }
        }

        private double GetViewportLeft()
        {
            var wFixed = InternalChildren.OfType<UIElement>().Where(e => GetIsLocked(e)).Sum(e => e.DesiredSize.Width);

            if (IsTruncatingItems)
            {
                if (wFixed > 0)
                    wFixed += 2; /*add margin between scroller and fixed content*/
                wFixed += _btnScrollLeft.DesiredSize.Width;
            }

            return wFixed;
        }

        private double GetViewportRight()
        {
            return ActualWidth - (IsTruncatingItems ? _btnScrollRight.DesiredSize.Width : 0);
        }

        private void UpdateHorizontalOffset(double viewportLeft, double viewportRight, IEnumerable<UIElement> floatingItems, object targetItem)
        {
            var lastItemX = 0.0;
            var lastItem = (UIElement)null;

            EnumerateFloatingItems(floatingItems, viewportLeft, (x, item) =>
            {
                if (item == targetItem)
                {
                    if (x <= viewportLeft)
                    {
                        HorizontalOffsetToSelectedItem += viewportLeft - x;
                    }
                    else if (x + item.DesiredSize.Width > viewportRight)
                    {
                        HorizontalOffsetToSelectedItem += viewportRight - x - item.DesiredSize.Width;
                    }
                }
                lastItemX = x + item.DesiredSize.Width;
                lastItem = item;
            });

            if (IsTruncatingItems)
            {
                if (lastItemX < viewportRight)
                {
                    HorizontalOffsetToSelectedItem += viewportRight - lastItemX;
                }
            }
            else
            {
                HorizontalOffsetToSelectedItem = 0;
            }
        }

        private void EnumerateFloatingItems(IEnumerable<UIElement> items, double x0, Action<double, UIElement> action)
        {
            var xAdjusted = x0 + HorizontalOffsetToSelectedItem;
            foreach (var item in items)
            {
                action(xAdjusted, item);
                xAdjusted += item.DesiredSize.Width;
            }
        }

        private void ScrollToLeft()
        {
            _keepSelectedItemInView = false;

            var allItems = InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e)).ToList();

            if (SelectedItem == null || !SelectItemOnScroll)
            {
                ScrollToNearestLeftItem(allItems);
                return;
            }

            var container = GetContainer(SelectedItem);
            if ((container != null && !GetIsLocked(container)) || (SelectedItem is UIElement && !GetIsLocked(SelectedItem as UIElement)))
            {
                var index = allItems.IndexOf(container ?? SelectedItem as UIElement);
                if (index > 0)
                {
                    SelectedItem =
                        container != null
                        ? ItemsControl.GetItemsOwner(this).ItemContainerGenerator.ItemFromContainer(allItems[index - 1])
                        : allItems[index - 1];
                }
            }
            else
            {
                ScrollToNearestLeftItem(allItems);
            }
        }

        private void ScrollToNearestLeftItem(List<UIElement> allItems)
        {
            var viewportLeft = GetViewportLeft();
            EnumerateFloatingItems(allItems, viewportLeft, (x, item) =>
            {
                var xRight = x + item.DesiredSize.Width;
                if (x <= viewportLeft && (xRight >= viewportLeft || Math.Abs(xRight - viewportLeft) < 0.0001))
                {
                    UpdateHorizontalOffset(viewportLeft, GetViewportRight(), allItems, item);
                    InvalidateArrange();
                }
            });
        }

        private void ScrollToRight()
        {
            _keepSelectedItemInView = false;

            var allItems = InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e)).ToList();

            if (SelectedItem == null || !SelectItemOnScroll)
            {
                ScrollToNearestRightItem(allItems);
                return;
            }

            var container = GetContainer(SelectedItem);
            if ((container != null && !GetIsLocked(container)) || (SelectedItem is UIElement && !GetIsLocked(SelectedItem as UIElement)))
            {
                var index = allItems.IndexOf(container ?? SelectedItem as UIElement);
                if (index >= 0 && index < allItems.Count - 1)
                {
                    SelectedItem =
                        container != null
                        ? ItemsControl.GetItemsOwner(this).ItemContainerGenerator.ItemFromContainer(allItems[index + 1])
                        : allItems[index + 1];
                }
            }
            else
            {
                ScrollToNearestRightItem(allItems);
            }
        }

        private void ScrollToNearestRightItem(List<UIElement> allItems)
        {
            var viewportLeft = GetViewportLeft();
            var viewportRight = GetViewportRight();
            EnumerateFloatingItems(allItems.OfType<UIElement>(), viewportLeft, (x, item) =>
            {
                var xRight = x + item.DesiredSize.Width;
                if ((x <= viewportRight || Math.Abs(x - viewportRight) < 0.0001) && xRight >= viewportRight && Math.Abs(xRight - viewportRight) > 0.0001)
                {
                    UpdateHorizontalOffset(viewportLeft, viewportRight, allItems, item);
                    InvalidateArrange();
                }
            });
        }

        private UIElement GetContainer(object selectedItem)
        {
            if (selectedItem == null)
            {
                return null;
            }

            if (_owner == null)
            {
                _owner = ItemsControl.GetItemsOwner(this);
            }
            if (_owner != null)
            {
                return _owner.ItemContainerGenerator.ContainerFromItem(selectedItem) as UIElement;
            }

            return selectedItem as UIElement;
        }

        #endregion Methods
    }
}