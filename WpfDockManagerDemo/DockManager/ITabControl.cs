using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDockManagerDemo.DockManager
{
    interface ITabControl
    {
        event System.EventHandler SelectionChanged;
        event System.EventHandler TabClosed;

        void AddUserControl(UserControl userControl);
        UserControl RemoveAt(int index);
        UserControl GetAt(int index);
        int Count { get; }
        int SelectedIndex { get ; }
        UserControl SelectedItem { get; }
    }
}
