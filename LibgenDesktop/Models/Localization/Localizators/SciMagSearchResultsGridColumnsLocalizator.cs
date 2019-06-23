using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SciMagSearchResultsGridColumnsLocalizator : Localizator
    {
        public SciMagSearchResultsGridColumnsLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Title = Format(translation => translation?.Title);
            Authors = Format(translation => translation?.Authors);
            Magazine = Format(translation => translation?.Magazine);
            Year = Format(translation => translation?.Year);
            FileSize = Format(translation => translation?.FileSize);
            Doi = Format(translation => translation?.Doi);
            InLocalLibrary = Format(translation => translation?.InLocalLibrary);
        }

        public string Title { get; }
        public string Authors { get; }
        public string Magazine { get; }
        public string Year { get; }
        public string FileSize { get; }
        public string Doi { get; }
        public string InLocalLibrary { get; }

        private string Format(Func<Translation.SciMagSearchResultsGridColumnsTranslation, string> field)
        {
            return Format(translation => field(translation?.SciMagSearchResultsTab?.Columns));
        }
    }
}
