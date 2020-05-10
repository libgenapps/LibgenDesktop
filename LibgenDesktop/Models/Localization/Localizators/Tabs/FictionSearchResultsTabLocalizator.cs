using System.Collections.Generic;
using LibgenDesktop.Models.Localization.Localizators.SearchResultGrids;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class FictionSearchResultsTabLocalizator : SearchResultsTabLocalizator<Translation.FictionSearchResultsTabTranslation>
    {
        public FictionSearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.FictionSearchResultsTab)
        {
            SearchBoxTooltip = Format(section => section?.SearchBoxTooltip);
            Columns = new FictionSearchResultsGridColumnsLocalizator(prioritizedTranslationList, formatter);
        }

        public string SearchBoxTooltip { get; }
        public FictionSearchResultsGridColumnsLocalizator Columns { get; }

        public string GetSearchProgressText(int count) => Format(section => section?.SearchProgress, new { count = Formatter.ToFormattedString(count) });

        public string GetStatusBarText(int count) => Format(section => section?.StatusBar, new { count = Formatter.ToFormattedString(count) });
    }
}
