using LibgenDesktop.Views.Windows;

namespace LibgenDesktop.Infrastructure
{
    internal class MessageBox : IMessageBox
    {
        public void ShowMessage(string title, string text, string ok, IWindowContext parentWindowContext)
        {
            MessageBoxWindow.ShowMessage(title, text, ok, parentWindowContext);
        }

        public bool ShowPrompt(string title, string text, string yes, string no, IWindowContext parentWindowContext)
        {
            return MessageBoxWindow.ShowPrompt(title, text, yes, no, parentWindowContext);
        }
    }
}
