using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class NonFictionSearchResultsTabLocalizator : SearchResultsTabLocalizator
    {
        public NonFictionSearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            SearchBoxTooltip = Format(translation => translation?.SearchBoxTooltip);
            Columns = new NonFictionSearchResultsGridColumnsLocalizator(prioritizedTranslationList, formatter);
        }

        public string SearchBoxTooltip { get; }
        public NonFictionSearchResultsGridColumnsLocalizator Columns { get; }

        public string GetSearchProgressText(int count) => Format(translation => translation?.SearchProgress, new { count = Formatter.ToFormattedString(count) });
        public string GetStatusBarText(int count) => Format(translation => translation?.StatusBar, new { count = Formatter.ToFormattedString(count) });

        private string Format(Func<Translation.NonFictionSearchResultsTabTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.NonFictionSearchResultsTab), templateArguments);
        }
    }
}
