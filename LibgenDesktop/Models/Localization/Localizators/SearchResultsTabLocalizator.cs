using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class SearchResultsTabLocalizator : Localizator
    {
        public SearchResultsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            SearchPlaceHolder = Format(translation => translation?.SearchPlaceHolder);
            SearchInProgress = Format(translation => translation?.SearchInProgress);
            ExportButtonTooltip = Format(translation => translation?.ExportButtonTooltip);
        }

        public string SearchPlaceHolder { get; }
        public string SearchInProgress { get; }
        public string ExportButtonTooltip { get; }

        private string Format(Func<Translation.SearchResultsTabsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.SearchResultsTabs), templateArguments);
        }
    }
}
