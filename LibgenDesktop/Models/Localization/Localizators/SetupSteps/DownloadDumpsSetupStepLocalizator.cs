using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class DownloadDumpsSetupStepLocalizator : Localizator<Translation.DownloadDumpsStepTranslation>
    {
        public DownloadDumpsSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.DownloadDumpsStep)
        {
            NonFictionDumpName = Format(section => section?.NonFictionDumpName);
            FictionDumpName = Format(section => section?.FictionDumpName);
            SciMagArticlesDumpName = Format(section => section?.SciMagArticlesDumpName);
            QueuedStatus = Format(section => section?.QueuedStatus);
            DownloadingStatus = Format(section => section?.DownloadingStatus);
            StoppedStatus = Format(section => section?.StoppedStatus);
            ErrorStatus = Format(section => section?.ErrorStatus);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            Retry = Format(section => section?.Retry);
        }

        public string NonFictionDumpName { get; }
        public string FictionDumpName { get; }
        public string SciMagArticlesDumpName { get; }
        public string QueuedStatus { get; }
        public string DownloadingStatus { get; }
        public string StoppedStatus { get; }
        public string ErrorStatus { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string Retry { get; }

        public string GetDownloadProgress(long downloaded, long total, int percent) =>
            Format(section => section?.DownloadProgress,
                new { downloaded = Formatter.ToFormattedString(downloaded), total = Formatter.ToFormattedString(total),
                    percent = Formatter.ToFormattedString(percent) });
    }
}
