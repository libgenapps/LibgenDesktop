using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class ErrorWindowViewModel : ViewModel
    {
        public ErrorWindowViewModel(string error, Language currentLanguage)
        {
            Error = error;
            if (currentLanguage != null)
            {
                try
                {
                    ErrorWindowLocalizator localization = currentLanguage.ErrorWindow;
                    if (localization != null)
                    {
                        WindowTitle = !String.IsNullOrWhiteSpace(localization.WindowTitle) ? localization.WindowTitle : DefaultWindowTitle;
                        UnexpectedError = !String.IsNullOrWhiteSpace(localization.UnexpectedError) ? localization.UnexpectedError : DefaultUnexpectedError;
                        Copy = !String.IsNullOrWhiteSpace(localization.Copy) ? localization.Copy : DefaultCopy;
                        Close = !String.IsNullOrWhiteSpace(localization.Close) ? localization.Close : DefaultClose;
                    }
                    else
                    {
                        PopulateDefaultMessages();
                    }
                }
                catch
                {
                    PopulateDefaultMessages();
                }
            }
            else
            {
                PopulateDefaultMessages();
            }
            CopyErrorCommand = new Command(CopyErrorToClipboard);
        }

        public string Error { get; }
        public string WindowTitle { get; private set; }
        public string UnexpectedError { get; private set; }
        public string Copy { get; private set; }
        public string Close { get; private set; }

        public Command CopyErrorCommand { get; }

        private static string DefaultWindowTitle => "Error";
        private static string DefaultUnexpectedError => "An unexpected error has occurred:";
        private static string DefaultCopy => "COPY TO CLIPBOARD";
        private static string DefaultClose => "CLOSE";

        private void CopyErrorToClipboard()
        {
            WindowManager.SetClipboardText(Error);
        }

        private void PopulateDefaultMessages()
        {
            WindowTitle = DefaultWindowTitle;
            UnexpectedError =  DefaultUnexpectedError;
            Copy = DefaultCopy;
            Close = DefaultClose;
        }
    }
}
