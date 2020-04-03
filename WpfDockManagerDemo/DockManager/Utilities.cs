using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    public static class Utilities
    {
        public const int VK_LBUTTON = 0x01;

        public static bool IsLeftMouseButtonDown()
        {
            return (User32.GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }
    }
}
