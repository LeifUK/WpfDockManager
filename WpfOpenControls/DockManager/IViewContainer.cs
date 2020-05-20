using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfOpenControls.DockManager
{
    internal interface IViewContainer
    {
        string Title { get; }
        void AddUserControl(UserControl userControl);
        void InsertUserControl(int index, UserControl userControl);
        UserControl ExtractUserControl(int index);
        int GetUserControlCount();
        int GetCurrentTabIndex();
        UserControl GetUserControl(int index);
        IViewModel GetIViewModel(int index);

        event EventHandler SelectionChanged;
        event EventHandler TabClosed;
        event EventHandler FloatTabRequest;

        double FontSize { set; }
        string FontFamily { set; }
        CornerRadius TabCornerRadius { set; }
        Brush Background { set; }
        Brush ButtonForeground { set; }
        Thickness SelectedTabBorderThickness { set; }
        Brush SelectedTabBorderBrush { set; }
        Thickness UnselectedTabBorderThickness { set; }
        Brush UnselectedTabBorderBrush { set; }
        Brush SelectedTabHeaderBackground { set; }
        Brush UnselectedTabHeaderBackground { set; }
        Brush SelectedTabHeaderForeground { set; }
        Brush UnselectedTabHeaderForeground { set; }
        Brush ActiveScrollIndicatorBrush { set; }
        Brush InactiveScrollIndicatorBrush { set; }
    }
}
