using System;

namespace LibgenDesktop.Infrastructure
{
    internal interface IWindowContext
    {
        object DataContext { get; }

        event EventHandler Activated;
        event EventHandler Closed;
        event EventHandler Closing;
        event EventHandler Showing;

        void Close();
        void CloseDialog(bool dialogResult);
        void Focus();
        void Show(bool showMaximized = false);
        bool? ShowDialog(int? width = null, int? height = null, bool showMaximized = false);
    }
}