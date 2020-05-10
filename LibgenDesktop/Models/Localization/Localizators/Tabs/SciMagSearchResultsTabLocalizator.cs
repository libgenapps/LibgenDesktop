using System.Collections.Generic;
using LibgenDesktop.Models.Localization.Localizators.SearchResultGrids;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal class SciMagSearchResultsTabLocalizator : SearchResultsTabLocalizator<Translation.SciMagSearchResultsTabTranslation>
    {
        public SciMagSearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SciMagSearchResultsTab)
        {
            SearchBoxTooltip = Format(section => section?.SearchBoxTooltip);
            Columns = new SciMagSearchResultsGridColumnsLocalizator(prioritizedTranslationList, formatter);
        }

        public string SearchBoxTooltip { get; }
        public SciMagSearchResultsGridColumnsLocalizator Columns { get; }

        public string GetSearchProgressText(int count) => Format(section => section?.SearchProgress, new { count = Formatter.ToFormattedString(count) });

        public string GetStatusBarText(int count) => Format(section => section?.StatusBar, new { count = Formatter.ToFormattedString(count) });
    }
}
