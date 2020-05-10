using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class LibraryTabLocalizator : Localizator<Translation.LibraryTranslation>
    {
        public LibraryTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Library)
        {
            TabTitle = Format(section => section?.TabTitle);
            ScanNonFiction = Format(section => section?.ScanNonFiction);
            ScanFiction = Format(section => section?.ScanFiction);
            ScanSciMag = Format(section => section?.ScanSciMag);
            BrowseDirectoryDialogTitle = Format(section => section?.BrowseDirectoryDialogTitle);
            CreatingIndexes = Format(section => section?.CreatingIndexes);
            ScanLog = Format(section => section?.ScanLog);
            AddAll = Format(section => section?.AddAll);
            Adding = Format(section => section?.Adding);
            Added = Format(section => section?.Added);
            Error = Format(section => section?.Error);
            ColumnsFile = Format(section => section?.File);
            ColumnsAuthors = Format(section => section?.Authors);
            ColumnsTitle = Format(section => section?.Title);
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

        public string GetScanStartedString(string directory) => Format(section => section?.ScanStarted, new { directory });

        public string GetFoundString(int count) => Format(section => section?.Found, new { count = Formatter.ToFormattedString(count) });

        public string GetNotFoundString(int count) => Format(section => section?.NotFound, new { count = Formatter.ToFormattedString(count) });

        public string GetScanCompleteString(int found, int notFound, int errors) => Format(section => section?.ScanComplete,
            new { found = Formatter.ToFormattedString(found), notFound = Formatter.ToFormattedString(notFound),
                errors = Formatter.ToFormattedString(errors) });

        private string Format(Func<Translation.LibraryColumnsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library?.Columns), templateArguments);
        }
    }
}
