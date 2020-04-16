using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal interface IDocumentContainer
    {
        void AddUserControl(UserControl userControl);
        UserControl ExtractUserControl(int index);
        int GetUserControlCount();
        int GetCurrentTabIndex();
        UserControl GetUserControl(int index);
    }
}
