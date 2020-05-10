using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class CollectionsSetupStepLocalizator : Localizator<Translation.CollectionsStepTranslation>
    {
        public CollectionsSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.CollectionsStep)
        {
            ChooseCollections = Format(section => section?.ChooseCollections);
            NonFiction = Format(section => section?.NonFiction);
            Fiction = Format(section => section?.Fiction);
            SciMagArticles = Format(section => section?.SciMagArticles);
            DownloadInto = Format(section => section?.DownloadInto);
            ErrorWarningTitle = Format(section => section?.ErrorWarningTitle);
            NoCollectionsSelected = Format(section => section?.NoCollectionsSelected);
        }

        public string ChooseCollections { get; }
        public string NonFiction { get; }
        public string Fiction { get; }
        public string SciMagArticles { get; }
        public string DownloadInto { get; }
        public string BrowseDirectoryDialogTitle { get; }
        public string ErrorWarningTitle { get; }
        public string NoCollectionsSelected { get; }

        public string GetDownloadSizeExactString(decimal size, string unit) =>
            Format(section => section?.DownloadSizeExact, new { size = $"{Formatter.ToFormattedString(size)} {unit}" });

        public string GetDownloadSizeApproximateString(decimal size, string unit) =>
            Format(section => section?.DownloadSizeApproximate, new { size = $"{Formatter.ToFormattedString(size)} {unit}" });

        public string GetDatabaseSizeString(decimal size, string unit) =>
            Format(section => section?.DatabaseSize, new { size = $"{Formatter.ToFormattedString(size)} {unit}" });

        public string GetImportTimeInMinutesString(int from, int to) =>
            Format(section => section?.ImportTimeInMinutes, new { from = Formatter.ToFormattedString(from), to = Formatter.ToFormattedString(to) });

        public string GetImportTimeInHoursString(decimal from, decimal to) =>
            Format(section => section?.ImportTimeInHours, new { from = Formatter.ToFormattedString(from), to = Formatter.ToFormattedString(to) });

        public string GetDirectoryNotFoundString(string directory) => Format(section => section?.DirectoryNotFound, new { directory });
    }
}
