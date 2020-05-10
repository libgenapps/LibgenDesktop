using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class ImportDumpsSetupStepLocalizator : Localizator<Translation.ImportDumpsStepTranslation>
    {
        public ImportDumpsSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.ImportDumpsStep)
        {
            NonFictionDumpName = Format(section => section?.NonFictionDumpName);
            FictionDumpName = Format(section => section?.FictionDumpName);
            SciMagArticlesDumpName = Format(section => section?.SciMagArticlesDumpName);
            NotImported = Format(section => section?.NotImported);
            Importing = Format(section => section?.Importing);
            ImportSuccessful = Format(section => section?.ImportSuccessful);
            ImportCancelled = Format(section => section?.ImportCancelled);
            ImportError = Format(section => section?.ImportError);
            ImportButton = Format(section => section?.ImportButton);
            ImportingButton = Format(section => section?.ImportingButton);
            DeleteDumps = Format(section => section?.DeleteDumps);
        }

        public string NonFictionDumpName { get; }
        public string FictionDumpName { get; }
        public string SciMagArticlesDumpName { get; }
        public string NotImported { get; }
        public string Importing { get; }
        public string ImportSuccessful { get; }
        public string ImportCancelled { get; }
        public string ImportError { get; }
        public string ImportButton { get; }
        public string ImportingButton { get; }
        public string DeleteDumps { get; }

        public string GetStatusString(string status) => Format(section => section?.Status, new { status });
    }
}
