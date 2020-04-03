using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    public interface IDocument
    {
        // A unique ID
        long ID { get; }
        // A user friendly title
        string Title { get; set; }

        void Closing();

        bool CanClose { get; }
        bool CanFloat { get; }
    }
}
