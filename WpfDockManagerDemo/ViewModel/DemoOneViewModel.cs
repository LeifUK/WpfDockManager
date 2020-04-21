using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.ViewModel
{
    public class DemoOneViewModel : WpfDockManagerDemo.DockManager.IDocument
    {
        public DemoOneViewModel()
        {
            Title = "Demo One View Model";
        }

        public long ID { get; set; }

        public string Title { get; set; }

        public event DockManager.CloseRequestHandler CloseRequest;

        public bool CanClose
        {
            get
            {
                //bool canClose = true;
                //if (CloseRequest != null)
                //{
                //    canClose = CloseRequest();
                //}
                //return canClose;
                return (System.Windows.Forms.MessageBox.Show("There are unsaved changes in the document. Do you wish to close without saving the changes?", "Close " + Title, System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK);
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
