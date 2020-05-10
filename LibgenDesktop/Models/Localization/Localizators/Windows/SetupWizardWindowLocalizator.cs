using System.Collections.Generic;
using LibgenDesktop.Models.Localization.Localizators.SetupSteps;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class SetupWizardWindowLocalizator : Localizator<Translation.SetupWizardWindowTranslation>
    {
        public SetupWizardWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            DownloadingDumpsStepHeader = Format(section => section?.DownloadingDumpsStepHeader);
            CreatingDatabaseStepHeader = Format(section => section?.CreatingDatabaseStepHeader);
            ImportingDumpsStepHeader = Format(section => section?.ImportingDumpsStepHeader);
            Back = Format(section => section?.Back);
            Next = Format(section => section?.Next);
            Finish = Format(section => section?.Finish);
            Cancel = Format(section => section?.Cancel);
            ExitSetupTitle = Format(section => section?.ExitSetupTitle);
            ExitSetupText = Format(section => section?.ExitSetupText);
            LanguageStep = new LanguageSetupStepLocalizator(prioritizedTranslationList, formatter);
            SetupModeStep = new SetupModeSetupStepLocalizator(prioritizedTranslationList, formatter);
            DatabaseOperationStep = new DatabaseOperationSetupStepLocalizator(prioritizedTranslationList, formatter);
            StepListStep = new StepListSetupStepLocalizator(prioritizedTranslationList, formatter);
            DownloadModeStep = new DownloadModeSetupStepLocalizator(prioritizedTranslationList, formatter);
            DownloadDumpInfoStep = new DownloadDumpInfoSetupStepLocalizator(prioritizedTranslationList, formatter);
            CollectionsStep = new CollectionsSetupStepLocalizator(prioritizedTranslationList, formatter);
            DownloadDumpsStep = new DownloadDumpsSetupStepLocalizator(prioritizedTranslationList, formatter);
            DownloadDumpLinksStep = new DownloadDumpLinksSetupStepLocalizator(prioritizedTranslationList, formatter);
            ImportDumpsStep = new ImportDumpsSetupStepLocalizator(prioritizedTranslationList, formatter);
            CreateDatabaseStep = new CreateDatabaseSetupStepLocalizator(prioritizedTranslationList, formatter);
            ConfirmationStep = new ConfirmationSetupStepLocalizator(prioritizedTranslationList, formatter);
        }

        public string WindowTitle { get; }
        public string DownloadingDumpsStepHeader { get; }
        public string CreatingDatabaseStepHeader { get; }
        public string ImportingDumpsStepHeader { get; }
        public string Back { get; }
        public string Next { get; }
        public string Finish { get; }
        public string Cancel { get; }
        public string ExitSetupTitle { get; }
        public string ExitSetupText { get; }
        public LanguageSetupStepLocalizator LanguageStep { get; }
        public SetupModeSetupStepLocalizator SetupModeStep { get; }
        public DatabaseOperationSetupStepLocalizator DatabaseOperationStep { get; }
        public StepListSetupStepLocalizator StepListStep { get; }
        public DownloadModeSetupStepLocalizator DownloadModeStep { get; }
        public DownloadDumpInfoSetupStepLocalizator DownloadDumpInfoStep { get; }
        public CollectionsSetupStepLocalizator CollectionsStep { get; }
        public DownloadDumpsSetupStepLocalizator DownloadDumpsStep { get; }
        public DownloadDumpLinksSetupStepLocalizator DownloadDumpLinksStep { get; }
        public ImportDumpsSetupStepLocalizator ImportDumpsStep { get; }
        public CreateDatabaseSetupStepLocalizator CreateDatabaseStep { get; }
        public ConfirmationSetupStepLocalizator ConfirmationStep { get; }

        public string GetStepHeader(int current, int total, string header) =>
            Format(section => section?.StepHeaderTemplate,
                new { current = Formatter.ToDecimalFormattedString(current), total = Formatter.ToDecimalFormattedString(total), header });
    }
}
