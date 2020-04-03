using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BetterWpfControls.Components
{
    public static class Extensions
    {
        #region Methods

        public static T GetParent<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            var t = obj;
            while (!(t is T) && (t = VisualTreeHelper.GetParent(t)) != null) ;
            return t as T;
        }

        public static void DoWhenLoaded(this FrameworkElement fe, Action action)
        {
            if (fe.IsLoaded)
            {
                action();
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = (sender, args) =>
                {
                    fe.Loaded -= handler;
                    action();
                };
                fe.Loaded += handler;
            }
        }

        public static void TraverseVisualTree(this DependencyObject obj, Action<DependencyObject> action)
        {
            TraverseVisualTree(obj, (o) => { action(o); return TraverseResult.Continue; });
        }

        public static TraverseResult TraverseVisualTree(this DependencyObject obj, Func<DependencyObject, TraverseResult> action)
        {
            if (obj == null)
            {
                return TraverseResult.Break;
            }
            if (action(obj) == TraverseResult.Break)
            {
                return TraverseResult.Break;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                if (TraverseVisualTree(VisualTreeHelper.GetChild(obj, i), action) == TraverseResult.Break)
                {
                    return TraverseResult.Break;
                }
            }
            return TraverseResult.Continue;
        }

        #endregion Methods

        #region Internal Classes

        public enum TraverseResult
        {
            Continue,
            Break
        }

        #endregion Internal Classes
    }
}