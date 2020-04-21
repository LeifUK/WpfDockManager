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
            _documentContainer = new DocumentContainer();
            Children.Add(_documentContainer);
            IDocumentContainer = _documentContainer;
        }

        public event EventHandler Close;
        public event EventHandler Float;
        public event EventHandler UngroupCurrent;
        public event EventHandler Ungroup;

        protected void FireClose()
        {
            Close?.Invoke(this, null);
        }

        protected void FireFloat()
        {
            Float?.Invoke(this, null);
        }

        protected void FireUngroupCurrent()
        {
            UngroupCurrent?.Invoke(this, null);
        }

        protected void FireUngroup()
        {
            Ungroup?.Invoke(this, null);
        }

        protected DocumentContainer _documentContainer;
        public readonly IDocumentContainer IDocumentContainer;
    }
}
