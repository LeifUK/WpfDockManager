using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BetterWpfControls
{
    public class TabItem : System.Windows.Controls.TabItem
    {
        #region .ctors

        static TabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItem), new FrameworkPropertyMetadata(typeof(TabItem)));
        }

        #endregion .ctors
    }
}
