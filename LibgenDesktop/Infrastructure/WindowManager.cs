using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;

namespace LibgenDesktop.Infrastructure
{
    internal static class WindowManager
    {
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SETICON = 0x0080;

        private static List<WindowContext> createdWindowContexts;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        static WindowManager()
        {
            createdWindowContexts = new List<WindowContext>();
        }

        public static int ScreenWidth
        {
            get
            {
                return (int)SystemParameters.WorkArea.Width;
            }
        }

        public static int ScreenHeight
        {
            get
            {
                return (int)SystemParameters.WorkArea.Height;
            }
        }

        public static IWindowContext CreateWindow(RegisteredWindows.WindowKey windowKey, object viewModel = null, IWindowContext parentWindowContext = null)
        {
            RegisteredWindows.RegisteredWindow registeredWindow = RegisteredWindows.AllWindows[windowKey];
            if (viewModel == null)
            {
                viewModel = Activator.CreateInstance(registeredWindow.ViewModelType);
                if (viewModel == null)
                {
                    throw new InvalidOperationException($"There was an error while trying to create an instance of {registeredWindow.WindowType.FullName} view model class.");
                }
            }
            Window window = Activator.CreateInstance(registeredWindow.WindowType) as Window;
            if (window == null)
            {
                throw new InvalidOperationException($"There was an error while trying to create an instance of {registeredWindow.WindowType.FullName} window class.");
            }
            window.DataContext = viewModel;
            Window parentWindow = (parentWindowContext as WindowContext)?.Window;
            WindowContext result = new WindowContext(window, viewModel, parentWindow);
            result.Closed += WindowContext_Closed;
            createdWindowContexts.Add(result);
            return result;
        }

        public static IWindowContext GetWindowContext(object viewModel)
        {
            return createdWindowContexts.FirstOrDefault(windowContext => ReferenceEquals(windowContext.DataContext, viewModel));
        }

        public static void ExecuteActionInBackground(Action action)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, action);
        }

        public static void ShowMessageBox(string title, string text, IWindowContext parentWindowContext = null)
        {
            if (parentWindowContext != null)
            {
                MessageBox.Show((parentWindowContext as WindowContext)?.Window, text, title);
            }
            else
            {
                MessageBox.Show(text, title);
            }
        }

        public static bool ShowPrompt(string title, string text, IWindowContext parentWindowContext = null)
        {
            if (parentWindowContext != null)
            {
                return MessageBox.Show((parentWindowContext as WindowContext)?.Window, text, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
            else
            {
                return MessageBox.Show(text, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
        }

        public static OpenFileDialogResult ShowOpenFileDialog(OpenFileDialogParameters openFileDialogParameters)
        {
            if (openFileDialogParameters == null)
            {
                throw new ArgumentNullException(nameof(openFileDialogParameters));
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!String.IsNullOrEmpty(openFileDialogParameters.DialogTitle))
            {
                openFileDialog.Title = openFileDialogParameters.DialogTitle;
            }
            if (!String.IsNullOrEmpty(openFileDialogParameters.Filter))
            {
                openFileDialog.Filter = openFileDialogParameters.Filter;
            }
            openFileDialog.Multiselect = openFileDialogParameters.Multiselect;
            if (!String.IsNullOrEmpty(openFileDialogParameters.InitialDirectory))
            {
                openFileDialog.InitialDirectory = openFileDialogParameters.InitialDirectory;
            }
            OpenFileDialogResult result = new OpenFileDialogResult();
            result.DialogResult = openFileDialog.ShowDialog() == true;
            result.SelectedFilePaths = result.DialogResult ? openFileDialog.FileNames.ToList() : new List<string>();
            return result;
        }

        public static SaveFileDialogResult ShowSaveFileDialog(SaveFileDialogParameters saveFileDialogParameters)
        {
            if (saveFileDialogParameters == null)
            {
                throw new ArgumentNullException(nameof(saveFileDialogParameters));
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (!String.IsNullOrEmpty(saveFileDialogParameters.DialogTitle))
            {
                saveFileDialog.Title = saveFileDialogParameters.DialogTitle;
            }
            if (!String.IsNullOrEmpty(saveFileDialogParameters.Filter))
            {
                saveFileDialog.Filter = saveFileDialogParameters.Filter;
            }
            saveFileDialog.OverwritePrompt = saveFileDialogParameters.OverwritePrompt;
            if (!String.IsNullOrEmpty(saveFileDialogParameters.InitialDirectory))
            {
                saveFileDialog.InitialDirectory = saveFileDialogParameters.InitialDirectory;
            }
            if (!String.IsNullOrEmpty(saveFileDialogParameters.InitialFileName))
            {
                saveFileDialog.FileName = saveFileDialogParameters.InitialFileName;
            }
            SaveFileDialogResult result = new SaveFileDialogResult();
            result.DialogResult = saveFileDialog.ShowDialog() == true;
            result.SelectedFilePath = result.DialogResult ? saveFileDialog.FileName : null;
            return result;
        }

        public static void RemoveWindowMaximizeButton(Window window)
        {
            RemoveWindowStyle(window, WS_MAXIMIZEBOX);
        }

        public static void RemoveWindowMinimizeButton(Window window)
        {
            RemoveWindowStyle(window, WS_MINIMIZEBOX);
        }

        public static void RemoveWindowIcon(Window window)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            int windowExStyle = GetWindowLong(windowHandle, GWL_EXSTYLE);
            SetWindowLong(windowHandle, GWL_EXSTYLE, windowExStyle | WS_EX_DLGMODALFRAME);
            SendMessage(windowHandle, WM_SETICON, IntPtr.Zero, IntPtr.Zero);
            SendMessage(windowHandle, WM_SETICON, new IntPtr(1), IntPtr.Zero);
            SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        public static void SetClipboardText(string text)
        {
            Clipboard.SetDataObject(text);
        }

        private static void RemoveWindowStyle(Window window, int styleAttribute)
        {
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            int windowStyle = GetWindowLong(windowHandle, GWL_STYLE);
            SetWindowLong(windowHandle, GWL_STYLE, windowStyle & ~styleAttribute);
        }

        private static void WindowContext_Closed(object sender, EventArgs e)
        {
            WindowContext closedWindowContext = (WindowContext)sender;
            createdWindowContexts.Remove(closedWindowContext);
            closedWindowContext.Closed -= WindowContext_Closed;
        }
    }
}
