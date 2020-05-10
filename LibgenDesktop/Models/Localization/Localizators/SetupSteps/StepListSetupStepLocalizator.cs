using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class StepListSetupStepLocalizator : Localizator<Translation.StepListStepTranslation>
    {
        public StepListSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.StepListStep)
        {
            StepListHeader = Format(section => section?.StepListHeader);
            DownloadingDumps = Format(section => section?.DownloadingDumps);
            CreatingDatabase = Format(section => section?.CreatingDatabase);
            ImportingDumps = Format(section => section?.ImportingDumps);
            ClickNextButton = Format(section => section?.ClickNextButton);
        }

        public string StepListHeader { get; }
        public string DownloadingDumps { get; }
        public string CreatingDatabase { get; }
        public string ImportingDumps { get; }
        public string ClickNextButton { get; }

        public string GetStepString(int step) => Format(section => section?.Step, new { step = Formatter.ToDecimalFormattedString(step) });
    }
}
