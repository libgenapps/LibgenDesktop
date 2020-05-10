using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SearchResultGrids
{
    internal class FictionSearchResultsGridColumnsLocalizator : Localizator<Translation.FictionSearchResultsGridColumnsTranslation>
    {
        public FictionSearchResultsGridColumnsLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.FictionSearchResultsTab?.Columns)
        {
            Title = Format(section => section?.Title);
            Authors = Format(section => section?.Authors);
            Series = Format(section => section?.Series);
            Year = Format(section => section?.Year);
            Language = Format(section => section?.Language);
            Publisher = Format(section => section?.Publisher);
            FormatColumn = Format(section => section?.Format);
            FileSize = Format(section => section?.FileSize);
            InLocalLibrary = Format(section => section?.InLocalLibrary);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Year { get; }
        public string Language { get; }
        public string Publisher { get; }
        public string FormatColumn { get; }
        public string FileSize { get; }
        public string Ocr { get; }
        public string InLocalLibrary { get; }
    }
}
