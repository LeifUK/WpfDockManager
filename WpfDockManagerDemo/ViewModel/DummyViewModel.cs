using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.ViewModel
{
    class DummyViewModel : WpfOpenControls.DockManager.IViewModel
    {
        public string URL { get; set; }
        public string Title { get; set; }

        public bool CanClose
        {
            get
            {
                return true;
            }
        }

        public bool CanFloat
        {
            get
            {
                return true;
            }
        }

        public bool HasChanged
        {
            get
            {
                return true;
            }
        }

        public void Save()
        {
            // Do nowt!
        }

        public void Close()
        {
            // Do nowt!
        }
    }
}
