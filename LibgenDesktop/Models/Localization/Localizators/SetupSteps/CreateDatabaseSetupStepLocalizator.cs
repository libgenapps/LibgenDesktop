using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class CreateDatabaseSetupStepLocalizator : Localizator<Translation.CreateDatabaseStepTranslation>
    {
        public CreateDatabaseSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.CreateDatabaseStep)
        {
            DatabaseFilePathPrompt = Format(section => section?.DatabaseFilePathPrompt);
            DatabaseCreated = Format(section => section?.DatabaseCreated);
            CannotCreateDatabase = Format(section => section?.CannotCreateDatabase);
            SelectDatabaseFilePathDialogTitle = Format(section => section?.SelectDatabaseFilePathDialogTitle);
            Databases = Format(section => section?.Databases);
            AllFiles = Format(section => section?.AllFiles);
            CreateDatabase = Format(section => section?.CreateDatabase);
            CreatingDatabase = Format(section => section?.CreatingDatabase);
            DatabaseFileOverwritePromptTitle = Format(section => section?.DatabaseFileOverwritePromptTitle);
        }

        public string DatabaseFilePathPrompt { get; }
        public string DatabaseCreated { get; }
        public string CannotCreateDatabase { get; }
        public string SelectDatabaseFilePathDialogTitle { get; }
        public string Databases { get; }
        public string AllFiles { get; }
        public string CreateDatabase { get; }
        public string CreatingDatabase { get; }
        public string DatabaseFileOverwritePromptTitle { get; }

        public string GetDiskSpaceRequirementsNoteString(decimal sizeInGigabytes) =>
            Format(section => section?.DiskSpaceRequirementsNote,
                new { size = $"{Formatter.ToFormattedString(sizeInGigabytes)} {Formatter.GigabytePostfix}" });

        public string GetDatabaseFileOverwritePromptTextString(string file) => Format(section => section?.DatabaseFileOverwritePromptText, new { file });
    }
}
