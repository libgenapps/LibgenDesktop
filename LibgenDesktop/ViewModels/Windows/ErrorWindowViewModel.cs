using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators;

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

        private string DefaultWindowTitle => "Error";
        private string DefaultUnexpectedError => "An unexpected error has occurred:";
        private string DefaultCopy => "COPY TO CLIPBOARD";
        private string DefaultClose => "CLOSE";

        public Command CopyErrorCommand { get; }

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
