using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Windows.Documents;

namespace BetterWpfControls
{
    public class ContentAdorner : Adorner
    {
        #region .ctors

        public ContentAdorner(UIElement element, UIElement adornedElement)
            : base(adornedElement)
        {
            _element = element;
            AddLogicalChild(element);
            AddVisualChild(element);
        }

        #endregion .ctors

        #region Fields

        private UIElement _element;
        private TranslateTransform _tTranslate;

        #endregion Fields

        #region Properties

        public Point Offset
        {
            get { return (Point)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Point), typeof(ContentAdorner), new UIPropertyMetadata(null));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ContentAdorner), new UIPropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ContentAdorner), new UIPropertyMetadata(0.0));

        #endregion Properties

        #region Methods

        protected override Int32 VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(Int32 index)
        {
            return this._element;
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                return new UIElement[] { _element }.GetEnumerator();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _element.Measure(availableSize);
            return _element.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _element.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var group = new TransformGroup();
            group.Children.Add((Transform)transform);
            group.Children.Add(_tTranslate = new TranslateTransform(HorizontalOffset, VerticalOffset));
            return group;
        }

        #endregion Methods
    }
}
