using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager
{
    public static class Utilities
    {
        internal static System.Windows.ResourceDictionary GetResourceDictionary()
        {
            return (System.Windows.ResourceDictionary)App.LoadComponent(new System.Uri("/WpfDockManagerDemo;component/DockManager/Dictionary.xaml", System.UriKind.Relative));
        }

        public const int VK_LBUTTON = 0x01;

        public static bool IsLeftMouseButtonDown()
        {
            return (User32.GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        public static void SendLeftMouseButtonDown(IntPtr wndHandle)
        {
            var inputMouseDown = new User32.INPUT();
            inputMouseDown.Type = User32.MOUSEINPUTTYPE; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = User32.LEFTMOUSEDOWN;

            var inputs = new User32.INPUT[] { inputMouseDown };
            User32.SendInput((uint)inputs.Length, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(User32.INPUT)));
        }

        // The WPF method does not work properly -> call into User32.dll
        public static Point GetCursorPosition()
        {
            if (User32.GetCursorPos(out User32.POINT point) == false)
            {
                return new Point(0, 0);
            }
            return new Point(point.X, point.Y);
        }
    }
}
