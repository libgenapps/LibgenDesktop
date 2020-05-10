using System;
using System.ComponentModel;
using System.Windows;

namespace LibgenDesktop.Infrastructure
{
    internal class WindowContext : IWindowContext
    {
        private readonly Window parentWindow;

        public WindowContext(Window window, object dataContext, Window parentWindow)
        {
            Window = window;
            DataContext = dataContext;
            this.parentWindow = parentWindow;
            Window.Activated += Window_Activated;
            Window.Closing += Window_Closing;
            Window.Closed += Window_Closed;
        }

        public Window Window { get; }
        public object DataContext { get; }

        public event EventHandler Activated;
        public event EventHandler Showing;
        public event EventHandler Closing;
        public event EventHandler Closed;

        public void Show(int? width = null, int? height = null, bool showMaximized = false)
        {
            if (!Window.IsVisible)
            {
                OnShowing();
                if (width.HasValue)
                {
                    Window.Width = width.Value;
                }
                if (height.HasValue)
                {
                    Window.Height = height.Value;
                }
                Window.WindowState = GetWindowState(showMaximized);
                Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Window.Show();
            }
            else
            {
                if (Window.WindowState == WindowState.Minimized)
                {
                    Window.WindowState = WindowState.Normal;
                }
                else
                {
                    Window.Activate();
                }
            }
        }

        public bool? ShowDialog(int? width = null, int? height = null, bool showMaximized = false)
        {
            OnShowing();
            if (width.HasValue)
            {
                Window.Width = width.Value;
            }
            if (height.HasValue)
            {
                Window.Height = height.Value;
            }
            if (parentWindow != null)
            {
                Window.Owner = parentWindow;
                Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            return ShowDialog(showMaximized, parentWindow == null);
        }

        public void Close()
        {
            Window.Close();
        }

        public void CloseDialog(bool dialogResult)
        {
            Window.DialogResult = dialogResult;
        }

        public void Focus()
        {
            Window.Focus();
        }

        public void AddWindowMinimizeButton()
        {
            Window.AddWindowMinimizeButton();
        }

        public void AddWindowMaximizeButton()
        {
            Window.AddWindowMaximizeButton();
        }

        public void AddWindowCloseButton()
        {
            Window.AddWindowCloseButton();
        }

        public void RemoveWindowMinimizeButton()
        {
            Window.RemoveWindowMinimizeButton();
        }

        public void RemoveWindowMaximizeButton()
        {
            Window.RemoveWindowMaximizeButton();
        }

        public void RemoveWindowCloseButton()
        {
            Window.RemoveWindowCloseButton();
        }

        protected virtual void OnActivated()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnShowing()
        {
            Showing?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClosing()
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private static WindowState GetWindowState(bool isMaximized)
        {
            return isMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private bool? ShowDialog(bool showMaximized, bool showInTaskbar)
        {
            Window.ShowInTaskbar = showInTaskbar;
            Window.WindowState = GetWindowState(showMaximized);
            return Window.ShowDialog();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            OnActivated();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            OnClosing();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            OnClosed();
            Window.Activated -= Window_Activated;
            Window.Closing -= Window_Closing;
            Window.Closed -= Window_Closed;
        }
    }
}
