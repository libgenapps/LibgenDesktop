using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class SetupWizardProxySettingsWindowLocalizator : Localizator<Translation.SetupWizardProxySettingsWindowTranslation>
    {
        public SetupWizardProxySettingsWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.ProxySettingsWindow)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            ProxyAddress = Format(section => section?.ProxyAddress);
            ProxyAddressRequired = Format(section => section?.ProxyAddressRequired);
            ProxyPort = Format(section => section?.ProxyPort);
            ProxyPortValidation = Format(section => section?.ProxyPortValidation);
            ProxyUserName = Format(section => section?.ProxyUserName);
            ProxyPassword = Format(section => section?.ProxyPassword);
            ProxyPasswordWarning = Format(section => section?.ProxyPasswordWarning);
            Ok = Format(section => section?.Ok);
            Cancel = Format(section => section?.Cancel);
        }

        public string WindowTitle { get; }
        public string ProxyAddress { get; }
        public string ProxyAddressRequired { get; }
        public string ProxyPort { get; }
        public string ProxyPortValidation { get; }
        public string ProxyUserName { get; }
        public string ProxyPassword { get; }
        public string ProxyPasswordWarning { get; }
        public string Ok { get; }
        public string Cancel { get; }

        public string GetProxyPortValidation(int min, int max) =>
            Format(section => section?.ProxyPortValidation,
                new { min = Formatter.ToDecimalFormattedString(min), max = Formatter.ToDecimalFormattedString(max) });
    }
}
