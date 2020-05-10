using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LibgenDesktop.Infrastructure
{
    internal static class WindowManager
    {
        private static readonly List<WindowContext> createdWindowContexts;
        private static readonly FieldInfo menuDropAlignmentField;

        static WindowManager()
        {
            createdWindowContexts = new List<WindowContext>();
            menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            ResetPopupAlignment();
            SystemParameters.StaticPropertyChanged += (sender, e) => ResetPopupAlignment();
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

        [SuppressMessage("Style", "IDE0019:Use pattern matching")]
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

        public static void ShowMessage(string title, string text, string ok, IWindowContext parentWindowContext)
        {
            RegisteredWindows.MessageBox.ShowMessage(title, text, ok, parentWindowContext);
        }

        public static bool ShowPrompt(string title, string text, string yes, string no, IWindowContext parentWindowContext)
        {
            return RegisteredWindows.MessageBox.ShowPrompt(title, text, yes, no, parentWindowContext);
        }

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
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

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
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

        [SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        public static SelectFolderDialogResult ShowSelectFolderDialog(SelectFolderDialogParameters selectFolderDialogParameters)
        {
            using (CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog())
            {
                commonOpenFileDialog.IsFolderPicker = true;
                commonOpenFileDialog.Title = selectFolderDialogParameters.DialogTitle;
                commonOpenFileDialog.InitialDirectory = selectFolderDialogParameters.InitialDirectory;
                SelectFolderDialogResult result = new SelectFolderDialogResult();
                result.DialogResult = commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok;
                result.SelectedFolderPath = result.DialogResult ? commonOpenFileDialog.FileName : null;
                return result;
            }
        }

        public static void SetClipboardText(string text)
        {
            Clipboard.SetDataObject(text);
        }

        private static void ResetPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null)
            {
                menuDropAlignmentField.SetValue(null, false);
            }
        }

        private static void WindowContext_Closed(object sender, EventArgs e)
        {
            WindowContext closedWindowContext = (WindowContext)sender;
            createdWindowContexts.Remove(closedWindowContext);
            closedWindowContext.Closed -= WindowContext_Closed;
        }
    }
}
