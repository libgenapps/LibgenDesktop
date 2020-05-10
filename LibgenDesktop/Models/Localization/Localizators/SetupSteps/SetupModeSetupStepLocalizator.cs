using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class SetupModeSetupStepLocalizator : Localizator<Translation.SetupModeStepTranslation>
    {
        public SetupModeSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.SetupModeStep)
        {
            WelcomeAndChooseOption = Format(section => section?.WelcomeAndChooseOption);
            FirstTimeUser = Format(section => section?.FirstTimeUser);
            ExperiencedUser = Format(section => section?.ExperiencedUser);
        }

        public string WelcomeAndChooseOption { get; }
        public string FirstTimeUser { get; }
        public string ExperiencedUser { get; }
    }
}
