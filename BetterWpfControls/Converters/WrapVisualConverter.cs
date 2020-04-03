using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace BetterWpfControls.Converters
{
    internal class WrapVisualConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ic = parameter as ItemsControl;
            var fe = ic.ItemContainerGenerator.ContainerFromItem(value) as FrameworkElement ?? value as FrameworkElement;
            if (fe != null)
            {
                var ti = fe as TabItem;
                if (ti != null)
                {
                    return ti.Header;
                }
                return new Rectangle() { Width = fe.ActualWidth, Height = fe.ActualHeight, Fill = new VisualBrush(fe) { Stretch = Stretch.None }, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
