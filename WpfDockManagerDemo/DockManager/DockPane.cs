using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal abstract class DockPane : Grid
    {
        public DockPane()
        {


        }
        protected abstract void MenuButton_Click(object sender, RoutedEventArgs e);

    }
}
