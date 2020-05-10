using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class ApplicationUpdateWindowLocalizator : Localizator<Translation.ApplicationUpdateTranslation>
    {
        public ApplicationUpdateWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.ApplicationUpdate)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            UpdateAvailable = Format(section => section?.UpdateAvailable);
            Download = Format(section => section?.Download);
            DownloadAndInstall = Format(section => section?.DownloadAndInstall);
            SkipThisVersion = Format(section => section?.SkipThisVersion);
            Cancel = Format(section => section?.Cancel);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            InterruptPromptTitle = Format(section => section?.InterruptPromptTitle);
            InterruptPromptText = Format(section => section?.InterruptPromptText);
            Error = Format(section => section?.Error);
            IncompleteDownload = Format(section => section?.IncompleteDownload);
            Close = Format(section => section?.Close);
        }

        public string WindowTitle { get; }
        public string UpdateAvailable { get; }
        public string Download { get; }
        public string DownloadAndInstall { get; }
        public string SkipThisVersion { get; }
        public string Cancel { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string InterruptPromptTitle { get; }
        public string InterruptPromptText { get; }
        public string Error { get; }
        public string IncompleteDownload { get; }
        public string Close { get; }

        public string GetNewVersionString(string version, DateTime date) =>
            Format(section => section?.NewVersion, new { version, date = Formatter.ToFormattedDateString(date) });
    }
}
