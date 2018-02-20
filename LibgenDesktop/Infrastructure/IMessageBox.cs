namespace LibgenDesktop.Infrastructure
{
    internal interface IMessageBox
    {
        void ShowMessage(string title, string text, string ok, IWindowContext parentWindowContext);
        bool ShowPrompt(string title, string text, string yes, string no, IWindowContext parentWindowContext);
    }
}