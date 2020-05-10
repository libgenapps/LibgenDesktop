using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class DownloadModeSetupStepLocalizator : Localizator<Translation.DownloadModeStepTranslation>
    {
        public DownloadModeSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.DownloadModeStep)
        {
            ChooseDownloadMode = Format(section => section?.ChooseDownloadMode);
            DownloadManagerMode = Format(section => section?.DownloadManagerMode);
            UseProxyServer = Format(section => section?.UseProxyServer);
            ProxySettings = Format(section => section?.ProxySettings);
            BrowserMode = Format(section => section?.BrowserMode);
        }

        public string ChooseDownloadMode { get; }
        public string DownloadManagerMode { get; }
        public string UseProxyServer { get; }
        public string ProxySettings { get; }
        public string BrowserMode { get; }
    }
}
