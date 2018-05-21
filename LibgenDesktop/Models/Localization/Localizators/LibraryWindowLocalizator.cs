using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class LibraryWindowLocalizator : Localizator
    {
        public LibraryWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            Scan = Format(translation => translation?.Scan);
            BrowseDirectoryDialogTitle = Format(translation => translation?.BrowseDirectoryDialogTitle);
            CreatingIndexes = Format(translation => translation?.CreatingIndexes);
            NotFound = Format(translation => translation?.NotFound);
        }

        public string WindowTitle { get; }
        public string Scan { get; }
        public string BrowseDirectoryDialogTitle { get; }
        public string CreatingIndexes { get; }
        public string NotFound { get; }

        public string GetScanStartedString(string directory) => Format(translation => translation?.ScanStarted, new { directory });
        public string GetScanCompleteString(int found, int notFound) => Format(translation => translation?.ScanComplete,
            new { found = Formatter.ToFormattedString(found), notFound = Formatter.ToFormattedString(notFound) });

        private string Format(Func<Translation.LibraryTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Library), templateArguments);
        }
    }
}
