using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.SearchResultGrids
{
    internal class SciMagSearchResultsGridColumnsLocalizator : Localizator<Translation.SciMagSearchResultsGridColumnsTranslation>
    {
        public SciMagSearchResultsGridColumnsLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SciMagSearchResultsTab?.Columns)
        {
            Title = Format(section => section?.Title);
            Authors = Format(section => section?.Authors);
            Magazine = Format(section => section?.Magazine);
            Year = Format(section => section?.Year);
            FileSize = Format(section => section?.FileSize);
            Doi = Format(section => section?.Doi);
            InLocalLibrary = Format(section => section?.InLocalLibrary);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Magazine { get; }
        public string Year { get; }
        public string FileSize { get; }
        public string Doi { get; }
        public string InLocalLibrary { get; }
    }
}
