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

        bool CanClose { get; }
        bool CanFloat { get; }
        /*
         * Return true if there are edits that need to be saved
         * Not used by Tool view model
         */
        bool HasChanged { get; }
        void Save();
        void Close();
    }
}
