using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class DatabaseOperationSetupStepLocalizator : Localizator<Translation.DatabaseOperationStepTranslation>
    {
        public DatabaseOperationSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.DatabaseOperationStep)
        {
            ChooseAction = Format(section => section?.ChooseAction);
            CreateNewDatabase = Format(section => section?.CreateNewDatabase);
            OpenExistingDatabase = Format(section => section?.OpenExistingDatabase);
        }

        public string ChooseAction { get; }
        public string CreateNewDatabase { get; }
        public string OpenExistingDatabase { get; }
    }
}
