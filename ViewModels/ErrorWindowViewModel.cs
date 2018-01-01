using System.Windows;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.ViewModels
{
    internal class ErrorWindowViewModel : ViewModel
    {
        public ErrorWindowViewModel(string error)
        {
            Error = error;
            CopyErrorCommand = new Command(CopyErrorToClipboard);
        }

        public string Error { get; }

        public Command CopyErrorCommand { get; }

        private void CopyErrorToClipboard()
        {
            WindowManager.SetClipboardText(Error);
        }
    }
}
