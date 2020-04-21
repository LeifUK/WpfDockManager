using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.ViewModel
{
    public class DemoThreeViewModel : WpfDockManagerDemo.DockManager.IDocument
    {
        public DemoThreeViewModel()
        {
            Title = "Demo Three View Model";
        }

        public long ID { get; set; }

        public string Title { get; set; }

        public event DockManager.CloseRequestHandler CloseRequest;

        public bool CanClose
        {
            get
            {
                bool canClose = true;
                if (CloseRequest != null)
                {
                    canClose = CloseRequest();
                }
                return canClose;
            }
        }

        public bool CanFloat
        {
            get
            {
                return true;
            }
        }
    }
}
