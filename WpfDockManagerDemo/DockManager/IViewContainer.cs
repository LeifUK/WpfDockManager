using System;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    internal interface IViewContainer
    {
        string Title { get; }
        void AddUserControl(UserControl userControl);
        UserControl ExtractUserControl(int index);
        int GetUserControlCount();
        int GetCurrentTabIndex();
        UserControl GetUserControl(int index);
        IViewModel GetIViewModel(int index);

        event EventHandler SelectionChanged;
        event EventHandler TabClosed;
    }
}
