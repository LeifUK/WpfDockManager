using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal interface IViewContainer
    {
        string Title { get; }
        string URL { get; }
        void AddUserControl(UserControl userControl);
        void InsertUserControl(int index, UserControl userControl);
        UserControl ExtractUserControl(int index);
        int GetUserControlCount();
        int GetCurrentTabIndex();
        UserControl GetUserControl(int index);
        IViewModel GetIViewModel(int index);

        event EventHandler SelectionChanged;
        event Events.TabClosedEventHandler TabClosed;
        event EventHandler FloatTabRequest;

        double FontSize { set; }
        FontFamily FontFamily { set; }
        CornerRadius TabCornerRadius { set; }
        Brush Background { set; }
        Brush GapBrush { set; }
        double GapHeight { set; }
        Brush ButtonForeground { set; }
        TabStyle SelectedTabStyle { set; }
        TabStyle UnselectedTabStyle { set; }
        Brush ActiveScrollIndicatorBrush { set; }
        Brush InactiveScrollIndicatorBrush { set; }
        Style TabItemStyle { set; }
        Style ListButtonStyle { set; }
    }
}
