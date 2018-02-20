using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SciMagSearchResultsTabLocalizator : SearchResultsTabLocalizator
    {
        public SciMagSearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            SearchBoxTooltip = Format(translation => translation?.SearchBoxTooltip);
            Columns = new SciMagSearchResultsGridColumnsLocalizator(prioritizedTranslationList, formatter);
        }

        public string SearchBoxTooltip { get; }
        public SciMagSearchResultsGridColumnsLocalizator Columns { get; }

        public string GetSearchProgressText(int count) => Format(translation => translation?.SearchProgress, new { count = Formatter.ToFormattedString(count) });
        public string GetStatusBarText(int count) => Format(translation => translation?.StatusBar, new { count = Formatter.ToFormattedString(count) });

        private string Format(Func<Translation.SciMagSearchResultsTabTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.SciMagSearchResultsTab), templateArguments);
        }
    }
}
