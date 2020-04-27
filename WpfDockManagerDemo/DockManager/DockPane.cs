using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace WpfDockManagerDemo.DockManager
{
    internal abstract class DockPane : Grid
    {
        public DockPane(IDocumentContainer iDocumentContainer)
        {
            IDocumentContainer = iDocumentContainer;
            Children.Add(iDocumentContainer as System.Windows.UIElement);
        }

        public event EventHandler Close;
        public event FloatEventHandler Float;
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

        protected void FireUngroupCurrent()
        {
            UngroupCurrent?.Invoke(this, null);
        }

        protected void FireUngroup()
        {
            Ungroup?.Invoke(this, null);
        }

        public readonly IDocumentContainer IDocumentContainer;
    }
}
