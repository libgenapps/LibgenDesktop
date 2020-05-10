using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SetupSteps
{
    internal class DownloadDumpLinksSetupStepLocalizator : Localizator<Translation.DownloadDumpLinksStepTranslation>
    {
        public DownloadDumpLinksSetupStepLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SetupWizardWindow?.DownloadDumpLinksStep)
        {
            OpenPage = Format(section => section?.OpenPage);
            CopyLink = Format(section => section?.CopyLink);
            DownloadFiles = Format(section => section?.DownloadFiles);
            NonFictionDumpName = Format(section => section?.NonFictionDumpName);
            FictionDumpName = Format(section => section?.FictionDumpName);
            SciMagArticlesDumpName = Format(section => section?.SciMagArticlesDumpName);
            YYYY = Format(section => section?.YYYY);
            MM = Format(section => section?.MM);
            DD = Format(section => section?.DD);
        }

        public string OpenPage { get; }
        public string CopyLink { get; }
        public string DownloadFiles { get; }
        public string NonFictionDumpName { get; }
        public string FictionDumpName { get; }
        public string SciMagArticlesDumpName { get; }
        public string YYYY { get; }
        public string MM { get; }
        public string DD { get; }

        public string GetFileNameString(string file) => Format(section => section?.FileName, new { file });

        public string GetMostRecentDateNoteString(string dateTemplate) => Format(section => section?.MostRecentDateNote, new { dateTemplate });
    }
}
