using System.Windows;
using System.Windows.Controls;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Windows
{
    public partial class MessageBoxWindow
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public static void ShowMessage(string title, string text, string ok, IWindowContext parentWindowContext = null)
        {
            MessageBoxWindow messageBoxWindow = CreateWindow(title, text, parentWindowContext);
            messageBoxWindow.AddButton(ok, true, true, true);
            messageBoxWindow.ShowDialog();
        }

        public static bool ShowPrompt(string title, string text, string yes, string no, IWindowContext parentWindowContext = null)
        {
            MessageBoxWindow messageBoxWindow = CreateWindow(title, text, parentWindowContext);
            messageBoxWindow.AddButton(yes, true, false, false);
            messageBoxWindow.AddButton(no, false, false, true);
            return messageBoxWindow.ShowDialog() == true;
        }

        private static MessageBoxWindow CreateWindow(string title, string text, IWindowContext parentWindowContext)
        {
            Window parentWindow = (parentWindowContext as WindowContext)?.Window;
            MessageBoxWindow messageBoxWindow = new MessageBoxWindow
            {
                Title = title,
                Owner = parentWindow,
                WindowStartupLocation = parentWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
                ShowInTaskbar = parentWindow == null
            };
            messageBoxWindow.messageTextBlock.Text = text;
            return messageBoxWindow;
        }

        private void AddButton(string text, bool? dialogResult, bool isDefault, bool isCancel)
        {
            Button button = new Button
            {
                Content = text,
                IsDefault = isDefault,
                IsCancel = isCancel
            };
            button.Click += (sender, e) => DialogResult = dialogResult;
            buttonPanel.Children.Add(button);
        }
    }
}
