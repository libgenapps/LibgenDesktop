using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LibgenDesktop.Infrastructure
{
    internal static class WindowExtensions
    {
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SETICON = 0x0080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        public static void AddWindowMinimizeButton(this Window window)
        {
            AddWindowStyle(window, WS_MINIMIZEBOX);
        }

        public static void AddWindowMaximizeButton(this Window window)
        {
            AddWindowStyle(window, WS_MAXIMIZEBOX);
        }

        public static void AddWindowCloseButton(this Window window)
        {
            AddWindowStyle(window, WS_SYSMENU);
        }

        public static void RemoveWindowMinimizeButton(this Window window)
        {
            RemoveWindowStyle(window, WS_MINIMIZEBOX);
        }

        public static void RemoveWindowMaximizeButton(this Window window)
        {
            RemoveWindowStyle(window, WS_MAXIMIZEBOX);
        }

        public static void RemoveWindowCloseButton(this Window window)
        {
            RemoveWindowStyle(window, WS_SYSMENU);
        }

        public static void RemoveWindowIcon(this Window window)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            int windowExStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
            SetWindowLong(windowHandle, GWL_EXSTYLE, windowExStyle | WS_EX_DLGMODALFRAME);
            SendMessage(windowHandle, WM_SETICON, IntPtr.Zero, IntPtr.Zero);
            SendMessage(windowHandle, WM_SETICON, new IntPtr(1), IntPtr.Zero);
            SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        private static void AddWindowStyle(Window window, int styleAttribute)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            int windowStyle = GetWindowLong(windowHandle, GWL_STYLE);
            SetWindowLong(windowHandle, GWL_STYLE, windowStyle | styleAttribute);
        }

        private static void RemoveWindowStyle(Window window, int styleAttribute)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            int windowStyle = GetWindowLong(windowHandle, GWL_STYLE);
            SetWindowLong(windowHandle, GWL_STYLE, windowStyle & ~styleAttribute);
        }
    }
}
