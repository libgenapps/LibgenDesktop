using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class ConfirmationSetupStepLocalizator : Localizator<Translation.ConfirmationStepTranslation>
    {
        public ConfirmationSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.ConfirmationStep)
        {
            StepHeader = Format(section => section?.StepHeader);
            SettingsHeader = Format(section => section?.SettingsHeader);
            AllowInternetConnection = Format(section => section?.AllowInternetConnection);
            UseDownloadManager = Format(section => section?.UseDownloadManager);
            UseBrowser = Format(section => section?.UseBrowser);
            YouCanChangeSettings = Format(section => section?.YouCanChangeSettings);
        }

        public string StepHeader { get; }
        public string SettingsHeader { get; }
        public string AllowInternetConnection { get; }
        public string UseDownloadManager { get; }
        public string UseBrowser { get; }
        public string YouCanChangeSettings { get; }
    }
}
