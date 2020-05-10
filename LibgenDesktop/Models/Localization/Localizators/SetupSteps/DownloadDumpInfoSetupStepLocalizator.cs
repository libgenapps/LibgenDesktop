using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class DownloadDumpInfoSetupStepLocalizator : Localizator<Translation.DownloadDumpInfoStepTranslation>
    {
        public DownloadDumpInfoSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.DownloadDumpInfoStep)
        {
            DownloadingDumpInfo = Format(section => section?.DownloadingDumpInfo);
            CannotDownloadDumpInfo = Format(section => section?.CannotDownloadDumpInfo);
            Interrupt = Format(section => section?.Interrupt);
            Interrupting = Format(section => section?.Interrupting);
            Retry = Format(section => section?.Retry);
        }

        public string DownloadingDumpInfo { get; }
        public string CannotDownloadDumpInfo { get; }
        public string Interrupt { get; }
        public string Interrupting { get; }
        public string Retry { get; }
    }
}
