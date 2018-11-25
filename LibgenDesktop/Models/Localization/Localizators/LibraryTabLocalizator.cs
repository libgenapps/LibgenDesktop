using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class LibraryTabLocalizator : Localizator
    {
        public LibraryTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            TabTitle = Format(translation => translation?.TabTitle);
            ScanNonFiction = Format(translation => translation?.ScanNonFiction);
            ScanFiction = Format(translation => translation?.ScanFiction);
            ScanSciMag = Format(translation => translation?.ScanSciMag);
            BrowseDirectoryDialogTitle = Format(translation => translation?.BrowseDirectoryDialogTitle);
            CreatingIndexes = Format(translation => translation?.CreatingIndexes);
            ScanLog = Format(translation => translation?.ScanLog);
            AddAll = Format(translation => translation?.AddAll);
            Adding = Format(translation => translation?.Adding);
            Added = Format(translation => translation?.Added);
            Error = Format(translation => translation?.Error);
            ColumnsFile = Format(translation => translation?.File);
            ColumnsAuthors = Format(translation => translation?.Authors);
            ColumnsTitle = Format(translation => translation?.Title);
        }

        public string TabTitle { get; }
        public string ScanNonFiction { get; }
        public string ScanFiction { get; }
        public string ScanSciMag { get; }
        public string BrowseDirectoryDialogTitle { get; }
        public string CreatingIndexes { get; }
        public string AddAll { get; }
        public string Adding { get; }
        public string Added { get; }
        public string ScanLog { get; }
        public string Error { get; }
        public string ColumnsFile { get; }
        public string ColumnsAuthors { get; }
        public string ColumnsTitle { get; }

        public string GetScanStartedString(string directory) => Format(translation => translation?.ScanStarted, new { directory });
        public string GetFoundString(int count) => Format(translation => translation?.Found, new { count });
        public string GetNotFoundString(int count) => Format(translation => translation?.NotFound, new { count });
        public string GetScanCompleteString(int found, int notFound, int errors) => Format(translation => translation?.ScanComplete,
            new { found = Formatter.ToFormattedString(found), notFound = Formatter.ToFormattedString(notFound),
                errors = Formatter.ToFormattedString(errors) });

        private string Format(Func<Translation.LibraryTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library), templateArguments);
        }

        private string Format(Func<Translation.LibraryColumnsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library?.Columns), templateArguments);
        }
    }
}
