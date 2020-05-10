using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class LanguageSetupStepLocalizator : Localizator<Translation.LanguageStepTranslation>
    {
        public LanguageSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.LanguageStep)
        {
            ChooseLanguage = Format(section => section?.ChooseLanguage);
        }

        public string ChooseLanguage { get; }
    }
}
