using System.Windows.Controls;

namespace WpfDockManagerDemo.View
{
    /// <summary>
    /// Interaction logic for DemoThreeView.xaml
    /// </summary>
    public partial class DemoFourView : UserControl, DockManager.IView
    {
        public DemoFourView()
        {
            InitializeComponent();
        }

        private DockManager.IDocument _iDocument;

        public DockManager.IDocument IDocument
        {
            get
            {
                return _iDocument;
            }
            set
            {
                if (IDocument != value)
                {
                    if (IDocument != null)
                    {
                        IDocument.CloseRequest -= _iDocument_CloseRequest;
                    }

                    _iDocument = value;
                    _iDocument.CloseRequest += _iDocument_CloseRequest;
                }
            }
        }

        private bool _iDocument_CloseRequest()
        {
            // Simple demonstration: in practive check if there are unsaved changes ... 
            return (System.Windows.Forms.MessageBox.Show("There are unsaved changes in the document. Do you wish to close without saving the changes?", "Close " + _iDocument.Title, System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK);
        }
    }
}
