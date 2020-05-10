using System.Collections.Generic;
using LibgenDesktop.Models.Localization.Localizators.SearchResultGrids;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class NonFictionSearchResultsTabLocalizator : SearchResultsTabLocalizator<Translation.NonFictionSearchResultsTabTranslation>
    {
        public NonFictionSearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.NonFictionSearchResultsTab)
        {
            SearchBoxTooltip = Format(section => section?.SearchBoxTooltip);
            Columns = new NonFictionSearchResultsGridColumnsLocalizator(prioritizedTranslationList, formatter);
        }

        public string SearchBoxTooltip { get; }
        public NonFictionSearchResultsGridColumnsLocalizator Columns { get; }

        public string GetSearchProgressText(int count) => Format(section => section?.SearchProgress, new { count = Formatter.ToFormattedString(count) });

        public string GetStatusBarText(int count) => Format(section => section?.StatusBar, new { count = Formatter.ToFormattedString(count) });
    }
}
