using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class FictionSearchResultsGridColumnsLocalizator : Localizator
    {
        public FictionSearchResultsGridColumnsLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Title = Format(translation => translation?.Title);
            Authors = Format(translation => translation?.Authors);
            Series = Format(translation => translation?.Series);
            Year = Format(translation => translation?.Year);
            Publisher = Format(translation => translation?.Publisher);
            FormatColumn = Format(translation => translation?.Format);
            FileSize = Format(translation => translation?.FileSize);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Series { get; }
        public string Year { get; }
        public string Publisher { get; }
        public string FormatColumn { get; }
        public string FileSize { get; }
        public string Ocr { get; }

        private string Format(Func<Translation.FictionSearchResultsGridColumnsTranslation, string> field)
        {
            return Format(translation => field(translation?.FictionSearchResultsTab?.Columns));
        }
    }
}
