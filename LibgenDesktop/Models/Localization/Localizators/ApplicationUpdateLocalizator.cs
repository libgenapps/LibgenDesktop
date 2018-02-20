using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class ApplicationUpdateLocalizator : Localizator
    {
        public ApplicationUpdateLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            UpdateAvailable = Format(translation => translation?.UpdateAvailable);
            Download = Format(translation => translation?.Download);
            DownloadAndInstall = Format(translation => translation?.DownloadAndInstall);
            SkipThisVersion = Format(translation => translation?.SkipThisVersion);
            Cancel = Format(translation => translation?.Cancel);
            Interrupt = Format(translation => translation?.Interrupt);
            Interrupting = Format(translation => translation?.Interrupting);
            InterruptPromptTitle = Format(translation => translation?.InterruptPromptTitle);
            InterruptPromptText = Format(translation => translation?.InterruptPromptText);
            Error = Format(translation => translation?.Error);
            IncompleteDownload = Format(translation => translation?.IncompleteDownload);
            Close = Format(translation => translation?.Close);
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
            Format(translation => translation?.NewVersion, new { version, date = Formatter.ToFormattedDateString(date) });

        private string Format(Func<Translation.ApplicationUpdateTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.ApplicationUpdate), templateArguments);
        }
    }
}
